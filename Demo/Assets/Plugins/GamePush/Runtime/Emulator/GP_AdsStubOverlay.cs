#if UNITY_EDITOR
using UnityEngine;

namespace GamePush
{
    public class GP_AdsStubOverlay : MonoBehaviour
    {
        private Texture2D _pixel;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _buttonStyle;

        private void OnEnable()
        {
            _pixel = new Texture2D(1, 1);
            _pixel.SetPixel(0, 0, Color.white);
            _pixel.Apply();
        }

        private void OnDisable()
        {
            if (_pixel != null)
                Destroy(_pixel);
            _pixel = null;
        }

        private void OnGUI()
        {
            if (Application.isBatchMode || !GP_AdsStub.IsPlaying)
                return;

            EnsureStyles();

            GUI.depth = -1000;
            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.82f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _pixel);
            GUI.color = previous;

            const float panelWidth = 420f;
            const float panelHeight = 220f;
            var panel = new Rect(
                (Screen.width - panelWidth) * 0.5f,
                (Screen.height - panelHeight) * 0.5f,
                panelWidth,
                panelHeight);

            GUI.color = new Color(0.12f, 0.12f, 0.14f, 0.96f);
            GUI.DrawTexture(panel, _pixel);
            GUI.color = Color.white;

            GUILayout.BeginArea(new Rect(panel.x + 20f, panel.y + 16f, panel.width - 40f, panel.height - 32f));
            GUILayout.Label(Title(), _titleStyle);
            GUILayout.Space(8f);
            GUILayout.Label(Body(), _bodyStyle);
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GP_AdsStub.Kind == GP_AdsStubKind.Rewarded)
            {
                if (GUILayout.Button("Complete (reward)", _buttonStyle, GUILayout.Height(40f)))
                    GP_AdsStub.Complete(true);
                if (GUILayout.Button("Fail / Skip", _buttonStyle, GUILayout.Height(40f)))
                    GP_AdsStub.Complete(false);
            }
            else
            {
                if (GUILayout.Button("Close (success)", _buttonStyle, GUILayout.Height(40f)))
                    GP_AdsStub.Complete(true);
                if (GUILayout.Button("Close (fail)", _buttonStyle, GUILayout.Height(40f)))
                    GP_AdsStub.Complete(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static string Title()
        {
            switch (GP_AdsStub.Kind)
            {
                case GP_AdsStubKind.Preloader:
                    return "AD STUB: PRELOADER";
                case GP_AdsStubKind.Fullscreen:
                    return "AD STUB: FULLSCREEN";
                case GP_AdsStubKind.Rewarded:
                    return "AD STUB: REWARDED";
                default:
                    return "AD STUB";
            }
        }

        private static string Body()
        {
            if (GP_AdsStub.Kind == GP_AdsStubKind.Rewarded)
                return $"Tag: {GP_AdsStub.RewardedTag}\nComplete grants the reward. Fail does not.";

            return "Close with success or fail to exercise OnClose callbacks.";
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            _titleStyle.normal.textColor = Color.white;

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            _bodyStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
#endif
