mergeInto(LibraryManager.library, {

    GP_UnityReady: function () {
        _UnityReady();
    },

    /* LANGUAGE */
    GP_Current_Language: function () {
        var value = _GP().Language();
        return _ToBuff(value);
    },
    GP_ChangeLanguage: function (language) {
        _GP().ChangeLanguage(UTF8ToString(language));
    },
    /* LANGUAGE */

    /* AVATAR GENERATOR */
    GP_Change_AvatarGenerator: function (generator) {
        _GP().ChangeAvatarGenerator(UTF8ToString(generator));
    },
    GP_Current_AvatarGenerator: function () {
        var value = _GP().AvatarGenerator();
        return _ToBuff(value);
    },
    /* AVATAR GENERATOR */

    /* PLATFORM */
    GP_Platform_Type: function () {
        var value = _GP().PlatformType();
        return _ToBuff(value);
    },

    GP_Platform_HasIntegratedAuth: function () {
        var value = _GP().PlatformHasIntegratedAuth();
        return _ToBuff(value);
    },
    GP_Platform_IsExternalLinksAllowed: function () {
        var value = _GP().PlatformIsExternalLinksAllowed();
        return _ToBuff(value);
    },
    /* PLATFORM */


    /* APP */
    GP_App_Title: function () {
        var value = _GP().AppTitle();
        return _ToBuff(value);
    },
    GP_App_Description: function () {
        var value = _GP().AppDescription();
        return _ToBuff(value);
    },
    GP_App_Image: function () {
        var value = _GP().AppImage();
        return _ToBuff(value);
    },
    GP_App_Url: function () {
        var value = _GP().AppUrl();
        return _ToBuff(value);
    },
    GP_App_ReviewRequest: function(){
        var value = _GP().AppRequestReview();
        return _ToBuff(value);
    },

    GP_App_CanReview: function(){
        var value = _GP().AppCanRequestReview();
        return _ToBuff(value);
    },

    GP_App_IsAlreadyReviewed: function(){
        var value = _GP().AppIsAlreadyReviewed();
        return _ToBuff(value);
    },

    GP_App_AddShortcut: function(){
        var value = _GP().AppAddShortcut();
        return _ToBuff(value);
    },

    GP_App_CanAddShortcut: function(){
        var value = _GP().AppCanAddShortcut();
        return _ToBuff(value);
    },

    /* APP */



    /* PLAYER */
    GP_Player_GetNumberInt: function (key) {
        return _GP().PlayerGet(UTF8ToString(key));
    },
    GP_Player_GetNumberFloat: function (key) {
        return _GP().PlayerGet(UTF8ToString(key));
    },

    GP_Player_GetMaxValue: function (key) {
        return _GP().PlayerGetMaxValue(UTF8ToString(key));
    },

    GP_Player_GetMinValue: function (key) {
        return _GP().PlayerGetMinValue(UTF8ToString(key));
    },

    GP_Player_GetBool: function (key) {
        var value = _GP().PlayerGet(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Player_GetString: function (key) {
        var value = _GP().PlayerGet(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Player_GetID: function () {
        return _GP().PlayerGetID();
    },
    GP_Player_GetScore: function () {
        return _GP().PlayerGetScore();
    },
    GP_Player_GetName: function () {
        var value = _GP().PlayerGetName();
        return _ToBuff(value);
    },
    GP_Player_GetAvatar: function () {
        var value = _GP().PlayerGetAvatar();
        return _ToBuff(value);
    },
    GP_Player_GetFieldName: function (key) {
        return _GP().PlayerGetFieldName(UTF8ToString(key));
    },
    GP_Player_GetFieldVariantName: function (key, value) {
        return _GP().PlayerGetFieldVariantName(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_GetFieldVariantAt: function (key, index) {
        return _GP().PlayerGetFieldVariantAt(UTF8ToString(key), UTF8ToString(index));
    },
    GP_Player_GetFieldVariantIndex: function (key, value) {
        return _GP().PlayerGetFieldVariantIndex(UTF8ToString(key), UTF8ToString(value));
    },

    GP_Player_SetName: function (name) {
        _GP().PlayerSetName(UTF8ToString(name));
    },
    GP_Player_SetAvatar: function (src) {
        _GP().PlayerSetAvatar(UTF8ToString(src));
    },
    GP_Player_SetScore: function (score) {
        _GP().PlayerSetScore(score);
    },
    GP_Player_AddScore: function (score) {
        _GP().PlayerAddScore(score);
    },
    GP_Player_Set_String: function (key, value) {
        _GP().PlayerSetString(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Set_Number: function (key, value) {
        _GP().PlayerSetNumber(UTF8ToString(key), value);
    },
    GP_Player_Set_Bool: function (key, value) {
        _GP().PlayerSetBool(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_SetFlag: function (key, value) {
        _GP().PlayerSetFlag(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Add: function (key, value) {
        _GP().PlayerAdd(UTF8ToString(key), UTF8ToString(value));
    },
    GP_Player_Toggle: function (key) {
        _GP().PlayerToggle(UTF8ToString(key));
    },
    GP_Player_Reset: function () {
        _GP().PlayerReset();
    },
    GP_Player_Remove: function () {
        _GP().PlayerRemove();
    },
    GP_Player_Sync: function (override) {
        _GP().PlayerSync(override);
    },
    GP_Player_Load: function () {
        _GP().PlayerLoad();
    },
    GP_Player_Login: function () {
        _GP().PlayerLogin();
    },
    GP_Player_Logout: function () {
        _GP().PlayerLogout();
    },
    GP_Player_FetchFields: function () {
        _GP().PlayerFetchFields();
    },

    GP_Player_Has: function (key) {
        var value = _GP().PlayerHas(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Player_IsLoggedIn: function () {
        var value = _GP().PlayerIsLoggedIn();
        return _ToBuff(value);
    },
    GP_Player_HasAnyCredentials: function () {
        var value = _GP().PlayerHasAnyCredentials();
        return _ToBuff(value);
    },
    GP_Player_IsStub: function () {
        var value = _GP().PlayerIsStub();
        return _ToBuff(value);
    },

    // Player stats 

    GP_Player_GetActiveDays: function () {
        return _GP().PlayerGetActiveDays();
    },
    GP_Player_GetActiveDaysConsecutive: function () {
        return _GP().PlayerGetActiveDaysConsecutive();
    },
    GP_Player_GetPlaytimeToday: function () {
        return _GP().PlayerGetPlaytimeToday();
    },
    GP_Player_GetPlaytimeAll: function () {
        return _GP().PlayerGetPlaytimeAll();
    },

    /* PLAYER */



    /* LEADER BOARD */
    GP_Leaderboard_Open: function (orderBy, order, limit, showNearest, withMe, includeFields, displayFields) {
        _GP().LeaderboardOpen(UTF8ToString(orderBy), UTF8ToString(order), limit, showNearest, UTF8ToString(withMe), UTF8ToString(includeFields), UTF8ToString(displayFields));
    },
    GP_Leaderboard_Fetch: function (tag, orderBy, order, limit, showNearest, withMe, includeFields) {
        _GP().LeaderboardFetch(UTF8ToString(tag), UTF8ToString(orderBy), UTF8ToString(order), limit, showNearest, UTF8ToString(withMe), UTF8ToString(includeFields));
    },
    GP_Leaderboard_FetchPlayerRating: function (tag, orderBy, order) {
        _GP().LeaderboardFetchPlayerRating(UTF8ToString(tag), UTF8ToString(orderBy), UTF8ToString(order));
    },
    /* LEADER BOARD */



    /* LEADER BOARD SCOPED */
    GP_Leaderboard_Scoped_Open: function (idOrTag, variant, order, limit, showNearest, includeFields, displayFields, withMe) {
        _GP().LeaderboardScopedOpen(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(order), limit, showNearest, UTF8ToString(includeFields), UTF8ToString(displayFields), UTF8ToString(withMe));
    },
    GP_Leaderboard_Scoped_Fetch: function (idOrTag, variant, order, limit, showNearest, includeFields, withMe) {
        _GP().LeaderboardScopedFetch(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(order), limit, showNearest, UTF8ToString(includeFields), UTF8ToString(withMe));
    },
    GP_Leaderboard_Scoped_PublishRecord: function (idOrTag, variant, override, key1, value1, key2, value2, key3, value3) {
        _GP().LeaderboardScopedPublishRecord(UTF8ToString(idOrTag), UTF8ToString(variant), override, UTF8ToString(key1), value1, UTF8ToString(key2), value2, UTF8ToString(key3), value3);
    },
    GP_Leaderboard_Scoped_FetchPlayerRating: function (idOrTag, variant, includeFields) {
        _GP().LeaderboardScopedFetchPlayerRating(UTF8ToString(idOrTag), UTF8ToString(variant), UTF8ToString(includeFields));
    },
    /* LEADER BOARD SCOPED */



    /* ACHIEVEMENTS */
    GP_Achievements_Open: function () {
        _GP().AchievementsOpen();
    },
    GP_Achievements_Fetch: function () {
        _GP().AchievementsFetch();
    },
    GP_Achievements_Unlock: function (idOrTag) {
        _GP().AchievementsUnlock(UTF8ToString(idOrTag));
    },


    GP_Achievements_SetProgress: function (idOrTag, progress) {
        _GP().AchievementsSetProgress(UTF8ToString(idOrTag), progress);
    },
    GP_Achievements_Has: function (idOrTag) {
        var value = _GP().AchievementsHas(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },
    GP_Achievements_GetProgress: function (idOrTag) {
        return _GP().AchievementsGetProgress(UTF8ToString(idOrTag));
    },
    /* ACHIEVEMENTS */


    /* PAYMENTS */
    GP_Payments_FetchProducts: function () {
        _GP().PaymentsFetchProducts();
    },
    GP_Payments_Purchase: function (idOrTag) {
        _GP().PaymentsPurchase(UTF8ToString(idOrTag));
    },
    GP_Payments_Consume: function (idOrTag) {
        _GP().PaymentsConsume(UTF8ToString(idOrTag));
    },

    GP_Payments_IsAvailable: function () {
        var value = _GP().PaymentsIsAvailable();
        return _ToBuff(value);
    },

    /* Subscription */
    GP_Payments_IsSubscriptionsAvailable: function () {
        var value = _GP().PaymentsIsSubscriptionsAvailable();
        return _ToBuff(value);
    },
    GP_Payments_Subscribe: function (idOrTag) {
        _GP().PaymentsSubscribe(UTF8ToString(idOrTag));
    },
    GP_Payments_Unsubscribe: function (idOrTag) {
        _GP().PaymentsUnsubscribe(UTF8ToString(idOrTag));
    },

    /* Subscription */

    /* PAYMENTS */



    /* FULLSCREEN */
    GP_Fullscreen_Open: function () {
        _GP().FullscreenOpen();
    },
    GP_Fullscreen_Close: function () {
        _GP().FullscreenClose();
    },
    GP_Fullscreen_Toggle: function () {
        _GP().FullscreenToggle();
    },
    /* FULLSCREEN */



    /* ADS */
    GP_Ads_ShowFullscreen: function () {
        _GP().AdsShowFullscreen();
    },
    GP_Ads_ShowRewarded: function (Tag) {
        _GP().AdsShowRewarded(UTF8ToString(Tag));
    },
    GP_Ads_ShowPreloader: function () {
        _GP().AdsShowPreloader();
    },
    GP_Ads_ShowSticky: function () {
        _GP().AdsShowSticky();
    },
    GP_Ads_CloseSticky: function () {
        _GP().AdsCloseSticky();
    },
    GP_Ads_RefreshSticky: function () {
        _GP().AdsRefreshSticky();
    },



    GP_Ads_IsAdblockEnabled: function () {
        var value = _GP().AdsIsAdblockEnabled();
        return _ToBuff(value);
    },


    GP_Ads_IsStickyAvailable: function () {
        var value = _GP().AdsIsStickyAvailable();
        return _ToBuff(value);

    },
    GP_Ads_IsFullscreenAvailable: function () {
        var value = _GP().AdsIsFullscreenAvailable();
        return _ToBuff(value);

    },
    GP_Ads_IsRewardedAvailable: function () {
        var value = _GP().AdsIsRewardedAvailable();
        return _ToBuff(value);
    },
    GP_Ads_IsPreloaderAvailable: function () {
        var value = _GP().AdsIsPreloaderAvailable();
        return _ToBuff(value);
    },
    GP_Ads_IsStickyPlaying: function () {
        var value = _GP().AdsIsStickyPlaying();
        return _ToBuff(value);
    },
    GP_Ads_IsFullscreenPlaying: function () {
        var value = _GP().AdsIsFullscreenPlaying();
        return _ToBuff(value);
    },
    GP_Ads_IsRewardedPlaying: function () {
        var value = _GP().AdsIsRewardedPlaying();
        return _ToBuff(value);
    },
    GP_Ads_IsPreloaderPlaying: function () {
        var value = _GP().AdsIsPreloaderPlaying();
        return _ToBuff(value);
    },
    GP_Ads_IsCountdownOverlayEnabled: function () {
        var value = _GP().AdsIsCountdownOverlayEnabled();
        return _ToBuff(value);
    },
    GP_Ads_IsRewardedFailedOverlayEnabled: function () {
        var value = _GP().AdsIsRewardedFailedOverlayEnabled();
        return _ToBuff(value);
    },
    GP_Ads_CanShowFullscreenBeforeGamePlay: function () {
        var value = _GP().AdsCanShowFullscreenBeforeGamePlay();
        return _ToBuff(value);
    },

    /* ADS */



    /* ANALYTICS */
    GP_Analytics_Hit: function (url) {
        _GP().AnalyticsHit(UTF8ToString(url));
    },
    GP_Analytics_Goal: function (event, value) {
        _GP().AnalyticsGoal(UTF8ToString(event), UTF8ToString(value));
    },
    /* ANALYTICS */



    /* SOCIALS */
    GP_Socials_Share: function (text, url, image) {
        _GP().SocialsShare(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },


    GP_Socials_Post: function (text, url, image) {
        _GP().SocialsPost(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },
    GP_Socials_Invite: function (text, url, image) {
        _GP().SocialsInvite(UTF8ToString(text), UTF8ToString(url), UTF8ToString(image));
    },
    GP_Socials_JoinCommunity: function () {
        _GP().SocialsJoinCommunity();
    },


    GP_Socials_CommunityLink: function () {
        var value = _GP().SocialsCommunityLink();
        return _ToBuff(value);
    },

    GP_Socials_IsSupportsShare: function () {
        var value = _GP().SocialsIsSupportsShare();
        return _ToBuff(value);
    },
    GP_Socials_IsSupportsNativeShare: function () {
        var value = _GP().SocialsIsSupportsNativeShare();
        return _ToBuff(value);
    },
    GP_Socials_IsSupportsNativePosts: function () {
        var value = _GP().SocialsIsSupportsNativePosts();
        return _ToBuff(value);
    },
    GP_Socials_IsSupportsNativeInvite: function () {
        var value = _GP().SocialsIsSupportsNativeInvite();
        return _ToBuff(value);
    },
    GP_Socials_CanJoinCommunity: function () {
        var value = _GP().SocialsCanJoinCommunity();
        return _ToBuff(value);
    },
    GP_Socials_IsSupportsNativeCommunityJoin: function () {
        var value = _GP().SocialsIsSupportsNativeCommunityJoin();
        return _ToBuff(value);

    },

    GP_Socials_MakeShareLink: function (content) {
        var value = _GP().SocialsMakeShareLink(UTF8ToString(content));
        return _ToBuff(value);
    },

    GP_Socials_GetSharePlayerID: function () {
        return _GP().SocialsGetSharePlayerID();
    },
    GP_Socials_GetShareContent: function () {
        var value = _GP().SocialsGetShareContent();
        return _ToBuff(value);
    },
    /* SOCIALS */



    /* GAMES COLLECTIONS */
    GP_GamesCollections_Open: function (idOrTag) {
        _GP().GamesCollectionsOpen(UTF8ToString(idOrTag));
    },
    GP_GamesCollections_Fetch: function (idOrTag) {
        _GP().GamesCollectionsFetch(UTF8ToString(idOrTag));
    },
    /* GAMES COLLECTIONS */



    /*GAME*/
    GP_IsPaused: function () {
        var value = _GP().IsPaused();
        return _ToBuff(value);
    },

    GP_Pause: function () {
        _GP().Pause();
    },
    GP_Resume: function () {
        _GP().Resume();
    },

    GP_GameplayStart: function () {
        _GP().GameplayStart();
    },

    GP_GameplayStop: function () {
        _GP().GameplayStop();
    },

    GP_GameReady: function(){
        _GP().GameReady();
    },

    GP_HappyTime: function(){
        _GP().HappyTime();
    },

    /*GAME*/



    /*DEVICE*/
    GP_IsMobile: function () {
        var value = _GP().IsMobile();
        return _ToBuff(value);
    },

    GP_IsPortrait: function () {
        var value = _GP().IsPortrait();
        return _ToBuff(value);
    },

    /*DEVICE*/



    /*SERVER*/
    GP_ServerTime: function () {
        var value = _GP().ServerTime();
        return _ToBuff(value);
    },
    /*SERVER*/



    /*SYSTEM*/
    GP_IsDev: function () {
        var value = _GP().IsDev();
        return _ToBuff(value);
    },

    GP_IsAllowedOrigin: function () {
        var value = _GP().IsAllowedOrigin();
        return _ToBuff(value);
    },
    /*SYSTEM*/

    /*VARIABLES*/
    GP_Variables_Fetch: function () {
        _GP().VariablesFetch();
    },

    GP_Variables_Has: function (key) {
        var value = _GP().VariablesHas(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Variables_GetNumberInt: function (key) {
        return _GP().VariablesGet(UTF8ToString(key));
    },

    GP_Variables_GetFloat: function (key) {
        return _GP().VariablesGet(UTF8ToString(key));
    },

    GP_Variables_GetString: function (key) {
        var value = _GP().VariablesGet(UTF8ToString(key));
        return _ToBuff(value);
    },
    GP_Variables_GetBool: function (key) {
        var value = _GP().VariablesGet(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Variables_GetImage: function (key) {
        var value = _GP().VariablesGet(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Variables_GetFile: function (key) {
        var value = _GP().VariablesGet(UTF8ToString(key));
        return _ToBuff(value);
    },

    GP_Variables_IsPlatformVariablesAvailable: function(){
        var value = _GP().VariablesIsPlatformVariablesAvailable();
        return _ToBuff(value);
    },
    
    GP_Variables_FetchPlatformVariables: function (params) {
        _GP().VariablesFetchPlatformVariables(UTF8ToString(params));
    },

    /*VARIABLES*/



    /* PLAYERS */
    GP_Players_Fetch: function (key) {
        _GP().PlayersFetch(UTF8ToString(key));
    },
    /* PLAYERS */



    /* DOCUMENTS */
    GP_Documents_Open: function () {
        _GP().DocumentsOpen();
    },
    GP_Documents_Fetch: function () {
        _GP().DocumentsFetch();
    },
    /* DOCUMENTS */



    /* FILES */
    GP_Files_Upload: function (tags) {
        _GP().FilesUpload(UTF8ToString(tags));
    },
    GP_Files_UploadUrl: function (url, filename, tags) {
        _GP().FilesUploadUrl(UTF8ToString(url), UTF8ToString(filename), UTF8ToString(tags));
    },
    GP_Files_UploadContent: function (content, filename, tags) {
        _GP().FilesUploadContent(UTF8ToString(content), UTF8ToString(filename), UTF8ToString(tags));
    },
    GP_Files_LoadContent: function (url) {
        _GP().FilesLoadContent(UTF8ToString(url));
    },
    GP_Files_ChooseFile: function (type) {
        _GP().FilesChooseFile(UTF8ToString(type));
    },
    GP_Files_Fetch: function (filter) {
        _GP().FilesFetch(UTF8ToString(filter));
    },
    GP_Files_FetchMore: function (filter) {
        _GP().FilesFetchMore(UTF8ToString(filter));
    },
    /* FILES */


    /* CHANNELS */
    GP_Channels_OpenChat: function (channel_ID) {
        _GP().Channels_Open_Chat(channel_ID);
    },

    GP_Channels_OpenChatWithTags: function (channel_ID, tags) {
        _GP().Channels_Open_Chat_WithTags(channel_ID, UTF8ToString(tags));
    },

    GP_Channels_OpenPersonalChat: function (player_ID, tags) {
        _GP().Channels_Open_Personal_Chat(player_ID, UTF8ToString(tags));
    },

    GP_Channels_OpenFeed: function (player_ID, tags) {
        _GP().Channels_Open_Feed(player_ID, UTF8ToString(tags));
    },

    GP_Channels_IsMainChatEnabled: function () {
        var value = _GP().Channels_IsMainChatEnabled();
        return _ToBuff(value);
    },
    GP_Channels_MainChatId: function () {
        return _GP().Channels_MainChatId();
    },

    GP_Channels_Join: function (channel_ID, password) {
        _GP().Channels_Join(channel_ID, UTF8ToString(password));
    },
    GP_Channels_CancelJoin: function (channel_ID) {
        _GP().Channels_CancelJoin(channel_ID);
    },
    GP_Channels_Leave: function (channel_ID) {
        _GP().Channels_Leave(channel_ID);
    },
    GP_Channels_Kick: function (channel_ID, player_ID) {
        _GP().Channels_Kick(channel_ID, player_ID);
    },
    GP_Channels_Mute_UnmuteAt: function (channel_ID, player_ID, unmuteAt) {
        _GP().Channels_Mute_UnmuteAt(channel_ID, player_ID, UTF8ToString(unmuteAt));
    },
    GP_Channels_Mute_Seconds: function (channel_ID, player_ID, seconds) {
        _GP().Channels_Mute_Seconds(channel_ID, player_ID, seconds);
    },
    GP_Channels_UnMute: function (channel_ID, player_ID) {
        _GP().Channels_UnMute(channel_ID, player_ID);
    },
    GP_Channels_SendInvite: function (channel_ID, player_ID) {
        _GP().Channels_SendInvite(channel_ID, player_ID);
    },
    GP_Channels_CancelInvite: function (channel_ID, player_ID) {
        _GP().Channels_CancelInvite(channel_ID, player_ID);
    },
    GP_Channels_AcceptInvite: function (channel_ID) {
        _GP().Channels_AcceptInvite(channel_ID);
    },
    GP_Channels_RejectInvite: function (channel_ID) {
        _GP().Channels_RejectInvite(channel_ID);
    },
    GP_Channels_FetchInvites: function (limit, offset) {
        _GP().Channels_FetchInvites(limit, offset);
    },
    GP_Channels_FetchMoreInvites: function (limit) {
        _GP().Channels_FetchMoreInvites(limit);
    },
    GP_Channels_FetchChannelInvites: function (channel_ID, limit, offset) {
        _GP().Channels_FetchChannelInvites(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreChannelInvites: function (channel_ID, limit) {
        _GP().Channels_FetchMoreChannelInvites(channel_ID, limit);
    },
    GP_Channels_FetchSentInvites: function (channel_ID, limit, offset) {
        _GP().Channels_FetchSentInvites(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreSentInvites: function (channel_ID, limit) {
        _GP().Channels_FetchMoreSentInvites(channel_ID, limit);
    },
    GP_Channels_AcceptJoinRequest: function (channel_ID, player_ID) {
        _GP().Channels_AcceptJoinRequest(channel_ID, player_ID);
    },
    GP_Channels_RejectJoinRequest: function (channel_ID, player_ID) {
        _GP().Channels_RejectJoinRequest(channel_ID, player_ID);
    },
    GP_Channels_FetchJoinRequests: function (channel_ID, limit, offset) {
        _GP().Channels_FetchJoinRequests(channel_ID, limit, offset);
    },
    GP_Channels_FetchMoreJoinRequests: function (channel_ID, limit) {
        _GP().Channels_FetchMoreJoinRequests(channel_ID, limit);
    },
    GP_Channels_FetchSentJoinRequests: function (limit, offset) {
        _GP().Channels_FetchSentJoinRequests(limit, offset);
    },
    GP_Channels_FetchMoreSentJoinRequests: function (limit) {
        _GP().Channels_FetchMoreSentJoinRequests(limit);
    },
    GP_Channels_SendMessage: function (channel_ID, text, tags) {
        _GP().Channels_SendMessage(channel_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_SendPersonalMessage: function (player_ID, text, tags) {
        _GP().Channels_SendPersonalMessage(player_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_SendFeedMessage: function (player_ID, text, tags) {
        _GP().Channels_SendFeedMessage(player_ID, UTF8ToString(text), UTF8ToString(tags));
    },
    GP_Channels_EditMessage: function (message_ID, text) {
        _GP().Channels_EditMessage(UTF8ToString(message_ID), UTF8ToString(text));
    },
    GP_Channels_DeleteMessage: function (message_ID) {
        _GP().Channels_DeleteMessage(UTF8ToString(message_ID));
    },
    GP_Channels_FetchMessages: function (channel_ID, tags, limit, offset) {
        _GP().Channels_FetchMessages(channel_ID, UTF8ToString(tags), limit, offset);
    },
    GP_Channels_FetchPersonalMessages: function (player_ID, tags, limit, offset) {
        _GP().Channels_FetchPersonalMessages(player_ID, UTF8ToString(tags), limit, offset);
    },
    GP_Channels_FetchFeedMessages: function (player_ID, tags, limit, offset) {
        _GP().Channels_FetchFeedMessages(player_ID, UTF8ToString(tags), limit, offset);
    },

    GP_Channels_FetchMoreMessages: function (channel_ID, tags, limit) {
        _GP().Channels_FetchMoreMessages(channel_ID, UTF8ToString(tags), limit);
    },
    GP_Channels_FetchMorePersonalMessages: function (player_ID, tags, limit) {
        _GP().Channels_FetchMorePersonalMessages(player_ID, UTF8ToString(tags), limit);
    },
    GP_Channels_FetchMoreFeedMessages: function (player_ID, tags, limit) {
        _GP().Channels_FetchMoreFeedMessages(player_ID, UTF8ToString(tags), limit);
    },

    GP_Channels_DeleteChannel: function (channel_ID) {
        _GP().Channels_DeleteChannel(channel_ID);
    },
    GP_Channels_FetchChannel: function (channel_ID) {
        _GP().Channels_FetchChannel(channel_ID);
    },

    GP_Channels_CreateChannel: function (filter) {
        _GP().Channels_CreateChannel(UTF8ToString(filter));
    },

    GP_Channels_UpdateChannel: function (filter) {
        _GP().Channels_UpdateChannel(UTF8ToString(filter));
    },

    GP_Channels_FetchChannels: function (filter) {
        _GP().Channels_FetchChannels(UTF8ToString(filter));
    },

    GP_Channels_FetchMoreChannels: function (filter) {
        _GP().Channels_FetchMoreChannels(UTF8ToString(filter));
    },

    GP_Channels_FetchMembers: function (filter) {
        _GP().Channels_FetchMembers(UTF8ToString(filter));
    },

    GP_Channels_FetchMoreMembers: function (filter) {
        _GP().Channels_FetchMoreMembers(UTF8ToString(filter));
    },

    /* CHANNELS */

    /* TRIGGERS */
    GP_Triggers_Claim: function (idOrTag) {
        _GP().Triggers_Claim(UTF8ToString(idOrTag));
    },

    GP_Triggers_List: function () {
        var value = _GP().Triggers_List();
        return _ToBuff(value);
    },

    GP_Triggers_ActivatedList: function () {
        var value = _GP().Triggers_ActivatedList();
        return _ToBuff(value);
    },

    GP_Triggers_GetTrigger: function (idOrTag) {
        var value = _GP().Triggers_GetTrigger(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Triggers_IsActivated: function (idOrTag) {
        var value = _GP().Triggers_IsActivated(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Triggers_IsClaimed: function (idOrTag) {
        var value = _GP().Triggers_IsClaimed(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },
    /* TRIGGERS */

    /* EVENTS */
    GP_Events_Join: function (idOrTag) {
        _GP().Events_Join(UTF8ToString(idOrTag));
    },

    GP_Events_List: function () {
        var value = _GP().Events_List();
        return _ToBuff(value);
    },

    GP_Events_ActiveList: function () {
        var value = _GP().Events_ActiveList();
        return _ToBuff(value);
    },

    GP_Events_GetEvent: function (idOrTag) {
        var value = _GP().Events_GetEvent(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Events_IsActive: function (idOrTag) {
        var value = _GP().Events_IsActive(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Events_IsJoined: function (idOrTag) {
        var value = _GP().Events_IsJoined(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },
    /* EVENTS */

    /* SEGMENTS */
    GP_Segments_List: function () {
        var value = _GP().Segments_List();
        return _ToBuff(value);
    },

    GP_Segments_Has: function (idOrTag) {
        var value = _GP().Segments_Has(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },
    /* SEGMENTS */

    /* EXPERIMENTS */
    GP_Experiments_Map: function () {
        var value = _GP().Experiments_Map();
        return _ToBuff(value);
    },

    GP_Experiments_Has: function (tag, cohort) {
        var value = _GP().Experiments_Has(UTF8ToString(tag), UTF8ToString(cohort));
        return _ToBuff(value);
    },
    /* EXPERIMENTS */

    /* REWARDS */
    GP_Rewards_Give: function (idOrTag, lazy) {
        _GP().Rewards_Give(UTF8ToString(idOrTag), UTF8ToString(lazy));
    },

    GP_Rewards_Accept: function (idOrTag) {
        _GP().Rewards_Accept(UTF8ToString(idOrTag));
    },

    GP_Rewards_List: function () {
        var value = _GP().Rewards_List();
        return _ToBuff(value);
    },

    GP_Rewards_GivenList: function () {
        var value = _GP().Rewards_GivenList();
        return _ToBuff(value);
    },

    GP_Rewards_GetReward: function (idOrTag) {
        var value = _GP().Rewards_GetReward(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Rewards_Has: function (idOrTag) {
        var value = _GP().Rewards_Has(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Rewards_HasAccepted: function (idOrTag) {
        var value = _GP().Rewards_HasAccepted(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Rewards_HasUnaccepted: function (idOrTag) {
        var value = _GP().Rewards_HasUnaccepted(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },
    /* REWARDS */

    /* SCHEDULERS */
    GP_Schedulers_Register: function (idOrTag) {
        _GP().Schedulers_Register(UTF8ToString(idOrTag));
    },

    GP_Schedulers_ClaimDay: function (idOrTag, day) {
        _GP().Schedulers_ClaimDay(UTF8ToString(idOrTag), day);
    },

    GP_Schedulers_ClaimDayAdditional: function (idOrTag, day, triggerIdOrTag) {
        _GP().Schedulers_ClaimDayAdditional(UTF8ToString(idOrTag), day, UTF8ToString(triggerIdOrTag));
    },

    GP_Schedulers_ClaimAllDay: function (idOrTag, day) {
        _GP().Schedulers_ClaimAllDay(UTF8ToString(idOrTag), day);
    },

    GP_Schedulers_ClaimAllDays: function (idOrTag) {
        _GP().Schedulers_ClaimAllDays(UTF8ToString(idOrTag));
    },

    GP_Schedulers_List: function () {
        var value = _GP().Schedulers_List();
        return _ToBuff(value);
    },

    GP_Schedulers_ActiveList: function () {
        var value = _GP().Schedulers_ActiveList();
        return _ToBuff(value);
    },

    GP_Schedulers_GetScheduler: function (idOrTag) {
        var value = _GP().Schedulers_GetScheduler(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Schedulers_GetSchedulerDay: function (idOrTag, day) {
        var value = _GP().Schedulers_GetSchedulerDay(UTF8ToString(idOrTag), day);
        return _ToBuff(value);
    },

    GP_Schedulers_GetSchedulerCurrentDay: function (idOrTag) {
        var value = _GP().Schedulers_GetSchedulerCurrentDay(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Schedulers_IsRegistered: function (idOrTag) {
        var value = _GP().Schedulers_IsRegistered(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Schedulers_IsTodayRewardClaimed: function (idOrTag) {
        var value = _GP().Schedulers_IsTodayRewardClaimed(UTF8ToString(idOrTag));
        return _ToBuff(value);
    },

    GP_Schedulers_CanClaimDay: function (idOrTag, day) {
        var value = _GP().Schedulers_CanClaimDay(UTF8ToString(idOrTag), day);
        return _ToBuff(value);
    },

    GP_Schedulers_CanClaimDayAdditional: function (idOrTag, day, trigger) {
        var value = _GP().Schedulers_CanClaimDayAdditional(UTF8ToString(idOrTag), day,  UTF8ToString(trigger));
        return _ToBuff(value);
    },

    GP_Schedulers_CanClaimAllDay: function (idOrTag, day) {
        var value = _GP().Schedulers_CanClaimAllDay(UTF8ToString(idOrTag), day);
        return _ToBuff(value);
    },

    /* SCHEDULERS */

    /* IMAGES */

    GP_Images_Upload: function (tags) {
        _GP().ImagesUpload(UTF8ToString(tags));
    },
    GP_Images_UploadUrl: function (url, tags) {
        _GP().ImagesUploadUrl(UTF8ToString(url), UTF8ToString(tags));
    },
    GP_Images_Choose: function () {
        _GP().ImagesChooseFile();
    },
    GP_Images_Fetch: function (filter) {
        _GP().ImagesFetch(UTF8ToString(filter));
    },
    GP_Images_FetchMore: function (filter) {
        _GP().ImagesFetchMore(UTF8ToString(filter));
    },
    GP_Images_Resize: function (params) {
        _GP().ImagesResize(UTF8ToString(params));
    },

    /* IMAGES */

    /* CUSTOM */
    GP_CustomCall: function(name, args){
        _GP().CustomCall(UTF8ToString(name), UTF8ToString(args));
    },

    GP_CustomReturn: function(name, args){
        var value = _GP().CustomReturn(UTF8ToString(name), UTF8ToString(args));
        return _ToBuff(value);
    },

    GP_CustomGetValue: function(name){
        var value = _GP().CustomGetValue(UTF8ToString(name));
        return _ToBuff(value);
    },

    GP_CustomAsyncReturn: function(name, args){
        _GP().CustomAsyncReturn(UTF8ToString(name), UTF8ToString(args));
    },
    
    /* CUSTOM */

    /* LOGGER */
    GP_LoggerInfo: function(title, text){
        _GP().LoggerInfo(UTF8ToString(title), UTF8ToString(text));
    },
    GP_LoggerWarn: function(title, text){
        _GP().LoggerWarn(UTF8ToString(title), UTF8ToString(text));
    },
    GP_LoggerError: function(title, text){
        _GP().LoggerError(UTF8ToString(title), UTF8ToString(text));
    },
    GP_LoggerLog: function(title, text){
        _GP().LoggerLog(UTF8ToString(title), UTF8ToString(text));
    },
    /* LOGGER */

     /* UNIQUES */
    GP_UniquesRegister: function(tag, value){
        _GP().UniquesRegister(UTF8ToString(tag), UTF8ToString(value));
    },
    GP_UniquesGet: function(tag){
        return _ToBuff(_GP().UniquesGet(UTF8ToString(tag)));
    },
    GP_UniquesList: function(){
        return _ToBuff(_GP().UniquesList());
    },
    GP_UniquesCheck: function(tag, value){
        _GP().UniquesCheck(UTF8ToString(tag), UTF8ToString(value));
    },
    GP_UniquesDelete: function(tag){
        _GP().UniquesDelete(UTF8ToString(tag));
    },
     /* UNIQUES */
});
