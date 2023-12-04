using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    [Serializable]
    public class PlatformSettings
    {
        public Platform Platform;
        [Header("Social")]
        public bool IsSupportsShare;
        public bool IsSupportsNativeShare;
        public bool IsSupportsNativeInvite;
        public bool IsSupportsNativePosts;
        public bool IsSupportsNativeCommunityJoin;
        public bool CanJoinCommunity;
        [Header("Payments")]
        public bool IsPaymentsAvailable;
        public bool IsSubscriptionsAvailable;
        [Header("Ads")]
        public bool IsFullscreenAvailable;
        public bool IsRewardedAvailable;
        public bool IsStickyAvailable;
        public bool IsPreloaderAvailable;
        public bool IsAdblockEnabled;
        [Header("Review")]
        public bool CanReview;
        [Header("Device")]
        public bool IsMobile;
        public bool IsDesktop;
        [Header("Platform")]
        public bool HasIntegratedAuth;
        public bool IsExternalLinksAllowed;
        [Header("Player")]
        public bool IsLoggedIn;
        public bool HasAnyCredentials;
        public bool IsStub;
        [Header("System")]
        public bool IsDev = true;
        public bool IsAllowedOrigin = true;
    }
    
    [CreateAssetMenu(fileName = "GP_PlatformSettings", menuName = "GP_Settings/GP_PlatformSettings")]
    public class GP_PlatformSettings : ScriptableObject
    {
        [SerializeField] public Platform PlatformToEmulate = Platform.None;
        [SerializeField] public Language Language = Language.English;
        [SerializeField] public List<PlatformSettings> Settings = new();

        public PlatformSettings GetPlatformSettings()
        {
            foreach (var s in Settings)
            {
                if (s.Platform == PlatformToEmulate)
                {
                    return s;
                }
            }
            Console.Log("PLATFORM SETTINGS: ", "DEFAULT");
            return new PlatformSettings();
        }
    }
}