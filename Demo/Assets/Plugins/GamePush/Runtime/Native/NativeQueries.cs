namespace GamePush.Native
{
    public static partial class NativeQueries
    {
        public const string FetchConfig = @"
query ($lang: Lang) {
  result: FetchPlayerProjectConfig {
    __typename
    ... on Problem { message }
    ... on PlayerProjectConfig {
      isDev
      isAllowedOrigin
      serverTime
      config { lang orientation avatarGenerator avatarGeneratorTemplate }
      project {
        name(lang: $lang)
        mainChatId
        enableMainChat
        ads { showCountdownOverlay showRewardedFailedOverlay }
      }
      platformConfig {
        type
        tag
        appId
        customAdsConfig {
          configs {
            android {
              implementation
              banners { type enabled bannerId adServer position frequency refreshInterval limits { hour day session } }
            }
          }
        }
        banners { type enabled bannerId adServer position frequency refreshInterval limits { hour day session } }
        paymentsConfig {
          id
          sandbox
          oneStoreConfig { publicKey }
          configs {
            android { implementation activeService }
          }
        }
        paymentsConfigId
        authConfig {
          id
          googleConfig { clientID }
          yandexConfig { clientID }
          xsollaConfig { loginProjectId }
          configs {
            android { implementation activeService }
          }
        }
        authConfigId
      }
      playerFields { key type default }
      products { " + ProductFields + @" }
    }
  }
}";

        public const string SyncPlayer = @"
mutation ($input: SyncPlayerInput!, $withToken: Boolean!) {
  result: SyncPlayer(input: $input) {
    __typename
    ... on Problem { message }
    ... on Player {
      id
      name
      avatar
      credentials
      secretCode
      selected
      authToken
      token @include(if: $withToken)
      state
      serverTime
      achievementsList { " + PlayerAchievementFields + @" }
    }
    ... on PlayerSyncConflict { players }
  }
}";

        public const string GetPlayer = @"
query ($input: GetPlayerInput!, $withToken: Boolean!) {
  result: GetPlayer(input: $input) {
    __typename
    ... on Problem { message }
    ... on Player {
      id
      name
      avatar
      credentials
      secretCode
      selected
      authToken
      token @include(if: $withToken)
      state
      serverTime
      achievementsList { " + PlayerAchievementFields + @" }
    }
  }
}";

        public const string FetchChannels = @"
query($input: PlayerFetchChannelsInput!, $lang: Lang) {
  result: PlayerFetchChannels(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelsList {
      items {
        id tags templateId capacity ownerId name(lang: $lang) description(lang: $lang)
        private visible permanent hasPassword password membersCount isJoined isRequestSent isInvited
      }
    }
  }
}";

        public const string FetchChannel = @"
query($input: PlayerFetchChannelInput!, $lang: Lang) {
  result: PlayerFetchChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Channel {
      id tags templateId projectId capacity ownerId name(lang: $lang) description(lang: $lang)
      private visible permanent hasPassword password membersCount isJoined isRequestSent isInvited
    }
  }
}";

        public const string CreateChannel = @"
mutation($input: PlayerCreateChannelInput!, $lang: Lang) {
  result: PlayerCreateChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Channel {
      id tags templateId capacity ownerId name(lang: $lang) description(lang: $lang)
      private visible permanent hasPassword password membersCount isJoined
    }
  }
}";

        public const string UpdateChannel = @"
mutation($input: PlayerUpdateChannelInput!, $lang: Lang) {
  result: PlayerUpdateChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Channel {
      id tags templateId capacity ownerId name(lang: $lang) description(lang: $lang)
      private visible permanent hasPassword password membersCount isJoined
    }
  }
}";

        public const string JoinChannel = @"
mutation($input: PlayerJoinChannelInput!) {
  result: PlayerJoinChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string LeaveChannel = @"
mutation($input: PlayerLeaveChannelInput!) {
  result: PlayerLeaveChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string FetchMembers = @"
query($input: PlayerFetchChannelMembersInput!) {
  result: PlayerFetchChannelMembers(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelMembersList {
      players { id state isOnline }
    }
  }
}";

        public const string ConnectMultiplayer = @"
mutation($input: ConnectPlayerMultiplayerInput!) {
  result: ConnectPlayerMultiplayer(input: $input) {
    __typename
    ... on Problem { message }
    ... on ConnectPlayerMultiplayerResult {
      channelId
      connection { endpoint token expiresAt }
      hostSubscription { channel token expiresAt }
      stateSubscription { channel token expiresAt }
      serverTime
    }
  }
}";

        public const string LoginPlayer = @"
mutation ($input: LoginPlayerInput!) {
  result: LoginPlayer(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string LogoutPlayer = @"
mutation {
  result: LogoutPlayer {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string GetPlayerLoginStatus = @"
query {
  result: GetPlayerLoginStatus {
    __typename
    ... on Problem { message }
    ... on PlayerLoginStatus { credentials }
  }
}";

        public const string DisconnectMultiplayer = @"
mutation($input: DisconnectPlayerMultiplayerInput!) {
  result: DisconnectPlayerMultiplayer(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        const string ProductFields = @"
            id tag price isSubscription period trialPeriod currency currencySymbol
            yandexId onestoreId googlePlayId
            name(lang: $lang) description(lang: $lang)
            icon iconSmall: icon(w: 48, h: 48, crop: false)";

        const string PlayerPurchaseFields =
            "_id productId payload gift subscribed createdAt orderStatus expiredAt needConsumeOnlyOnPlatform";

        const string PurchaseOutput = @"{
    __typename
    ... on Problem { message }
    ... on PlayerPurchaseOutput {
      product { " + ProductFields + @" }
      purchase { " + PlayerPurchaseFields + @" }
    }
  }";

        public const string FetchPlayerPurchases = @"
query ($lang: Lang) {
  result: FetchPlayerPurchases {
    __typename
    ... on Problem { message }
    ... on PlayerPurchasesOutput {
      products { " + ProductFields + @" }
      playerPurchases { " + PlayerPurchaseFields + @" }
    }
  }
}";

        public const string PurchasePlayerPurchase = @"
mutation ($input: PurchasePlayerPurchaseInput!, $lang: Lang) {
  result: PurchasePlayerPurchase(input: $input) " + PurchaseOutput + @"
}";

        public const string ConsumePlayerPurchase = @"
mutation ($input: ConsumePlayerPurchaseInput!, $lang: Lang) {
  result: ConsumePlayerPurchase(input: $input) " + PurchaseOutput + @"
}";

        public const string CancelPlayerSubscription = @"
mutation ($input: CancelPlayerSubscriptionInput!, $lang: Lang) {
  result: CancelPlayerSubscription(input: $input) " + PurchaseOutput + @"
}";

        public const string ResumePlayerSubscription = @"
mutation ($input: ResumePlayerSubscriptionInput!, $lang: Lang) {
  result: ResumePlayerSubscription(input: $input) " + PurchaseOutput + @"
}";

        public const string GetPlayerPurchase = @"
query ($input: GetPlayerPurchaseInput!) {
  result: GetPlayerPurchase(input: $input) {
    __typename
    ... on PlayerPurchase { " + PlayerPurchaseFields + @" }
    ... on Problem { message }
  }
}";

        public const string SyncPlayerPurchases = @"
mutation ($input: SyncPlayerPurchasesInput!) {
  result: SyncPlayerPurchases(input: $input) {
    __typename
    ... on Problem { message }
    ... on PlayerProductsPurchasedOutput {
      purchases { " + PlayerPurchaseFields + @" }
    }
  }
}";

    }
}
