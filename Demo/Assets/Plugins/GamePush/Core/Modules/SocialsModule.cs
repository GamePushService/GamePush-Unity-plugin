using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    public class SocialsModule
    {
        private string _communityLink;
        private string _gameLink;

        public event Action<bool> OnShare;
        public event Action<bool> OnPost;
        public event Action<bool> OnInvite;
        public event Action<bool> OnJoinCommunity;

        //Action parameters: title, text, url, image, callback
        public event Action<string, string, string, string, Action<bool>> OnOpenOverlay; 

        public void Init(Config config)
        {
            Translations links = config.communityLinks;
            _communityLink = LanguageTypes.GetTranslation(CoreSDK.currentLang, links);

            _gameLink = CoreSDK.Platform.gameLink;
        }

        public void OpenPanel(string title = "", string text = "", string url = "", string image = "") =>
            OnOpenOverlay?.Invoke(title, text, url, image, null);

        public void Share(string text = "", string url = "", string image = "")
        {
            OnOpenOverlay?.Invoke(GetTitle(ShareType.share), text, url, image, OnShare);
            //OnShare?.Invoke(false);
        }

        public void Post(string text = "", string url = "", string image = "")
        {
            OnOpenOverlay?.Invoke(GetTitle(ShareType.post), text, url, image, OnPost);
            //OnPost?.Invoke(false);
        }

        public void Invite(string text = "", string url = "", string image = "")
        {
            OnOpenOverlay?.Invoke(GetTitle(ShareType.invite), text, url, image, OnInvite);
            //OnInvite?.Invoke(false);
        }

        private string GetTitle(ShareType type)
        {
            return type switch
            {
                ShareType.share => CoreSDK.Language.localization.share.title_share,
                ShareType.post => CoreSDK.Language.localization.share.title_post,
                ShareType.invite => CoreSDK.Language.localization.share.title_invite,
                _ => ""
            };
        }

        public void JoinCommunity()
        {
            OnJoinCommunity?.Invoke(false);
        }

        public string CommunityLink()
        {
            return _communityLink;
        }

        public bool IsSupportsShare()
        {
            return false;
        }

        public bool IsSupportsNativeShare()
        {
            return false;
        }

        public bool IsSupportsNativePosts()
        {
            return false;
        }

        public bool IsSupportsNativeInvite()
        {
            return false;
        }

        public bool CanJoinCommunity()
        {
            return false;
        }

        public bool IsSupportsNativeCommunityJoin()
        {
            return false;
        }

        public string MakeShareLink(string content = "")
        {
            return _gameLink;
        }

        public int GetSharePlayerID()
        {
            return 0;
        }

        public string GetShareContent()
        {
            return "";
        }
    }
}
