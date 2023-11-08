using UnityEngine;

using GamePush;

namespace Examples.AvatarGenerator
{
    public class AvatarGenerator : MonoBehaviour
    {
        public void Current()
        {
            GP_AvatarGenerator.Current();
        }

        public void Change()
        {
            GP_AvatarGenerator.Change(GeneratorType.dicebear_identicon);
        }
    }
}