using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.UIBuilder;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
    public class UserAttributesUI : MonoBehaviour
    {
		[SerializeField] WidgetProvider AttributePrefabProvider = new WidgetProvider();
		[Space]
		[SerializeField] Transform ReadOnlyAttributesRoot = default;
		[SerializeField] private GameObject[] ReadOnlyAttributesFull = default;
		[SerializeField] private GameObject[] ReadOnlyAttributesEmpty = default;
		[SerializeField] private GameObject ReadOnlyAttributesTab = default;
		[Space]
		[SerializeField] Transform CustomAttributesRoot = default;
		[SerializeField] private GameObject[] CustomAttributesFull = default;
		[SerializeField] private GameObject[] CustomAttributesEmpty = default;
		[SerializeField] private GameObject CustomAttributesTab = default;
		[Space]
		[SerializeField] SimpleButton ShowReadonlyButton = default;
		[SerializeField] SimpleButton ShowCustomButton = default;
		[Space]
		[SerializeField] SimpleButton NewButton = default;
		[SerializeField] SimpleButton SaveButton = default;
		[Space]
		[SerializeField] private RectTransform[] Layouts = default;

		private const string DEFAULT_KEY = "CustomAttribute";
		private const string DEFAULT_VALUE = "Custom value";

		private int _initFlag = 0;
		private bool _isInitialized;
		private bool _isAlive;

		private void Awake()
		{
			_isAlive = true;

			ShowReadonlyButton.onClick += () => Show(readOnly: true);
			ShowCustomButton.onClick += () => Show(readOnly: false);

			NewButton.onClick += AddNewAttribute;
			SaveButton.onClick += SaveAttributes;

			AttributeItemUI.OnKeyChanged += HandleKeyChanged;
			AttributeItemUI.OnValueChanged += HandleValueChanged;
			AttributeItemUI.OnRemoveRequest += HandleRemoveRequest;
		}

		private void OnDestroy()
		{
			_isAlive = false;
			AttributeItemUI.OnKeyChanged -= HandleKeyChanged;
			AttributeItemUI.OnValueChanged -= HandleValueChanged;
			AttributeItemUI.OnRemoveRequest -= HandleRemoveRequest;

			if (gameObject.scene.isLoaded)
				UserAttributes.Instance.RevertChanges();
		}

		private void Start()
		{
			UserAttributes.Instance.GetReadonlyAttributes(attrs => InstantiateAttributes(attrs,true), StoreDemoPopup.ShowError);
			UserAttributes.Instance.GetCustomAttributes(attrs => InstantiateAttributes(attrs,false), StoreDemoPopup.ShowError);
			Show(readOnly: true);
		}

		private void InstantiateAttributes(List<UserAttribute> userAttributes, bool readOnly)
		{
			if (!_isAlive)
				return;

			var isFull = userAttributes != null && userAttributes.Count > 0;

			if (isFull)
			{
				var root = readOnly ? ReadOnlyAttributesRoot : CustomAttributesRoot;
				foreach (var attribute in userAttributes)
				{
					var attributeObject = Instantiate<GameObject>(AttributePrefabProvider.GetValue(), root);
					var uiScript = attributeObject.GetComponent<AttributeItemUI>();

					uiScript.IsReadOnly = readOnly;
					uiScript.IsPublic = attribute.permission != "private";
					uiScript.Key = attribute.key;
					uiScript.Value = attribute.value;
				}
			}

			var fullObjects = readOnly ? ReadOnlyAttributesFull : CustomAttributesFull;
			var emptyObjects = readOnly ? ReadOnlyAttributesEmpty : CustomAttributesEmpty;

			foreach (var fullObject in fullObjects)
				fullObject.SetActive(isFull);

			foreach (var emptyObject in emptyObjects)
				emptyObject.SetActive(!isFull);

			if (readOnly)
				_initFlag |= 1;
			else
				_initFlag |= 10;

			_isInitialized = _initFlag == 11;

			RebuildLayouts();
		}

		private void Show(bool readOnly)
		{
			ReadOnlyAttributesTab.SetActive(readOnly);
			CustomAttributesTab.SetActive(!readOnly);

			RebuildLayouts ();
		}

		private void RebuildLayouts()
		{
			foreach (var layout in Layouts)
				LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
		}

		private void HandleKeyChanged(AttributeItemUI attributeItem, string oldKey, string newKey)
		{
			if (attributeItem.IsReadOnly)
			{
				XDebug.LogError($"UserAttributesUI.HandleKeyChanged: Attempt to change key of read-only attribute. OldKey:'{oldKey}' NewKey:'{newKey}'");
				attributeItem.Key = oldKey;
				return;
			}

			var successChange = UserAttributes.Instance.ChangeKey(oldKey, newKey, out var error);
			if (!successChange)
			{
				XDebug.LogWarning($"UserAttributesUI.HandleKeyChanged: Failed attempt to change key. OldKey:'{oldKey}' NewKey:'{newKey}' Error:{error}");
				attributeItem.Key = oldKey;
			}
		}

		private void HandleValueChanged(AttributeItemUI attributeItem, string oldValue, string newValue)
		{
			if (attributeItem.IsReadOnly)
			{
				XDebug.LogError($"UserAttributesUI.HandleValueChanged: Attempt to change value of read-only attribute. Key:{attributeItem.Key} OldValue:{oldValue} NewValue:{newValue}");
				attributeItem.Value = oldValue;
				return;
			}

			var successChange = UserAttributes.Instance.ChangeValue(attributeItem.Key, newValue, out var error);
			if (!successChange)
			{
				XDebug.LogWarning($"UserAttributesUI.HandleValueChanged: Failed attempt to change value. Key:'{attributeItem.Key}' OldValue:'{oldValue}' NewValue:'{newValue}' Error:{error}");
				attributeItem.Value = oldValue;
			}
		}

		private void HandleRemoveRequest(AttributeItemUI attributeItem)
		{
			if (attributeItem.IsReadOnly)
			{
				XDebug.LogError($"UserAttributesUI.HandleRemoveRequest: Attempt to remove read-only attribute. Key:{attributeItem.Key}");
				return;
			}

			var successRemove = UserAttributes.Instance.RemoveAttribute(attributeItem.Key, out var error);
			if (!successRemove)
			{
				XDebug.LogWarning($"UserAttributesUI.HandleRemoveRequest: Failed attempt to remove attribute. Key:'{attributeItem.Key}' Error:{error}");
				return;
			}

			Destroy(attributeItem.gameObject);
		}

		private void AddNewAttribute()
		{
			if (!_isInitialized)
				return;

			var customCount = 0;
			var newValue = DEFAULT_VALUE;

			void CreateNewAttribute(List<UserAttribute> userAttributes)
			{
				var newKey = (customCount == 0) ? DEFAULT_KEY : $"{DEFAULT_KEY}_{customCount}";
				var isUnique = true;

				foreach (var attribute in userAttributes)
				{
					if (attribute.key == newKey)
					{
						isUnique = false;
						break;
					}
				}

				if (!isUnique)
				{
					customCount += 1;
					CreateNewAttribute(userAttributes);
					return;
				}

				var newAttribute = UserAttributes.Instance.AddNewAttribute(newKey, newValue, out var error);
				if (newAttribute != null)
					InstantiateAttributes(new List<UserAttribute>() { newAttribute }, readOnly: false);
				else
					XDebug.LogWarning($"UserAttributesUI.AddNewAttribute: Failed attempt to add new attribute. Error:{error}");
			}

			UserAttributes.Instance.GetCustomAttributes(CreateNewAttribute,StoreDemoPopup.ShowError);
		}

		private void SaveAttributes()
		{
			if (!_isInitialized)
				return;

			UserAttributes.Instance.PushChanges(onSuccess: () => StoreDemoPopup.ShowSuccess("Attributes saved"), onError: StoreDemoPopup.ShowError);
		}
	}
}
