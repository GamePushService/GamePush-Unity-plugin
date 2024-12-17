using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerUpHandler
	{
		[SerializeField] Image image = default;

		[SerializeField] Text text = default;

		[SerializeField] Sprite normalStateSprite = default;
		[SerializeField] Sprite selectedStateSprite = default;
		[SerializeField] Sprite hoverStateSprite = default;
		[SerializeField] Sprite pressedStateSprite = default;
		
		[SerializeField] bool capitalizeText = true;

		[SerializeField] bool isSelectable = true;
		[SerializeField] private DecoratorPointerEvents DecoratorPointerEvents = default;
		[SerializeField] private Text SelectedText = default;
		[SerializeField] private ColorProvider NormalTextColor = default;
		[SerializeField] private ColorProvider SelectedTextColor = default;
		[SerializeField] private GameObject OnSelectedObject = default;

		bool _isClickInProgress;
		bool _isSelected;

		public static event Action OnCursorEnter;
		public static event Action OnCursorExit;

		public Action<string> onClick;
		private DateTime _lastClick;
		private float _rateLimitMs = StoreConstants.DEFAULT_BUTTON_RATE_LIMIT_MS;

		string _buttonId = "";

		void Awake()
		{
			_lastClick = DateTime.MinValue;
		}

		private void PerformClickEvent()
		{
			TimeSpan ts = DateTime.Now - _lastClick;
			if (ts.TotalMilliseconds > _rateLimitMs)
			{
				_lastClick += ts;
				onClick?.Invoke(_buttonId);
			}
		}

		public void Select(bool triggerClickEvent = true)
		{
			_isSelected = true;
			_isClickInProgress = false;

			if (triggerClickEvent)
			{
				PerformClickEvent();
			}

			OnSelected();
		}

		public void Deselect()
		{
			_isSelected = false;
			_isClickInProgress = false;

			OnNormal();
		}

		public string Text
		{
			get { return text.text; }
			set { text.text = capitalizeText ? value.ToUpper() : value; }
		}

		public string Id
		{
			get { return _buttonId; }
			set { _buttonId = value; }
		}

		public bool IsSelected
		{
			get { return _isSelected && isSelectable; }
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnCursorEnter?.Invoke();

			if (!IsSelected)
			{
				OnHover();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnCursorExit?.Invoke();

			if (IsSelected)
			{
				OnSelected();
			}
			else
			{
				OnNormal();
			}

			_isClickInProgress = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isClickInProgress = true;

			if (!IsSelected)
			{
				OnPressed();
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_isClickInProgress && !IsSelected)
			{
				_isSelected = true;

				OnSelected();

				PerformClickEvent();
			}

			_isClickInProgress = false;
		}

		protected void SetImageSprite(Image i, Sprite sprite)
		{
			if(i != null && sprite != null)
				i.sprite = sprite;
		}

		protected virtual void OnNormal()
		{
			SetImageSprite(image, normalStateSprite);
			DecoratorPointerEvents.IsMute = false;
			StartCoroutine(DoChangeColorOnNextFrame(NormalTextColor.GetValue()));

			if (OnSelectedObject)
				OnSelectedObject.SetActive(false);
		}

		protected virtual void OnHover()
		{
			SetImageSprite(image, hoverStateSprite);
		}

		protected virtual void OnPressed()
		{
			SetImageSprite(image, pressedStateSprite);
		}

		protected virtual void OnSelected()
		{
			SetImageSprite(image, selectedStateSprite);
			DecoratorPointerEvents.IsMute = false;

			if(gameObject.activeInHierarchy && gameObject.activeSelf)
				StartCoroutine(DoChangeColorOnNextFrame(SelectedTextColor.GetValue()));

			if (OnSelectedObject)
				OnSelectedObject.SetActive(true);
		}

		private IEnumerator DoChangeColorOnNextFrame(Color color)
		{
			yield return null;
			DecoratorPointerEvents.IsMute = false;
			SelectedText.color = color;
		}
	}
}