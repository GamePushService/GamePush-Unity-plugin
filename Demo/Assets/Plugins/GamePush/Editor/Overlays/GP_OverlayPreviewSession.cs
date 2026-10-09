using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using GamePush.Overlays;
using GamePush.Overlays.Widgets;

namespace GamePushEditor.Overlays
{
    internal sealed class GP_OverlayPreviewSession
    {
        const int MinTexture = 256;
        const int MaxTexture = 4096;
        const int SizeStep = 32;
        const float SuperSample = 2f;

        Camera _camera;
        Canvas _canvas;
        CanvasScaler _scaler;
        RenderTexture _texture;
        GameObject _root;
        GP_OverlaySkin _skin;
        GP_OverlayKind _kind;
        Vector2Int _logical = new Vector2Int(1280, 720);
        bool _needsRender = true;
        bool _interactionPreviewApplied;

        internal RenderTexture Texture => _texture;

        /// <summary>
        /// Fit-to-pane size at 2x supersample. Zoom is a blit, so this must not depend on it —
        /// otherwise every wheel tick reallocates an MSAA target and rebuilds TMP layout.
        /// </summary>
        internal static Vector2Int RenderTextureSize(Rect pane, Vector2Int logical, float pixelsPerPoint)
        {
            if (logical.x < 1 || logical.y < 1)
                logical = new Vector2Int(1280, 720);
            pixelsPerPoint = Mathf.Max(0.5f, pixelsPerPoint);
            if (pane.width < 16f || pane.height < 16f)
                return ClampSize(logical);

            var fit = Mathf.Min(pane.width / logical.x, pane.height / logical.y);
            var height = AlignUp(Mathf.RoundToInt(logical.y * fit * pixelsPerPoint * SuperSample), SizeStep);
            var width = Mathf.RoundToInt(height * (logical.x / (float)logical.y));
            if (width > MaxTexture || height > MaxTexture)
            {
                var scale = MaxTexture / (float)Mathf.Max(width, height);
                width = Mathf.Max(1, Mathf.RoundToInt(width * scale));
                height = Mathf.Max(1, Mathf.RoundToInt(height * scale));
            }

            return ClampSize(new Vector2Int(width, height));
        }

        static Vector2Int ClampSize(Vector2Int size)
        {
            return new Vector2Int(
                Mathf.Clamp(size.x, MinTexture, MaxTexture),
                Mathf.Clamp(size.y, MinTexture, MaxTexture));
        }

        static int AlignUp(int value, int step)
        {
            if (step <= 1)
                return value;
            return (Mathf.Max(1, value) + step - 1) / step * step;
        }

        internal void Rebuild(GP_OverlaySkin skin, GP_OverlayKind kind, GP_OverlayPreviewState state,
            GP_OverlayPreviewLanguage language, Vector2Int resolution)
        {
            Cleanup();
            if (skin == null)
                return;

            _skin = skin;
            _kind = kind;
            _logical = new Vector2Int(Mathf.Max(240, resolution.x), Mathf.Max(240, resolution.y));

            var cameraObject = new GameObject("GP Overlay Preview Camera");
            cameraObject.hideFlags = HideFlags.HideAndDontSave;
            _camera = cameraObject.AddComponent<Camera>();
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.025f, 0.03f, 0.035f, 1f);
            _camera.orthographic = true;
            _camera.allowHDR = false;
            _camera.nearClipPlane = 0.01f;
            _camera.farClipPlane = 100f;
            _camera.transform.position = new Vector3(0f, 0f, -10f);

            var canvasObject = new GameObject("GP Overlay Preview Canvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.hideFlags = HideFlags.HideAndDontSave;
            _canvas = canvasObject.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = _camera;
            _canvas.planeDistance = 1f;
            _canvas.pixelPerfect = false;
            _canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                               AdditionalCanvasShaderChannels.Normal |
                                               AdditionalCanvasShaderChannels.Tangent;

            _scaler = canvasObject.GetComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = skin.referenceResolution;
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            _root = CreatePreviewRoot(skin, kind);
            _root.name += " (Preview)";
            _root.hideFlags = HideFlags.HideAndDontSave;
            _root.transform.SetParent(canvasObject.transform, false);
            var rootRect = (RectTransform)_root.transform;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            ApplyPreviewLayout();
            var view = _root.GetComponent<GP_OverlayView>();
            if (view != null)
                view.ApplyChrome(skin);
            GP_OverlayPreviewFixtures.Populate(_root, kind, skin, state, language);
            _interactionPreviewApplied = false;
            _needsRender = true;
        }

