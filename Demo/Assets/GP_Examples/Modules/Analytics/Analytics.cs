using UnityEngine;

using GamePush;

namespace Examples.Analytics
{
    public class Analytics : MonoBehaviour
    {
        public void Hit() => GP_Analytics.Hit("URL");

        public void Goal() => GP_Analytics.Goal("Event Name", "Value");
    }
}