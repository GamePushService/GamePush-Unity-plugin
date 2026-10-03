using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays
{
    public enum GP_OverlayColorRole
    {
        None,
        Backdrop,
        Panel,
        Header,
        Sidebar,
        Row,
        RowAlt,
        Input,
        Border,
        Button,
        ButtonSelected,
        Hover,
        Pressed,
        Disabled,
        Accent,
        Danger,
        Text,
        TextMuted
    }

    /// <summary>
    /// Palette slot on a graphic. Preview highlight and live/undo retints both use this,
    /// never a guessed Image.color match.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GP_OverlayTone : MonoBehaviour
    {
        public GP_OverlayColorRole role;

        public static readonly GP_OverlayColorRole[] Roles =
        {
            GP_OverlayColorRole.Backdrop,
            GP_OverlayColorRole.Panel,
            GP_OverlayColorRole.Header,
            GP_OverlayColorRole.Row,
            GP_OverlayColorRole.RowAlt,
            GP_OverlayColorRole.Input,
            GP_OverlayColorRole.Border,
            GP_OverlayColorRole.Hover,
            GP_OverlayColorRole.Pressed,
            GP_OverlayColorRole.Disabled,
            GP_OverlayColorRole.Accent,
            GP_OverlayColorRole.Danger,
            GP_OverlayColorRole.Text,
            GP_OverlayColorRole.TextMuted,
            GP_OverlayColorRole.Sidebar,
            GP_OverlayColorRole.Button,
            GP_OverlayColorRole.ButtonSelected
        };

        public static GP_OverlayColorRole RowRole(int index) =>
            index % 2 == 0 ? GP_OverlayColorRole.Row : GP_OverlayColorRole.RowAlt;

        public static void Bind(GameObject target, GP_OverlayColorRole role)
        {
            if (target == null || role == GP_OverlayColorRole.None)
                return;
            var tone = target.GetComponent<GP_OverlayTone>() ?? target.AddComponent<GP_OverlayTone>();
            tone.role = role;
        }

        public static void Bind(GameObject target, GP_OverlaySkin skin, Color color)
        {
            if (TryMatch(skin, color, out var role))
                Bind(target, role);
        }

        public static void Paint(Graphic graphic, GP_OverlaySkin skin, GP_OverlayColorRole role)
        {
            if (graphic == null || skin == null || role == GP_OverlayColorRole.None)
                return;
            Bind(graphic.gameObject, role);
            ApplyTo(graphic, skin, role);
        }

        public static void Apply(GameObject root, GP_OverlaySkin skin)
        {
            if (root == null || skin == null)
                return;
            Ensure(root);
            var tones = root.GetComponentsInChildren<GP_OverlayTone>(true);
            for (var i = 0; i < tones.Length; i++)
            {
                var tone = tones[i];
                if (tone == null || tone.role == GP_OverlayColorRole.None)
                    continue;
                var graphic = tone.GetComponent<Graphic>();
                if (graphic != null)
                    ApplyTo(graphic, skin, tone.role);
            }

            ApplyChipOutlines(root, skin);
            ApplyAccentRichText(root, skin);
        }

        public static bool TryMatch(GP_OverlaySkin skin, Color color, out GP_OverlayColorRole role)
        {
            role = GP_OverlayColorRole.None;
            if (skin == null)
                return false;
            const float maxDistanceSq = 0.00025f;
            var best = maxDistanceSq;
            for (var i = 0; i < Roles.Length; i++)
            {
                var candidate = Roles[i];
                var distance = DistanceSq(skin.ColorOf(candidate), color);
                if (distance >= best)
                    continue;
                best = distance;
                role = candidate;
            }

            return role != GP_OverlayColorRole.None;
        }

        static void ApplyTo(Graphic graphic, GP_OverlaySkin skin, GP_OverlayColorRole role)
        {
            var color = skin.ColorOf(role);
            var button = graphic.GetComponent<Button>();
            if (button != null && button.transition == Selectable.Transition.ColorTint)
            {
                graphic.color = Color.white;
                button.colors = skin.ButtonColors(color);
                return;
            }

            graphic.color = color;
            if (button != null)
                button.colors = skin.ButtonColors(color);

            var selectable = graphic.GetComponent<Selectable>();
            if (selectable != null && button == null && selectable.transition == Selectable.Transition.ColorTint)
            {
                var block = selectable.colors;
                block.normalColor = Color.white;
                block.highlightedColor = Color.white;
                block.pressedColor = Color.white;
                block.selectedColor = Color.white;
                selectable.colors = block;
            }
        }

        static void ApplyChipOutlines(GameObject root, GP_OverlaySkin skin)
        {
            var chips = root.GetComponentsInChildren<GP_OverlayChip>(true);
            for (var i = 0; i < chips.Length; i++)
            {
                var chip = chips[i];
                if (chip == null || chip.selectedOutline == null)
                    continue;
                chip.selectedOutline.effectColor = skin.accent;
            }
        }

        static void ApplyAccentRichText(GameObject root, GP_OverlaySkin skin)
        {
            var texts = root.GetComponentsInChildren<TMP_Text>(true);
            var hex = skin.AccentHex;
            for (var i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                if (text == null || string.IsNullOrEmpty(text.text))
                    continue;
                if (text.text.IndexOf("<color=#", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                text.text = ReplaceColorTags(text.text, hex);
            }
        }

        static string ReplaceColorTags(string value, string hex)
        {
            const string prefix = "<color=#";
            var start = 0;
            while (start < value.Length)
            {
                var index = value.IndexOf(prefix, start, StringComparison.OrdinalIgnoreCase);
                if (index < 0)
                    return value;
                var digits = index + prefix.Length;
                var end = digits;
                while (end < value.Length && IsHex(value[end]))
                    end++;
                if (end - digits == 6)
                {
                    value = value.Substring(0, digits) + hex + value.Substring(end);
                    start = digits + hex.Length;
                }
                else
                {
                    start = digits + 1;
                }
            }

            return value;
        }

        static bool IsHex(char c) =>
            (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

        static void Ensure(GameObject root)
        {
            var graphics = root.GetComponentsInChildren<Graphic>(true);
            for (var i = 0; i < graphics.Length; i++)
            {
                var graphic = graphics[i];
                if (graphic == null)
                    continue;
                var name = graphic.gameObject.name;
                var existing = graphic.GetComponent<GP_OverlayTone>();
                if (TryChromeRoleFromName(name, out var chrome))
                {
                    if (existing == null || NeedsChromeRemap(existing.role, chrome))
                        Bind(graphic.gameObject, chrome);
                    continue;
                }

                if (existing != null)
                    continue;
                if (!TryRoleFromName(name, out var role))
                    continue;
                Bind(graphic.gameObject, role);
            }
        }

        static bool NeedsChromeRemap(GP_OverlayColorRole current, GP_OverlayColorRole chrome)
        {
            if (current == chrome)
                return false;
            return current == GP_OverlayColorRole.None
                || current == GP_OverlayColorRole.Header
                || current == GP_OverlayColorRole.Row
                || current == GP_OverlayColorRole.RowAlt;
        }

        static bool TryChromeRoleFromName(string name, out GP_OverlayColorRole role)
        {
            role = GP_OverlayColorRole.None;
            name = NormalizeName(name);
            if (string.IsNullOrEmpty(name))
                return false;
            switch (name)
            {
                case "Backdrop":
                    role = GP_OverlayColorRole.Backdrop;
                    return true;
                case "Panel":
                    role = GP_OverlayColorRole.Panel;
                    return true;
                case "Header":
                    role = GP_OverlayColorRole.Header;
                    return true;
                case "WideGroups":
                case "CompactTabs":
                case "Members":
                case "Composer":
                case "ThreadPanel":
                    role = GP_OverlayColorRole.Sidebar;
                    return true;
                case "Close":
                case "Cancel":
                case "Skip":
                case "Back":
                case "GroupButtonTemplate":
                case "MessagesTab":
                case "MembersTab":
                case "Delete":
                case "Mute":
                case "Unlocked":
                case "Locked":
                    role = GP_OverlayColorRole.Button;
                    return true;
                case "Confirm":
                case "Send":
                case "Ok":
                case "New":
                    role = GP_OverlayColorRole.Accent;
                    return true;
                case "Kick":
                    role = GP_OverlayColorRole.Danger;
                    return true;
                case "ProgressBar":
                    role = GP_OverlayColorRole.Input;
                    return true;
                default:
                    return false;
            }
        }

        static bool TryRoleFromName(string name, out GP_OverlayColorRole role)
        {
            if (TryChromeRoleFromName(name, out role))
                return true;
            role = GP_OverlayColorRole.None;
            name = NormalizeName(name);
            if (string.IsNullOrEmpty(name))
                return false;
            switch (name)
            {
                case "Title":
                case "Message":
                case "Text":
                    role = GP_OverlayColorRole.Text;
                    return true;
                case "Status":
                case "Counter":
                case "Subtitle":
                case "Placeholder":
                case "Online":
                    role = GP_OverlayColorRole.TextMuted;
                    return true;
                case "Bubble":
                    role = GP_OverlayColorRole.Row;
                    return true;
                case "SelectedBar":
                case "Fill":
                    role = GP_OverlayColorRole.Accent;
                    return true;
                default:
                    return false;
            }
        }

        static string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            const string clone = "(Clone)";
            if (name.EndsWith(clone))
                name = name.Substring(0, name.Length - clone.Length);
            return name.Trim();
        }

        static float DistanceSq(Color left, Color right)
        {
            var dr = left.r - right.r;
            var dg = left.g - right.g;
            var db = left.b - right.b;
            var da = left.a - right.a;
            return dr * dr + dg * dg + db * db + da * da;
        }
    }
}
