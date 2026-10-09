using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// Row host shared by the achievement, leaderboard, member, message and feedback screens.
    /// Instances are pooled and reused instead of being destroyed between binds.
    /// </summary>
    public sealed class GP_OverlayList : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform content;
        public GameObject rowPrefab;

        [Tooltip("Fraction of the scroll range from the far edge that triggers OnNeedMore.")]
        public float loadMoreThreshold = 0.1f;

        readonly List<GameObject> _active = new List<GameObject>();
        readonly Stack<GameObject> _pool = new Stack<GameObject>();

        bool _canLoadMore;
        bool _moreRequested;

        public event Action OnNeedMore;

        public IReadOnlyList<GameObject> Rows => _active;

        void Awake()
        {
            if (content == null && scrollRect != null)
                content = scrollRect.content;
            if (scrollRect != null)
                scrollRect.onValueChanged.AddListener(OnScrolled);
        }

        public void SetCanLoadMore(bool value)
        {
            _canLoadMore = value;
            _moreRequested = false;
        }

        public void Clear()
        {
            foreach (var row in _active)
            {
                if (row == null)
                    continue;
                row.SetActive(false);
                _pool.Push(row);
            }
            _active.Clear();
        }

        /// <summary>Rebinds the whole list; <paramref name="bind"/> receives the row and its index.</summary>
        public void Bind(int count, Action<GameObject, int> bind, GameObject prefabOverride = null)
        {
            Clear();
            for (var i = 0; i < count; i++)
            {
                var row = Take(prefabOverride);
                if (row == null)
                    return;
                _active.Add(row);
                row.transform.SetSiblingIndex(i);
                bind?.Invoke(row, i);
            }
        }

        public GameObject Append(Action<GameObject, int> bind, GameObject prefabOverride = null)
        {
            var row = Take(prefabOverride);
            if (row == null)
                return null;
            _active.Add(row);
            row.transform.SetAsLastSibling();
            bind?.Invoke(row, _active.Count - 1);
            return row;
        }

        public void ScrollToBottom()
        {
            if (scrollRect == null)
                return;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void ScrollToTop()
        {
            if (scrollRect == null)
                return;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        GameObject Take(GameObject prefabOverride)
        {
            var prefab = prefabOverride != null ? prefabOverride : rowPrefab;
            if (prefab == null)
            {
                GP_Logger.Warn("OVERLAYS", "GP_OverlayList on " + name + " has no row prefab");
                return null;
            }

            while (_pool.Count > 0)
            {
                var pooled = _pool.Pop();
                if (pooled == null)
                    continue;
                pooled.SetActive(true);
                return pooled;
            }
            return Instantiate(prefab, content != null ? content : (RectTransform)transform);
        }

        void OnScrolled(Vector2 position)
        {
            if (!_canLoadMore || _moreRequested || OnNeedMore == null)
                return;
            // Chat grows upwards, everything else downwards; react to whichever edge is reached.
            if (position.y >= 1f - loadMoreThreshold || position.y <= loadMoreThreshold)
            {
                _moreRequested = true;
                OnNeedMore.Invoke();
            }
        }
    }
}