        internal bool Retint(GP_OverlaySkin skin)
        {
            if (_root == null || _camera == null || skin == null)
                return false;
            _skin = skin;
            var view = _root.GetComponent<GP_OverlayView>();
            if (view != null)
                view.ApplyChrome(skin);
            else
                GP_OverlayTone.Apply(_root, skin);
            _interactionPreviewApplied = false;
            _needsRender = true;
            return true;
        }

        internal void Present(Rect pane, float zoom, Vector2 pan)
        {
            if (_camera == null || Event.current.type != EventType.Repaint)
                return;
            if (pane.width < 16f || pane.height < 16f)
                return;

            var size = RenderTextureSize(pane, _logical, EditorGUIUtility.pixelsPerPoint);
            if (EnsureTexture(size))
            {
                ApplyPreviewLayout();
                _needsRender = true;
            }

            ApplyInteractionPreview();
            if (_needsRender && _texture != null)
            {
                Canvas.ForceUpdateCanvases();
                _camera.Render();
                _needsRender = false;
            }

            if (_texture == null)
                return;

            GUI.BeginGroup(pane);
            var dest = FittedRect(new Rect(0f, 0f, pane.width, pane.height), _logical, zoom, pan);
            GUI.DrawTexture(dest, _texture, ScaleMode.StretchToFill, true);
            DrawPaletteOverlay(dest, GP_OverlayPalette.Selected);
            GUI.EndGroup();
        }

        internal static bool TryMapPaneToViewport(Rect pane, Vector2 mouse, Vector2Int logical, float zoom,
            Vector2 pan, out Vector2 viewport)
        {
            var dest = FittedRect(pane, logical, zoom, pan);
            if (dest.width < 1f || dest.height < 1f || !dest.Contains(mouse))
            {
                viewport = default;
                return false;
            }

            viewport = new Vector2(
                (mouse.x - dest.x) / dest.width,
                1f - (mouse.y - dest.y) / dest.height);
            return true;
        }

        void ApplyInteractionPreview()
        {
            var slot = GP_OverlayPalette.Selected;
            var interaction = GP_OverlayPalette.IsInteraction(slot);
            if (interaction == _interactionPreviewApplied && !_needsRender)
                return;
            if (interaction)
            {
                ApplyButtonStatePreview(slot);
                _interactionPreviewApplied = true;
                _needsRender = true;
                return;
            }

            if (!_interactionPreviewApplied)
                return;
            if (_root != null && _skin != null)
            {
                var view = _root.GetComponent<GP_OverlayView>();
                if (view != null)
                    view.ApplyChrome(_skin);
                else
                    GP_OverlayTone.Apply(_root, _skin);
            }

            _interactionPreviewApplied = false;
            _needsRender = true;
        }

        void ApplyButtonStatePreview(GP_OverlayColorRole slot)
        {
            if (_root == null || _skin == null)
                return;
            var color = _skin.ColorOf(slot);
            var buttons = _root.GetComponentsInChildren<Button>(true);
            for (var i = 0; i < buttons.Length; i++)
            {
                var button = buttons[i];
                if (button == null || button.transition != Selectable.Transition.ColorTint)
                    continue;
                var block = button.colors;
                block.normalColor = color;
                block.highlightedColor = color;
                block.pressedColor = color;
                block.selectedColor = color;
                button.colors = block;
            }
        }

        void DrawPaletteOverlay(Rect dest, GP_OverlayColorRole slot)
        {
            if (slot == GP_OverlayColorRole.None || slot == GP_OverlayColorRole.Backdrop ||
                _root == null || _skin == null || _camera == null)
                return;

            if (GP_OverlayPalette.IsInteraction(slot))
            {
                var buttons = _root.GetComponentsInChildren<Button>(false);
                for (var i = 0; i < buttons.Length; i++)
                {
                    var button = buttons[i];
                    if (button == null || !button.isActiveAndEnabled)
                        continue;
                    if (button.transition != Selectable.Transition.ColorTint)
                        continue;
                    var graphic = button.targetGraphic;
                    if (graphic == null || !graphic.isActiveAndEnabled)
                        continue;
                    if (!TryGraphicToPane(graphic, dest, out var rect))
                        continue;
                    EditorGUI.DrawRect(rect, new Color(1f, 0.82f, 0.18f, 0.10f));
                    DrawOutline(rect, new Color(1f, 0.82f, 0.18f, 0.95f));
                }

                return;
            }

            var tones = _root.GetComponentsInChildren<GP_OverlayTone>(false);
            for (var i = 0; i < tones.Length; i++)
            {
                var tone = tones[i];
                if (tone == null || tone.role != slot)
                    continue;
                var graphic = tone.GetComponent<Graphic>();
                if (graphic == null || !graphic.isActiveAndEnabled)
                    continue;
                if (!TryGraphicToPane(graphic, dest, out var rect))
                    continue;
                EditorGUI.DrawRect(rect, new Color(1f, 0.82f, 0.18f, 0.10f));
                DrawOutline(rect, new Color(1f, 0.82f, 0.18f, 0.95f));
            }
        }

