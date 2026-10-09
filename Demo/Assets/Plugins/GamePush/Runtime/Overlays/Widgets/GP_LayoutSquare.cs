using UnityEngine;
using UnityEngine.UI;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// Keeps a layout-controlled graphic square after Horizontal/VerticalLayoutGroup runs.
    /// Avatars and status dots otherwise stretch with childForceExpandHeight.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class GP_LayoutSquare : MonoBehaviour, ILayoutSelfController
    {
        [SerializeField] float size = 14f;

        DrivenRectTransformTracker _tracker;

        public float Size
        {
            get => size;
            set
            {
                var next = Mathf.Max(1f, value);
                if (Mathf.Approximately(size, next))
                    return;
                size = next;
                ApplyLayoutElement();
                SetDirty();
            }
        }

        public static GP_LayoutSquare Lock(Component target, float side)
        {
            if (target == null)
                return null;
            var square = target.GetComponent<GP_LayoutSquare>() ??
                         target.gameObject.AddComponent<GP_LayoutSquare>();
            square.size = Mathf.Max(1f, side);
            square.ApplyGraphic();
            square.ApplyLayoutElement();
            square.SetDirty();
            return square;
        }

        void OnEnable()
        {
            ApplyGraphic();
            ApplyLayoutElement();
            SetDirty();
        }

        void OnDisable()
        {
            _tracker.Clear();
        }

        public void SetLayoutHorizontal()
        {
            var rect = (RectTransform)transform;
            _tracker.Clear();
            _tracker.Add(this, rect, DrivenTransformProperties.SizeDelta);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }

        public void SetLayoutVertical()
        {
            var rect = (RectTransform)transform;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }

        void ApplyGraphic()
        {
            var image = GetComponent<Image>();
            if (image == null)
                return;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        void ApplyLayoutElement()
        {
            var layout = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
            layout.minWidth = size;
            layout.minHeight = size;
            layout.preferredWidth = size;
            layout.preferredHeight = size;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;
            layout.layoutPriority = 100;
        }

        void SetDirty()
        {
            if (!isActiveAndEnabled)
                return;
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
        }
    }
}
