using UnityEngine;
using UnityEngine.UI;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// GridLayoutGroup with a cell size derived from the current width instead of a fixed value,
    /// so the same prefab gives two columns on a phone and four on a wide panel.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    [DisallowMultipleComponent]
    public sealed class GP_FlexibleGrid : MonoBehaviour
    {
        public int compactColumns = 2;
        public int wideColumns = 4;

        [Tooltip("When positive, column count is derived from available width and clamped by compact/wide columns.")]
        public float minCellWidth = 0f;

        [Tooltip("Cell height divided by cell width.")]
        public float cellRatio = 1f;

        [Tooltip("When positive, overrides cellRatio in Compact mode.")]
        public float compactCellRatio;

        [Tooltip("When positive, overrides cellRatio in Wide mode.")]
        public float wideCellRatio;

        [Tooltip("When positive, cell height is this value instead of width * ratio.")]
        public float cellHeight;

        GridLayoutGroup _grid;
        RectTransform _rect;
        GP_OverlayLayoutMode _mode;
        float _lastWidth = -1f;
        int _lastColumns = -1;
        float _lastRatio = -1f;
        float _lastCellHeight = -1f;

        void OnEnable()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rect = (RectTransform)transform;
            _mode = GetComponentInParent<GP_OverlayLayoutMode>();
            if (_mode != null)
                _mode.ModeChanged += OnModeChanged;
            Rebuild();
        }

        void OnDisable()
        {
            if (_mode != null)
                _mode.ModeChanged -= OnModeChanged;
        }

        void OnRectTransformDimensionsChange() => Rebuild();

        void OnModeChanged(GP_LayoutMode mode) => Invalidate();

        public void Invalidate()
        {
            _lastWidth = -1f;
            Rebuild();
        }

        public void Rebuild()
        {
            if (_grid == null || _rect == null)
                return;
            var width = _rect.rect.width;
            if (width <= 0f)
                return;

            var maxColumns = _mode != null && _mode.Mode == GP_LayoutMode.Wide ? wideColumns : compactColumns;
            var columns = maxColumns;
            if (minCellWidth > 0f)
            {
                var usable = width - _grid.padding.left - _grid.padding.right + _grid.spacing.x;
                columns = Mathf.Max(1, Mathf.FloorToInt(usable / (minCellWidth + _grid.spacing.x)));
                columns = Mathf.Min(columns, Mathf.Max(1, maxColumns));
            }
            columns = Mathf.Max(1, columns);
            var ratio = cellRatio;
            var wide = _mode != null && _mode.Mode == GP_LayoutMode.Wide;
            if (wide && wideCellRatio > 0f)
                ratio = wideCellRatio;
            else if (!wide && compactCellRatio > 0f)
                ratio = compactCellRatio;

            if (Mathf.Approximately(width, _lastWidth) && columns == _lastColumns &&
                Mathf.Approximately(ratio, _lastRatio) && Mathf.Approximately(cellHeight, _lastCellHeight))
                return;
            _lastWidth = width;
            _lastColumns = columns;
            _lastRatio = ratio;
            _lastCellHeight = cellHeight;

            var padding = _grid.padding.left + _grid.padding.right;
            var spacing = _grid.spacing.x * (columns - 1);
            var cellWidth = Mathf.Max(1f, (width - padding - spacing) / columns);
            var height = cellHeight > 0f ? cellHeight : cellWidth * Mathf.Max(0.1f, ratio);

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = columns;
            _grid.cellSize = new Vector2(cellWidth, height);
        }
    }
}
