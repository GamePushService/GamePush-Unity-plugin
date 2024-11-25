using UnityEngine;

using GamePush;

namespace Examples.System_GP
{
    public class System : MonoBehaviour
    {
        public void IsDev() => GP_System.IsDev();
        public void IsAllowedOrigin() => GP_System.IsAllowedOrigin();
    }
}