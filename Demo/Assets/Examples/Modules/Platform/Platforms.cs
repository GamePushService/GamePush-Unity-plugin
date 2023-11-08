using UnityEngine;

using GamePush;

namespace Examples.Platforms
{
    public class Platforms : MonoBehaviour
    {
        public void Type() => GP_Platform.Type();

        public void Define()
        {
            if (GP_Platform.Type() == Platform.YANDEX)
            {
                // YANDEX
            }
        }

        public void HasIntegratedAuth() => GP_Platform.HasIntegratedAuth();
        public void IsExternalLinksAllowed() => GP_Platform.IsExternalLinksAllowed();
    }
}