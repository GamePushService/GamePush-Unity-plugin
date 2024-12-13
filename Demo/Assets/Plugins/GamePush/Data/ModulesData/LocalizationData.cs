using System;
using Newtonsoft.Json;

namespace GamePush.Localization
{
    [Serializable]
    public class LocalizationData
    {
        public PlayersConflict playersConflict;
        public Leaderboard leaderboard;
        public Auth auth;
        public Purchase purchase;
        public Achievements achievements;
        public Share share;
        public Socials socials;
        public Login login;
        public LoginOnSaveError loginOnSaveError;
        public Platforms platforms;
        public Helpers helpers;
        public Player player;
        public Buttons buttons;
        public Chat chat;
        public Rare rare;
        public Documents documents;
        public OrientationOverlay orientationOverlay;
        public UnsupportedDeviceOverlay unsupportedDeviceOverlay;
        public PreAdCountdownOverlay preAdCountdownOverlay;
        public PreAdCountdownOverlayFinish preAdCountdownOverlayFinish;
        public RewardedFailedOverlay rewardedFailedOverlay;

        [Serializable]
        public class PlayersConflict
        {
            public string title;
            public string description;
            public string actionInfo;
            public string attention;
            public string saved;
            public string current;
        }

        [Serializable]
        public class Leaderboard
        {
            public string title;
            public string onlyAuthorized;
            public string nearestPlayers;
            public string inviteDivider;
            public string inviteFriends;
            public string shareRecord;
            public string shareRecordText;
        }

        [Serializable]
        public class Auth
        {
            public Register register;
            public Social social;
            public string errorCode;

            [Serializable]
            public class Register
            {
                public string success;
                public string successDescription;
                public string failed;
                public string failedDescription;
            }

            [Serializable]
            public class Social
            {
                public string success;
                public string successDescription;
                public string failed;
                public string failedDescription;
            }
        }

        [Serializable]
        public class Purchase
        {
            public Titles title;
            public Texts text;
            public Buttons btn;
            public Errors errors;

            [Serializable]
            public class Titles
            {
                public string approve;
                public string waiting;
                public string success;
                public string error;
            }

            [Serializable]
            public class Texts
            {
                public string waiting;
                public string tooLong;
                public string delayedPaymentAlert;
            }

            [Serializable]
            public class Buttons
            {
                public string buy;
                public string close;
                public string continuePlaying;
            }

            [Serializable]
            public class Errors
            {
                public string unathorized_purchase;
                public string failed_balance_deduction;
                public string internal_error;
                public string deactivated_profile;
                public string invalid_payment_model;
                public string disabled_payments;
                public string insufficient_funds;
                public string purchase_timeout;
                public string purchaseAmountTooLarge;
                public string mobileOperatorNotSupported;
                public string paymentDeclinedSecurity;
                public string cardDeclined;
                public string paymentMethodUnavailable;
                public string attemptsLimitExceeded;
                public string paymentSystemError;
                public string invalidUserDetails;
                public string paymentMethodNotAvailableRegion;
                public string paymentCurrentlyUnavailable;
                public string canceled;
                public string cardNotValid;
                public string unexpected;
            }
        }

        [Serializable]
        public class Achievements
        {
            public string title;
            public string unlocked;
            public string unlockedTotal;
            public string progress;
        }

        [Serializable]
        public class Share
        {
            [JsonProperty("title-share")]
            public string title_share;
            [JsonProperty("title-post")]
            public string title_post;
            [JsonProperty("title-invite")]
            public string title_invite;
            public string link;
        }

        [Serializable]
        public class Socials
        {
            public string vkontakte;
            public string odnoklassniki;
            public string telegram;
            public string twitter;
            public string facebook;
            public string moymir;
            public string whatsapp;
            public string viber;
        }

        [Serializable]
        public class Login
        {
            public string title;
            public string description;
        }

        [Serializable]
        public class LoginOnSaveError
        {
            public string enterCode;
            public string codeWrongError;
        }

        [Serializable]
        public class Platforms
        {
            public string YANDEX;
            public string VK;
            public string OK;
            public string NONE;
            public string CRAZY_GAMES;
        }

        [Serializable]
        public class Helpers
        {
            public string or;
            public string trueValue;
            public string falseValue;
        }

        [Serializable]
        public class Player
        {
            public string defaultName;
        }

        [Serializable]
        public class Buttons
        {
            public string confirm;
            public string continueValue;
            public string close;
            public string signIn;
            public string auth;
            public string linkCrazyGames;
            public string changeAccount;
        }

        [Serializable]
        public class Chat
        {
            public string administrator;
            public string muteTime;
            public string sendPlaceholder;
            public string online;
            public string emptyMessages;
            public Censor censor;
            public Errors errors;

            [Serializable]
            public class Censor
            {
                public string linksAndSpam;
                public string ageWarning;
            }

            [Serializable]
            public class Errors
            {
                public string connection_error;
                public string origin_not_allowed;
                public string access_denied;
                public string internal_error;
                public string player_muted;
                public string chat_disabled;
                public string rate_limit_reached;
                public string bad_value_filter_censor;
            }
        }

        [Serializable]
        public class Rare
        {
            public string COMMON;
            public string UNCOMMON;
            public string RARE;
            public string EPIC;
            public string LEGENDARY;
            public string MYTHIC;
        }

        [Serializable]
        public class Documents
        {
            public PlayerPrivacyPolicy PLAYER_PRIVACY_POLICY;

            [Serializable]
            public class PlayerPrivacyPolicy
            {
                public string title;
            }
        }

        [Serializable]
        public class OrientationOverlay
        {
            public string title;
            public Description description;

            [Serializable]
            public class Description
            {
                public string PORTRAIT;
                public string LANDSCAPE;
            }
        }

        [Serializable]
        public class UnsupportedDeviceOverlay
        {
            public string notice;
        }

        [Serializable]
        public class PreAdCountdownOverlay
        {
            public string title;
            public string description;
        }

        [Serializable]
        public class PreAdCountdownOverlayFinish
        {
            public string title;
        }

        [Serializable]
        public class RewardedFailedOverlay
        {
            public string title;
            public string noAds;
            public string adblock;
            public string notFinishedWatch;
        }
    }
}