using UnityEngine;

namespace GamePush.Overlays.Widgets
{
    internal static class GP_OverlayKeyboardInset
    {
        internal static void Apply(RectTransform target, RectTransform canvasRoot, ref float currentInset)
        {
            if (target == null)
                return;

            var inset = 0f;
            if (TouchScreenKeyboard.visible)
            {
                var area = TouchScreenKeyboard.area;
                if (area.height > 0f && Screen.height > 0)
                {
                    var canvasHeight = canvasRoot != null ? canvasRoot.rect.height : Screen.height;
                    inset = area.height / Screen.height * canvasHeight;
                }
            }

            if (Mathf.Approximately(inset, currentInset))
                return;
            currentInset = inset;
            target.anchoredPosition = new Vector2(target.anchoredPosition.x, inset);
        }
    }
}
