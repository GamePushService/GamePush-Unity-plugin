using UnityEngine;
using GamePush;

namespace Examples.Analytics
{
    public class Analytics : MonoBehaviour
    {
        private void Start()
        {
            GP_Analytics.Goal("open", "");
        }

        public void Hit() => GP_Analytics.Hit("URL");

        public void Goal() => GP_Analytics.Goal("send", "");
    }
}