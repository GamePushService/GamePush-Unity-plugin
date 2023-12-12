mergeInto(LibraryManager.library, {

    /* LANGUAGE */
    GP_Current_Language: function () {
        var value = GamePush.Language();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_ChangeLanguage: function (language) {
        GamePush.ChangeLanguage(UTF8ToString(language));
    },
    /* LANGUAGE */



    /* AVATAR GENERATOR */
    GP_Change_AvatarGenerator: function (generator) {
        GamePush.ChangeAvatarGenerator(UTF8ToString(generator));
    },
    GP_Current_AvatarGenerator: function () {
        var value = GamePush.AvatarGenerator();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    /* AVATAR GENERATOR */



    /* PLATFORM */
    GP_Platform_Type: function () {
        var value = GamePush.PlatformType();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Platform_HasIntegratedAuth: function () {
        var value = GamePush.PlatformHasIntegratedAuth();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Platform_IsExternalLinksAllowed: function () {
        var value = GamePush.PlatformIsExternalLinksAllowed();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    /* PLATFORM */


    /* APP */
    GP_App_Title: function () {
        var value = GamePush.AppTitle();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_App_Description: function () {
        var value = GamePush.AppDescription();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_App_Image: function () {
        var value = GamePush.AppImage();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_App_Url: function () {
        var value = GamePush.AppUrl();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_App_ReviewRequest: function(){
        var value = GamePush.AppRequestReview();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_App_CanReview: function(){
        var value = GamePush.AppCanRequestReview();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

        GP_App_IsAlreadyReviewed: function(){
        var value = GamePush.AppIsAlreadyReviewed();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_App_AddShortcut: function(){
        var value = GamePush.AppAddShortcut();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_App_CanAddShortcut: function(){
        var value = GamePush.AppCanAddShortcut();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    /* APP */



    /* PLAYER */
    GP_Player_GetNumberInt: function (key) {
        return GamePush.PlayerGet(UTF8ToString(key));
    },
    GP_Player_GetNumberFloat: function (key) {
        return GamePush.PlayerGet(UTF8ToString(key));
    },
    GP_Player_GetBool: function (key) {
        var value = GamePush.PlayerGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Player_GetString: function (key) {
        var value = GamePush.PlayerGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Player_GetID: function () {
        return GamePush.PlayerGetID();
    },
    GP_Player_GetScore: function () {
        return GamePush.PlayerGetScore();
    },
    GP_Player_GetName: function () {
        var value = GamePush.PlayerGetName();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Player_GetAvatar: function () {
        var value = GamePush.PlayerGetAvatar();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Player_GetFieldName: function (key) {
        return GamePush.PlayerGetFieldName(UTF8ToString(key));
    },
    GP_Player_GetFieldVariantName: function (key, value) {
        return GamePush.PlayerGetFieldVariantName(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_GetFieldVariantAt: function (key, index) {
        return GamePush.PlayerGetFieldVariantAt(UTF8ToString(key), UTF8ToString(index));
    },
    GP_Player_GetFieldVariantIndex: function (key, value) {
        return GamePush.PlayerGetFieldVariantIndex(UTF8ToString(key), UTF8ToString(value));
    },

    GP_Player_SetName: function (name) {
        GamePush.PlayerSetName(UTF8ToString(name));
    },
    GP_Player_SetAvatar: function (src) {
        GamePush.PlayerSetAvatar(UTF8ToString(src));
    },
    GP_Player_SetScore: function (score) {
        GamePush.PlayerSetScore(score);
    },
    GP_Player_AddScore: function (score) {
        GamePush.PlayerAddScore(score);
    },
    GP_Player_Set_String: function (key, value) {
        GamePush.PlayerSetString(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Set_Number: function (key, value) {
        GamePush.PlayerSetNumber(UTF8ToString(key), value);
    },
    GP_Player_Set_Bool: function (key, value) {
        GamePush.PlayerSetBool(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_SetFlag: function (key, value) {
        GamePush.PlayerSetFlag(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Add: function (key, value) {
        GamePush.PlayerAdd(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Toggle: function (key) {
        GamePush.PlayerToggle(UTF8ToString(key));
    },
    GP_Player_Reset: function () {
        GamePush.PlayerReset();
    },
    GP_Player_Remove: function () {
        GamePush.PlayerRemove();
    },
    GP_Player_Sync: function (override) {
        GamePush.PlayerSync(override);
    },
    GP_Player_Load: function () {
        GamePush.PlayerLoad();
    },
    GP_Player_Login: function () {
        GamePush.PlayerLogin();
    },
    GP_Player_FetchFields: function () {
        GamePush.PlayerFetchFields();
    },

    GP_Player_Has: function (key) {
        var value = GamePush.PlayerHas(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Player_IsLoggedIn: function () {
        var value = GamePush.PlayerIsLoggedIn();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Player_HasAnyCredentials: function () {
        var value = GamePush.PlayerHasAnyCredentials();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Player_IsStub: function () {
        var value = GamePush.PlayerIsStub();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    // Player stats 

    GP_Player_GetActiveDays: function () {
        return GamePush.PlayerGetActiveDays();
    },
    GP_Player_GetActiveDaysConsecutive: function () {
        return GamePush.PlayerGetActiveDaysConsecutive();
    },
    GP_Player_GetPlaytimeToday: function () {
        return GamePush.PlayerGetPlaytimeToday();
    },
    GP_Player_GetPlaytimeAll: function () {
        return GamePush.PlayerGetPlaytimeAll();
    },

    /* PLAYER */



    /* LEADER BOARD */
    GP_Leaderboard_Open: function (orderBy, order, limit, showNearest, withMe, includeFields, displayFields) {
        GamePush.LeaderboardOpen(UTF8ToString(orderBy), UTF8ToString(order), limit, showNearest, UTF8ToString(withMe), UTF8ToString(includeFields), UTF8ToString(displayFields));
    },
    GP_Leaderboard_Fetch: function (tag, orderBy, order, limit, showNearest, withMe, includeFields) {
        GamePush.LeaderboardFetch(UTF8ToString(tag), UTF8ToString(orderBy), UTF8ToString(order), limit, showNearest, UTF8ToString(withMe), UTF8ToString(includeFields));
    },
    GP_Leaderboard_FetchPlayerRating: function (tag, orderBy, order) {
        GamePush.LeaderboardFetchPlayerRating(UTF8ToString(tag), UTF8ToString(orderBy), UTF8ToString(order));
    },
    /* LEADER BOARD */



    /* LEADER BOARD SCOPED */
    GP_Leaderboard_Scoped_Open: function (idOrTag, variant, order, limit, showNearest, includeFields, displayFields, withMe) {
        GamePush.LeaderboardScopedOpen(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(order), limit, showNearest, UTF8ToString(includeFields), UTF8ToString(displayFields), UTF8ToString(withMe));
    },
    GP_Leaderboard_Scoped_Fetch: function (idOrTag, variant, order, limit, showNearest, includeFields, withMe) {
        GamePush.LeaderboardScopedFetch(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(order), limit, showNearest, UTF8ToString(includeFields), UTF8ToString(withMe));
    },
    GP_Leaderboard_Scoped_PublishRecord: function (idOrTag, variant, override, key1, value1, key2, value2, key3, value3) {
        GamePush.LeaderboardScopedPublishRecord(UTF8ToString(idOrTag), UTF8ToString(variant), override, UTF8ToString(key1), value1, UTF8ToString(key2), value2, UTF8ToString(key3), value3);
    },
    GP_Leaderboard_Scoped_FetchPlayerRating: function (idOrTag, variant, includeFields) {
        GamePush.LeaderboardScopedFetchPlayerRating(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(includeFields));
    },
    /* LEADER BOARD SCOPED */



    /* ACHIEVEMENTS */
    GP_Achievements_Open: function () {
        GamePush.AchievementsOpen();
    },
    GP_Achievements_Fetch: function () {
        GamePush.AchievementsFetch();
    },
    GP_Achievements_Unlock: function (idOrTag) {
        GamePush.AchievementsUnlock(UTF8ToString(idOrTag));
    },


    GP_Achievements_SetProgress: function (idOrTag, progress) {
        GamePush.AchievementsSetProgress(UTF8ToString(idOrTag), progress);
    },
    GP_Achievements_Has: function (idOrTag) {
        var value = GamePush.AchievementsHas(UTF8ToString(idOrTag));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Achievements_GetProgress: function (idOrTag) {
        return GamePush.AchievementsGetProgress(UTF8ToString(idOrTag));
    },
    /* ACHIEVEMENTS */


    /* PAYMENTS */
    GP_Payments_FetchProducts: function () {
        GamePush.PaymentsFetchProducts();
    },
    GP_Payments_Purchase: function (idOrTag) {
        GamePush.PaymentsPurchase(UTF8ToString(idOrTag));
    },
    GP_Payments_Consume: function (idOrTag) {
        GamePush.PaymentsConsume(UTF8ToString(idOrTag));
    },

    GP_Payments_IsAvailable: function () {
        var value = GamePush.PaymentsIsAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    /* Subscription */
    GP_Payments_IsSubscriptionsAvailable: function () {
        var value = GamePush.PaymentsIsSubscriptionsAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Payments_Subscribe: function (idOrTag) {
        GamePush.PaymentsSubscribe(UTF8ToString(idOrTag));
    },
    GP_Payments_Unsubscribe: function (idOrTag) {
        GamePush.PaymentsUnsubscribe(UTF8ToString(idOrTag));
    },

    /* Subscription */

    /* PAYMENTS */



    /* FULLSCREEN */
    GP_Fullscreen_Open: function () {
        GamePush.FullscreenOpen();
    },
    GP_Fullscreen_Close: function () {
        GamePush.FullscreenClose();
    },
    GP_Fullscreen_Toggle: function () {
        GamePush.FullscreenToggle();
    },
    /* FULLSCREEN */



    /* ADS */
    GP_Ads_ShowFullscreen: function () {
        GamePush.AdsShowFullscreen();
    },
    GP_Ads_ShowRewarded: function (Tag) {
        GamePush.AdsShowRewarded(UTF8ToString(Tag));
    },
    GP_Ads_ShowPreloader: function () {
        GamePush.AdsShowPreloader();
    },
    GP_Ads_ShowSticky: function () {
        GamePush.AdsShowSticky();
    },
    GP_Ads_CloseSticky: function () {
        GamePush.AdsCloseSticky();
    },
    GP_Ads_RefreshSticky: function () {
        GamePush.AdsRefreshSticky();
    },



    GP_Ads_IsAdblockEnabled: function () {
        var value = GamePush.AdsIsAdblockEnabled();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },


    GP_Ads_IsStickyAvailable: function () {
        var value = GamePush.AdsIsStickyAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;

    },
    GP_Ads_IsFullscreenAvailable: function () {
        var value = GamePush.AdsIsFullscreenAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;

    },
    GP_Ads_IsRewardedAvailable: function () {
        var value = GamePush.AdsIsRewardedAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsPreloaderAvailable: function () {
        var value = GamePush.AdsIsPreloaderAvailable();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsStickyPlaying: function () {
        var value = GamePush.AdsIsStickyPlaying();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsFullscreenPlaying: function () {
        var value = GamePush.AdsIsFullscreenPlaying();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsRewardedPlaying: function () {
        var value = GamePush.AdsIsRewardedPlaying();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsPreloaderPlaying: function () {
        var value = GamePush.AdsIsPreloaderPlaying();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsCountdownOverlayEnabled: function () {
        var value = GamePush.AdsIsCountdownOverlayEnabled();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_IsRewardedFailedOverlayEnabled: function () {
        var value = GamePush.AdsIsRewardedFailedOverlayEnabled();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Ads_CanShowFullscreenBeforeGamePlay: function () {
        var value = GamePush.AdsCanShowFullscreenBeforeGamePlay();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    /* ADS */



    /* ANALYTICS */
    GP_Analytics_Hit: function (url) {
        GamePush.AnalyticsHit(UTF8ToString(url));
    },
    GP_Analytics_Goal: function (event, value) {
        GamePush.AnalyticsGoal(UTF8ToString(event), UTF8ToString(value));
    },
    /* ANALYTICS */



    /* SOCIALS */
    GP_Socials_Share: function (text, url, image) {
        GamePush.SocialsShare(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },


    GP_Socials_Post: function (text, url, image) {
        GamePush.SocialsPost(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },
    GP_Socials_Invite: function (text, url, image) {
        GamePush.SocialsInvite(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },
    GP_Socials_JoinCommunity: function () {
        GamePush.SocialsJoinCommunity();
    },


    GP_Socials_CommunityLink: function () {
        var value = GamePush.SocialsCommunityLink();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Socials_IsSupportsShare: function () {
        var value = GamePush.SocialsIsSupportsShare();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Socials_IsSupportsNativeShare: function () {
        var value = GamePush.SocialsIsSupportsNativeShare();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Socials_IsSupportsNativePosts: function () {
        var value = GamePush.SocialsIsSupportsNativePosts();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Socials_IsSupportsNativeInvite: function () {
        var value = GamePush.SocialsIsSupportsNativeInvite();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Socials_CanJoinCommunity: function () {
        var value = GamePush.SocialsCanJoinCommunity();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Socials_IsSupportsNativeCommunityJoin: function () {
        var value = GamePush.SocialsIsSupportsNativeCommunityJoin();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;

    },

    GP_Socials_MakeShareLink: function (content) {
        var value = GamePush.SocialsMakeShareLink(UTF8ToString(content));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Socials_GetSharePlayerID: function () {
        return GamePush.SocialsGetSharePlayerID();
    },
    GP_Socials_GetShareContent: function () {
        var value = GamePush.SocialsGetShareContent();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    /* SOCIALS */



    /* GAMES COLLECTIONS */
    GP_GamesCollections_Open: function (idOrTag) {
        GamePush.GamesCollectionsOpen(UTF8ToString(idOrTag));
    },
    GP_GamesCollections_Fetch: function (idOrTag) {
        GamePush.GamesCollectionsFetch(UTF8ToString(idOrTag));
    },
    /* GAMES COLLECTIONS */



    /*GAME*/
    GP_IsPaused: function () {
        var value = GamePush.IsPaused();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Pause: function () {
        GamePush.Pause();
    },
    GP_Resume: function () {
        GamePush.Resume();
    },

    GP_GameplayStart: function () {
        GamePush.GameplayStart();
    },

    GP_GameplayStop: function () {
        GamePush.GameplayStop();
    },

    GP_GameReady: function(){
        GamePush.GameReady();
    },

    GP_HappyTime: function(){
        GamePush.HappyTime();
    },

    /*GAME*/



    /*DEVICE*/
    GP_IsMobile: function () {
        var value = GamePush.IsMobile();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_IsPortrait: function () {
        var value = GamePush.IsPortrait();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    /*DEVICE*/



    /*SERVER*/
    GP_ServerTime: function () {
        var value = GamePush.ServerTime();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    /*SERVER*/



    /*SYSTEM*/
    GP_IsDev: function () {
        var value = GamePush.IsDev();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_IsAllowedOrigin: function () {
        var value = GamePush.IsAllowedOrigin();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    /*SYSTEM*/

    /*CUSTOM*/
    GP_CustomCall1: function(custom){
        GamePush.CustomCall(UTF8ToString(custom));
    },
    GP_CustomCall2: function(custom){
        GamePush.CustomCall(custom);
    },
    /*CUSTOM*/

    /*VARIABLES*/
    GP_Variables_Fetch: function () {
        GamePush.VariablesFetch();
    },

    GP_Variables_Has: function (key) {
        var value = GamePush.VariablesHas(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Variables_GetNumberInt: function (key) {
        return GamePush.VariablesGet(UTF8ToString(key));
    },
    GP_Variables_GetFloat: function (key) {
        return GamePush.VariablesGet(UTF8ToString(key));
    },
    GP_Variables_GetString: function (key) {
        var value = GamePush.VariablesGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Variables_GetBool: function (key) {
        var value = GamePush.VariablesGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Variables_GetImage: function (key) {
        var value = GamePush.VariablesGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    GP_Variables_GetFile: function (key) {
        var value = GamePush.VariablesGet(UTF8ToString(key));
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    /*VARIABLES*/



    /* PLAYERS */
    GP_Players_Fetch: function (key) {
        GamePush.PlayersFetch(UTF8ToString(key));
    },
    /* PLAYERS */



    /* DOCUMENTS */
    GP_Documents_Open: function () {
        GamePush.DocumentsOpen();
    },
    GP_Documents_Fetch: function () {
        GamePush.DocumentsFetch();
    },
    /* DOCUMENTS */



    /* FILES */
    GP_Files_Upload: function (tags) {
        GamePush.FilesUpload(UTF8ToString(tags));
    },
    GP_Files_UploadUrl: function (url, filename, tags) {
        GamePush.FilesUploadUrl(UTF8ToString(url), UTF8ToString(filename), UTF8ToString(tags));
    },
    GP_Files_UploadContent: function (content, filename, tags) {
        GamePush.FilesUploadContent(UTF8ToString(content), UTF8ToString(filename), UTF8ToString(tags));
    },
    GP_Files_LoadContent: function (url) {
        GamePush.FilesLoadContent(UTF8ToString(url));
    },
    GP_Files_ChooseFile: function (type) {
        GamePush.FilesChooseFile(UTF8ToString(type));
    },
    GP_Files_Fetch: function (filter) {
        GamePush.FilesFetch(UTF8ToString(filter));
    },
    GP_Files_FetchMore: function (filter) {
        GamePush.FilesFetchMore(UTF8ToString(filter));
    },
    /* FILES */



    /* CHANNELS */
    GP_Channels_Open: function (channel_ID) {
        GamePush.Channels_Open_Chat(channel_ID);
    },

    GP_Channels_IsMainChatEnabled: function () {
        var value = GamePush.Channels_IsMainChatEnabled();
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },
    GP_Channels_MainChatId: function () {
        return GamePush.Channels_MainChatId();
    },


    GP_Channels_Join: function (channel_ID, password) {
        GamePush.Channels_Join(channel_ID, UTF8ToString(password));
    },
    GP_Channels_CancelJoin: function (channel_ID) {
        GamePush.Channels_CancelJoin(channel_ID);
    },
    GP_Channels_Leave: function (channel_ID) {
        GamePush.Channels_Leave(channel_ID);
    },
    GP_Channels_Kick: function (channel_ID, player_ID) {
        GamePush.Channels_Kick(channel_ID, player_ID);
    },
    GP_Channels_Mute_UnmuteAt: function (channel_ID, player_ID, unmuteAt) {
        GamePush.Channels_Mute_UnmuteAt(channel_ID, player_ID, UTF8ToString(unmuteAt));
    },
    GP_Channels_Mute_Seconds: function (channel_ID, player_ID, seconds) {
        GamePush.Channels_Mute_Seconds(channel_ID, player_ID, seconds);
    },
    GP_Channels_UnMute: function (channel_ID, player_ID) {
        GamePush.Channels_UnMute(channel_ID, player_ID);
    },
    GP_Channels_SendInvite: function (channel_ID, player_ID) {
        GamePush.Channels_SendInvite(channel_ID, player_ID);
    },
    GP_Channels_CancelInvite: function (channel_ID, player_ID) {
        GamePush.Channels_CancelInvite(channel_ID, player_ID);
    },
    GP_Channels_AcceptInvite: function (channel_ID) {
        GamePush.Channels_AcceptInvite(channel_ID);
    },
    GP_Channels_RejectInvite: function (channel_ID) {
        GamePush.Channels_RejectInvite(channel_ID);
    },
    GP_Channels_FetchInvites: function (limit, offset) {
        GamePush.Channels_FetchInvites(limit, offset);
    },
    GP_Channels_FetchMoreInvites: function (limit) {
        GamePush.Channels_FetchMoreInvites(limit);
    },
    GP_Channels_FetchChannelInvites: function (channel_ID, limit, offset) {
        GamePush.Channels_FetchChannelInvites(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreChannelInvites: function (channel_ID, limit) {
        GamePush.Channels_FetchMoreChannelInvites(channel_ID, limit);
    },
    GP_Channels_FetchSentInvites: function (channel_ID, limit, offset) {
        GamePush.Channels_FetchSentInvites(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreSentInvites: function (channel_ID, limit) {
        GamePush.Channels_FetchMoreSentInvites(channel_ID, limit);
    },
    GP_Channels_AcceptJoinRequest: function (channel_ID, player_ID) {
        GamePush.Channels_AcceptJoinRequest(channel_ID, player_ID);
    },
    GP_Channels_RejectJoinRequest: function (channel_ID, player_ID) {
        GamePush.Channels_RejectJoinRequest(channel_ID, player_ID);
    },
    GP_Channels_FetchJoinRequests: function (channel_ID, limit, offset) {
        GamePush.Channels_FetchJoinRequests(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreJoinRequests: function (channel_ID, limit) {
        GamePush.Channels_FetchMoreJoinRequests(channel_ID, limit);
    },
    GP_Channels_FetchSentJoinRequests: function (channel_ID, limit, offset) {
        GamePush.Channels_FetchSentJoinRequests(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreSentJoinRequests: function (channel_ID, limit) {
        GamePush.Channels_FetchMoreSentJoinRequests(channel_ID, limit);
    },
    GP_Channels_SendMessage: function (channel_ID, text, tags) {
        GamePush.Channels_SendMessage(channel_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_SendPersonalMessage: function (player_ID, text, tags) {
        GamePush.Channels_SendPersonalMessage(player_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_SendFeedMessage: function (player_ID, text, tags) {
        GamePush.Channels_SendFeedMessage(player_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_EditMessage: function (message_ID, text) {
        GamePush.Channels_EditMessage(UTF8ToString(message_ID), UTF8ToString(text));
    },
    GP_Channels_DeleteMessage: function (message_ID) {
        GamePush.Channels_DeleteMessage(UTF8ToString(message_ID));
    },
    GP_Channels_FetchMessages: function (channel_ID, tags, limit, offset) {
        GamePush.Channels_FetchMessages(channel_ID, UTF8ToString(tags), limit, offset);
    },
    GP_Channels_FetchPersonalMessages: function (player_ID, tags, limit, offset) {
        GamePush.Channels_FetchPersonalMessages(player_ID, UTF8ToString(tags), limit, offset);
    },
    GP_Channels_FetchFeedMessages: function (player_ID, tags, limit, offset) {
        GamePush.Channels_FetchFeedMessages(player_ID, UTF8ToString(tags), limit, offset);
    },

    GP_Channels_FetchMoreMessages: function (channel_ID, tags, limit) {
        GamePush.Channels_FetchMoreMessages(channel_ID, UTF8ToString(tags), limit);
    },
    GP_Channels_FetchMorePersonalMessages: function (player_ID, tags, limit) {
        GamePush.Channels_FetchMorePersonalMessages(player_ID, UTF8ToString(tags), limit);
    },
    GP_Channels_FetchMoreFeedMessages: function (player_ID, tags, limit) {
        GamePush.Channels_FetchMoreFeedMessages(player_ID, UTF8ToString(tags), limit);
    },

    GP_Channels_DeleteChannel: function (channel_ID) {
        GamePush.Channels_DeleteChannel(channel_ID);
    },
    GP_Channels_FetchChannel: function (channel_ID) {
        GamePush.Channels_FetchChannel(channel_ID);
    },

    GP_Channels_CreateChannel: function (filter) {
        GamePush.Channels_CreateChannel(UTF8ToString(filter));
    },

    GP_Channels_UpdateChannel: function (filter) {
        GamePush.Channels_UpdateChannel(UTF8ToString(filter));
    },

    GP_Channels_FetchChannels: function (filter) {
        GamePush.Channels_FetchChannels(UTF8ToString(filter));
    },

    GP_Channels_FetchMoreChannels: function (filter) {
        GamePush.Channels_FetchMoreChannels(UTF8ToString(filter));
    },

    GP_Channels_FetchMembers: function (filter) {
        GamePush.Channels_FetchMembers(UTF8ToString(filter));
    },

    GP_Channels_FetchMoreMembers: function (filter) {
        GamePush.Channels_FetchMoreMembers(UTF8ToString(filter));
    },

    /* CHANNELS */

});
