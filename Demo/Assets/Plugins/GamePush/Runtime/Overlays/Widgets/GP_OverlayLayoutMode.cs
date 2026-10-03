using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Overlays.Widgets
{
    public enum GP_LayoutMode
    {
        Compact,
        Wide
    }

    /// <summary>
    /// Broadcasts Compact / Wide based on the aspect of the panel itself rather than the screen,
    /// so a square panel on a landscape phone and a narrow desktop window behave identically.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class GP_OverlayLayoutMode : MonoBehaviour
    {
        [Tooltip("Panel aspect at which Compact turns into Wide. Falls back to the skin value when 0.")]
        public float threshold;

        [Tooltip("Objects shown only in Compact mode.")]
        public List<GameObject> compactOnly = new List<GameObject>();

        [Tooltip("Objects shown only in Wide mode.")]
        public List<GameObject> wideOnly = new List<GameObject>();

        GP_LayoutMode _mode = GP_LayoutMode.Compact;
        bool _evaluated;

        public GP_LayoutMode Mode => _mode;

        public event Action<GP_LayoutMode> ModeChanged;

        public void Evaluate(float width, float height)
        {
            if (height <= 0f)
                return;
            var limit = threshold > 0f ? threshold : GP_OverlaySkin.Instance.wideThreshold;
            var next = width / height >= limit ? GP_LayoutMode.Wide : GP_LayoutMode.Compact;
            if (_evaluated && next == _mode)
                return;
            _evaluated = true;
            _mode = next;
            ApplyVisibility();
            ModeChanged?.Invoke(_mode);
        }

        void ApplyVisibility()
        {
            foreach (var item in compactOnly)
            {
                if (item != null)
                    item.SetActive(_mode == GP_LayoutMode.Compact);
            }
            foreach (var item in wideOnly)
            {
                if (item != null)
                    item.SetActive(_mode == GP_LayoutMode.Wide);
            }
        }
    }
}
