#if UNITY_EDITOR
using UnityEngine;

namespace GamePush
{
    public class GP_PaymentsStubOverlay : MonoBehaviour
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
            if (Application.isBatchMode || !GP_PaymentsStubSession.IsOpen)
                return;

            EnsureStyles();

            GUI.depth = -1000;
            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.82f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _pixel);
            GUI.color = previous;

            const float panelWidth = 440f;
            const float panelHeight = 260f;
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
            string confirm = GP_PaymentsStubSession.Kind == GP_PaymentsStubKind.Subscribe ? "Subscribe" : "Buy";
            if (GUILayout.Button(confirm, _buttonStyle, GUILayout.Height(40f)))
                GP_PaymentsStubSession.Confirm();
            if (GUILayout.Button("Cancel", _buttonStyle, GUILayout.Height(40f)))
                GP_PaymentsStubSession.Cancel();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static string Title()
        {
            return GP_PaymentsStubSession.Kind == GP_PaymentsStubKind.Subscribe
                ? "PAYMENT STUB: SUBSCRIBE"
                : "PAYMENT STUB: PURCHASE";
        }

        private static string Body()
        {
            FetchProducts product = GP_PaymentsStubSession.Product;
            string name = product != null && !string.IsNullOrEmpty(product.name)
                ? product.name
                : GP_PaymentsStubSession.IdOrTag;
            string tag = product != null && !string.IsNullOrEmpty(product.tag)
                ? product.tag
                : GP_PaymentsStubSession.IdOrTag;
            string description = product != null ? product.description : string.Empty;
            string price = product != null
                ? $"{product.price} {product.currencySymbol ?? product.currency}"
                : string.Empty;

            return $"Name: {name}\nTag: {tag}\nPrice: {price}\n{description}";
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
