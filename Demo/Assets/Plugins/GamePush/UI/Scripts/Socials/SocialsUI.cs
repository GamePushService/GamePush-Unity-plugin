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


        public void Init(ShareType type)
        {
            _shareType = type;
            _title.text = GetTitle(type);

            StartCoroutine(MoveUp());
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

        public void Close()
        {
            OverlayCanvas.Controller.Close();
        }

        private void SocialInteract(SocialType type)
        {
            print(type.ToString());
            print(_shareType.ToString());
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
    }
}
