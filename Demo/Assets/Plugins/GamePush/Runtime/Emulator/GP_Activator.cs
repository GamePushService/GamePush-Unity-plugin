using System;
using UnityEngine;

namespace GamePush
{
    public enum ECloudActivatorType
    {
        SocialShare = 0,
        SocialInvite = 1,
        SocialJoinCommunity = 2,
        InApp = 3,
        FullScreenAd = 4,
        RewardedAd = 5,
        PlatformType = 6,
    }
    
    public enum EActivatorActionType
    {
        None = 0,
        Deactivate = 1,
    }
    public class GP_Activator : MonoBehaviour
    {
        [SerializeField] private ECloudActivatorType type;
        [SerializeField] private Platform platformType;
        [SerializeField] private EActivatorActionType action;
        [SerializeField] private bool applyOnStart;

        public Action Activated = () => {};
        
        private void Start()
        {
            if(applyOnStart)
            {
                ApplyAction();
            }
        }

        public bool CheckActive()
        {
            var active = false;
            switch (type)
            {
                case ECloudActivatorType.SocialShare:
                    if (GP_Socials.IsSupportsShare() ||
                        GP_Socials.IsSupportsNativeShare())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.SocialInvite:
                    if (GP_Socials.IsSupportsNativeInvite())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.SocialJoinCommunity:
                    if (GP_Socials.IsSupportsNativeCommunityJoin() && GP_Socials.CanJoinCommunity())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.RewardedAd:
                    if (GP_Ads.IsRewardedAvailable())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.FullScreenAd:
                    if (GP_Ads.IsFullscreenAvailable())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.InApp:
                    if (GP_Payments.IsPaymentsAvailable())
                    {
                        active = true;
                    }
                    break;
                case ECloudActivatorType.PlatformType:
                    if (GP_Platform.Type() != platformType)
                    {
                        active = true;
                    }
                    break;
            }

            return active;
        }
        
        private void ApplyAction()
        {
            switch (action)
            {
                case EActivatorActionType.Deactivate:
                    if(!CheckActive())
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                default:
                    if(CheckActive())
                    {
                        Activated.Invoke();
                    }
                    break;
            }
        }
    }
}