using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public class ValueCounterVisibility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public float Speed = 1.0F;
		public HorizontalSizeChanger consumeText;
		public HorizontalSizeChanger valueCounter;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = gameObject.GetComponent<RectTransform>();
		}

		private void Start()
		{
			SetVisibility(false);
		}

		IEnumerator ChangeVisibilityCoroutine(bool visible)
		{
			float maxWidth = _rectTransform.rect.width;
			float visibleWidthLimit = maxWidth * 0.05F;
			float targetTextWidth = (visible ? 0.6F : 1.0F) * maxWidth;
			float targetCounterWidth = maxWidth - targetTextWidth;

			if (visible)
			{
				StartCoroutine(ChangeSizeCoroutine(consumeText, targetTextWidth, maxWidth));
				StartCoroutine(ChangeSizeCoroutine(valueCounter, targetCounterWidth, maxWidth));
			}
			else
			{
				StartCoroutine(ChangeSizeCoroutine(valueCounter, targetCounterWidth, maxWidth));
				StartCoroutine(ChangeSizeCoroutine(consumeText, targetTextWidth, maxWidth));
			}

			while (true)
			{
				yield return new WaitForEndOfFrame();

				if (visible)
				{
					if (valueCounter.GetWidth() > visibleWidthLimit)
					{
						break;
					}
				}
				else
				{
					if (valueCounter.GetWidth() < visibleWidthLimit)
					{
						break;
					}
				}
			}

			valueCounter.gameObject.SetActive(visible);
		}

		IEnumerator ChangeSizeCoroutine(HorizontalSizeChanger sizeChanger, float newValue, float maxSize)
		{
			float step = maxSize * 0.01F;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (!ChangeSize(sizeChanger, newValue, step))
					break;
			}
		}

		bool ChangeSize(HorizontalSizeChanger sizeChanger, float newSize, float step)
		{
			float width = sizeChanger.GetWidth();
			float delta = newSize - width;

			if (Mathf.Abs(delta) > step)
			{
				delta *= Speed * Time.deltaTime;
				sizeChanger.SetWidth(width + delta);
				return true;
			}
			else
			{
				sizeChanger.SetWidth(newSize);
				return false;
			}
		}

		void SetVisibility(bool visibility)
		{
			StopAllCoroutines();
			StartCoroutine(ChangeVisibilityCoroutine(visibility));
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			SetVisibility(true);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			SetVisibility(false);
		}
	}
}
