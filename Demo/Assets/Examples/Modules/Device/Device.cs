using UnityEngine;

using GamePush;

namespace Examples.Device
{
    public class Device : MonoBehaviour
    {
        public void IsMoble() => GP_Device.IsMobile();

        public void IsDesktop() => GP_Device.IsDesktop();
    }
}