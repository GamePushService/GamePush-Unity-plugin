using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    
    [CreateAssetMenu(fileName = "GP_PlatformSettings", menuName = "GP_Settings/GP_PlatformSettings")]
    public class GP_PlatformSettings : ScriptableObject
    {
        [SerializeField] public Platform PlatformToEmulate = Platform.None;
        [SerializeField] public Language Language = Language.English;
        [Header("Device")]
        [SerializeField] public bool IsMobile = true;
        [SerializeField] public bool IsPortrait = true;
        [Header("Platform")]
        [SerializeField] public bool HasIntegratedAuth = true;
        [SerializeField] public bool IsExternalLinksAllowed = true;
        [Header("Player")]
        [SerializeField] public bool IsLoggedIn = true;
        [SerializeField] public bool HasAnyCredentials = true;
        [SerializeField] public bool IsStub = true;
        [Header("System")]
        [SerializeField] public bool IsDev = true;
        [SerializeField] public bool IsAllowedOrigin = true;
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
        public bool IsAlreadyReviewed;
    }
}