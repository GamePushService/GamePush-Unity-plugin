namespace GamePush.Native
{
    // Documents for the modules that back the native overlays. Field sets mirror the
    // official GamePush JS SDK so the native path returns the same shapes as WebGL.
    public static partial class NativeQueries
    {
        public const string PlayerAchievementFields = "achievementId createdAt progress unlocked";

        const string AchievementFields = @"
            id tag rare progress maxProgress unlocked progressStep
            isPublished isLockedVisible isLockedDescriptionVisible
            name(lang: $lang) description(lang: $lang)
            icon(w: 256, h: 256, crop: false)
            iconSmall: icon(w: 48, h: 48, crop: false)
            lockedIcon(w: 256, h: 256, crop: false)
            lockedIconSmall: lockedIcon(w: 48, h: 48, crop: false)";

        public const string FetchAchievementsConfig = @"
query ($lang: Lang) {
  result: FetchPlayerProjectConfig {
    __typename
    ... on Problem { message }
    ... on PlayerProjectConfig {
      achievements { " + AchievementFields + @" }
      achievementsGroups {
        id tag name(lang: $lang) description(lang: $lang) achievements
      }
    }
  }
}";

        public const string UnlockAchievement = @"
mutation ($input: UnlockPlayerAchievementInput!, $lang: Lang) {
  result: UnlockPlayerAchievement(input: $input) {
    __typename
    ... on Problem { message }
    ... on PlayerAchievement {
      " + PlayerAchievementFields + @"
      achievement { " + AchievementFields + @" }
    }
  }
}";

        public const string SetAchievementProgress = @"
mutation ($input: PlayerSetAchievementProgressInput!, $lang: Lang) {
  result: PlayerSetAchievementProgress(input: $input) {
    __typename
    ... on Problem { message }
    ... on PlayerAchievement {
      " + PlayerAchievementFields + @"
      achievement { " + AchievementFields + @" }
    }
  }
}";

        const string LeaderboardFieldFields = @"
            key type name(lang: $lang) default important public";

        const string PlayersTopFields = @"
    ... on PlayersTop {
      leaderboard {
        id tag name(lang: $lang) description(lang: $lang) shareText(lang: $lang)
        isAuthorizedOnly limit
      }
      players
      fields { " + LeaderboardFieldFields + @" }
    }";

        const string PlayerTopFields = @"
    ... on PlayerTop {
      player
      abovePlayers
      belowPlayers
      fields { " + LeaderboardFieldFields + @" }
    }";

        public const string FetchTop = @"
query ($input: FetchTopInput!, $lang: Lang, $withMe: Boolean!) {
  result: FetchTop(input: $input) {
    __typename
    ... on Problem { message }
    " + PlayersTopFields + @"
  }
  playerResult: FetchPlayerRating(input: $input) @include(if: $withMe) {
    __typename
    ... on Problem { message }
    " + PlayerTopFields + @"
  }
}";

        public const string FetchPlayerRating = @"
query ($input: FetchTopInput!, $lang: Lang) {
  result: FetchPlayerRating(input: $input) {
    __typename
    ... on Problem { message }
    " + PlayerTopFields + @"
  }
}";

        public const string FetchTopScoped = @"
query ($input: FetchPlayerTopScopedInput!, $lang: Lang, $withMe: Boolean!) {
  result: FetchPlayerTopScoped(input: $input) {
    __typename
    ... on Problem { message }
    " + PlayersTopFields + @"
  }
  playerResult: FetchPlayerRatingScoped(input: $input) @include(if: $withMe) {
    __typename
    ... on Problem { message }
    " + PlayerTopFields + @"
  }
}";

        public const string FetchPlayerRatingScoped = @"
query ($input: FetchPlayerTopScopedInput!, $lang: Lang) {
  result: FetchPlayerRatingScoped(input: $input) {
    __typename
    ... on Problem { message }
    " + PlayerTopFields + @"
  }
}";

        public const string PublishRecord = @"
mutation ($input: PlayerPublishRecordInput!, $lang: Lang) {
  result: PlayerPublishRecord(input: $input) {
    __typename
    ... on Problem { message }
    ... on PlayerRecord {
      record
      fields { " + LeaderboardFieldFields + @" }
    }
  }
}";

        public const string FetchDocument = @"
query ($input: FetchPlayerDocumentInput!, $lang: Lang, $format: DocumentFormat) {
  result: FetchPlayerDocument(input: $input) {
    __typename
    ... on Problem { message }
    ... on Document {
      type
      content(lang: $lang, format: $format)
    }
  }
}";

        public const string FetchGamesCollection = @"
fragment project on Project {
  id
  url(from: $url)
  name(lang: $lang)
  description(lang: $lang)
  assets {
    icon(lang: $lang) {
      resources { src(w: 256, h: 256, crop: true) }
    }
  }
}
query ($input: FetchPlayerGamesCollectionInput!, $lang: Lang, $url: String) {
  result: FetchPlayerGamesCollection(input: $input) {
    __typename
    ... on Problem { message }
    ... on GamesCollection {
      id
      tag
      name(lang: $lang)
      description(lang: $lang)
      games { ...project }
    }
  }
}";

        const string FeedbackMessageFields = "id feedbackId text files author time";

        const string FeedbackFields = @"
      id projectId userAgent playerEmail text type status platformStatus files
      messages { " + FeedbackMessageFields + @" }
      createdAt updatedAt";

        public const string CreateFeedback = @"
mutation ($input: PlayerCreateFeedbackInput!) {
  result: PlayerCreateFeedback(input: $input) {
    __typename
    ... on Problem { message }
    ... on Feedback { " + FeedbackFields + @" }
  }
}";

        public const string FetchFeedbacks = @"
query ($input: PlayerFetchFeedbacksInput!) {
  result: PlayerFetchFeedbacks(input: $input) {
    __typename
    ... on Problem { message }
    ... on FeedbacksList {
      feedbacks { " + FeedbackFields + @" }
      totalCount
    }
  }
}";

        public const string FetchFeedback = @"
query ($input: PlayerFetchFeedbackInput!) {
  result: PlayerFetchFeedback(input: $input) {
    __typename
    ... on Problem { message }
    ... on Feedback { " + FeedbackFields + @" }
  }
}";

        public const string SendFeedbackMessage = @"
mutation ($input: PlayerSendFeedbackMessageInput!) {
  result: PlayerSendFeedbackMessage(input: $input) {
    __typename
    ... on Problem { message }
    ... on FeedbackMessage { " + FeedbackMessageFields + @" }
  }
}";

        const string ChannelMessageFields = @"
      id channelId authorId text tags player createdAt";

        const string ChannelMessagesListFields = @"
    ... on ChannelMessagesList {
      items { " + ChannelMessageFields + @" }
    }";

        const string ChannelMessageResult = @"
    ... on ChannelMessage { " + ChannelMessageFields + @" }";

        public const string FetchChannelMessages = @"
query ($input: PlayerFetchChannelMessagesInput!) {
  result: PlayerFetchChannelMessages(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessagesListFields + @"
  }
}";

        public const string FetchPersonalMessages = @"
query ($input: PlayerFetchPersonalMessagesInput!) {
  result: PlayerFetchPersonalMessages(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessagesListFields + @"
  }
}";

        public const string FetchFeedMessages = @"
query ($input: PlayerFetchFeedMessagesInput!) {
  result: PlayerFetchFeedMessages(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessagesListFields + @"
  }
}";

        public const string SendMessage = @"
mutation ($input: PlayerSendMessageInput!) {
  result: PlayerSendMessage(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessageResult + @"
  }
}";

        public const string SendPersonalMessage = @"
mutation ($input: PlayerSendPersonalMessageInput!) {
  result: PlayerSendPersonalMessage(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessageResult + @"
  }
}";

        public const string SendFeedMessage = @"
mutation ($input: PlayerSendFeedMessageInput!) {
  result: PlayerSendFeedMessage(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessageResult + @"
  }
}";

        public const string EditMessage = @"
mutation ($input: PlayerEditMessageInput!) {
  result: PlayerEditMessage(input: $input) {
    __typename
    ... on Problem { message }
    " + ChannelMessageResult + @"
  }
}";

        public const string DeleteMessage = @"
mutation ($input: PlayerDeleteMessageInput!) {
  result: PlayerDeleteMessage(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string DeleteChannel = @"
mutation ($input: PlayerDeleteChannelInput!) {
  result: PlayerDeleteChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string CancelJoinChannel = @"
mutation ($input: PlayerCancelJoinChannelInput!) {
  result: PlayerCancelJoinChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string KickFromChannel = @"
mutation ($input: PlayerKickFromChannelInput!) {
  result: PlayerKickFromChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string MutePlayerInChannel = @"
mutation ($input: PlayerMutePlayerInChannelInput!) {
  result: PlayerMutePlayerInChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string UnmutePlayerInChannel = @"
mutation ($input: PlayerUnmutePlayerInChannelInput!) {
  result: PlayerUnmutePlayerInChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string SendInviteToChannel = @"
mutation ($input: PlayerSendInviteToChannelInput!) {
  result: PlayerSendInviteToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelInvite { channelId playerToId playerFromId date }
  }
}";

        public const string CancelInviteToChannel = @"
mutation ($input: PlayerCancelInviteToChannelInput!) {
  result: PlayerCancelInviteToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string AcceptInviteToChannel = @"
mutation ($input: PlayerAcceptInviteToChannelInput!) {
  result: PlayerAcceptInviteToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string RejectInviteToChannel = @"
mutation ($input: PlayerRejectInviteToChannelInput!) {
  result: PlayerRejectInviteToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";

        public const string FetchChannelInvites = @"
query ($input: PlayerFetchChannelInvitesInput!) {
  result: PlayerFetchChannelInvites(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelInvitesList {
      items { channelId playerTo playerFrom date }
    }
  }
}";

        public const string FetchJoinRequests = @"
query ($input: PlayerFetchChannelJoinRequestsInput!) {
  result: PlayerFetchChannelJoinRequests(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelJoinRequestsList {
      items { channelId player date }
    }
  }
}";

        public const string AcceptJoinRequest = @"
mutation ($input: PlayerAcceptJoinRequestToChannelInput!) {
  result: PlayerAcceptJoinRequestToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on ChannelMember {
      id state channelId isOnline
      mute { isMuted unmuteAt }
    }
  }
}";

        public const string RejectJoinRequest = @"
mutation ($input: PlayerRejectJoinRequestToChannelInput!) {
  result: PlayerRejectJoinRequestToChannel(input: $input) {
    __typename
    ... on Problem { message }
    ... on Success { success }
  }
}";
    }
}
