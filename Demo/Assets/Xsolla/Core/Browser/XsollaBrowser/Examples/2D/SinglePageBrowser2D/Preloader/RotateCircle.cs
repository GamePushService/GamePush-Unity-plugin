using UnityEngine;

namespace Xsolla.Core.Browser
{
	public class RotateCircle : MonoBehaviour
	{
		[SerializeField] private float Speed = 60.0F;

		private RectTransform _circle;

		private void Awake()
		{
			_circle = (RectTransform) transform;
		}

		private void Update()
		{
			var delta = Vector3.forward * Speed * Time.deltaTime;
			_circle.Rotate(delta);
		}
	}
}