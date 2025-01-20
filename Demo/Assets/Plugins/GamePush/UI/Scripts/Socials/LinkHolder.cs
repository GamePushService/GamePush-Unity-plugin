using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.UI
{
    public class LinkHolder : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _link;

        void Start()
        {
            _link.text = CoreSDK.Language.localization.share.link;
        }
    }
}
