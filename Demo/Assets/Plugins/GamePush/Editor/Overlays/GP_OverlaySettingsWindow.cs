using UnityEditor;
using UnityEngine;
using GamePush.Overlays;

namespace GamePushEditor.Overlays
{
    public sealed class GP_OverlaySettingsWindow : EditorWindow
    {
        enum PreviewPreset
        {
            PcLandscape,
            MobilePortrait,
            MobileLandscape,
            Square
        }

        GP_OverlaySkin _skin;
        Editor _skinEditor;
        GP_OverlayKind _kind;
        GP_OverlayPreviewState _state;
        GP_OverlayPreviewLanguage _language;
        PreviewPreset _preset;
        Vector2 _settingsScroll;
        GP_OverlayPreviewSession _preview;
        bool _previewDirty = true;
        float _previewZoom = 1f;
        Vector2 _previewPan;
        int _previewDragId;

        public static void Open()
        {
            var window = GetWindow<GP_OverlaySettingsWindow>();
            window.titleContent = new GUIContent("GamePush Overlays");
            window.minSize = new Vector2(900f, 560f);
            window.Show();
        }

        void OnEnable()
        {
            _preview = new GP_OverlayPreviewSession();
            SetSkin(GP_OverlayPrefabBuilder.DefaultSkin);
            EditorApplication.update += PreviewUpdate;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDisable()
        {
            EditorApplication.update -= PreviewUpdate;
            Undo.undoRedoPerformed -= OnUndoRedo;
            _preview?.Cleanup();
            _preview = null;
            if (_skinEditor != null)
                DestroyImmediate(_skinEditor);
            GP_OverlayPalette.Selected = GP_OverlayColorRole.None;
        }

        void PreviewUpdate()
        {
            if (!_previewDirty)
                return;
            _previewDirty = false;
            _preview?.Rebuild(_skin, _kind, _state, _language, PreviewResolution());
            Repaint();
        }

        void OnUndoRedo()
        {
            _previewDirty = true;
            Repaint();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawSettings();
            DrawPreview();
            EditorGUILayout.EndHorizontal();
        }

        void DrawSettings()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(390f));
            EditorGUILayout.Space(8f);

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Shared template", _skin, typeof(GP_OverlaySkin), false);

            EditorGUI.BeginChangeCheck();
            _kind = (GP_OverlayKind)EditorGUILayout.EnumPopup("Overlay", _kind);
            _state = (GP_OverlayPreviewState)EditorGUILayout.EnumPopup("State", _state);
            _language = (GP_OverlayPreviewLanguage)EditorGUILayout.EnumPopup("Language", _language);
            _preset = (PreviewPreset)EditorGUILayout.EnumPopup("Viewport", _preset);
            if (EditorGUI.EndChangeCheck())
            {
                _previewDirty = true;
                _previewPan = Vector2.zero;
            }

            EditorGUILayout.Space(8f);
            DrawPrefabActions();
            EditorGUILayout.Space(8f);

            _settingsScroll = EditorGUILayout.BeginScrollView(_settingsScroll);
            if (_skinEditor != null)
            {
                EditorGUI.BeginChangeCheck();
                _skinEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_skin);
                    if (GP_OverlayPalette.LastChangeWasPalette && _preview != null && _preview.Retint(_skin))
                        Repaint();
                    else
                        _previewDirty = true;
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawPrefabActions()
        {
            var entry = _skin != null ? _skin.Find(_kind) : null;
            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(entry == null || entry.prefab == null))
            {
                if (GUILayout.Button("Open default prefab"))
                    AssetDatabase.OpenAsset(entry.prefab);
                if (GUILayout.Button("Create and assign custom copy"))
                    CreateCustomCopy(entry);
            }

