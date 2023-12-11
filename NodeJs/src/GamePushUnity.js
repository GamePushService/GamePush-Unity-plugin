export default class GamePushUnity {
    constructor(gp) {
        this.gp = gp;

        this.gp.player.on('change', () => this.trigger('CallPlayerChange'));
        this.gp.player.on('sync', (success) => {
            this.trigger(success ? 'CallPlayerSyncComplete' : 'CallPlayerSyncError');
        });
        this.gp.player.on('load', (success) => {
            this.trigger(success ? 'CallPlayerLoadComplete' : 'CallPlayerLoadError');
        });
        this.gp.player.on('login', (success) => {
            this.trigger(success ? 'CallPlayerLoginComplete' : 'CallPlayerLoginError');
        });

        this.gp.on('event:connect', () => this.trigger('CallPlayerConnect'))

        this.gp.player.on('fetchFields', (success) => {
            if (success) {
                this.trigger('CallPlayerFetchFieldsComplete', JSON.stringify(this.gp.player.fields.map((field) => ({ ...field, defaultValue: field.default }))));
            } else {
                this.trigger('CallPlayerFetchFieldsError');
            }
        });

        // leaderboard
        this.gp.leaderboard.on('open', () => this.trigger('CallLeaderboardOpen'));
        this.gp.leaderboard.on('close', () => this.trigger('CallLeaderboardClose'));

        // achievements
        this.gp.achievements.on('open', () => this.trigger('CallAchievementsOpen'));
        this.gp.achievements.on('close', () => {
            this.trigger('CallAchievementsClose');
            window.focus();
        });

        this.gp.achievements.on('unlock', () => this.trigger('CallAchievementsUnlock'));
        this.gp.achievements.on('error:unlock', () => this.trigger('CallAchievementsUnlockError'));

        // games collections
        this.gp.gamesCollections.on('open', () => this.trigger('CallGamesCollectionsOpen'));
        this.gp.gamesCollections.on('close', () => {
            this.trigger('CallGamesCollectionsClose');
            window.focus();
        });

        // fullscreen
        this.gp.fullscreen.on('open', () => this.trigger('CallFullscreenOpen'));
        this.gp.fullscreen.on('close', () => this.trigger('CallFullscreenClose'));
        this.gp.fullscreen.on('change', () => this.trigger('CallFullscreenChange'));

        // ads
        this.gp.ads.on('start', () => this.trigger('CallAdsStart'));
        this.gp.ads.on('close', (success) => {
            this.trigger('CallAdsClose', success);
            window.focus();
        });

        this.gp.ads.on('fullscreen:start', () => this.trigger('CallAdsFullscreenStart'));
        this.gp.ads.on('fullscreen:close', (success) => this.trigger('CallAdsFullscreenClose', success));

        this.gp.ads.on('preloader:start', () => this.trigger('CallAdsPreloaderStart'));
        this.gp.ads.on('preloader:close', (success) => this.trigger('CallAdsPreloaderClose', success));

        this.gp.ads.on('rewarded:start', () => this.trigger('CallAdsRewardedStart'));
        this.gp.ads.on('rewarded:close', (success) => this.trigger('CallAdsRewardedClose', (success)));
        this.gp.ads.on('rewarded:reward', () => this.trigger('CallAdsRewardedReward', this.lastRewardedTag));

        this.gp.ads.on('sticky:start', () => this.trigger('CallAdsStickyStart'));
        this.gp.ads.on('sticky:close', () => this.trigger('CallAdsStickyClose'));
        this.gp.ads.on('sticky:refresh', () => this.trigger('CallAdsStickyRefresh'));
        this.gp.ads.on('sticky:render', () => this.trigger('CallAdsStickyRender'));

        // socials
        this.gp.socials.on('share', (success) => {
            this.trigger('CallSocialsShare', success);
            window.focus();
        });
        this.gp.socials.on('post', (success) => {
            this.trigger('CallSocialsPost', success);
            window.focus();
        });
        this.gp.socials.on('invite', (success) => {
            this.trigger('CallSocialsInvite', success);
            window.focus();
        });
        this.gp.socials.on('joinCommunity', (success) => {
            this.trigger('CallSocialsJoinCommunity', success);
            window.focus();
        });

        // gp
        this.gp.on('change:language', (lang) => this.trigger('CallChangeLanguage', lang));
        this.gp.on('change:avatarGenerator', (ag) => this.trigger('CallChangeAvatarGenerator', ag));
        this.gp.on('pause', () => this.trigger('CallOnPause'));
        this.gp.on('resume', () => this.trigger('CallOnResume'));
        
        //device
        this.gp.on('change:orientation', () => this.trigger('CallChangeOrientation'));
        
        // app
        //this.gp.app.on('requestReview', (result) => this.trigger('CallReviewResult', result));
        //this.gp.app.on('addShortcut', (success) => this.trigger('CallAddShortcut', success));

        //documents
        this.gp.documents.on('open', () => this.trigger('CallOnDocumentsOpen'));
        this.gp.documents.on('close', () => {
            this.trigger('CallOnDocumentsClose');
            window.focus();
        });

        this.gp.documents.on('fetch', (document) => this.trigger('CallOnDocumentsFetchSuccess', document.content));
        this.gp.documents.on('error:fetch', () => this.trigger('CallOnDocumentsFetchError'));


        // channels
        this.gp.channels.on('createChannel', (channel) => {
            this.trigger('CallOnCreateChannel', JSON.stringify(mapChannel(channel)));
        });
        this.gp.channels.on('error:createChannel', (err) => this.trigger('CallOnCreateChannelError'));

        this.gp.channels.on('updateChannel', (channel) => {
            this.trigger('CallOnUpdateChannel', JSON.stringify(mapChannel(channel)));
        });
        this.gp.channels.on('error:updateChannel', (err) => this.trigger('CallOnUpdateChannelError'));

        this.gp.channels.on('deleteChannel', () => this.trigger('CallOnDeleteChannelSuccess'));
        this.gp.channels.on('event:deleteChannel', (channel) => { this.trigger('CallOnDeleteChannelEvent', channel.id); });
        this.gp.channels.on('error:deleteChannel', (err) => this.trigger('CallOnDeleteChannelError'));

        this.gp.channels.on('fetchChannel', (channel) => {
            this.trigger('CallOnFetchChannel', JSON.stringify(mapChannel(channel)));
        });
        this.gp.channels.on('error:fetchChannel', (err) => this.trigger('CallOnFetchChannelError'));

        this.gp.channels.on('fetchChannels', (result) => {
            this.trigger('CallOnFetchChannelsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchChannels', JSON.stringify(result.items.map(mapChannel)));
        });
        this.gp.channels.on('error:fetchChannels', (err) => this.trigger('CallOnFetchChannelsError'));

        this.gp.channels.on('fetchMoreChannels', (result) => {
            this.trigger('CallOnFetchMoreChannelsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreChannels', JSON.stringify(result.items.map(mapChannel)));
        });
        this.gp.channels.on('error:fetchMoreChannels', (err) => this.trigger('CallOnFetchMoreChannelsError'));

        gp.channels.on('openChat', () => this.trigger('CallOnOpenChat'));
        gp.channels.on('closeChat', () => this.trigger('CallOnCloseChat'));
        gp.channels.on('error:openChat', (err) => this.trigger('CallOnOpenChatError'));

        this.gp.channels.on('join', () => this.trigger('CallOnJoinSuccess'));
        this.gp.channels.on('event:join', (member) => { this.trigger('CallOnJoinEvent', JSON.stringify(member)); });
        this.gp.channels.on('error:join', (err) => this.trigger('CallOnJoinError'));


        this.gp.channels.on('event:joinRequest', (joinRequest) => { this.trigger('CallOnJoinRequest', JSON.stringify(joinRequest)); });

        this.gp.channels.on('cancelJoin', () => this.trigger('CallOnCancelJoinSuccess'));
        this.gp.channels.on('event:cancelJoin', (joinRequest) => { this.trigger('CallOnCancelJoinEvent', JSON.stringify(joinRequest)); });
        this.gp.channels.on('error:cancelJoin', (err) => this.trigger('CallOnCancelJoinError'));

        this.gp.channels.on('leave', () => this.trigger('CallOnLeaveSuccess'));
        this.gp.channels.on('event:leave', (memberLeave) => { this.trigger('CallOnLeaveEvent', JSON.stringify(memberLeave)); });
        this.gp.channels.on('error:leave', (err) => this.trigger('CallOnLeaveError'));

        this.gp.channels.on('kick', () => this.trigger('CallOnKick'));
        this.gp.channels.on('error:kick', (err) => this.trigger('CallOnKickError'));

        this.gp.channels.on('fetchMembers', (result) => {
            this.trigger('CallOnFetchMembersCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMembers', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchMembers', (err) => this.trigger('CallOnFetchMembersError'));

        this.gp.channels.on('fetchMoreMembers', (result) => {
            this.trigger('CallOnFetchMoreMembersCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreMembers', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchMoreMembers', (err) => this.trigger('CallOnFetchMoreMembersError'));

        this.gp.channels.on('mute', () => this.trigger('CallOnMuteSuccess'));
        this.gp.channels.on('event:mute', (mute) => { this.trigger('CallOnMuteEvent', JSON.stringify(mute)); });
        this.gp.channels.on('error:mute', (err) => this.trigger('CallOnMuteError'));

        this.gp.channels.on('unmute', () => this.trigger('CallOnUnmuteSuccess'));
        this.gp.channels.on('event:unmute', (mute) => { this.trigger('CallOnUnmuteEvent', JSON.stringify(mute)); });
        this.gp.channels.on('error:unmute', (err) => this.trigger('CallOnUnmuteError'));

        this.gp.channels.on('sendInvite', () => this.trigger('CallOnSendInvite'));
        this.gp.channels.on('error:sendInvite', (err) => this.trigger('CallOnSendInviteError'));

        this.gp.channels.on('event:invite', (invite) => { this.trigger('CallOnInvite', JSON.stringify(invite)); });

        this.gp.channels.on('cancelInvite', () => this.trigger('CallOnCancelInviteSuccess'));
        this.gp.channels.on('event:cancelInvite', (invite) => {
            this.trigger('CallOnCancelInviteEvent', JSON.stringify(invite));
        });
        this.gp.channels.on('error:cancelInvite', (err) => this.trigger('CallOnCancelInviteError'));

        this.gp.channels.on('acceptInvite', () => this.trigger('CallOnAcceptInvite'));
        this.gp.channels.on('error:acceptInvite', (err) => this.trigger('CallOnAcceptInviteError'));


        this.gp.channels.on('rejectInvite', () => this.trigger('CallOnRejectInviteSuccess'));
        this.gp.channels.on('event:rejectInvite', (invite) => { this.trigger('CallOnRejectInviteEvent', JSON.stringify(invite)); });
        this.gp.channels.on('error:rejectInvite', (err) => this.trigger('CallOnRejectInviteError'));

        this.gp.channels.on('fetchInvites', (result) => {
            this.trigger('CallOnFetchInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchInvites', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchInvites', (err) => this.trigger('CallOnFetchInvitesError'));


        this.gp.channels.on('fetchMoreInvites', (result) => {
            this.trigger('CallOnFetchMoreInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreInvites', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchMoreInvites', (err) => this.trigger('CallOnFetchMoreInvitesError'));


        this.gp.channels.on('fetchChannelInvites', (result) => {
            this.trigger('CallOnFetchChannelInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchChannelInvites', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchChannelInvites', (err) => this.trigger('CallOnFetchChannelInvitesError'));


        this.gp.channels.on('fetchMoreChannelInvites', (result) => {
            this.trigger('CallOnFetchMoreChannelInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreChannelInvites', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchMoreChannelInvites', (err) => this.trigger('CallOnFetchMoreChannelInvitesError'));


        this.gp.channels.on('fetchSentInvites', (result) => {
            this.trigger('CallOnFetchSentInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchSentInvites', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchSentInvites', (err) => this.trigger('CallOnFetchSentInvitesError'));


        this.gp.channels.on('fetchMoreSentInvites', (result) => {
            this.trigger('CallOnFetchMoreSentInvitesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreSentInvites', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchMoreSentInvites', (err) => this.trigger('CallOnFetchMoreSentInvitesError'));


        this.gp.channels.on('acceptJoinRequest', () => this.trigger('CallOnAcceptJoinRequest'));
        this.gp.channels.on('error:acceptJoinRequest', (err) => this.trigger('CallOnAcceptJoinRequestError'));


        this.gp.channels.on('rejectJoinRequest', () => this.trigger('CallOnRejectJoinRequestSuccess'));
        this.gp.channels.on('event:rejectJoinRequest', (joinRequest) => {
            this.trigger('CallOnRejectJoinRequestEvent', JSON.stringify(joinRequest));
        });
        this.gp.channels.on('error:rejectJoinRequest', (err) => this.trigger('CallOnRejectJoinRequestError'));


        this.gp.channels.on('fetchJoinRequests', (result) => {
            this.trigger('CallOnFetchJoinRequestsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchJoinRequests', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchJoinRequests', (err) => this.trigger('CallOnFetchJoinRequestsError'));



        this.gp.channels.on('fetchMoreJoinRequests', (result) => {
            this.trigger('CallOnFetchMoreJoinRequestsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreJoinRequests', JSON.stringify(result.items));
        });
        this.gp.channels.on('error:fetchMoreJoinRequests', (err) => this.trigger('CallOnFetchMoreJoinRequestsError'));



        this.gp.channels.on('fetchSentJoinRequests', (result) => {
            this.trigger('CallOnFetchSentJoinRequestsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchSentJoinRequests', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchSentJoinRequests', (err) => this.trigger('CallOnFetchSentJoinRequestsError'));


        this.gp.channels.on('fetchMoreSentJoinRequests', (result) => {
            this.trigger('CallOnFetchMoreSentJoinRequestsCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreSentJoinRequests', JSON.stringify(result.items.map(mapItemWithChannel)));
        });
        this.gp.channels.on('error:fetchMoreSentJoinRequests', (err) => this.trigger('CallOnFetchMoreSentJoinRequestsError'));

        this.gp.channels.on('sendMessage', (message) => { this.trigger('CallOnSendMessage', JSON.stringify(message)); });
        this.gp.channels.on('error:sendMessage', (err) => this.trigger('CallOnSendMessageError'));

        this.gp.channels.on('event:message', (message) => { this.trigger('CallOnMessage', JSON.stringify(message)); });

        this.gp.channels.on('editMessage', (message) => { this.trigger('CallOnEditMessageSuccess', JSON.stringify(message)); });
        this.gp.channels.on('event:editMessage', (message) => { this.trigger('CallOnEditMessageEvent', JSON.stringify(message)); });
        this.gp.channels.on('error:editMessage', (err) => this.trigger('CallOnEditMessageError'));


        this.gp.channels.on('deleteMessage', () => this.trigger('CallOnDeleteMessageSuccess'));
        this.gp.channels.on('event:deleteMessage', (message) => { this.trigger('CallOnDeleteMessageEvent', JSON.stringify(message)); });
        this.gp.channels.on('error:deleteMessage', (err) => this.trigger('CallOnDeleteMessageError'));
    }

    trigger(eventName, value) {
        if (window.unityInstance !== null) {
            window.unityInstance.SendMessage('GamePushSDK', eventName, this.toUnity(value));
        }
    }

    toUnity(value) {
        switch (typeof (value)) {
            case 'boolean': {
                return String(value);
            }
            case 'number': {
                return value;
            }
            case 'string': {
                return value;
            }
            case 'object': {
                return JSON.stringify(value);
            }
        }
        return value;
    }

    mapItemsWithChannel(items = {}) {
        return {
            ...items,
            ch_private: items.private
        };
    }

    Language() {
        return this.gp.language;
    }
    AvatarGenerator() {
        return this.gp.avatarGenerator;
    }
    PlatformType() {
        return this.gp.platform.type;
    }
    PlatformHasIntegratedAuth() {
        return this.toUnity(this.gp.platform.hasIntegratedAuth);
    }
    PlatformIsExternalLinksAllowed() {
        return this.toUnity(this.gp.platform.isExternalLinksAllowed);
    }

    AppTitle() {
        return this.gp.app.title;
    }
    AppDescription() {
        return this.gp.app.description;
    }
    AppImage() {
        return this.gp.app.image;
    }
    AppUrl() {
        return this.gp.app.url;
    }
    AppRequestReview(){
        return this.gp.app
        .requestReview()
        .then((result) => {
            if(result.success){
                this.trigger('CallReviewResult', result.rating);
            }
            else{
                this.trigger('CallReviewClose', result.error);
            }
            
        });
    }

    AppCanRequestReview(){
        return this.toUnity(this.gp.app.canRequestReview); 
    }

    AppIsAlreadyReviewed(){
        return this.toUnity(this.gp.app.isAlreadyReviewed); 
    }

    AppAddShortcut(){
        return this.gp.app
        .addShortcut()
        .then((success) => this.trigger('CallAddShortcut', success));
    }

    AppCanAddShortcut(){
        return this.toUnity(this.gp.app.canAddShortcut); 
    }


    // PLAYER 

    PlayerGetID() {
        return this.gp.player.id;
    }

    PlayerGetScore() {
        return this.gp.player.score;
    }
    PlayerSetScore(score) {
        this.gp.player.score = Number(score);
    }
    PlayerAddScore(score) {
        this.gp.player.score += Number(score);
    }

    PlayerGetName() {
        return this.gp.player.name;
    }
    PlayerSetName(name) {
        this.gp.player.name = name;
    }

    PlayerGetAvatar() {
        return this.gp.player.avatar;
    }
    PlayerSetAvatar(src) {
        this.gp.player.avatar = src;
    }

    PlayerGet(key) {
        return this.toUnity(this.gp.player.get(key));
    }

    PlayerSetString(key, value) {
        this.gp.player.set(key, value);
    }
    PlayerSetNumber(key, value) {
        this.gp.player.set(key, value);
    }
    PlayerSetBool(key, value) {
        if (value == "True")
            value = true;
        else if (value == "False")
            value = false;
        this.gp.player.set(key, value);
    }
    PlayerAdd(key, value) {
        this.gp.player.add(key, Number(value));
    }

    PlayerHas(key) {
        return this.toUnity(this.gp.player.has(key));
    }

    PlayerSetFlag(key, value) {
        this.gp.player.set(key, !Boolean(value));
    }
    PlayerToggle(key) {
        this.gp.player.toggle(key);
    }

    PlayerGetFieldName(key) {
        return this.gp.player.getFieldName(key);
    }
    PlayerGetFieldVariantName(key, value) {
        return this.gp.player.getFieldVariantName(key, value);
    }
    PlayerGetFieldVariantAt(key, index) {
        const variant = this.gp.player.getField(key).variants[index];
        return variant ? variant.value : '';
    }
    PlayerGetFieldVariantIndex(key, value) {
        return this.gp.player.getField(key).variants.findIndex((v) => v.value === value);
    }

    PlayerReset() {
        this.gp.player.reset();
    }
    PlayerRemove() {
        this.gp.player.remove();
    }
    PlayerSync(override = false) {
        return this.gp.player.sync({ override: Boolean(override) });
    }
    PlayerLoad() {
        return this.gp.player.load();
    }
    PlayerLogin() {
        return this.gp.player.login();
    }
    PlayerFetchFields() {
        this.gp.player.fetchFields();
    }

    PlayerIsLoggedIn() {
        return this.toUnity(this.gp.player.isLoggedIn);
    }

    PlayerHasAnyCredentials(key) {
        return this.toUnity(this.gp.player.hasAnyCredentials);
    }

    PlayerIsStub(key) {
        return this.toUnity(this.gp.player.isStub);
    }

    PlayerGetActiveDays() {
        return this.toUnity(this.gp.player.stats.activeDays);
    }

    PlayerGetActiveDaysConsecutive() {
        return this.toUnity(this.gp.player.stats.activeDaysConsecutive);
    }

    PlayerGetPlaytimeToday() {
        return this.toUnity(this.gp.player.stats.playtimeToday);
    }

    PlayerGetPlaytimeAll() {
        return this.toUnity(this.gp.player.stats.playtimeAll);
    }


    // LEADERBOARD 

    LeaderboardOpen(orderBy, order, limit, showNearest, withMe, includeFields, displayFields) {
        return this.gp.leaderboard
            .open({
                id: this.gp.player.id,
                orderBy: orderBy
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                order: order,
                limit: limit,
                includeFields: includeFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                displayFields: displayFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                withMe: withMe,
                showNearest: showNearest,
            })
            .catch(console.warn);
    }

    LeaderboardFetch(tag, orderBy, order, limit, showNearest, withMe, includeFields) {
        return this.gp.leaderboard
            .fetch({
                id: this.gp.player.id,
                orderBy: orderBy
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                order: order,
                limit: limit,
                includeFields: includeFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                withMe: withMe,
                showNearest: showNearest,
            })
            .then((leaderboardInfo) => {
                this.trigger('CallLeaderboardFetchTag', tag);
                this.trigger('CallLeaderboardFetch', JSON.stringify(leaderboardInfo.players));
                this.trigger('CallLeaderboardFetchTop', JSON.stringify(leaderboardInfo.topPlayers));
                this.trigger('CallLeaderboardFetchAbove', JSON.stringify(leaderboardInfo.abovePlayers));
                this.trigger('CallLeaderboardFetchBelow', JSON.stringify(leaderboardInfo.belowPlayers));
                this.trigger('CallLeaderboardFetchOnlyPlayer', JSON.stringify(leaderboardInfo.player));
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallLeaderboardFetchError');
            });
    }

    LeaderboardFetchPlayerRating(tag, orderBy, order) {
        return this.gp.leaderboard
            .fetchPlayerRating({
                id: this.gp.player.id,
                orderBy: orderBy
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                order,
            })
            .then((result) => {
                this.trigger('CallLeaderboardFetchPlayerTag', tag);
                this.trigger('CallLeaderboardFetchPlayerRating', result.player.position);
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallLeaderboardFetchPlayerError');
            });
    }




    // LEADERBOARD SCOPED 

    LeaderboardScopedOpen(idOrTag, variant, order, limit, showNearest, includeFields, displayFields, withMe) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.leaderboard
            .openScoped({
                ...query,
                variant,
                order,
                limit: limit,
                includeFields: includeFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                displayFields: displayFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                withMe: withMe,
                showNearest: showNearest,
            })
            .catch(console.warn);
    }

    LeaderboardScopedFetch(idOrTag, variant, order, limit, showNearest, includeFields, withMe) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.leaderboard
            .fetchScoped({
                ...query,
                variant: variant,
                order: order,
                limit: limit,
                includeFields: includeFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
                withMe,
                showNearest: showNearest,
            })
            .then((leaderboardScopedInfo) => {
                this.trigger('CallLeaderboardScopedFetchTag', idOrTag);
                this.trigger('CallLeaderboardScopedFetchVariant', variant);
                this.trigger('CallLeaderboardScopedFetch', JSON.stringify(leaderboardScopedInfo.players));
                this.trigger('CallLeaderboardScopedFetchTop', JSON.stringify(leaderboardScopedInfo.topPlayers));
                this.trigger('CallLeaderboardScopedFetchAbove', JSON.stringify(leaderboardScopedInfo.abovePlayers));
                this.trigger('CallLeaderboardScopedFetchBelow', JSON.stringify(leaderboardScopedInfo.belowPlayers));
                this.trigger('CallLeaderboardScopedFetchOnlyPlayer', JSON.stringify(leaderboardScopedInfo.player));
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallLeaderboardScopedFetchError');
            });
    }

    LeaderboardScopedPublishRecord(idOrTag, variant, override, key1, value1, key2, value2, key3, value3) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.leaderboard
            .publishRecord({
                ...query,
                variant,
                override: Boolean(override),
                record: {
                    [key1]: value1,
                    [key2]: value2,
                    [key3]: value3,
                },
            })
            .then(() => {
                this.trigger('CallLeaderboardScopedPublishRecordComplete');
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallLeaderboardScopedPublishRecordError');
            });
    }

    LeaderboardScopedFetchPlayerRating(idOrTag, variant, includeFields) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.leaderboard
            .fetchPlayerRatingScoped({
                ...query,
                variant,
                includeFields: includeFields
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
            })
            .then((result) => {
                this.trigger('CallLeaderboardScopedFetchPlayerTag', idOrTag);
                this.trigger('CallLeaderboardScopedFetchPlayerVariant', variant);
                this.trigger('CallLeaderboardScopedFetchPlayerRating', result.player.position);
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallLeaderboardFetchPlayerError');
            });
    }


    // ACHIEVEMENTS
    AchievementsOpen() {
        return this.gp.achievements.open().catch(console.warn);
    }
    AchievementsFetch() {
        return this.gp.achievements
            .fetch()
            .then((result) => {
                this.trigger('CallAchievementsFetch', JSON.stringify(result.achievements));
                this.trigger('CallAchievementsFetchGroups', JSON.stringify(result.achievementsGroups));
                this.trigger('CallPlayerAchievementsFetch', JSON.stringify(result.playerAchievements));
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallAchievementsFetchError');
            });
    }
    AchievementsUnlock(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.achievements
            .unlock(query)
            .then((result) => {
                if (result.success) {
                    this.trigger('CallAchievementsUnlock', idOrTag);
                    return;
                }

                this.trigger('CallAchievementsUnlockError');
            })

            .catch((err) => {
                console.warn(err);
                this.trigger('CallAchievementsUnlockError');
            });
    }

    AchievementsSetProgress(idOrTag, progress) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };

        return this.gp.achievements
            .setProgress({ ...query, progress })

            .then((result) => {
                if (result.success) {
                    this.trigger('CallAchievementsProgress', idOrTag);
                    return;
                }
                this.trigger('CallAchievementsProgressError');
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallAchievementsProgressError');
            });
    }

    AchievementsHas(idOrTag) {
        return this.toUnity(this.gp.achievements.has(idOrTag));
    }
    AchievementsGetProgress(idOrTag) {
        return this.gp.achievements.getProgress(idOrTag);
    }



    // PAYMENTS 
    PaymentsFetchProducts() {
        return this.gp.payments
            .fetchProducts()
            .then((result) => {
                this.trigger('CallPaymentsFetchProducts', JSON.stringify(result.products));
                this.trigger('CallPaymentsFetchPlayerPurcahses', JSON.stringify(result.playerPurchases));
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallPaymentsFetchProductsError');
            });
    }
    PaymentsPurchase(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.payments
            .purchase(query)
            .then((result) => {

                if (result.success) {
                    this.trigger('CallPaymentsPurchase', idOrTag);
                    window.focus();
                    return;
                }

                this.trigger('CallPaymentsPurchaseError');

                window.focus();
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallPaymentsPurchaseError');
                window.focus();
            });
    }
    PaymentsConsume(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.payments
            .consume(query)
            .then((result) => {
                if (result.success) {
                    this.trigger('CallPaymentsConsume', idOrTag);
                    window.focus();
                    return;
                }

                this.trigger('CallPaymentsConsumeError');
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallPaymentsConsumeError');
            });
    }
    PaymentsIsAvailable() {
        return this.toUnity(this.gp.payments.isAvailable);
    }


    // Subscriptions
    PaymentsIsSubscriptionsAvailable() {
        return this.toUnity(this.gp.payments.isSubscriptionsAvailable);
    }

    PaymentsSubscribe(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.payments.subscribe(query)
            .then((result) => {
                if (result.success) {
                    this.trigger('CallPaymentsSubscribeSuccess', idOrTag);
                } else {
                    this.trigger('CallPaymentsSubscribeError');
                }
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallPaymentsSubscribeError');
            });
    }


    PaymentsUnsubscribe(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.payments.unsubscribe(query)
            .then((result) => {
                if (result.success) {
                    this.trigger('CallPaymentsUnsubscribeSuccess', idOrTag);
                } else {
                    this.trigger('CallPaymentsUnsubscribeError');
                }
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallPaymentsUnsubscribeError');
            });
    }


    FullscreenOpen() {
        return this.gp.fullscreen.open();
    }
    FullscreenClose() {
        return this.gp.fullscreen.close();
    }
    FullscreenToggle() {
        return this.gp.fullscreen.toggle();
    }


    // ADS 
    AdsShowFullscreen() {
        return this.gp.ads.showFullscreen();
    }
    AdsShowRewarded(idOrTag) {
        this.lastRewardedTag = idOrTag;
        return this.gp.ads.showRewardedVideo();
    }
    AdsShowPreloader() {
        return this.gp.ads.showPreloader();
    }
    AdsShowSticky() {
        return this.gp.ads.showSticky();
    }
    AdsCloseSticky() {
        return this.gp.ads.closeSticky();
    }
    AdsRefreshSticky() {
        return this.gp.ads.refreshSticky();
    }


    AdsIsAdblockEnabled() {
        return this.toUnity(this.gp.ads.isAdblockEnabled);
    }

    AdsIsStickyAvailable() {
        return this.toUnity(this.gp.ads.isStickyAvailable);
    }
    AdsIsFullscreenAvailable() {
        return this.toUnity(this.gp.ads.isFullscreenAvailable);
    }
    AdsIsRewardedAvailable() {
        return this.toUnity(this.gp.ads.isRewardedAvailable);
    }
    AdsIsPreloaderAvailable() {
        return this.toUnity(this.gp.ads.isPreloaderAvailable);
    }


    AdsIsStickyPlaying() {
        return this.toUnity(this.gp.ads.isStickyPlaying);
    }
    AdsIsFullscreenPlaying() {
        return this.toUnity(this.gp.ads.isFullscreenPlaying);
    }
    AdsIsRewardedPlaying() {
        return this.toUnity(this.gp.ads.isRewardedPlaying);
    }
    AdsIsPreloaderPlaying() {
        return this.toUnity(this.gp.ads.isPreloaderPlaying);
    }

    AdsIsCountdownOverlayEnabled() {
        return this.toUnity(this.gp.ads.isCountdownOverlayEnabled);
    }

    AdsIsRewardedFailedOverlayEnabled() {
        return this.toUnity(this.gp.ads.isRewardedFailedOverlayEnabled);
    }

    AdsCanShowFullscreenBeforeGamePlay() {
        return this.toUnity(this.gp.ads.canShowFullscreenBeforeGamePlay);
    }



    // ANALYTICS 
    AnalyticsHit(url) {
        return this.gp.analytics.hit(url);
    }
    AnalyticsGoal(event, value) {
        return this.gp.analytics.goal(event, value);
    }


    /* SOCIAL */
    SocialsShare(text, url, image) {
        return this.gp.socials.share({ text, url, image });
    }
    SocialsPost(text, url, image) {
        return this.gp.socials.post({ text, url, image });
    }
    SocialsInvite(text, url, image) {
        return this.gp.socials.invite({ text, url, image });
    }
    SocialsJoinCommunity() {
        return this.gp.socials.joinCommunity();
    }
    SocialsCommunityLink() {
        return this.toUnity(this.gp.socials.communityLink);
    }

    SocialsIsSupportsShare() {
        return this.toUnity(this.gp.socials.isSupportsShare);
    }
    SocialsIsSupportsNativeShare() {
        return this.toUnity(this.gp.socials.isSupportsNativeShare);
    }
    SocialsIsSupportsNativePosts() {
        return this.toUnity(this.gp.socials.isSupportsNativePosts);
    }
    SocialsIsSupportsNativeInvite() {
        return this.toUnity(this.gp.socials.isSupportsNativeInvite);
    }
    SocialsCanJoinCommunity() {
        return this.toUnity(this.gp.socials.canJoinCommunity);
    }
    SocialsIsSupportsNativeCommunityJoin() {
        return this.toUnity(this.gp.socials.isSupportsNativeCommunityJoin);
    }
    
    SocialsMakeShareLink(shareContent){
        return this.toUnity(this.gp.socials.makeShareUrl({
            fromId: this.gp.player.id,
            content: shareContent,
        }));
    }
    SocialsGetSharePlayerID(){
        return this.toUnity(this.gp.socials.getShareParam('fromId'));
    }
    SocialsGetShareContent(){
        return this.toUnity(this.gp.socials.getShareParam('content'));
    }
    


    /* GAMES COLLECTIONS */
    GamesCollectionsOpen(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag || 'ANY' };
        return this.gp.gamesCollections.open(query);
    }
    GamesCollectionsFetch(idOrTag) {
        const id = parseInt(idOrTag, 10) || 0;
        const query = id > 0 ? { id } : { tag: idOrTag };
        return this.gp.gamesCollections
            .fetch(query)
            .then((result) => {
                this.trigger('CallGamesCollectionsFetchTag', idOrTag);
                this.trigger('CallGamesCollectionsFetch', JSON.stringify(result));

            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallGamesCollectionsFetchError');
            });
    }

    ChangeLanguage(language) {
        return this.gp.changeLanguage(language);
    }
    ChangeLanguageByCode(language = '') {
        return this.gp.changeLanguage(language.toLowerCase());
    }
    ChangeAvatarGenerator(generator) {
        return this.gp.changeAvatarGenerator(generator);
    }
    LoadOverlay() {
        return this.gp.loadOverlay();
    }

    // GAME
    IsPaused() {
        return this.toUnity(this.gp.isPaused);
    }

    Pause() {
        return this.gp.pause();
    }
    Resume() {
        return this.gp.resume();
    }

    GameReady(){
        return this.gp.gameStart();
    }

    GameplayStart() {
        return this.gp.gameplayStart();
    }
    GameplayStop() {
        return this.gp.gameplayStop();
    }
    
    HappyTime(){
        if(this.gp.platform.type == "CRAZY_GAMES")
            this.gp.platform.getNativeSDK().game.happytime();  
    }


    // GAME


    //Device
    IsMobile() {
        return this.toUnity(this.gp.isMobile);
    }
    IsPortrait() {
        return this.toUnity(this.gp.isPortrait);
    }
    //Device


    // Server
    ServerTime() {
        return this.toUnity(this.gp.serverTime);
    }
    // Server



    // System
    IsDev() {
        return this.toUnity(this.gp.isDev);
    }

    IsAllowedOrigin() {
        return this.toUnity(this.gp.isAllowedOrigin);
    }
    // System

    //Custom
    CustomCall(custom) {
        return this.custom;
    }

    CustomReturn(custom) {
        return this.toUnity(this.custom)
    }

    //Custom

    // Variables
    VariablesFetch() {
        return this.gp.variables
            .fetch()
            .then((result) => {
                this.trigger('CallVariablesFetchSuccess', JSON.stringify(result));
            })
            .catch((err) => {
                console.warn(err);
                this.trigger('CallVariablesFetchError');
            });
    }

    VariablesHas(key) {
        return this.toUnity(this.gp.variables.has(key));
    }

    VariablesGet(key) {
        return this.toUnity(this.gp.variables.get(key));
    }
    // Variables



    // Players
    PlayersFetch(key) {
        var obj = JSON.parse(key);
        let ids = [];

        if (parseInt(obj, 10) > 0) {
            ids = [parseInt(obj, 10)];
        } else {
            ids = (obj.idsList || obj.idsArray).map(Number).filter(Boolean)
        }

        this.gp.players.fetch({ ids })
            .then((result) => {
                this.trigger('CallPlayersFetchSuccess', JSON.stringify(result.players));
            }).catch((err) => {
                console.warn(err);
                this.trigger('CallPlayersFetchError');
            });
    }
    //Players


    // Documents
    DocumentsOpen() {
        this.gp.documents.open({ type: 'PLAYER_PRIVACY_POLICY' });
    }

    DocumentsFetch() {
        this.gp.documents.fetch({
            type: 'PLAYER_PRIVACY_POLICY',
            format: 'TXT'
        });
    }
    // Documents





    // Files
    FilesUpload(tags) {
        this.gp.files
            .upload({
                tags: tags
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
            })
            .then((result) => {
                this.trigger('CallFilesUploadSuccess', JSON.stringify(result));
            })
            .catch((err) => {
                this.trigger('CallFilesUploadError');
            });
    }

    FilesUploadUrl(url, filename, tags) {
        this.gp.files
            .uploadUrl({
                url,
                filename,
                tags: tags
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
            })
            .then((result) => {
                this.trigger('CallFilesUploadUrlSuccess', JSON.stringify(result));
            })
            .catch((err) => {
                this.trigger('CallFilesUploadUrlError');
            });
    }

    FilesUploadContent(content, filename, tags) {
        this.gp.files
            .uploadContent({
                content,
                filename,
                tags: tags
                    .split(',')
                    .map((o) => o.trim())
                    .filter((f) => f),
            })
            .then((result) => {
                this.trigger('CallFilesUploadContentSuccess', JSON.stringify(result));
            })
            .catch((err) => {
                this.trigger('CallFilesUploadContentError');
            });
    }

    FilesLoadContent(url) {
        this.gp.files
            .loadContent(url)
            .then((result) => {
                this.trigger('CallFilesLoadContentSuccess', result);
            })
            .catch((err) => {
                this.trigger('CallFilesLoadContentError');
            });
    }

    FilesChooseFile(type) {
        this.gp.files
            .chooseFile(type)
            .then((result) => {
                this.trigger('CallFilesChooseFile', result.tempUrl);
            })
            .catch((err) => {
                this.trigger('CallFilesChooseFileError');
            });
    }

    FilesFetch(filter) {
        const query = JSON.parse(filter);
        this.gp.files.fetch(query)
            .then((result) => {
                this.trigger('CallFilesFetchCanLoadMore', result.canLoadMore);
                this.trigger('CallFilesFetchSuccess', JSON.stringify(result.items));
            })
            .catch((err) => {
                this.trigger('CallFilesFetchError');
            });
    }

    FilesFetchMore(filter) {
        const query = JSON.parse(filter);
        this.gp.files.fetchMore(query)
            .then((result) => {
                this.trigger('CallFilesFetchMoreCanLoadMore', result.canLoadMore);
                this.trigger('CallFilesFetchMoreSuccess', JSON.stringify(result.items));
            })
            .catch((err) => {
                this.trigger('CallFilesFetchMoreError');
            });
    }

    // Files


    // Channels

    Channels_Open_Chat(channel_ID) {
        if (channel_ID == -10) {
            this.gp.channels.openChat();
        }
        else {
            this.gp.channels.openChat({ channel_ID });
        }
    }
    Channels_IsMainChatEnabled() {
        return this.toUnity(this.gp.channels.isMainChatEnabled);
    }

    Channels_MainChatId() {
        return this.gp.channels.mainChatId;
    }

    Channels_Join(channelId, password) {
        this.gp.channels.join({ channelId, password, });
    }

    Channels_CancelJoin(channelId) {
        this.gp.channels.cancelJoin({ channelId, });
    }

    Channels_Leave(channelId) {
        this.gp.channels.leave({ channelId, });
    }

    Channels_Kick(channelId, playerId) {
        this.gp.channels.kick({ channelId, playerId, });
    }

    Channels_Mute_UnmuteAt(channelId, playerId, unmuteAt) {
        this.gp.channels.mute({ channelId, playerId, unmuteAt, });
    }

    Channels_Mute_Seconds(channelId, playerId, seconds) {
        this.gp.channels.mute({ channelId, playerId, seconds: Number(seconds), });
    }

    Channels_UnMute(channelId, playerId) {
        this.gp.channels.unmute({ channelId, playerId, });
    }

    Channels_SendInvite(channelId, playerId) {
        this.gp.channels.sendInvite({ channelId, playerId, });
    }

    Channels_CancelInvite(channelId, playerId) {
        this.gp.channels.cancelInvite({ channelId, playerId, });
    }

    Channels_AcceptInvite(channelId) {
        this.gp.channels.acceptInvite({ channelId, });
    }

    Channels_RejectInvite(channelId) {
        this.gp.channels.rejectInvite({ channelId });
    }

    Channels_FetchInvites(limit, offset) {
        this.gp.channels.fetchInvites({ limit, offset, });
    }

    Channels_FetchMoreInvites(limit) {
        this.gp.channels.fetchMoreInvites({ limit, });
    }

    Channels_FetchChannelInvites(channelId, limit, offset) {
        this.gp.channels.fetchChannelInvites({ channelId, limit, offset, });
    }

    Channels_FetchMoreChannelInvites(channelId, limit) {
        this.gp.channels.fetchMoreChannelInvites({ channelId, limit, });
    }

    Channels_FetchSentInvites(channelId, limit, offset) {
        this.gp.channels.fetchSentInvites({ channelId, limit, offset, });
    }

    Channels_FetchMoreSentInvites(channelId, limit) {
        this.gp.channels.fetchMoreSentInvites({ channelId, limit, });
    }

    Channels_AcceptJoinRequest(channelId, playerId) {
        this.gp.channels.acceptJoinRequest({ channelId, playerId, });
    }

    Channels_RejectJoinRequest(channelId, playerId) {
        this.gp.channels.rejectJoinRequest({ channelId, playerId, });
    }

    Channels_FetchJoinRequests(channelId, limit, offset) {
        this.gp.channels.fetchJoinRequests({ channelId, limit, offset });
    }

    Channels_FetchMoreJoinRequests(channelId, limit) {
        this.gp.channels.fetchMoreJoinRequests({ channelId, limit, });
    }

    Channels_FetchSentJoinRequests(channelId, limit, offset) {
        this.gp.channels.fetchSentJoinRequests({ channelId, limit, offset, });
    }

    Channels_FetchMoreSentJoinRequests(channelId, limit) {
        this.gp.channels.fetchMoreSentJoinRequests({ channelId, limit, });
    }

    Channels_SendMessage(channelId, text, tags) {
        this.gp.channels.sendMessage({
            channelId,
            text,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
        });
    }

    Channels_SendPersonalMessage(playerId, text, tags) {
        this.gp.channels.sendPersonalMessage({
            playerId,
            text,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
        });
    }

    Channels_SendFeedMessage(playerId, text, tags) {
        this.gp.channels.sendFeedMessage({
            playerId,
            text,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
        });
    }

    Channels_EditMessage(messageId, text) {
        this.gp.channels.editMessage({ messageId, text, });
    }

    Channels_DeleteMessage(messageId) {
        this.gp.channels.deleteMessage({ messageId });
    }

    Channels_FetchMessages(channelId, tags, limit, offset) {
        this.gp.channels.fetchMessages({
            channelId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
            offset,
        }).then((result) => {
            this.trigger('CallOnFetchMessagesCanLoadMore', JSON.stringify(result.canLoadMore));
            this.trigger('CallOnFetchMessages', JSON.stringify(result.items));

        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchMessagesError');
        });
    }

    Channels_FetchPersonalMessages(playerId, tags, limit, offset) {
        this.gp.channels.fetchPersonalMessages({
            playerId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
            offset,
        }).then((result) => {
            this.trigger('CallOnFetchPersonalMessagesCanLoadMore', JSON.stringify(result.canLoadMore));
            this.trigger('CallOnFetchPersonalMessages', JSON.stringify(result.items));
        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchPersonalMessagesError');
        });
    }

    Channels_FetchFeedMessages(playerId, tags, limit, offset) {
        this.gp.channels.fetchFeedMessages({
            playerId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
            offset,
        }).then((result) => {
            this.trigger('CallOnFetchFeedMessagesCanLoadMore', JSON.stringify(result.canLoadMore));
            this.trigger('CallOnFetchFeedMessages', JSON.stringify(result.items));
        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchFeedMessagesError');
        });
    }

    Channels_FetchMoreMessages(channelId, tags, limit) {
        this.gp.channels.fetchMoreMessages({
            channelId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
        }).then((result) => {
            this.trigger('CallOnFetchMoreMessagesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreMessages', JSON.stringify(result.items));
        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchMoreMessagesError');
        });
    }

    Channels_FetchMorePersonalMessages(playerId, tags, limit) {
        this.gp.channels.fetchMorePersonalMessages({
            playerId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
        }).then((result) => {
            this.trigger('CallOnFetchMorePersonalMessagesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMorePersonalMessages', JSON.stringify(result.items));
        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchMorePersonalMessagesError');
        });
    }

    Channels_FetchMoreFeedMessages(playerId, tags, limit) {
        this.gp.channels.fetchMoreFeedMessages({
            playerId,
            tags: tags
                .split(',')
                .map((o) => o.trim())
                .filter((f) => f),
            limit,
        }).then((result) => {
            this.trigger('CallOnFetchMoreFeedMessagesCanLoadMore', result.canLoadMore);
            this.trigger('CallOnFetchMoreFeedMessages', JSON.stringify(result.items));
        }).catch((err) => {
            console.warn(err);
            this.trigger('CallOnFetchMoreFeedMessagesError');
        });
    }

    Channels_DeleteChannel(channelId) {
        this.gp.channels.deleteChannel({ channelId, });
    }

    Channels_FetchChannel(channelId) {
        this.gp.channels.fetchChannel({ channelId, });
    }

    Channels_CreateChannel(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.createChannel({ ...query, private: query.ch_private });
    }

    Channels_UpdateChannel(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.updateChannel({ ...query, private: query.ch_private });
    }

    Channels_FetchChannels(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.fetchChannels(query);
    }

    Channels_FetchMoreChannels(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.fetchMoreChannels(query);
    }

    Channels_FetchMembers(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.fetchMembers(query);
    }

    Channels_FetchMoreMembers(filter) {
        const query = JSON.parse(filter);
        this.gp.channels.fetchMoreMembers(query);
    }

    // Channels
}

function mapChannel(channel = {}) {
    return {
        ...channel,
        ch_private: channel.private
    };
}

function mapItemWithChannel(item = {}) {
    return {
        ...item,
        channel: mapChannel(item.channel)
    };
}

window.GamePushUnity = GamePushUnity;
