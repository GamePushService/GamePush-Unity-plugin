using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamePush.Data;
using GamePush;

namespace GamePush.UI
{
    public class SocialsUI : MonoBehaviour
    {
        [SerializeField]
        private float _startOffset = 400f;
        [SerializeField]
        private float _moveUpSpeed = 1200f;
        [Space]
        [SerializeField]
        private TMPro.TMP_Text _title;
        [Space]
        [SerializeField]
        private List<SocialHolder> _socials;
        [SerializeField]
        private LinkHolder _link;

        private ShareType _shareType;
        private string _text, _url, _image;


        public void Init(ShareType type, string text, string url, string image)
        {
            _shareType = type;
            _title.text = GetTitle(type);

            _text = text == "" ? CoreSDK.language.localization.leaderboard.inviteDivider : text;
            _url = url == "" ? CoreSDK.platform.gameLink : url;
            _image = image == "" ? CoreSDK.app.ProjectIcon() : image;

            StartCoroutine(MoveUp());
        }

        private void OnEnable()
        {
            foreach(SocialHolder social in _socials)
            {
                social.GetComponentInChildren<Button>().onClick.AddListener(() => SocialInteract(social));
            }
        }

        public void Close()
        {
            OverlayCanvas.Controller.Close();
        }

        private void SocialInteract(SocialHolder social)
        {
            print(social.Type().ToString());
            print(_shareType.ToString());

            string link = MakeLink(social.Type());
            OpenLink(link);
        }

        public string MakeLink(SocialType type)
        {
            return type switch
            {
                SocialType.vkontakte => SocialLinks.vkontakte + "url=" + _url + "&title=" + _text + "&image=" + _image,
                SocialType.odnoklassniki => SocialLinks.odnoklassniki + "url=" + _url + "&title=" + _text + "&imageUrl=" + _image,
                SocialType.telegram => SocialLinks.telegram + "url=" + _url + "&text=" + _text,
                SocialType.twitter => SocialLinks.twitter + "text=" + _text + "&url=" + _url,
                SocialType.facebook => SocialLinks.facebook + _url,
                SocialType.moymir => SocialLinks.moymir + "url=" + _url + "&title=" + _text + "&image_url=" + _image,
                SocialType.whatsapp => SocialLinks.whatsapp + "text=" + _text + _url,
                SocialType.viber => SocialLinks.viber + "text=" + _text + _url,
                _ => _url
            };
        }

        public void OpenLink(string url)
        {
            if (IsValidURL(url))
            {
                Application.OpenURL(url);
            }
        }

        public bool IsValidURL(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
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

        private IEnumerator MoveUp()
        {
            Vector3 endPos = transform.position;
            Vector3 startPos = endPos;
            startPos.y -= _startOffset;

            transform.position = startPos;

            while (transform.position.y < endPos.y)
            {
                transform.Translate(Vector2.up * _moveUpSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        
    }
}
