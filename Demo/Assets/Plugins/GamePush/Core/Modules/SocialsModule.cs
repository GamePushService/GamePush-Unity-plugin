using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class SocialsModule
    {
        public event Action<bool> OnShare;
        public event Action<bool> OnPost;
        public event Action<bool> OnInvite;
        public event Action<bool> OnJoinCommunity;

        public event Action<bool> OpenOverlay;

        public void Share(string text = "", string url = "", string image = "")
        {
            OnShare?.Invoke(false);
        }

        public void Post(string text = "", string url = "", string image = "")
        {
            OnPost?.Invoke(false);
        }

        public void Invite(string text = "", string url = "", string image = "")
        {
            OnInvite?.Invoke(false);
        }

        public void JoinCommunity()
        {
            OnJoinCommunity?.Invoke(false);
        }

        public string CommunityLink()
        {
            return "";
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
            return "";
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
