using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MultiplayerExample = Examples.Multiplayer.Multiplayer;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Examples.EditorTools
{
    public static class MultiplayerSceneBuilder
    {
        private const string ExamplesScenePath = "Assets/GP_Examples/Scenes/ExamplesScene.unity";
        private const string ModuleName = "Multiplayer";
        private const string MenuButtonName = "MULTIPLAYER";
        private const string TemplateRootName = "Variables";
        private const string TemplateMenuButtonName = "VARIABLES";
        private static readonly Vector2 ContentPosition = new(0f, 450f);
        private static readonly Vector2 ContentSize = new(1100f, 1220f);
        private static readonly Vector2 ConsolePosition = new(0f, -930f);
        private static readonly Vector2 ConsoleSize = new(1100f, 280f);
        private static readonly Color CardColor = new(0.97f, 0.97f, 0.97f, 1f);
        private static readonly Color HeaderColor = new(0.27f, 0.78f, 0.24f, 1f);

        public static void Build()
        {
            Scene scene = EditorSceneManager.OpenScene(ExamplesScenePath, OpenSceneMode.Single);

            GameObject templateRoot = FindByName(scene, TemplateRootName);
            GameObject templateMenuButton = FindByName(scene, TemplateMenuButtonName);

            if (templateRoot == null || templateMenuButton == null)
                throw new InvalidOperationException("Multiplayer builder: template objects were not found in ExamplesScene.");

            DeleteIfExists(scene, ModuleName);
            DeleteIfExists(scene, MenuButtonName);

            GameObject multiplayerRoot = UnityEngine.Object.Instantiate(templateRoot, templateRoot.transform.parent);
            multiplayerRoot.name = ModuleName;
            multiplayerRoot.SetActive(false);

            GameObject multiplayerMenuButton = UnityEngine.Object.Instantiate(templateMenuButton, templateMenuButton.transform.parent);
            multiplayerMenuButton.name = MenuButtonName;

            ReplacePersistentTarget(multiplayerMenuButton, templateRoot, multiplayerRoot, "Multiplayer Scene");
            ReplacePersistentTarget(
                FindDescendant(multiplayerRoot.transform, "Main Menu Button")?.gameObject,
                templateRoot,
                multiplayerRoot,
                "Main Menu");

            SetFirstText(multiplayerMenuButton, MenuButtonName);
            PositionMenuButton((RectTransform)multiplayerMenuButton.transform);

            MultiplayerExample multiplayer = PrepareModule(multiplayerRoot, templateRoot);
            ConfigureSerializedReferences(multiplayer);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Multiplayer example scene has been built successfully.");
        }

        private static MultiplayerExample PrepareModule(GameObject multiplayerRoot, GameObject templateRoot)
        {
            foreach (MonoBehaviour component in multiplayerRoot.GetComponents<MonoBehaviour>())
            {
                if (component != null)
                    UnityEngine.Object.DestroyImmediate(component);
            }

            MultiplayerExample multiplayer = multiplayerRoot.AddComponent<MultiplayerExample>();

            CreateBackground(multiplayerRoot.transform);

            Transform content = FindDescendant(multiplayerRoot.transform, "Content");
            if (content == null)
                throw new InvalidOperationException("Multiplayer builder: duplicated module root does not contain Content.");

            Transform console = FindDescendant(multiplayerRoot.transform, "Console");
            TuneModuleLayout((RectTransform)content, console as RectTransform);

            Transform titleImage = FindDescendant(content, "Title Image");
            List<Transform> childrenToRemove = new List<Transform>();
            for (int index = 0; index < content.childCount; index++)
            {
                Transform child = content.GetChild(index);
                if (child != titleImage)
                    childrenToRemove.Add(child);
            }

            foreach (Transform child in childrenToRemove)
                UnityEngine.Object.DestroyImmediate(child.gameObject);

            if (titleImage != null)
                SetFirstText(titleImage.gameObject, MenuButtonName);

            GameObject buttonTemplate = FindButtonTemplate(templateRoot);
            GameObject fieldTemplate = FindFieldTemplate(templateRoot);
            TMP_Text labelTemplate = FindLabelTemplate(buttonTemplate);

            if (buttonTemplate == null || fieldTemplate == null || labelTemplate == null)
                throw new InvalidOperationException("Multiplayer builder: failed to locate button, label, or field template.");

            CardLayout sessionCard = CreateCard(content, labelTemplate, "SESSION", new Vector2(-285f, -88f), new Vector2(430f, 245f));
            TMP_InputField channelIdInput = CreateInputField(fieldTemplate, labelTemplate, sessionCard.Body, "CHANNEL ID", "e.g. 38777", 360f);
            RectTransform sessionButtonsRow = CreateHorizontalRow(sessionCard.Body, 10f);
            Button connectButton = CreateActionButton(buttonTemplate, sessionButtonsRow, "CONNECT", 145f);
            Button disconnectButton = CreateActionButton(buttonTemplate, sessionButtonsRow, "DISCONNECT", 145f);
            RectTransform sessionModeRow = CreateHorizontalRow(sessionCard.Body, 10f);
            Button fastModeButton = CreateActionButton(buttonTemplate, sessionModeRow, "FAST", 145f);
            Button smoothModeButton = CreateActionButton(buttonTemplate, sessionModeRow, "SMOOTH", 145f);

            CardLayout schemaCard = CreateCard(content, labelTemplate, "SCHEMA & INITIALIZER", new Vector2(285f, -88f), new Vector2(470f, 295f));
            TMP_InputField schemaJsonInput = CreateInputField(fieldTemplate, labelTemplate, schemaCard.Body, "SCHEMA JSON", "{\"score\":\"number\"}", 400f);
            Button defineSchemaButton = CreateActionButton(buttonTemplate, schemaCard.Body, "DEFINE SCHEMA", 200f);
            TMP_InputField initializerStateJsonInput = CreateInputField(fieldTemplate, labelTemplate, schemaCard.Body, "INITIALIZER JSON", "{\"score\":0}", 400f);
            RectTransform initializerButtonsRow = CreateHorizontalRow(schemaCard.Body, 10f);
            Button enableInitializerButton = CreateActionButton(buttonTemplate, initializerButtonsRow, "INIT SYNC", 120f);
            Button enableAsyncInitializerButton = CreateActionButton(buttonTemplate, initializerButtonsRow, "INIT ASYNC", 120f);
            Button disableInitializerButton = CreateActionButton(buttonTemplate, initializerButtonsRow, "CLEAR INIT", 120f);

            CardLayout stateCard = CreateCard(content, labelTemplate, "PLAYER STATE", new Vector2(-285f, -425f), new Vector2(430f, 185f));
            TMP_InputField stateJsonInput = CreateInputField(fieldTemplate, labelTemplate, stateCard.Body, "STATE JSON", "{\"score\":1}", 360f);
            Button setPlayerStateButton = CreateActionButton(buttonTemplate, stateCard.Body, "SET STATE", 190f);

            CardLayout messageCard = CreateCard(content, labelTemplate, "MESSAGING", new Vector2(285f, -455f), new Vector2(470f, 350f));
            TMP_InputField eventNameInput = CreateInputField(fieldTemplate, labelTemplate, messageCard.Body, "EVENT NAME", "demo:message", 400f);
            TMP_InputField messageJsonInput = CreateInputField(fieldTemplate, labelTemplate, messageCard.Body, "MESSAGE JSON", "{\"text\":\"hello\"}", 400f);
            RectTransform messageMetaRow = CreateHorizontalRow(messageCard.Body, 12f);
            TMP_InputField targetInput = CreateInputField(fieldTemplate, labelTemplate, messageMetaRow, "TARGET", "all", 194f);
            TMP_InputField echoInput = CreateInputField(fieldTemplate, labelTemplate, messageMetaRow, "ECHO", "true / false", 194f);
            Button sendMessageButton = CreateActionButton(buttonTemplate, messageCard.Body, "SEND MESSAGE", 190f);

            CardLayout runtimeCard = CreateCard(content, labelTemplate, "RUNTIME INFO", new Vector2(0f, -770f), new Vector2(1030f, 200f));
            RectTransform diagnosticsTopRow = CreateHorizontalRow(runtimeCard.Body, 12f);
            Button readTickRateButton = CreateActionButton(buttonTemplate, diagnosticsTopRow, "READ TICK", 132f);
            Button readIsConnectedButton = CreateActionButton(buttonTemplate, diagnosticsTopRow, "READ CONNECTED", 132f);
            Button readIsHostButton = CreateActionButton(buttonTemplate, diagnosticsTopRow, "READ HOST", 132f);
            Button readMyStateButton = CreateActionButton(buttonTemplate, diagnosticsTopRow, "READ MY STATE", 132f);
            RectTransform diagnosticsBottomRow = CreateHorizontalRow(runtimeCard.Body, 12f);
            Button readPlayersStateButton = CreateActionButton(buttonTemplate, diagnosticsBottomRow, "READ PLAYERS", 155f);
            Button readConnectedPlayersButton = CreateActionButton(buttonTemplate, diagnosticsBottomRow, "READ PEERS", 155f);
            Button readNetworkStatsButton = CreateActionButton(buttonTemplate, diagnosticsBottomRow, "READ NETWORK", 155f);

            SerializedObject serializedObject = new SerializedObject(multiplayer);
            SetReference(serializedObject, "_channelIdInput", channelIdInput);
            SetReference(serializedObject, "_schemaJsonInput", schemaJsonInput);
            SetReference(serializedObject, "_initializerStateJsonInput", initializerStateJsonInput);
            SetReference(serializedObject, "_stateJsonInput", stateJsonInput);
            SetReference(serializedObject, "_eventNameInput", eventNameInput);
            SetReference(serializedObject, "_messageJsonInput", messageJsonInput);
            SetReference(serializedObject, "_targetInput", targetInput);
            SetReference(serializedObject, "_echoInput", echoInput);
            SetReference(serializedObject, "_connectButton", connectButton);
            SetReference(serializedObject, "_disconnectButton", disconnectButton);
            SetReference(serializedObject, "_fastModeButton", fastModeButton);
            SetReference(serializedObject, "_smoothModeButton", smoothModeButton);
            SetReference(serializedObject, "_defineSchemaButton", defineSchemaButton);
            SetReference(serializedObject, "_enableInitializerButton", enableInitializerButton);
            SetReference(serializedObject, "_enableAsyncInitializerButton", enableAsyncInitializerButton);
            SetReference(serializedObject, "_disableInitializerButton", disableInitializerButton);
            SetReference(serializedObject, "_setPlayerStateButton", setPlayerStateButton);
            SetReference(serializedObject, "_sendMessageButton", sendMessageButton);
            SetReference(serializedObject, "_readTickRateButton", readTickRateButton);
            SetReference(serializedObject, "_readIsConnectedButton", readIsConnectedButton);
            SetReference(serializedObject, "_readIsHostButton", readIsHostButton);
            SetReference(serializedObject, "_readMyStateButton", readMyStateButton);
            SetReference(serializedObject, "_readPlayersStateButton", readPlayersStateButton);
            SetReference(serializedObject, "_readConnectedPlayersButton", readConnectedPlayersButton);
            SetReference(serializedObject, "_readNetworkStatsButton", readNetworkStatsButton);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return multiplayer;
        }

        private static void ConfigureSerializedReferences(MultiplayerExample multiplayer)
        {
            EditorUtility.SetDirty(multiplayer);
            EditorUtility.SetDirty(multiplayer.gameObject);
        }

        private static CardLayout CreateCard(Transform parent, TMP_Text labelTemplate, string title, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject card = new GameObject(title, typeof(RectTransform), typeof(Image));
            card.transform.SetParent(parent, false);

            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.anchorMin = new Vector2(0.5f, 1f);
            cardRect.anchorMax = new Vector2(0.5f, 1f);
            cardRect.pivot = new Vector2(0.5f, 1f);
            cardRect.anchoredPosition = anchoredPosition;
            cardRect.sizeDelta = size;

            Image cardImage = card.GetComponent<Image>();
            cardImage.color = CardColor;
            cardImage.raycastTarget = false;

            GameObject header = new GameObject("Header", typeof(RectTransform), typeof(Image));
            header.transform.SetParent(card.transform, false);
            RectTransform headerRect = (RectTransform)header.transform;
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0f, 54f);

            Image headerImage = header.GetComponent<Image>();
            headerImage.color = HeaderColor;
            headerImage.raycastTarget = false;

            TMP_Text titleText = UnityEngine.Object.Instantiate(labelTemplate, header.transform);
            titleText.name = "Title";
            titleText.text = title;
            titleText.fontSize = 26f;
            titleText.enableAutoSizing = true;
            titleText.fontSizeMin = 20f;
            titleText.fontSizeMax = 30f;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;

            RectTransform titleRect = (RectTransform)titleText.transform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(size.x - 40f, 34f);

            GameObject body = new GameObject("Body", typeof(RectTransform), typeof(LayoutElement), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            body.transform.SetParent(card.transform, false);

            RectTransform bodyRect = (RectTransform)body.transform;
            bodyRect.anchorMin = new Vector2(0.5f, 1f);
            bodyRect.anchorMax = new Vector2(0.5f, 1f);
            bodyRect.pivot = new Vector2(0.5f, 1f);
            bodyRect.anchoredPosition = new Vector2(0f, -72f);
            bodyRect.sizeDelta = new Vector2(size.x - 36f, size.y - 92f);

            LayoutElement bodyLayout = body.GetComponent<LayoutElement>();
            bodyLayout.preferredWidth = size.x - 36f;

            VerticalLayoutGroup bodyGroup = body.GetComponent<VerticalLayoutGroup>();
            bodyGroup.spacing = 10f;
            bodyGroup.childAlignment = TextAnchor.UpperCenter;
            bodyGroup.childControlWidth = false;
            bodyGroup.childControlHeight = false;
            bodyGroup.childForceExpandWidth = false;
            bodyGroup.childForceExpandHeight = false;

            ContentSizeFitter bodyFitter = body.GetComponent<ContentSizeFitter>();
            bodyFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            bodyFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            return new CardLayout
            {
                Root = cardRect,
                Body = (RectTransform)body.transform
            };
        }

        private static RectTransform CreateHorizontalRow(Transform parent, float spacing)
        {
            GameObject row = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
            row.transform.SetParent(parent, false);

            HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = row.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return (RectTransform)row.transform;
        }

        private static RectTransform CreateVerticalColumn(Transform parent, float spacing)
        {
            GameObject column = new GameObject("Column", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            column.transform.SetParent(parent, false);

            VerticalLayoutGroup layout = column.GetComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = column.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return (RectTransform)column.transform;
        }

        private static Button CreateActionButton(GameObject template, Transform parent, string label, float width)
        {
            GameObject button = UnityEngine.Object.Instantiate(template, parent);
            button.name = label;
            button.SetActive(true);
            SetFirstText(button, label);

            RectTransform rectTransform = (RectTransform)button.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = new Vector2(width, 38f);

            LayoutElement layoutElement = button.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = button.AddComponent<LayoutElement>();

            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = 38f;

            Button component = button.GetComponent<Button>();
            if (component == null)
                throw new InvalidOperationException("Multiplayer builder: template action button does not contain Button.");

            component.onClick = new Button.ButtonClickedEvent();

            TMP_Text text = button.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(candidate => candidate.gameObject.name != "Placeholder");
            if (text != null)
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = 14f;
                text.fontSizeMax = 32f;
                text.overflowMode = TextOverflowModes.Ellipsis;
            }

            return component;
        }

        private static TMP_InputField CreateInputField(
            GameObject template,
            TMP_Text labelTemplate,
            Transform parent,
            string label,
            string placeholder,
            float width)
        {
            GameObject fieldGroup = new GameObject(
                label,
                typeof(RectTransform),
                typeof(LayoutElement),
                typeof(VerticalLayoutGroup));
            fieldGroup.transform.SetParent(parent, false);

            LayoutElement layoutElement = fieldGroup.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = 68f;

            VerticalLayoutGroup verticalLayout = fieldGroup.GetComponent<VerticalLayoutGroup>();
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.childControlHeight = false;
            verticalLayout.childControlWidth = false;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.spacing = 4f;
            verticalLayout.padding = new RectOffset(0, 0, 0, 0);

            TMP_Text labelText = UnityEngine.Object.Instantiate(labelTemplate, fieldGroup.transform);
            labelText.name = "Label";
            labelText.text = label;
            labelText.enableAutoSizing = true;
            labelText.fontSizeMin = 16f;
            labelText.fontSizeMax = 22f;
            labelText.fontSize = 20f;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.overflowMode = TextOverflowModes.Ellipsis;
            labelText.raycastTarget = false;

            RectTransform labelRect = (RectTransform)labelText.transform;
            labelRect.anchorMin = new Vector2(0.5f, 1f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.pivot = new Vector2(0.5f, 1f);
            labelRect.sizeDelta = new Vector2(width, 22f);

            GameObject inputRoot = UnityEngine.Object.Instantiate(template, fieldGroup.transform);
            inputRoot.name = "InputField";
            inputRoot.SetActive(true);

            TMP_InputField inputField = inputRoot.GetComponent<TMP_InputField>();
            if (inputField == null)
                throw new InvalidOperationException("Multiplayer builder: template field does not contain TMP_InputField.");

            RectTransform inputRect = (RectTransform)inputRoot.transform;
            inputRect.anchorMin = new Vector2(0.5f, 1f);
            inputRect.anchorMax = new Vector2(0.5f, 1f);
            inputRect.pivot = new Vector2(0.5f, 1f);
            inputRect.sizeDelta = new Vector2(width, 44f);
            inputRect.anchoredPosition = Vector2.zero;
            inputRect.localScale = Vector3.one;

            LayoutElement inputLayout = inputRoot.GetComponent<LayoutElement>();
            if (inputLayout == null)
                inputLayout = inputRoot.AddComponent<LayoutElement>();

            inputLayout.preferredWidth = width;
            inputLayout.preferredHeight = 44f;

            if (inputField.placeholder is TMP_Text placeholderText)
                placeholderText.text = placeholder;

            inputField.text = string.Empty;
            return inputField;
        }

        private static void TuneModuleLayout(RectTransform content, RectTransform console)
        {
            if (content != null)
            {
                content.anchoredPosition = ContentPosition;
                content.sizeDelta = ContentSize;
            }

            if (console != null)
            {
                console.anchoredPosition = ConsolePosition;
                console.sizeDelta = ConsoleSize;
            }
        }

        private static void CreateBackground(Transform parent)
        {
            Transform existing = FindDescendant(parent, "Background");
            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing.gameObject);

            GameObject background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            background.transform.SetParent(parent, false);
            background.transform.SetSiblingIndex(0);

            RectTransform rectTransform = (RectTransform)background.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Image image = background.GetComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            image.raycastTarget = false;
        }

        private static GameObject FindButtonTemplate(GameObject root)
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button == null || button.gameObject.name == "Main Menu Button")
                    continue;

                if (FindAncestor(button.transform, "Console") != null)
                    continue;

                return button.gameObject;
            }

            return null;
        }

        private static GameObject FindFieldTemplate(GameObject root)
        {
            foreach (TMP_InputField inputField in root.GetComponentsInChildren<TMP_InputField>(true))
            {
                if (inputField == null)
                    continue;

                Transform group = inputField.transform.parent;
                if (group == null || FindAncestor(group, "Console") != null)
                    continue;

                return inputField.gameObject;
            }

            return null;
        }

        private static TMP_Text FindLabelTemplate(GameObject buttonTemplate)
        {
            return buttonTemplate.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(candidate => candidate.gameObject.name != "Placeholder");
        }

        private static Transform FindAncestor(Transform transform, string name)
        {
            Transform current = transform;
            while (current != null)
            {
                if (current.name == name)
                    return current;

                current = current.parent;
            }

            return null;
        }

        private static void PositionMenuButton(RectTransform menuButton)
        {
            RectTransform parent = menuButton.parent as RectTransform;
            if (parent == null)
                return;

            List<RectTransform> buttons = parent
                .Cast<Transform>()
                .Select(child => child as RectTransform)
                .Where(rect => rect != null && rect.GetComponent<Button>() != null && rect.gameObject != menuButton.gameObject)
                .ToList();

            if (buttons.Count == 0)
                return;

            List<float> columns = buttons
                .Select(rect => rect.anchoredPosition.x)
                .Distinct(new FloatToleranceComparer())
                .OrderBy(value => value)
                .ToList();

            List<float> rows = buttons
                .Select(rect => rect.anchoredPosition.y)
                .Distinct(new FloatToleranceComparer())
                .OrderByDescending(value => value)
                .ToList();

            if (columns.Count == 0 || rows.Count == 0)
                return;

            float rowSpacing = rows.Count > 1 ? Mathf.Abs(rows[0] - rows[1]) : 115f;
            int index = buttons.Count;
            int columnIndex = index % columns.Count;
            int rowIndex = index / columns.Count;

            float x = columns[columnIndex];
            float y = rowIndex < rows.Count ? rows[rowIndex] : rows.Last() - rowSpacing * (rowIndex - rows.Count + 1);
            menuButton.anchoredPosition = new Vector2(x, y);
        }

        private static void ReplacePersistentTarget(GameObject host, GameObject oldTarget, GameObject newTarget, string stringArgument)
        {
            if (host == null || oldTarget == null || newTarget == null)
                return;

            Button button = host.GetComponent<Button>();
            if (button == null)
                return;

            SerializedObject serializedObject = new SerializedObject(button);
            SerializedProperty calls = serializedObject.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");

            for (int index = 0; index < calls.arraySize; index++)
            {
                SerializedProperty call = calls.GetArrayElementAtIndex(index);
                SerializedProperty targetProperty = call.FindPropertyRelative("m_Target");
                SerializedProperty argumentsProperty = call.FindPropertyRelative("m_Arguments");
                SerializedProperty stringArgumentProperty = argumentsProperty?.FindPropertyRelative("m_StringArgument");

                if (targetProperty.objectReferenceValue == oldTarget)
                {
                    targetProperty.objectReferenceValue = newTarget;
                    if (stringArgumentProperty != null)
                        stringArgumentProperty.stringValue = stringArgument;
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void DeleteIfExists(Scene scene, string name)
        {
            GameObject existing = FindByName(scene, name);
            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing);
        }

        private static void SetReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.objectReferenceValue = value;
        }

        private static void SetFirstText(GameObject root, string value)
        {
            TMP_Text text = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(candidate => candidate.gameObject.name != "Placeholder");

            if (text != null)
                text.text = value;
        }

        private static GameObject FindByName(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform found = FindDescendant(root.transform, name);
                if (found != null)
                    return found.gameObject;
            }

            return null;
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            if (root.name == name)
                return root;

            for (int index = 0; index < root.childCount; index++)
            {
                Transform found = FindDescendant(root.GetChild(index), name);
                if (found != null)
                    return found;
            }

            return null;
        }

        private sealed class FloatToleranceComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y)
            {
                return Mathf.Abs(x - y) < 0.5f;
            }

            public int GetHashCode(float obj)
            {
                return Mathf.RoundToInt(obj).GetHashCode();
            }
        }

        private sealed class CardLayout
        {
            public RectTransform Root { get; set; }
            public RectTransform Body { get; set; }
        }
    }
}
