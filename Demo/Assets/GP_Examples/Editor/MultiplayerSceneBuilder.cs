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
        private static readonly Vector2 ContentSize = new(1100f, 860f);
        private static readonly Vector2 ConsolePosition = new(0f, -435f);
        private static readonly Vector2 ConsoleSize = new(1100f, 280f);
        private static readonly Vector2 ButtonsPosition = new(0f, -90f);
        private static readonly Vector2 ButtonsSize = new(820f, 300f);
        private static readonly Vector2 ButtonCellSize = new(185f, 50f);
        private static readonly Vector2 ButtonSpacing = new(15f, 10f);
        private static readonly Vector2 FieldsPosition = new(0f, -405f);
        private static readonly Vector2 FieldsSize = new(830f, 360f);
        private static readonly Vector2 FieldCellSize = new(395f, 78f);
        private static readonly Vector2 FieldSpacing = new(18f, 10f);

        [MenuItem("GamePush/Examples/Build Multiplayer Scene")]
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

            RectTransform buttonsContainer = CreateButtonsContainer(content);
            RectTransform fieldsContainer = CreateFieldsContainer(content);

            Button connectButton = CreateActionButton(buttonTemplate, buttonsContainer, "CONNECT");
            Button disconnectButton = CreateActionButton(buttonTemplate, buttonsContainer, "DISCONNECT");
            Button fastModeButton = CreateActionButton(buttonTemplate, buttonsContainer, "FAST");
            Button smoothModeButton = CreateActionButton(buttonTemplate, buttonsContainer, "SMOOTH");
            Button defineSchemaButton = CreateActionButton(buttonTemplate, buttonsContainer, "DEFINE SCHEMA");
            Button enableInitializerButton = CreateActionButton(buttonTemplate, buttonsContainer, "INIT SYNC");
            Button enableAsyncInitializerButton = CreateActionButton(buttonTemplate, buttonsContainer, "INIT ASYNC");
            Button disableInitializerButton = CreateActionButton(buttonTemplate, buttonsContainer, "CLEAR INIT");
            Button setPlayerStateButton = CreateActionButton(buttonTemplate, buttonsContainer, "SET STATE");
            Button sendMessageButton = CreateActionButton(buttonTemplate, buttonsContainer, "SEND MESSAGE");
            Button readTickRateButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ TICK");
            Button readIsConnectedButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ CONNECTED");
            Button readIsHostButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ HOST");
            Button readMyStateButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ MY STATE");
            Button readPlayersStateButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ PLAYERS");
            Button readConnectedPlayersButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ PEERS");
            Button readNetworkStatsButton = CreateActionButton(buttonTemplate, buttonsContainer, "READ NETWORK");

            TMP_InputField channelIdInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "CHANNEL ID", "e.g. 38777");
            TMP_InputField schemaJsonInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "SCHEMA JSON", "{\"score\":\"number\"}");
            TMP_InputField initializerStateJsonInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "INITIALIZER JSON", "{\"score\":0}");
            TMP_InputField stateJsonInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "STATE JSON", "{\"score\":1}");
            TMP_InputField eventNameInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "EVENT NAME", "demo:message");
            TMP_InputField messageJsonInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "MESSAGE JSON", "{\"text\":\"hello\"}");
            TMP_InputField targetInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "TARGET", "all");
            TMP_InputField echoInput = CreateInputField(fieldTemplate, labelTemplate, fieldsContainer, "ECHO", "true / false");

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

        private static RectTransform CreateButtonsContainer(Transform parent)
        {
            GameObject container = new GameObject("Buttons", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            container.transform.SetParent(parent, false);

            RectTransform rectTransform = (RectTransform)container.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = ButtonsPosition;
            rectTransform.sizeDelta = ButtonsSize;

            GridLayoutGroup grid = container.GetComponent<GridLayoutGroup>();
            grid.cellSize = ButtonCellSize;
            grid.spacing = ButtonSpacing;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.childAlignment = TextAnchor.UpperCenter;

            ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            return rectTransform;
        }

        private static RectTransform CreateFieldsContainer(Transform parent)
        {
            GameObject container = new GameObject("Fields", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            container.transform.SetParent(parent, false);

            RectTransform rectTransform = (RectTransform)container.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = FieldsPosition;
            rectTransform.sizeDelta = FieldsSize;

            GridLayoutGroup grid = container.GetComponent<GridLayoutGroup>();
            grid.cellSize = FieldCellSize;
            grid.spacing = FieldSpacing;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.childAlignment = TextAnchor.UpperCenter;

            ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            return rectTransform;
        }

        private static Button CreateActionButton(GameObject template, Transform parent, string label)
        {
            GameObject button = UnityEngine.Object.Instantiate(template, parent);
            button.name = label;
            button.SetActive(true);
            SetFirstText(button, label);

            LayoutElement layoutElement = button.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = button.AddComponent<LayoutElement>();

            layoutElement.preferredWidth = 185f;
            layoutElement.preferredHeight = 52f;

            Button component = button.GetComponent<Button>();
            if (component == null)
                throw new InvalidOperationException("Multiplayer builder: template action button does not contain Button.");

            component.onClick = new Button.ButtonClickedEvent();

            TMP_Text text = button.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(candidate => candidate.gameObject.name != "Placeholder");
            if (text != null)
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = 18f;
                text.fontSizeMax = 44f;
                text.overflowMode = TextOverflowModes.Ellipsis;
            }

            return component;
        }

        private static TMP_InputField CreateInputField(GameObject template, TMP_Text labelTemplate, Transform parent, string label, string placeholder)
        {
            GameObject fieldGroup = new GameObject(
                label,
                typeof(RectTransform),
                typeof(LayoutElement),
                typeof(VerticalLayoutGroup));
            fieldGroup.transform.SetParent(parent, false);

            LayoutElement layoutElement = fieldGroup.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = 395f;
            layoutElement.preferredHeight = 78f;

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
            labelRect.sizeDelta = new Vector2(395f, 22f);

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
            inputRect.sizeDelta = new Vector2(395f, 46f);

            LayoutElement inputLayout = inputRoot.GetComponent<LayoutElement>();
            if (inputLayout == null)
                inputLayout = inputRoot.AddComponent<LayoutElement>();

            inputLayout.preferredWidth = 395f;
            inputLayout.preferredHeight = 46f;

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
    }
}