            if (entry != null)
            {
                EditorGUI.BeginChangeCheck();
                var custom = (GameObject)EditorGUILayout.ObjectField("Custom override", entry.customPrefab,
                    typeof(GameObject), false);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_skin, "Assign overlay prefab");
                    entry.customPrefab = custom;
                    EditorUtility.SetDirty(_skin);
                    _previewDirty = true;
                }
                using (new EditorGUI.DisabledScope(entry.customPrefab == null))
                {
                    if (GUILayout.Button("Reset to generated default"))
                    {
                        Undo.RecordObject(_skin, "Reset overlay prefab");
                        entry.customPrefab = null;
                        EditorUtility.SetDirty(_skin);
                        _previewDirty = true;
                    }
                }
            }

            EditorGUILayout.Space(4f);
            if (GUILayout.Button("Reset to Default"))
                ResetToDefaults();
            if (GUILayout.Button("Rebuild selected default"))
                RebuildSelected();
            if (GUILayout.Button("Apply template and rebuild all defaults"))
                RebuildAll();
            if (!GP_OverlayPrefabBuilder.IsTextMeshProReady)
                EditorGUILayout.HelpBox("Import TextMeshPro Essential Resources before rebuilding.",
                    MessageType.Warning);
            EditorGUILayout.HelpBox(
                "Click a palette row to highlight the surfaces that use that slot. Palette colors update the preview immediately; Hover, Pressed and Disabled preview on buttons. Rebuild is only needed for layout, sprites and generated prefabs.",
                MessageType.Info);
        }

        void DrawPreview()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawPreviewToolbar();
            var rect = GUILayoutUtility.GetRect(300f, 10000f, 300f, 10000f,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, new Color(0.025f, 0.03f, 0.035f, 1f));
            HandlePreviewInput(rect);
            _preview?.Present(rect, _previewZoom, _previewPan);
            EditorGUILayout.EndVertical();
        }

        void DrawPreviewToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Fit", EditorStyles.toolbarButton, GUILayout.Width(40f)))
            {
                _previewZoom = 1f;
                _previewPan = Vector2.zero;
                Repaint();
            }

            EditorGUI.BeginChangeCheck();
            _previewZoom = GUILayout.HorizontalSlider(_previewZoom, 0.5f, 4f, GUILayout.Width(140f));
            if (EditorGUI.EndChangeCheck())
            {
                if (_previewZoom <= 1.01f)
                    _previewPan = Vector2.zero;
                Repaint();
            }

            var zoomLabel = Mathf.Approximately(_previewZoom, 1f)
                ? "Fit"
                : _previewZoom.ToString("0.0") + "×";
            GUILayout.Label(zoomLabel, EditorStyles.miniLabel, GUILayout.Width(40f));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Scroll to zoom, drag to pan", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }

        void HandlePreviewInput(Rect rect)
        {
            var e = Event.current;
            if (e == null)
                return;

            var logical = PreviewResolution();
            if (e.type == EventType.ScrollWheel && rect.Contains(e.mousePosition))
            {
                var oldZoom = _previewZoom;
                _previewZoom = Mathf.Clamp(_previewZoom * (e.delta.y > 0f ? 0.9f : 1.11f), 0.5f, 4f);
                var center = rect.center + _previewPan;
                if (!Mathf.Approximately(oldZoom, 0f))
                    _previewPan += (e.mousePosition - center) * (1f - _previewZoom / oldZoom);
                if (_previewZoom <= 1.01f)
                    _previewPan = Vector2.zero;
                else
                    _previewPan = ClampPan(rect, logical, _previewZoom, _previewPan);
                e.Use();
                Repaint();
            }

            var id = GUIUtility.GetControlID(FocusType.Passive);
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition) &&
                (e.button == 0 || e.button == 2) && _previewZoom > 1.01f)
            {
                GUIUtility.hotControl = id;
                _previewDragId = id;
                e.Use();
            }

            if ((e.type == EventType.MouseDrag || e.type == EventType.MouseUp) &&
                GUIUtility.hotControl == _previewDragId && _previewDragId != 0)
            {
                if (e.type == EventType.MouseDrag)
                {
                    _previewPan += e.delta;
                    _previewPan = ClampPan(rect, logical, _previewZoom, _previewPan);
                    Repaint();
                }
                else
                {
                    GUIUtility.hotControl = 0;
                    _previewDragId = 0;
                }

                e.Use();
            }
        }

        static Vector2 ClampPan(Rect pane, Vector2Int logical, float zoom, Vector2 pan)
        {
            var dest = GP_OverlayPreviewSession.FittedRect(
                new Rect(0f, 0f, pane.width, pane.height), logical, zoom, Vector2.zero);
            var extraX = Mathf.Max(0f, dest.width - pane.width) * 0.5f;
            var extraY = Mathf.Max(0f, dest.height - pane.height) * 0.5f;
            return new Vector2(
                Mathf.Clamp(pan.x, -extraX - 24f, extraX + 24f),
                Mathf.Clamp(pan.y, -extraY - 24f, extraY + 24f));
        }

        void SetSkin(GP_OverlaySkin skin)
        {
            if (_skinEditor != null)
                DestroyImmediate(_skinEditor);
            _skin = skin != null ? skin : GP_OverlayPrefabBuilder.DefaultSkin;
            _skinEditor = _skin != null ? Editor.CreateEditor(_skin) : null;
            _previewDirty = true;
        }

        void ResetToDefaults()
        {
            if (_skin == null)
                return;
            if (!EditorUtility.DisplayDialog("GamePush",
                    "Reset palette, spacing and type sizes to plugin defaults and rebuild generated prefabs?\n\nCustom copies, sprites, icons and font stay as they are.",
                    "Reset to Default", "Cancel"))
                return;
            Undo.RecordObject(_skin, "Reset overlay skin");
            _skin.ResetAppearanceToDefaults();
            EditorUtility.SetDirty(_skin);
            GP_OverlayPalette.Selected = GP_OverlayColorRole.None;
            if (GP_OverlayPrefabBuilder.EnsureTextMeshPro())
                GP_OverlayPrefabBuilder.Rebuild(_skin);
            SetSkin(_skin);
        }

        void RebuildSelected()
        {
            if (!GP_OverlayPrefabBuilder.EnsureTextMeshPro())
                return;
            if (!EditorUtility.DisplayDialog("GamePush",
                    "Rebuild the generated " + _kind + " prefab? Its generated asset will be overwritten.",
                    "Rebuild", "Cancel"))
                return;
            GP_OverlayPrefabBuilder.RebuildScreen(_kind, _skin);
            _previewDirty = true;
        }

        void RebuildAll()
        {
            if (!GP_OverlayPrefabBuilder.EnsureTextMeshPro())
                return;
            if (!EditorUtility.DisplayDialog("GamePush",
                    "Apply this template and rebuild all generated overlay prefabs? Custom copies are preserved.",
                    "Rebuild all", "Cancel"))
                return;
            GP_OverlayPrefabBuilder.Rebuild(_skin);
            _previewDirty = true;
        }

        void CreateCustomCopy(GP_OverlayPrefabEntry entry)
        {
            var source = AssetDatabase.GetAssetPath(entry.prefab);
            var path = EditorUtility.SaveFilePanelInProject("Create custom overlay prefab",
                _kind + "Custom", "prefab", "Choose a project-owned location for the custom prefab.");
            if (string.IsNullOrEmpty(path))
                return;
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            if (!AssetDatabase.CopyAsset(source, path))
            {
                EditorUtility.DisplayDialog("GamePush", "Could not copy the prefab.", "OK");
                return;
            }
            var copy = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Undo.RecordObject(_skin, "Create custom overlay prefab");
            entry.customPrefab = copy;
            EditorUtility.SetDirty(_skin);
            AssetDatabase.SaveAssets();
            Selection.activeObject = copy;
            EditorGUIUtility.PingObject(copy);
            _previewDirty = true;
        }

        Vector2Int PreviewResolution()
        {
            switch (_preset)
            {
                case PreviewPreset.MobilePortrait: return new Vector2Int(540, 960);
                case PreviewPreset.MobileLandscape: return new Vector2Int(960, 540);
                case PreviewPreset.Square: return new Vector2Int(720, 720);
                default: return new Vector2Int(1280, 720);
            }
        }
    }
}
