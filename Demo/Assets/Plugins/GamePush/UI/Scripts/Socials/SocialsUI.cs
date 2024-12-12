using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamePush.Data;

namespace GamePush.UI
{
    
    public class SocialsUI : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _title;
        [Space]
        [SerializeField]
        private List<SocialHolder> _socials;
        [SerializeField]
        private LinkHolder _link;

        private ShareType _shareType;


        public void Init(ShareType type)
        {
            _shareType = type;
            _title.text = GetTitle(type);

        }

        private void OnEnable()
        {
            foreach(SocialHolder social in _socials)
            {
                social.GetComponent<Button>().onClick.AddListener(() => SocialInteract(social.Type()));
            }
        }

        private void OnDisable()
        {
            foreach (SocialHolder social in _socials)
            {
                social.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        private void SocialInteract(SocialType type)
        {
            print(type.ToString());
        }

        private string GetTitle(ShareType type)
        {
            return type switch
            {
                ShareType.share => CoreSDK.language.localization.share.title_share,
                ShareType.post => CoreSDK.language.localization.share.title_post,
                ShareType.invite => CoreSDK.language.localization.share.title_invite,
                _ => ""
            };
        }
    }
}
