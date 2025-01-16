using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.UI
{
    public class SocialHolder : MonoBehaviour
    {
        [SerializeField]
        private SocialType _socialType;
        public SocialType Type() => _socialType;
        [SerializeField]
        private TMPro.TMP_Text _title;

        void Start()
        {
            _title.text = GetTitle(_socialType);
        }

        private string GetTitle(SocialType socialType)
        {
            return socialType switch
            {
                SocialType.vkontakte => CoreSDK.Language.localization.socials.vkontakte,
                SocialType.odnoklassniki => CoreSDK.Language.localization.socials.odnoklassniki,
                SocialType.telegram => CoreSDK.Language.localization.socials.telegram,
                SocialType.twitter => CoreSDK.Language.localization.socials.twitter,
                SocialType.facebook => CoreSDK.Language.localization.socials.facebook,
                SocialType.moymir => CoreSDK.Language.localization.socials.moymir,
                SocialType.whatsapp => CoreSDK.Language.localization.socials.whatsapp,
                SocialType.viber => CoreSDK.Language.localization.socials.viber,
                _ => ""
            };
        }
    }
}
