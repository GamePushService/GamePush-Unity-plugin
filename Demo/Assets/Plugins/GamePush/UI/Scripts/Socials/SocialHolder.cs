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
                SocialType.vkontakte => CoreSDK.language.localization.socials.vkontakte,
                SocialType.odnoklassniki => CoreSDK.language.localization.socials.odnoklassniki,
                SocialType.telegram => CoreSDK.language.localization.socials.telegram,
                SocialType.twitter => CoreSDK.language.localization.socials.twitter,
                SocialType.facebook => CoreSDK.language.localization.socials.facebook,
                SocialType.moymir => CoreSDK.language.localization.socials.moymir,
                SocialType.whatsapp => CoreSDK.language.localization.socials.whatsapp,
                SocialType.viber => CoreSDK.language.localization.socials.viber,
                _ => ""
            };
        }
    }
}
