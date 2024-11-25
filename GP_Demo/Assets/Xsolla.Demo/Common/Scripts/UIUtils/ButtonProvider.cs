using UnityEngine;

namespace Xsolla.Demo
{
	public class ButtonProvider : MonoBehaviour
    {
		[SerializeField] private SimpleButton button = default;

		public SimpleButton Button => button;
	}
}
