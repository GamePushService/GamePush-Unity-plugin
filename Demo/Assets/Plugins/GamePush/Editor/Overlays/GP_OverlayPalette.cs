using UnityEngine;
using GamePush.Overlays;

namespace GamePushEditor.Overlays
{
    internal static class GP_OverlayPalette
    {
        internal static readonly GP_OverlayColorRole[] Slots = GP_OverlayTone.Roles;

        internal static GP_OverlayColorRole Selected;

        internal static bool LastChangeWasPalette;

        internal static Color Get(GP_OverlaySkin skin, GP_OverlayColorRole slot) =>
            skin != null ? skin.ColorOf(slot) : Color.clear;

        internal static bool IsInteraction(GP_OverlayColorRole slot) =>
            slot == GP_OverlayColorRole.Hover
            || slot == GP_OverlayColorRole.Pressed
            || slot == GP_OverlayColorRole.Disabled;

        internal static string GroupTitle(string propertyName)
        {
            switch (propertyName)
            {
                case "backdrop": return "Panels";
                case "row": return "Lists";
                case "button": return "Buttons";
                case "text": return "Text";
                default: return null;
            }
        }

        internal static string Label(GP_OverlayColorRole slot)
        {
            switch (slot)
            {
                case GP_OverlayColorRole.RowAlt: return "Row Alt";
                case GP_OverlayColorRole.ButtonSelected: return "Button Selected";
                case GP_OverlayColorRole.TextMuted: return "Text Muted";
                default: return slot.ToString();
            }
        }

        internal static string Tooltip(GP_OverlayColorRole slot)
        {
            switch (slot)
            {
                case GP_OverlayColorRole.Backdrop:
                    return "Full-screen dimmer behind the panel.";
                case GP_OverlayColorRole.Panel:
                    return "Main overlay panel.";
                case GP_OverlayColorRole.Header:
                    return "Title bar and leaderboard column header.";
                case GP_OverlayColorRole.Sidebar:
                    return "Tab rails, members column, composer and thread chrome.";
                case GP_OverlayColorRole.Border:
                    return "Dividers and hairlines.";
                case GP_OverlayColorRole.Input:
                    return "Input fields and progress tracks.";
                case GP_OverlayColorRole.Row:
                    return "Even list rows, cards and other-message bubbles.";
                case GP_OverlayColorRole.RowAlt:
                    return "Odd list rows, cards and own-message bubbles.";
                case GP_OverlayColorRole.Button:
                    return "Secondary buttons, unselected chips and status badges.";
                case GP_OverlayColorRole.ButtonSelected:
                    return "Selected group and tab chips.";
                case GP_OverlayColorRole.Accent:
                    return "Primary actions, progress fill and selected chip accent.";
                case GP_OverlayColorRole.Danger:
                    return "Destructive actions such as Kick.";
                case GP_OverlayColorRole.Hover:
                    return "Button hover tint. Select this row to preview it on buttons.";
                case GP_OverlayColorRole.Pressed:
                    return "Primary button press tint. Select this row to preview it on buttons.";
                case GP_OverlayColorRole.Disabled:
                    return "Disabled button tint. Select this row to preview it on buttons.";
                case GP_OverlayColorRole.Text:
                    return "Primary labels and icon glyphs.";
                case GP_OverlayColorRole.TextMuted:
                    return "Captions, placeholders and secondary labels.";
                default:
                    return "";
            }
        }

        internal static GP_OverlayColorRole SlotFromProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return GP_OverlayColorRole.None;
            return System.Enum.TryParse(propertyName, true, out GP_OverlayColorRole slot)
                ? slot
                : GP_OverlayColorRole.None;
        }

        internal static bool TryMatch(GP_OverlaySkin skin, Color color, out GP_OverlayColorRole slot) =>
            GP_OverlayTone.TryMatch(skin, color, out slot);

        internal static bool Approximately(Color left, Color right)
        {
            var dr = left.r - right.r;
            var dg = left.g - right.g;
            var db = left.b - right.b;
            var da = left.a - right.a;
            return dr * dr + dg * dg + db * db + da * da <= 0.00025f;
        }
    }
}
