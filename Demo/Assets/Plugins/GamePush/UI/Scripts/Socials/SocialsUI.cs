using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamePush.Data;

namespace GamePush.UI
{
    public class SocialsUI : ModuleUI
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

        private string _text, _url, _image;
        private event Action<bool> SocialCallback;

        public void Init(string title, string text, string url, string image, Action<bool> callback)
        {
            _title.text = title;

            _text = text == "" ? CoreSDK.language.localization.leaderboard.inviteDivider : text;
            _url = url == "" ? CoreSDK.platform.gameLink : url;
            _image = image == "" ? CoreSDK.app.ProjectIcon() : image;

            SocialCallback = callback;

            StartCoroutine(MoveUp());
        }

        private void OnEnable()
        {
            foreach(SocialHolder social in _socials)
            {
                social.GetComponentInChildren<Button>().onClick.AddListener(() => SocialInteract(social));
            }

            _link.GetComponentInChildren<Button>().onClick.AddListener(() => CopyToClickboard());
        }

        public void Close()
        {
            SocialCallback?.Invoke(false);
            OverlayCanvas.Controller.Close();
        }

        private void SocialInteract(SocialHolder social)
        {
            SocialType socialType = social.Type();
            string link = GetBase(socialType) + GetQuery(socialType);

            print(link);
            OpenLink(link);
        }

        private string GetBase(SocialType type)
        {
            return type switch
            {
                SocialType.vkontakte => SocialLinks.vkontakte,
                SocialType.odnoklassniki => SocialLinks.odnoklassniki,
                SocialType.telegram => SocialLinks.telegram,
                SocialType.twitter => SocialLinks.twitter,
                SocialType.facebook => SocialLinks.facebook,
                SocialType.moymir => SocialLinks.moymir,
                SocialType.whatsapp => SocialLinks.whatsapp,
                SocialType.viber => SocialLinks.viber,
                _ => _url
            };
        }

        private string GetQuery(SocialType type)
        {
            _url = UnityEngine.Networking.UnityWebRequest.EscapeURL(_url);
            _text = UnityEngine.Networking.UnityWebRequest.EscapeURL(_text);
            _image = UnityEngine.Networking.UnityWebRequest.EscapeURL(_image);

            return type switch
            {
                SocialType.vkontakte => "?url=" + _url + "&title=" + _text,
                SocialType.odnoklassniki => "?url=" + _url + "&title=" + _text + "&imageUrl=" + _image,
                SocialType.telegram => "?url=" + _url + "&text=" + _text,
                SocialType.twitter => "?url=" + _url + "&text=" + _text,
                SocialType.facebook => "?u=" + _url,
                SocialType.moymir => "?url=" + _url + "&title=" + _text + "&image_url=" + _image,
                SocialType.whatsapp => "?url=" + _url + "&text=" + _text,
                SocialType.viber => "?url=" + _url + "&text=" + _text,
                _ => _url
            };
        }


        private void OpenLink(string url)
        {
            if (url != "")
            {
                Application.OpenURL(url);
                SocialCallback?.Invoke(true);
            }
            else
                SocialCallback?.Invoke(false);
        }

        private bool IsValidURL(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
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

        private void CopyToClickboard()
        {
            GUIUtility.systemCopyBuffer = _url;
            SocialCallback?.Invoke(true);
            OverlayCanvas.Controller.Close();
        }
    }
}