        bool TryGraphicToPane(Graphic graphic, Rect dest, out Rect paneRect)
        {
            paneRect = default;
            if (graphic == null)
                return false;

            var corners = new Vector3[4];
            graphic.rectTransform.GetWorldCorners(corners);
            var min = new Vector2(1f, 1f);
            var max = new Vector2(0f, 0f);
            var any = false;
            for (var i = 0; i < 4; i++)
            {
                var view = _camera.WorldToViewportPoint(corners[i]);
                if (view.z < 0f)
                    continue;
                any = true;
                min = Vector2.Min(min, view);
                max = Vector2.Max(max, view);
            }

            if (!any)
                return false;

            paneRect = new Rect(
                dest.x + min.x * dest.width,
                dest.y + (1f - max.y) * dest.height,
                (max.x - min.x) * dest.width,
                (max.y - min.y) * dest.height);
            return paneRect.width >= 1f && paneRect.height >= 1f;
        }

        static void DrawOutline(Rect rect, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 2f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 2f, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - 2f, rect.y, 2f, rect.height), color);
        }

        internal static Rect FittedRect(Rect pane, Vector2Int logical, float zoom, Vector2 pan)
        {
            if (logical.x < 1 || logical.y < 1)
                return pane;
            zoom = Mathf.Clamp(zoom, 0.25f, 4f);
            var fit = Mathf.Min(pane.width / logical.x, pane.height / logical.y);
            var width = logical.x * fit * zoom;
            var height = logical.y * fit * zoom;
            var x = pane.x + (pane.width - width) * 0.5f + pan.x;
            var y = pane.y + (pane.height - height) * 0.5f + pan.y;
            return new Rect(x, y, width, height);
        }

        bool EnsureTexture(Vector2Int size)
        {
            size = ClampSize(size);
            if (_texture != null && _texture.IsCreated() &&
                _texture.width >= size.x && _texture.height >= size.y)
                return false;

            if (_texture != null)
            {
                _texture.Release();
                Object.DestroyImmediate(_texture);
            }

            _texture = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32)
            {
                name = "GamePush Overlay Preview",
                hideFlags = HideFlags.HideAndDontSave,
                antiAliasing = 2,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            _texture.Create();
            if (_camera != null)
                _camera.targetTexture = _texture;
            return true;
        }

        void ApplyPreviewLayout()
        {
            if (_root == null || _skin == null)
                return;

            var width = _texture != null ? _texture.width : _logical.x;
            var height = _texture != null ? _texture.height : _logical.y;
            if (width < 1 || height < 1)
                return;
            if (_scaler != null)
            {
                var reference = GP_OverlayFit.ReferenceFor(_skin.referenceResolution, width, height);
                _scaler.referenceResolution = reference;
                _scaler.matchWidthOrHeight = GP_OverlayFit.MatchWidthOrHeight(width, height, reference);
            }

            Canvas.ForceUpdateCanvases();
            var responsive = _root.GetComponentInChildren<GP_OverlayResponsive>(true);
            var entry = _skin.Find(_kind);
            var viewportSize = GP_OverlayFit.CanvasLogicalSize(_skin.referenceResolution, width, height);
            if (responsive != null)
            {
                responsive.Configure(entry, _kind, GP_OverlayFit.IsMobilePreview(_logical));
                responsive.ApplyForPreview(viewportSize);
            }
        }

        static GameObject CreatePreviewRoot(GP_OverlaySkin skin, GP_OverlayKind kind)
        {
            var prefab = skin != null ? skin.PrefabFor(kind) : null;
            if (prefab != null)
                return Object.Instantiate(prefab);
            return GP_OverlayPrefabBuilder.BuildPreview(kind, skin);
        }

        internal void Cleanup()
        {
            if (_root != null)
                Object.DestroyImmediate(_root);
            if (_canvas != null)
                Object.DestroyImmediate(_canvas.gameObject);
            if (_camera != null)
                Object.DestroyImmediate(_camera.gameObject);
            if (_texture != null)
            {
                _texture.Release();
                Object.DestroyImmediate(_texture);
            }
            _root = null;
            _canvas = null;
            _scaler = null;
            _camera = null;
            _texture = null;
            _skin = null;
            _interactionPreviewApplied = false;
        }
    }
}
