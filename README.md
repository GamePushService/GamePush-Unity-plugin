# GamePush - Unity Plugin

Plugin for using [GamePush](https://gamepush.com/?r=NzQ4) in Unity games

Development and support of GamePush Unity plugin - [Aristarkh Abramovsky](https://gitlab.com/kerioth)

Creator of early version GamePush Unity plugin - [Dinar Shagidullin](https://gitlab.com/shagidullin)

## Download

Latest version of GamePush Unity plugin:  
[GamePush unitypackage](https://github.com/GamePushService/GamePush-Unity-plugin/tree/main/Releases/GamePush)

Examples for GamePush plugin modules:  
[GP_Examples](https://github.com/GamePushService/GamePush-Unity-plugin/blob/main/Releases/GamePush/GP_Examples.unitypackage)

GamePush Web Template:  
[GP_Template](https://github.com/GamePushService/GamePush-Unity-plugin/blob/main/Releases/GamePush/GP_Template.unitypackage)

##

## Get started:

[English](https://docs.gamepush.com/tutorials/adding-plugin-to-a-unity-project/) and [Russian](https://docs.gamepush.com/ru/tutorials/adding-plugin-to-a-unity-project/) tutorials

## Documentation:

### English:

https://docs.gamepush.com/docs/get-start/

### Russian:

https://docs.gamepush.com/ru/docs/get-start/

# Methods List

| Plugin modules                                |
| --------------------------------------------- |
| [GP_Achievements](#GP_Achievements)           |
| [GP_Ads](#GP_Ads)                             |
| [GP_Analytics](#GP_Analytics)                 |
| [GP_App](#GP_App)                             |
| [GP_AvatarGenerator](#GP_AvatarGenerator)     |
| [GP_Channels](#GP_Channels)                   |
| [GP_Custom](#GP_Custom)                       |
| [GP_Device](#GP_Device)                       |
| [GP_Documents](#GP_Documents)                 |
| [GP_Events](#GP_Events)                       |
| [GP_Experiments](#GP_Experiments)             |
| [GP_Fullscreen](#GP_Fullscreen)               |
| [GP_Game](#GP_Game)                           |
| [GP_GamesCollections](#GP_GamesCollections)   |
| [GP_Images](#GP_Images)                       |
| [GP_Language](#GP_Language)                   |
| [GP_Leaderboard](#GP_Leaderboard)             |
| [GP_LeaderboardScoped](#GP_LeaderboardScoped) |
| [GP_Payments](#GP_Payments)                   |
| [GP_Platform](#GP_Platform)                   |
| [GP_Player](#GP_Player)                       |
| [GP_Players](#GP_Players)                     |
| [GP_Rewards](#GP_Rewards)                     |
| [GP_Schedulers](#GP_Schedulers)               |
| [GP_Segments](#GP_Segments)                   |
| [GP_Server](#GP_Server)                       |
| [GP_Socials](#GP_Socials)                     |
| [GP_System](#GP_System)                       |
| [GP_Triggers](#GP_Triggers)                   |
| [GP_Variables](#GP_Variables)                 |
| [GP_Uniques](#GP_Uniques)                     |
| [GP_Storage](#GP_Storage)                     |

## GP_Achievements

[Achievements documentation](https://docs.gamepush.com/docs/achievements/)

### Methods

| Method name   | Method parameters                                                                               | Return value |
| ------------- | ----------------------------------------------------------------------------------------------- | ------------ |
| `Open`        | `Action onOpen = null, Action onClose = null`                                                   | void         |
| `Fetch`       | void                                                                                            | void         |
| `Unlock`      | `string idOrTag, Action<string> onUnlock = null, Action<string> onUnlockError = null`           | void         |
| `SetProgress` | `string idOrTag, int progress, Action<string> onProgress = null, Action onProgressError = null` | void         |
| `Has`         | `string idOrTag`                                                                                | void         |
| `GetProgress` | `string idOrTag`                                                                                | void         |

### Actions

| Action name                   | Return value                    |
| ----------------------------- | ------------------------------- |
| `OnAchievementsOpen`          | void                            |
| `OnAchievementsClose`         | void                            |
| `OnAchievementsFetch`         | `List <AchievementsFetch>`      |
| `OnAchievementsFetchError`    | void                            |
| `OnAchievementsFetchGroups`   | `List<AchievementsFetchGroups>` |
| `OnAchievementsFetchPlayer`   | `List<AchievementsFetchGroups>` |
| `OnAchievementsUnlock`        | `string`                        |
| `OnAchievementsUnlockError`   | `string`                        |
| `OnAchievementsProgress`      | `string`                        |
| `OnAchievementsProgressError` | void                            |

### Data structures

```c
public class AchievementsFetch
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public string icon;
    public string iconSmall;
    public string lockedIcon;
    public string lockedIconSmall;
    public string rare;
    public int maxProgress;
    public int progressStep;
    public bool lockedVisible;
    public bool lockedDescriptionVisible;
}
```

```c
public class AchievementsFetchGroups
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public int[] achievements;
}
```

```c
public class AchievementsFetchPlayer
{
    public int achievementId;
    public string createdAt;
    public int progress;
    public bool unlocked;
}
```

## GP_Ads

[Ads documentation](https://docs.gamepush.com/docs/advertising/)

### Methods

| Method name                       | Method parameters                                                                                                                      | Return value |
| --------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- | ------------ |
| `ShowFullscreen`                  | `Action onFullscreenStart = null, Action<bool> onFullscreenClose = null`                                                               | void         |
| `ShowRewarded`                    | `string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null` | void         |
| `ShowPreloader`                   | `Action onPreloaderStart = null, Action<bool> onPreloaderClose = null`                                                                 | void         |
| `ShowSticky`                      | void                                                                                                                                   | void         |
| `CloseSticky`                     | void                                                                                                                                   | void         |
| `RefreshSticky`                   | void                                                                                                                                   | void         |
| `IsAdblockEnabled`                | void                                                                                                                                   | `bool`       |
| `IsStickyAvailable`               | void                                                                                                                                   | `bool`       |
| `IsFullscreenAvailable`           | void                                                                                                                                   | `bool`       |
| `IsRewardedAvailable`             | void                                                                                                                                   | `bool`       |
| `IsPreloaderAvailable`            | void                                                                                                                                   | `bool`       |
| `IsStickyPlaying`                 | void                                                                                                                                   | `bool`       |
| `IsFullscreenPlaying`             | void                                                                                                                                   | `bool`       |
| `IsRewardPlaying`                 | void                                                                                                                                   | `bool`       |
| `IsPreloaderPlaying`              | void                                                                                                                                   | `bool`       |
| `IsCountdownOverlayEnabled`       | void                                                                                                                                   | `bool`       |
| `IsRewardedFailedOverlayEnabled`  | void                                                                                                                                   | `bool`       |
| `CanShowFullscreenBeforeGamePlay` | void                                                                                                                                   | `bool`       |

### Actions

| Action name         | Return value |
| ------------------- | ------------ |
| `OnAdsStart`        | void         |
| `OnAdsClose`        | `bool`       |
| `OnFullscreenStart` | void         |
| `OnFullscreenClose` | `bool`       |
| `OnPreloaderStart`  | void         |
| `OnPreloaderClose`  | `bool`       |
| `OnRewardedStart`   | void         |
| `OnRewardedClose`   | `bool`       |
| `OnRewardedReward`  | `string`     |
| `OnStickyStart`     | void         |
| `OnStickyClose`     | void         |
| `OnStickyRefresh`   | void         |
| `OnStickyRender`    | void         |

## GP_Analytics

[Analytics documentation](https://docs.gamepush.com/docs/analytics/)

### Methods

| Method name | Method parameters                | Return value |
| ----------- | -------------------------------- | ------------ |
| `Hit`       | `string url`                     | void         |
| `Goal`      | `string eventName, string value` | void         |
| `Goal`      | `string eventName, int value`    | void         |

## GP_App

[Application documentation](https://docs.gamepush.com/docs/application/)

### Methods

| Method name         | Method parameters                                                        | Return value |
| ------------------- | ------------------------------------------------------------------------ | ------------ |
| `Title`             | void                                                                     | `string`     |
| `Description`       | void                                                                     | `string`     |
| `GetImage`          | `Image image`                                                            | void         |
| `ImageUrl`          | void                                                                     | `string`     |
| `Url`               | void                                                                     | `string`     |
| `ReviewRequest`     | `Action<int> onReviewResult = null, Action<string> onReviewClose = null` | void         |
| `IsAlreadyReviewed` | void                                                                     | `bool`       |
| `CanReview`         | void                                                                     | `bool`       |
| `AddShortcut`       | `Action<bool> onAddShortcut = null`                                      | void         |
| `CanAddShortcut`    | void                                                                     | `bool`       |

### Actions

| Action name      | Return value |
| ---------------- | ------------ |
| `OnReviewResult` | `int`        |
| `OnReviewClose`  | `string`     |
| `OnAddShortcut`  | `bool`       |

## GP_AvatarGenerator

### Methods

| Method name | Method parameters         | Return value |
| ----------- | ------------------------- | ------------ |
| `Current`   | void                      | `string`     |
| `Change`    | `GeneratorType generator` | void         |

### Actions

| Action name | Return value |
| ----------- | ------------ |
| `OnChange`  | `string`     |

### Data structures

```c
public enum GeneratorType : byte
{
    dicebear_retro,
    dicebear_identicon,
    dicebear_human,
    dicebear_micah,
    dicebear_bottts,
    icotar,
    robohash_robots,
    robohash_cats,
}
```

## GP_Channels

[Channels documentation](https://docs.gamepush.com/docs/channels/)

### Methods

| Method name                 | Method parameters                                                                                     | Return value |
| --------------------------- | ----------------------------------------------------------------------------------------------------- | ------------ |
| `OpenChat`                  | `Action onOpen = null, Action onClose = null, Action onOpenError = null`                              | void         |
| `OpenChat`                  | `int channel_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null` | void         |
| `OpenPersonalChat`          | `int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null`  | void         |
| `OpenFeed`                  | `int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null`  | void         |
| `IsMainChatEnabled`         | void                                                                                                  | `bool`       |
| `MainChatId`                | void                                                                                                  | `int`        |
| `Join`                      | `int channel_ID`                                                                                      | void         |
| `Join`                      | `int channel_ID, string password`                                                                     | void         |
| `CancelJoin`                | `int channel_ID`                                                                                      | void         |
| `Leave`                     | `int channel_ID`                                                                                      | void         |
| `Kick`                      | `int channel_ID, int player_ID`                                                                       | void         |
| `Mute`                      | `int channel_ID, int player_ID, int seconds`                                                          | void         |
| `Mute`                      | `int channel_ID, int player_ID, string unmuteAT`                                                      | void         |
| `UnMute`                    | `int channel_ID, int player_ID`                                                                       | void         |
| `SendInvite`                | `int channel_ID, int player_ID`                                                                       | void         |
| `CancelInvite`              | `int channel_ID, int player_ID`                                                                       | void         |
| `AcceptInvite`              | `int channel_ID`                                                                                      | void         |
| `RejectInvite`              | `int channel_ID`                                                                                      | void         |
| `FetchInvites`              | `int limit = 50, int offset = 0`                                                                      | void         |
| `FetchMoreInvites`          | `int limit = 50`                                                                                      | void         |
| `FetchChannelInvites`       | `int channel_ID, int limit = 50, int offset = 0`                                                      | void         |
| `FetchMoreChannelInvites`   | `int channel_ID, int limit = 50`                                                                      | void         |
| `FetchSentInvites`          | `int channel_ID, int limit = 50, int offset = 0`                                                      | void         |
| `FetchMoreSentInvites`      | `int channel_ID, int limit = 50`                                                                      | void         |
| `AcceptJoinRequest`         | `int channel_ID, int player_ID`                                                                       | void         |
| `RejectJoinRequest`         | `int channel_ID, int player_ID`                                                                       | void         |
| `FetchJoinRequests`         | `int channel_ID, int limit = 50, int offset = 0`                                                      | void         |
| `FetchMoreJoinRequests`     | `int channel_ID, int limit = 50`                                                                      | void         |
| `FetchSentJoinRequests`     | `int limit = 50, int offset = 0`                                                                      | void         |
| `FetchMoreSentJoinRequests` | `int limit = 50`                                                                                      | void         |
| `SendMessage`               | `int channel_ID, string text, string tags = ""`                                                       | void         |
| `SendPersonalMessage`       | `int player_ID, string text, string tags = ""`                                                        | void         |
| `SendFeedMessage`           | `int player_ID, string text, string tags = ""`                                                        | void         |
| `EditMessage`               | `string message_ID, string text`                                                                      | void         |
| `DeleteMessage`             | `string message_ID`                                                                                   | void         |
| `FetchMessages`             | `int channel_ID, string tags, int limit = 50, int offset = 0`                                         | void         |
| `FetchPersonalMessages`     | `int player_ID, string tags, int limit = 50, int offset = 0`                                          | void         |
| `FetchFeedMessages`         | `int player_ID, string tags, int limit = 50, int offset = 0`                                          | void         |
| `FetchMoreMessages`         | `int channel_ID, string tags, int limit = 50`                                                         | void         |
| `FetchMorePersonalMessages` | `int player_ID, string tags, int limit = 50`                                                          | void         |
| `FetchMoreFeedMessages`     | `int player_ID, string tags, int limit = 50`                                                          | void         |
| `DeleteChannel`             | `int channel_ID`                                                                                      | void         |
| `FetchChannel`              | `int channel_ID`                                                                                      | void         |
| `CreateChannel`             | `CreateChannelFilter filter`                                                                          | void         |
| `UpdateChannel`             | `UpdateChannelFilter filter`                                                                          | void         |
| `FetchChannels`             | `FetchChannelsFilter filter`                                                                          | void         |
| `FetchMoreChannels`         | `FetchMoreChannelsFilter filter`                                                                      | void         |
| `FetchMembers`              | `FetchMembersFilter filter`                                                                           | void         |
| `FetchMoreMembers`          | `FetchMoreMembersFilter filter`                                                                       | void         |

### Actions

| Action name                        | Return value                   |
| ---------------------------------- | ------------------------------ |
| `OnCreateChannel`                  | `CreateChannelData`            |
| `OnCreateChannelError`             | void                           |
| `OnUpdateChannel`                  | `UpdateChannelData`            |
| `OnUpdateChannelError`             | void                           |
| `OnDeleteChannelSuccess`           | void                           |
| `OnCreateChannel`                  | `int`                          |
| `OnDeleteChannelError`             | void                           |
| `OnFetchChannel`                   | `FetchChannelData`             |
| `OnFetchChannelError`              | void                           |
| `OnFetchChannels`                  | `List<FetchChannelData>, bool` |
| `OnFetchChannelsError`             | void                           |
| `OnFetchMoreChannels`              | `List<FetchChannelData>, bool` |
| `OnFetchMoreChannelsError`         | void                           |
| `OnJoinSuccess`                    | void                           |
| `OnJoinEvent`                      | `GP_Data`                      |
| `OnJoinError`                      | void                           |
| `OnJoinRequest`                    | `GP_Data`                      |
| `OnCancelJoinSuccess`              | void                           |
| `OnCancelJoinEvent`                | `CancelJoinData`               |
| `OnCancelJoinError`                | void                           |
| `OnLeaveSuccess`                   | void                           |
| `OnLeaveEvent`                     | `MemberLeaveData`              |
| `OnLeaveError`                     | void                           |
| `OnKick`                           | void                           |
| `OnKickError`                      | void                           |
| `OnFetchMembers`                   | `GP_Data, bool`                |
| `OnFetchMembersError`              | void                           |
| `OnFetchMoreMembers`               | `GP_Data, bool`                |
| `OnFetchMoreMembersError`          | void                           |
| `OnMuteSuccess`                    | void                           |
| `OnMuteEvent`                      | `MuteData`                     |
| `OnMuteError`                      | void                           |
| `OnUnmuteSuccess`                  | void                           |
| `OnUnmuteEvent`                    | `UnmuteData`                   |
| `OnUnmuteError`                    | void                           |
| `OnSendInvite`                     | void                           |
| `OnSendInviteError`                | void                           |
| `OnInvite`                         | `InviteData`                   |
| `OnCancelInviteSuccess`            | void                           |
| `OnCancelInviteEvent`              | `CancelInviteData`             |
| `OnCancelInviteError`              | void                           |
| `OnAcceptInvite`                   | void                           |
| `OnAcceptInviteError`              | void                           |
| `OnRejectInviteSuccess`            | void                           |
| `OnRejectInviteEvent`              | `RejectInviteData`             |
| `OnRejectInviteError`              | void                           |
| `OnFetchInvites`                   | `GP_Data, bool`                |
| `OnFetchInvitesError`              | void                           |
| `OnFetchMoreInvites`               | `GP_Data, bool`                |
| `OnFetchMoreInvitesError`          | void                           |
| `OnFetchChannelInvites`            | `GP_Data, bool`                |
| `OnFetchChannelInvitesError`       | void                           |
| `OnFetchMoreChannelInvites`        | `GP_Data, bool`                |
| `OnFetchMoreChannelInvitesError`   | void                           |
| `OnFetchSentInvites`               | `GP_Data, bool`                |
| `OnFetchSentInvitesError`          | void                           |
| `OnFetchMoreSentInvites`           | `GP_Data, bool`                |
| `OnFetchMoreSentInvitesError`      | void                           |
| `OnAcceptJoinRequest`              | void                           |
| `OnAcceptJoinRequestError`         | void                           |
| `OnRejectJoinRequestSuccess`       | void                           |
| `OnRejectJoinRequestEvent`         | `RejectJoinRequestData`        |
| `OnRejectJoinRequestError`         | void                           |
| `OnFetchJoinRequests`              | `GP_Data, bool`                |
| `OnFetchJoinRequestsError`         | void                           |
| `OnFetchMoreJoinRequests`          | `GP_Data, bool`                |
| `OnFetchMoreJoinRequestsError`     | void                           |
| `OnFetchSentJoinRequests`          | `List<JoinRequestsData>, bool` |
| `OnFetchSentJoinRequestsError`     | void                           |
| `OnFetchMoreSentJoinRequests`      | `List<JoinRequestsData>, bool` |
| `OnFetchMoreSentJoinRequestsError` | void                           |
| `OnSendMessage`                    | `GP_Data`                      |
| `OnSendMessageError`               | void                           |
| `OnMessage`                        | `GP_Data`                      |
| `OnEditMessageSuccess`             | `GP_Data`                      |
| `OnEditMessageEvent`               | `MessageData`                  |
| `OnEditMessageError`               | void                           |
| `OnDeleteMessageSuccess`           | void                           |
| `OnDeleteMessageEvent`             | `MessageData`                  |
| `OnDeleteMessageError`             | void                           |
| `OnFetchMessages`                  | `GP_Data, bool`                |
| `OnFetchMessagesError`             | void                           |
| `OnFetchPersonalMessages`          | `GP_Data, bool`                |
| `OnFetchPersonalMessagesError`     | void                           |
| `OnFetchFeedMessages`              | `GP_Data, bool`                |
| `OnFetchFeedMessagesError`         | void                           |
| `OnFetchMoreMessages`              | `GP_Data, bool`                |
| `OnFetchMoreMessagesError`         | void                           |
| `OnFetchMorePersonalMessages`      | `GP_Data, bool`                |
| `OnFetchMorePersonalMessagesError` | void                           |
| `OnFetchMoreFeedMessages`          | `GP_Data, bool`                |
| `OnFetchMoreFeedMessagesError`     | void                           |
| `OnOpenChat`                       | void                           |
| `OnOpenChatError`                  | void                           |
| `OnCloseChat`                      | void                           |

### Data structures

```c
public class CreateChannelData
{
    public int id;
    public string[] tags;
    public string[] messageTags;
    public int templateId;
    public int capacity;
    public int ownerId;
    public string name;
    public string description;
    public bool ch_private;
    public bool visible;
    public bool permanent;
    public bool hasPassword;
    public bool isJoined;
    public bool isRequestSent;
    public bool isInvited;
    public bool isMuted;
    public string password;
    public int membersCount;
    public OwnerAcl ownerAcl;
    public MemberAcl memberAcl;
    public GuestAcl guestAcl;
}
```

```c
public class UpdateChannelData
{
    public int id;
    public string[] tags;
    public string[] messageTags;
    public int channelId;
    public int capacity;
    public int ownerId;
    public string name;
    public string description;
    public bool ch_private;
    public bool visible;
    public bool permanent;
    public bool hasPassword;
    public bool isJoined;
    public bool isRequestSent;
    public bool isInvited;
    public bool isMuted;
    public string password;
    public int membersCount;
    public OwnerAcl ownerAcl;
    public MemberAcl memberAcl;
    public GuestAcl guestAcl;
}
```

```c
public class FetchChannelData
{
    public int id;
    public string[] tags;
    public string[] messageTags;
    public int templateId;
    public int projectId;
    public int capacity;
    public int ownerId;
    public string name;
    public string description;
    public bool ch_private;
    public bool visible;
    public bool permanent;
    public bool hasPassword;
    public string password;
    public bool isJoined;
    public bool isInvited;
    public bool isMuted;
    public bool isRequestSent;
    public int membersCount;
    public OwnerAcl ownerAcl;
    public MemberAcl memberAcl;
    public GuestAcl guestAcl;
}
```

```c
public class JoinRequestsData
{
    public FetchChannelData channel;
    public string date;
}
```

```c
public class MessageData
{
    public string id;
    public int channelId;
    public int authorId;
    public string text;
    public string[] tags;
    public string createdAt;
}
```

```c
public class CancelJoinData
{
    public int channelId;
    public int playerId;
}
```

```c
public class MemberLeaveData
{
    public int channelId;
    public int playerId;
    public string reason;
}
```

```c
public class MuteData
{
    public int channelId;
    public int playerId;
    public string unmuteAt;
}
```

```c
public class UnmuteData
{
    public int channelId;
    public int playerId;
}
```

```c
public class InviteData
{
    public int channelId;
    public int playerFromId;
    public int playerToId;
    public string date;
}
```

```c
public class CancelInviteData
{
    public int channelId;
    public int playerFromId;
    public int playerToId;
}
```

```c
public class RejectInviteData
{
    public int channelId;
    public int playerFromId;
    public int playerToId;
}
```

```c
public class RejectJoinRequestData
{
    public int channelId;
    public int playerId;
}
```

```c
public class CreateChannelFilter
{
    public CreateChannelFilter(int Template_ID)
    {
        template = Template_ID;
    }
    public int template;
    public string[] tags;
    public int capacity;
    public string name;
    public string description;
    public bool ch_private;
    public bool visible;
    public string password;
    public OwnerAcl ownerAcl;
    public MemberAcl memberAcl;
    public GuestAcl guestAcl;
}
```

```c
public class UpdateChannelFilter
{
    public UpdateChannelFilter(int Channel_ID)
    {
        channelId = Channel_ID;
    }
    public int channelId;
    public string[] tags;
    public int capacity;
    public string name;
    public string description;
    public bool ch_private;
    public bool visible;
    public string password;
    public int ownerId;
    public OwnerAcl ownerAcl;
    public MemberAcl memberAcl;
    public GuestAcl guestAcl;

}
```

```c
public class FetchChannelsFilter
{
    public int[] ids;
    public string[] tags;
    public string search;
    public bool onlyJoined = false;
    public bool onlyOwned = false;
    public int limit = 100;
    public int offset = 0;
}
```

```c
public class FetchMoreChannelsFilter
{
    public int[] ids;
    public string[] tags;
    public string search;
    public bool onlyJoined = false;
    public bool onlyOwned = false;
    public int limit;
}
```

```c
public class FetchMembersFilter
{
    public FetchMembersFilter(int Channel_ID)
    {
        channelId = Channel_ID;
    }
    public int channelId;
    public string search;
    public bool onlyOnline = false;
    public int limit = 100;
    public int offset = 0;
}

```

```c
public class FetchMoreMembersFilter
{
    public FetchMoreMembersFilter(int Channel_ID)
    {
        channelId = Channel_ID;
    }
    public int channelId;
    public string search;
    public bool onlyOnline = false;
    public int limit = 100;
}
```

```c
public class OwnerAcl
{
    public bool canViewMessages = true;
    public bool canAddMessage = true;
    public bool canEditMessage = true;
    public bool canDeleteMessage = true;
    public bool canViewMembers = true;
    public bool canInvitePlayer = true;
    public bool canKickPlayer = true;
    public bool canAcceptJoinRequest = true;
    public bool canMutePlayer = true;
}
```

```c
public class MemberAcl
{
    public bool canViewMessages = true;
    public bool canAddMessage = true;
    public bool canEditMessage = true;
    public bool canDeleteMessage = true;
    public bool canViewMembers = true;
    public bool canInvitePlayer = false;
    public bool canKickPlayer = false;
    public bool canAcceptJoinRequest = false;
    public bool canMutePlayer = false;
}
```

```c
public class GuestAcl
{
    public bool canViewMessages = false;
    public bool canAddMessage = false;
    public bool canEditMessage = false;
    public bool canDeleteMessage = false;
    public bool canViewMembers = false;
    public bool canInvitePlayer = false;
    public bool canKickPlayer = false;
    public bool canAcceptJoinRequest = false;
    public bool canMutePlayer = false;
}
```

## GP_Custom

### Methods

| Method name   | Method parameters                                                                                                      | Return value |
| ------------- | ---------------------------------------------------------------------------------------------------------------------- | ------------ |
| `Call`        | `string name, string args = null`                                                                                      | void         |
| `Value`       | `string path`                                                                                                          | `string`     |
| `Return`      | `string name, string args = null`                                                                                      | `string`     |
| `AsyncReturn` | `string name, string args = null, Action<string> onCustomAsyncReturn = null, Action<string> onCustomAsyncError = null` | void         |

### Actions

| Action name           | Return value |
| --------------------- | ------------ |
| `OnCustomAsyncReturn` | `string`     |
| `OnCustomAsyncError`  | `string`     |

## GP_Device

[Device documentation](https://docs.gamepush.com/docs/get-start/common-features/#device-detection)

### Methods

| Method name  | Method parameters | Return value |
| ------------ | ----------------- | ------------ |
| `IsMobile`   | void              | `bool`       |
| `IsDesktop`  | void              | `bool`       |
| `IsPortrait` | void              | `bool`       |

### Actions

| Action name           | Return value |
| --------------------- | ------------ |
| `OnChangeOrientation` | void         |

## GP_Documents

[Documents documentation](https://docs.gamepush.com/docs/documents/)

### Methods

| Method name | Method parameters                                                  | Return value |
| ----------- | ------------------------------------------------------------------ | ------------ |
| `Open`      | `Action onDocumentsOpen = null, Action onDocumentsClose = null`    | void         |
| `Fetch`     | `Action<string> onFetchSuccess = null, Action onFetchError = null` | void         |

### Actions

| Action name        | Return value |
| ------------------ | ------------ |
| `OnDocumentsOpen`  | void         |
| `OnDocumentsClose` | void         |
| `OnFetchSuccess`   | `string`     |
| `OnFetchError`     | void         |

## GP_Events

[Events documentation](https://docs.gamepush.com/docs/events/)

### Methods

| Method name  | Method parameters | Return value     |
| ------------ | ----------------- | ---------------- |
| `Join`       | `string idOrTag`  | void             |
| `List`       | void              | `EventData[]`    |
| `ActiveList` | void              | `PlayerEvents[]` |
| `GetEvent`   | `string idOrTag`  | `EventData`      |
| `IsActive`   | `string idOrTag`  | `bool`           |
| `IsJoined`   | `string idOrTag`  | `bool`           |

### Actions

| Action name        | Return value   |
| ------------------ | -------------- |
| `OnEventJoin`      | `PlayerEvents` |
| `OnEventJoinError` | `string`       |

### Data structures

```c
public class EventData
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public string icon;
    public string iconSmall;
    public string dateStart;
    public string dateEnd;
    public bool isActive;
    public int timeLeft;
    public bool isAutoJoin;
    public TriggerData[] triggers;
}
```

```c
public class PlayerEvents
{
    public int eventId;
    public EventStats stats;
}
```

```c
public class EventStats
{
    public int activeDays;
    public int activeDaysConsecutive;
}
```

## GP_Experiments

[Experiments documentation](https://docs.gamepush.com/docs/experiments/)

### Methods

| Method name | Method parameters           | Return value |
| ----------- | --------------------------- | ------------ |
| `Map`       | void                        | `string`     |
| `Has`       | `string tag, string cohort` | `bool`       |

## GP_Files

[Files documentation](https://docs.gamepush.com/docs/files/)

### Methods

| Method name     | Method parameters                                                                                                           | Return value |
| --------------- | --------------------------------------------------------------------------------------------------------------------------- | ------------ |
| `Upload`        | `string tags, Action<FileData> onUpload = null, Action onUploadError = null`                                                | void         |
| `UploadUrl`     | `string url, string filename = "", string tags = "", Action<FileData> onUploadUrl = null, Action onUploadUrlError = null`   | void         |
| `UploadContent` | `string content, string filename, string tags, Action<FileData> onUploadContent = null, Action onUploadContentError = null` | void         |
| `LoadContent`   | `sstring url, Action<string> onLoadContent = null, Action onLoadContentError = null`                                        | void         |
| `ChooseFile`    | `string type, Action<string> onFileChoose = null, Action onFileChooseError = null`                                          | void         |
| `Fetch`         | `FilesFetchFilter filter = null, Action<List<FileData>, bool> onFetch = null, Action onFetchError = null`                   | void         |
| `FetchMore`     | `FilesFetchMoreFilter filter = null, Action<List<FileData>, bool> onFetchMore = null, Action onFetchMoreError = null`       | void         |

### Actions

| Action name              | Return value           |
| ------------------------ | ---------------------- |
| `OnUploadSuccess`        | `FileData`             |
| `OnUploadError`          | void                   |
| `OnUploadUrlSuccess`     | `FileData`             |
| `OnUploadUrlError`       | void                   |
| `OnUploadContentSuccess` | `FileData`             |
| `OnUploadContentError`   | void                   |
| `OnUploadContentSuccess` | `FileData`             |
| `OnUploadContentError`   | void                   |
| `OnChooseFile`           | `string`               |
| `OnChooseFileError`      | void                   |
| `OnLoadContent`          | `string`               |
| `OnLoadContentError`     | void                   |
| `OnFetchSuccess`         | `List<FileData>, bool` |
| `OnFetchError`           | void                   |
| `OnFetchMoreSuccess`     | `List<FileData>, bool` |
| `OnFetchMoreError`       | void                   |

### Data structures

```c
public class FileData
{
    public string id;
    public int playerId;
    public string name;
    public string src;
    public float size;
    public string[] tags;
}
```

```c
public class FilesFetchFilter
{
    public string[] tags;
    public int playerId;
    public int limit = 10;
    public int offset = 0;
}
```

```c
public class FilesFetchMoreFilter
{
    public string[] tags;
    public int playerId;
    public int limit = 10;
}
```

## GP_Fullscreen

[Fullscreen documentation](https://docs.gamepush.com/docs/fullscreen/)

### Methods

| Method name | Method parameters                 | Return value |
| ----------- | --------------------------------- | ------------ |
| `Open`      | `Action onFullscreenOpen = null`  | void         |
| `Close`     | `Action onFullscreenClose = null` | void         |
| `Toggle`    | void                              | void         |

### Actions

| Action name          | Return value |
| -------------------- | ------------ |
| `OnFullscreenOpen`   | void         |
| `OnFullscreenClose`  | void         |
| `OnFullscreenChange` | void         |

## GP_Game

[Game documentation](https://docs.gamepush.com/docs/get-start/common-features/#pause)

### Methods

| Method name     | Method parameters        | Return value |
| --------------- | ------------------------ | ------------ |
| `IsPaused`      | void                     | `bool`       |
| `Pause`         | `Action onPause = null`  | void         |
| `Resume`        | `Action onResume = null` | void         |
| `GameplayStart` | void                     | void         |
| `GameplayStop`  | void                     | void         |
| `GameReady`     | void                     | void         |
| `HappyTime`     | void                     | void         |

### Actions

| Action name | Return value |
| ----------- | ------------ |
| `OnPause`   | void         |
| `OnResume`  | void         |

## GP_GamesCollections

[GamesCollections documentation](https://docs.gamepush.com/docs/games-collections/)

### Methods

| Method name | Method parameters                                                                                             | Return value |
| ----------- | ------------------------------------------------------------------------------------------------------------- | ------------ |
| `Open`      | `tring idOrTag, Action onGamesCollectionsOpen = null, Action onGamesCollectionsClose = null`                  | void         |
| `Fetch`     | `string idOrTag, Action<string, GamesCollectionsFetchData> onFetchSuccess = null, Action onFetchError = null` | void         |

### Actions

| Action name                    | Return value                        |
| ------------------------------ | ----------------------------------- |
| `OnGamesCollectionsOpen`       | void                                |
| `OnGamesCollectionsClose`      | void                                |
| `OnGamesCollectionsFetch`      | `string, GamesCollectionsFetchData` |
| `OnGamesCollectionsFetchError` | void                                |

### Data structures

```c
public class GamesCollectionsFetchData
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public Games[] games;
}
```

```c
public class Games
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public string url;
}
```

## GP_Images

[Images documentation](https://docs.gamepush.com/docs/images/)

### Methods

| Method name   | Method parameters                                                                                                                   | Return value |
| ------------- | ----------------------------------------------------------------------------------------------------------------------------------- | ------------ |
| `Choose`      | `Action<string> onImagesChooseFile = null, Action<string> onImagesChooseError = null`                                               | void         |
| `Upload`      | `string[] tags = null, Action<ImageData> onImagesUploadSuccess = null, Action<string> onImagesUploadError = null`                   | void         |
| `UploadUrl`   | `string url, string[] tags = null, Action<ImageData> onImagesUploadUrlSuccess = null, Action<string> onImagesUploadUrlError = null` | void         |
| `Fetch`       | `ImagesFetchFilter filter = null, Action<List<ImageData>> onImagesFetchSuccess = null, Action<string> onImagesFetchError = null`    | void         |
| `FetchMore`   | `ImagesFetchFilter filter = null, Action<List<ImageData>> onImagesFetchSuccess = null, Action<string> onImagesFetchError = null`    | void         |
| `Resize`      | `ImageResizeData resizeData = null, Action<string> onImagesResize = null, Action<string> onImagesResizeError = null`                | void         |
| `Resize`      | `ImageResizeData resizeData = null, Action<string> onImagesResize = null, Action<string> onImagesResizeError = null`                | void         |
| `FormatToPng` | `string url`                                                                                                                        | `string`     |
| `FormatUrl`   | `string url, string format`                                                                                                         | `string`     |

### Actions

| Action name                | Return value      |
| -------------------------- | ----------------- |
| `OnImagesFetchSuccess`     | `List<ImageData>` |
| `OnImagesFetchError`       | `string`          |
| `OnImagesCanLoadMore`      | `bool`            |
| `OnImagesUploadSuccess`    | `ImageData`       |
| `OnImagesUploadError`      | `string`          |
| `OnImagesUploadUrlSuccess` | `ImageData`       |
| `OnImagesUploadUrlError`   | `string`          |
| `OnImagesChooseFile`       | `string`          |
| `OnImagesChooseError`      | `string`          |
| `OnImagesResize`           | `string`          |
| `OnImagesResizeError`      | `string`          |

### Data structures

```c
public class ImageData
{
    public string id;
    public int playerId;
    public string src;
    public string[] tags;
    public int width;
    public int height;
}
```

```c
public class ImageResizeData
{
    public string url;
    public int width = 256;
    public int height = 256;
    public bool cutBySize = true;
}
```

```c
public class ImagesFetchFilter
{
    public string[] tags;
    public int playerId;
    public int limit = 10;
    public int offset = 0;
}
```

## GP_Language

[Language documentation](https://docs.gamepush.com/docs/get-start/common-features/#language)

### Methods

| Method name | Method parameters                                         | Return value |
| ----------- | --------------------------------------------------------- | ------------ |
| `Current`   | void                                                      | `Language`   |
| `Change`    | `Language lang, Action<Language> onLanguageChange = null` | void         |

### Actions

| Action name        | Return value |
| ------------------ | ------------ |
| `OnChangeLanguage` | `Language`   |

### Data structures

```c
public enum Language : byte
{
    English,
    Russian,
    Turkish,
    French,
    Italian,
    German,
    Spanish,
    Chineese,
    Portuguese,
    Korean,
    Japanese,
    Arab,
    Hindi,
    Indonesian,
}
```

## GP_Leaderboard

[Leaderboard documentation](https://docs.gamepush.com/docs/leaderboards/leaderboard/)

### Methods

| Method name         | Method parameters                                                                                                                                                            | Return value |
| ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------ |
| `Open`              | `string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 5, WithMe withMe = WithMe.none, string includeFields = "", string displayFields = ""` | void         |
| `Fetch`             | `string tag = "", string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = ""`           | void         |
| `FetchPlayerRating` | `string tag = "", string orderBy = "score", Order order = Order.DESC`                                                                                                        | void         |

### Actions

| Action name                  | Return value      |
| ---------------------------- | ----------------- |
| `OnFetchSuccess`             | `string, GP_Data` |
| `OnFetchTopPlayers`          | `string, GP_Data` |
| `OnFetchAbovePlayers`        | `string, GP_Data` |
| `OnFetchSuccess`             | `string, GP_Data` |
| `OnFetchBelowPlayers`        | `string, GP_Data` |
| `OnFetchPlayer`              | `string, GP_Data` |
| `OnFetchError`               | void              |
| `OnFetchPlayerRatingSuccess` | `string, int`     |
| `OnFetchPlayerRatingError`   | void              |
| `OnLeaderboardOpen`          | void              |
| `OnLeaderboardClose`         | void              |

### Data structures

```c
public enum Order : byte
{
    DESC,
    ASC
}
```

```c
public enum WithMe : byte
{
    none,
    first,
    last
}
```

## GP_LeaderboardScoped

[Scoped Leaderboard documentation](https://docs.gamepush.com/docs/leaderboards/scoped-leaderboard/)

### Methods

| Method name         | Method parameters                                                                                                                                                                                         | Return value |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------ |
| `Open`              | `string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", string displayFields = "", WithMe withMe = WithMe.first` | void         |
| `Fetch`             | `string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", WithMe withMe = WithMe.none`                             | void         |
| `PublishRecord`     | `string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", int record_value1 = 0, string key2 = "", int record_value2 = 0, string key3 = "", int record_value3 = 0`   | void         |
| `FetchPlayerRating` | `string idOrTag = "", string variant = "some_variant", string includeFields = ""`                                                                                                                         | void         |

### Actions

| Action name                     | Return value              |
| ------------------------------- | ------------------------- |
| `OnFetchSuccess`                | `string, GP_Data`         |
| `OnFetchTopPlayers`             | `string, GP_Data`         |
| `OnFetchAbovePlayers`           | `string, GP_Data`         |
| `OnFetchSuccess`                | `string, GP_Data`         |
| `OnFetchBelowPlayers`           | `string, GP_Data`         |
| `OnFetchPlayer`                 | `string, GP_Data`         |
| `OnFetchTagVariant`             | `string, string, GP_Data` |
| `OnFetchError`                  | void                      |
| `OnFetchPlayerRating`           | `string, int`             |
| `OnFetchPlayerRatingTagVariant` | `string, string, int`     |
| `OnFetchPlayerRatingError`      | void                      |
| `OnOpen`                        | void                      |
| `OnClose`                       | void                      |
| `OnPublishRecordComplete`       | void                      |
| `OnPublishRecordError`          | void                      |

## GP_Payments

[Payments documentation](https://docs.gamepush.com/docs/purchases/)

### Methods

| Method name                | Method parameters                                                                              | Return value |
| -------------------------- | ---------------------------------------------------------------------------------------------- | ------------ |
| `Fetch`                    | void                                                                                           | void         |
| `Purchase`                 | `string idOrTag, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null`       | void         |
| `Consume`                  | `string idOrTag, Action<string> onConsumeSuccess = null, Action onConsumeError = null`         | void         |
| `IsPaymentsAvailable`      | void                                                                                           | `bool`       |
| `IsSubscriptionsAvailable` | void                                                                                           | `bool`       |
| `Subscribe`                | `string idOrTag, Action<string> onSubscribeSuccess = null, Action onSubscribeError = null`     | void         |
| `Unsubscribe`              | `string idOrTag, Action<string> onUnsubscribeSuccess = null, Action onUnsubscribeError = null` | void         |

### Actions

| Action name              | Return value                 |
| ------------------------ | ---------------------------- |
| `OnFetchProducts`        | `List<FetchProducts>`        |
| `OnFetchProductsError`   | void                         |
| `OnFetchPlayerPurchases` | `List<FetchPlayerPurchases>` |
| `OnPurchaseSuccess`      | `string`                     |
| `OnPurchaseError`        | void                         |
| `OnConsumeSuccess`       | `string`                     |
| `OnConsumeError`         | void                         |
| `OnSubscribeSuccess`     | `string`                     |
| `OnSubscribeError`       | void                         |
| `OnUnsubscribeSuccess`   | `string`                     |
| `OnUnsubscribeError`     | void                         |

### Data structures

```c
public class FetchProducts
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public string icon;
    public string iconSmall;
    public int price;
    public string currency;
    public string currencySymbol;
    public bool isSubscription;
    public int period;
    public int trialPeriod;
}
```

```c
public class FetchPlayerPurchases
{
    public string tag;
    public int productId;
    public string payload;
    public string createdAt;
    public string expiredAt;
    public bool gift;
    public bool subscribed;
}
```

## GP_Platform

[Platform documentation](https://docs.gamepush.com/docs/get-start/platform/)

### Methods

| Method name              | Method parameters | Return value |
| ------------------------ | ----------------- | ------------ |
| `Type`                   | void              | `Platform`   |
| `TypeAsString`           | void              | `string`     |
| `HasIntegratedAuth`      | void              | `bool`       |
| `IsExternalLinksAllowed` | void              | `bool`       |

### Data structures

```c
public enum Platform : byte
{
    None = 0,
    YANDEX = 1,
    VK = 2,
    CRAZY_GAMES = 3,
    GAME_DISTRIBUTION = 4,
    GAME_MONETIZE = 5,
    OK = 6,
    SMARTMARKET = 7,
    GAMEPIX = 8,
    POKI = 9,
    VK_PLAY = 10,
    WG_PLAYGROUND = 11,
    KONGREGATE = 12,
    GOOGLE_PLAY = 13,
    PLAYDECK = 14,
    CUSTOM = 15
}
```

## GP_Player

[Player documentation](https://docs.gamepush.com/docs/player/)

### Methods

| Method name                | Method parameters                                          | Return value |
| -------------------------- | ---------------------------------------------------------- | ------------ |
| `GetID`                    | void                                                       | `int`        |
| `GetScore`                 | void                                                       | `float`      |
| `GetName`                  | void                                                       | `string`     |
| `GetAvatarUrl`             | void                                                       | `string`     |
| `GetAvatar`                | `Image image`                                              | void         |
| `GetFieldName`             | `string key`                                               | `string`     |
| `GetFieldVariantName`      | `string key, string value`                                 | `string`     |
| `GetFieldVariantAt`        | `string key, int index`                                    | `string`     |
| `GetFieldVariantIndex`     | `string key, string value`                                 | `string`     |
| `SetName`                  | `string name`                                              | void         |
| `SetAvatar`                | `string src`                                               | void         |
| `SetScore`                 | `float score`                                              | void         |
| `SetScore`                 | `int score`                                                | void         |
| `AddScore`                 | `float score`                                              | void         |
| `AddScore`                 | `int score`                                                | void         |
| `GetInt`                   | `string key`                                               | `int`        |
| `GetFloat`                 | `string key`                                               | `float`      |
| `GetString`                | `string key`                                               | `string`     |
| `GetBool`                  | `string key`                                               | `bool`       |
| `Set`                      | `string key, string value`                                 | void         |
| `Set`                      | `string key, int value`                                    | void         |
| `Set`                      | `string key, bool value`                                   | void         |
| `Set`                      | `string key, float value`                                  | void         |
| `SetFlag`                  | `string key, bool value`                                   | void         |
| `Add`                      | `string key, float value`                                  | void         |
| `Add`                      | `string key, int value`                                    | void         |
| `Toggle`                   | `string key`                                               | void         |
| `ResetPlayer`              | void                                                       | void         |
| `Remove`                   | void                                                       | void         |
| `Load`                     | void                                                       | void         |
| `Sync`                     | `bool forceOverride = false`                               | void         |
| `Login`                    | void                                                       | void         |
| `Logout`                   | void                                                       | void         |
| `FetchFields`              | `Action<List<PlayerFetchFieldsData>> onFetchFields = null` | void         |
| `Has`                      | `string key`                                               | `bool`       |
| `IsLoggedIn`               | void                                                       | `bool`       |
| `HasAnyCredentials`        | void                                                       | `bool`       |
| `IsStub`                   | void                                                       | `bool`       |
| `GetActiveDays`            | void                                                       | `int`        |
| `GetActiveDaysConsecutive` | void                                                       | `int`        |
| `GetPlaytimeToday`         | void                                                       | `int`        |
| `GetPlaytimeAll`           | void                                                       | `int`        |

### Actions

| Action name                   | Return value                  |
| ----------------------------- | ----------------------------- |
| `OnConnect`                   | void                          |
| `OnPlayerChange`              | void                          |
| `OnSyncComplete`              | void                          |
| `OnSyncError`                 | void                          |
| `OnLoadComplete`              | void                          |
| `OnLoadError`                 | void                          |
| `OnLoginComplete`             | void                          |
| `OnLoginError`                | void                          |
| `OnLogoutComplete`            | void                          |
| `OnLogoutError`               | void                          |
| `OnPlayerFetchFieldsComplete` | `List<PlayerFetchFieldsData>` |
| `OnPlayerFetchFieldsError`    | void                          |

### Data structures

```c
public class PlayerFetchFieldsData
{
    public string name;
    public string key;
    public string type;
    public string defaultValue; // string | bool | number
    public bool important;
    public Variants[] variants;
}
```

```c
public class PlayerFieldIncrement
{
    public float interval;
    public float increment;
}
```

```c
public class PlayerFieldLimits
{
    public float min;
    public float max;
    public bool couldGoOverLimit;
}
```

```c
public class PlayerFieldVariant
{
    public string value; // string | number
    public string name;
}
```

## GP_Players

[Players documentation](https://docs.gamepush.com/docs/player/players-data/)

### Methods

| Method name              | Method parameters                                                                        | Return value |
| ------------------------ | ---------------------------------------------------------------------------------------- | ------------ |
| `Fetch`                  | `int playerId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null`        | void         |
| `Fetch`                  | `List<int> playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null` | void         |
| `Fetch`                  | `int[] playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null`     | void         |
| `IsExternalLinksAllowed` | void                                                                                     | `bool`       |

### Actions

| Action name      | Return value |
| ---------------- | ------------ |
| `OnFetchSuccess` | `GP_Data`    |
| `OnFetchError`   | void         |

## GP_Rewards

[Rewards documentation](https://docs.gamepush.com/docs/rewards/)

### Methods

| Method name     | Method parameters                   | Return value     |
| --------------- | ----------------------------------- | ---------------- |
| `Give`          | `string idOrTag, bool lazy = false` | void             |
| `Accept`        | `string idOrTag`                    | void             |
| `List`          | void                                | `RewardData[]`   |
| `GivenList`     | void                                | `PlayerReward[]` |
| `GetReward`     | `string idOrTag`                    | `AllRewardData`  |
| `Has`           | `string idOrTag`                    | `bool`           |
| `HasAccepted`   | `string idOrTag`                    | `bool`           |
| `HasUnaccepted` | `string idOrTag`                    | `bool`           |

### Actions

| Action name            | Return value    |
| ---------------------- | --------------- |
| `OnRewardsGive`        | `AllRewardData` |
| `OnRewardsGiveError`   | `string`        |
| `OnRewardsAccept`      | `AllRewardData` |
| `OnRewardsAcceptError` | `string`        |

### Data structures

```c
public class AllRewardData
{
    public RewardData reward;
    public PlayerReward playerReward;
}
```

```c
public class RewardData
{
    public int id;
    public string tag;
    public string name;
    public string description;
    public string icon;
    public string iconSmall;
    public DataMutation[] mutations;
    public bool isAutoAccept;
}
```

```c
public class PlayerReward
{
    public int rewardId;
    public int countTotal;
    public int countAccepted;
}
```

```c
public class DataMutation
{
    public string type;
    public string key;
    public MutationAction action;
    public string value;
}
```

```c
public enum MutationAction
{
    ADD,
    REMOVE,
    SET
};
```

## GP_Schedulers

[Schedulers documentation](https://docs.gamepush.com/docs/schedulers/)

### Methods

| Method name              | Method parameters                                | Return value        |
| ------------------------ | ------------------------------------------------ | ------------------- |
| `Register`               | `string idOrTag`                                 | void                |
| `ClaimDay`               | `string idOrTag, int day`                        | void                |
| `ClaimDayAdditional`     | `string idOrTag, int day, string triggerIdOrTag` | void                |
| `ClaimAllDay`            | `string idOrTag, int day`                        | void                |
| `ClaimAllDays`           | `string idOrTag`                                 | void                |
| `List`                   | void                                             | `SchedulerData[]`   |
| `ActiveList`             | void                                             | `PlayerScheduler[]` |
| `GetScheduler`           | void                                             | `SchedulerInfo`     |
| `GetSchedulerDay`        | `string idOrTag, int day`                        | `SchedulerDayInfo`  |
| `GetSchedulerCurrentDay` | `string idOrTag`                                 | `SchedulerDayInfo`  |
| `IsRegistered`           | `string idOrTag`                                 | `bool`              |
| `IsTodayRewardClaimed`   | `string idOrTag`                                 | `bool`              |
| `CanClaimDay`            | `string idOrTag, int day`                        | `bool`              |
| `CanClaimDayAdditional`  | `string idOrTag, int day, string triggerIdOrTag` | `bool`              |
| `CanClaimAllDay`         | `string idOrTag, int day`                        | `bool`              |

### Actions

| Action name                          | Return value       |
| ------------------------------------ | ------------------ |
| `OnSchedulerRegister`                | `SchedulerInfo`    |
| `OnSchedulerRegisterError`           | `string`           |
| `OnSchedulerClaimDay`                | `SchedulerDayInfo` |
| `OnSchedulerClaimDayError`           | `string`           |
| `OnSchedulerClaimDayAdditional`      | `SchedulerDayInfo` |
| `OnSchedulerClaimDayAdditionalError` | `string`           |
| `OnSchedulerClaimAllDay`             | `SchedulerDayInfo` |
| `OnSchedulerClaimAllDayError`        | `string`           |
| `OnSchedulerClaimAllDays`            | `SchedulerInfo`    |
| `OnSchedulerClaimAllDaysError`       | `string`           |
| `OnSchedulerJoin`                    | `PlayerScheduler`  |
| `OnSchedulerJoinError`               | `string`           |

### Data structures

```c
public enum SchedulerType
{
    ACTIVE_DAYS,
    ACTIVE_DAYS_CONSECUTIVE
}
```

```c
public class SchedulerData
{
    public int id;
    public string tag;
    public SchedulerType type;
    public int days;
    public bool isRepeat;
    public bool isAutoRegister;
    public TriggerData[] triggers;
}
```

```c
public class PlayerScheduler
{
    public int schedulerId;
    public int daysClaimed;
    public PlayerSchedulerStats stats;
}
```

```c
public class PlayerSchedulerStats
{
    public int activeDays;
    public int activeDaysConsecutive;
}
```

```c
public class SchedulerInfo
{
    public SchedulerData scheduler;
    public PlayerSchedulerStats stats;
    public int[] daysClaimed;
    public bool isRegistered;
    public int currentDay;
}
```

```c
public class SchedulerDayInfo
{
    public SchedulerData scheduler;
    public int day;
    public bool isDayReached;
    public bool isDayComplete;
    public bool isDayClaimed;
    public bool isAllDayClaimed;
    public TriggerData[] triggers;
}
```

## GP_Segments

[Segments documentation](https://docs.gamepush.com/docs/segments/)

### Methods

| Method name | Method parameters | Return value |
| ----------- | ----------------- | ------------ |
| `List`      | void              | `string`     |
| `Has`       | `string tag`      | `bool`       |

### Actions

| Action name      | Return value |
| ---------------- | ------------ |
| `OnSegmentEnter` | `string`     |
| `OnSegmentLeave` | `string`     |

## GP_Server

[Server documentation](https://docs.gamepush.com/docs/get-start/common-features/#server-time)

### Methods

| Method name | Method parameters | Return value |
| ----------- | ----------------- | ------------ |
| `Time`      | void              | `DateTime`   |

## GP_Socials

[Socials documentation](https://docs.gamepush.com/docs/social-actions/)

### Methods

| Method name                     | Method parameters                                      | Return value |
| ------------------------------- | ------------------------------------------------------ | ------------ |
| `Share`                         | `string text = "", string url = "", string image = ""` | void         |
| `Post`                          | `string text = "", string url = "", string image = ""` | void         |
| `Invite`                        | `string text = "", string url = "", string image = ""` | void         |
| `JoinCommunity`                 | void                                                   | void         |
| `CommunityLink`                 | void                                                   | `string`     |
| `IsSupportsShare`               | void                                                   | `bool`       |
| `IsSupportsNativeShare`         | void                                                   | `bool`       |
| `IsSupportsNativePosts`         | void                                                   | `bool`       |
| `IsSupportsNativeInvite`        | void                                                   | `bool`       |
| `CanJoinCommunity`              | void                                                   | `bool`       |
| `IsSupportsNativeCommunityJoin` | void                                                   | `bool`       |
| `MakeShareLink`                 | `string content = ""`                                  | `string`     |
| `GetSharePlayerID`              | void                                                   | `int`        |
| `GetShareContent`               | void                                                   | `string`     |

### Actions

| Action name       | Return value |
| ----------------- | ------------ |
| `OnShare`         | `bool`       |
| `OnPost`          | `bool`       |
| `OnInvite`        | `bool`       |
| `OnJoinCommunity` | `bool`       |

## GP_System

[Game Host documentation](https://docs.gamepush.com/docs/get-start/common-features/#game-host-information)

### Methods

| Method name       | Method parameters | Return value |
| ----------------- | ----------------- | ------------ |
| `IsDev`           | void              | `bool`       |
| `IsAllowedOrigin` | void              | `bool`       |

## GP_Triggers

[Triggers documentation](https://docs.gamepush.com/docs/triggers/)

### Methods

| Method name     | Method parameters | Return value      |
| --------------- | ----------------- | ----------------- |
| `Claim`         | `string idOrTag`  | void              |
| `List`          | void              | `TriggerData[]`   |
| `ActivatedList` | void              | `TriggerActive[]` |
| `GetTrigger`    | `string idOrTag`  | `TriggerAllData`  |
| `IsActivated`   | `string idOrTag`  | `bool`            |
| `IsClaimed`     | `string idOrTag`  | `bool`            |

### Actions

| Action name           | Return value  |
| --------------------- | ------------- |
| `OnTriggerActivate`   | `TriggerData` |
| `OnTriggerClaim`      | `TriggerData` |
| `OnTriggerClaimError` | `string`      |

### Data structures

```c
public class TriggerAllData
{
    public TriggerData trigger;
    public bool isActivated;
    public bool isClaimed;
}
```

```c
public class TriggerData
{
    public string id;
    public string tag;
    public bool isAutoClaim;
    public string description;
    public TriggerCondition[] conditions;
    public TriggerBonus[] bonuses;
}
```

```c
public class TriggerCondition
{
    public string conditionType;
    public string key;
    public string operatorType;
    public string[] value;
}
```

```c
public class TriggerBonus
{
    public string type;
    public string id;
}
```

```c
public class TriggerActive
{
    public string triggerId;
    public bool claimed;
}
```

## GP_Variables

[Game variables documentation](https://docs.gamepush.com/docs/game-variables/)

### Methods

| Method name                    | Method parameters                                                                                                                                      | Return value |
| ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------ |
| `Fetch`                        | `Action<List<VariablesData>> onFetchSuccess = null, Action onFetchError = null`                                                                        | void         |
| `Has`                          | `string key`                                                                                                                                           | `bool`       |
| `GetInt`                       | `string key`                                                                                                                                           | `int`        |
| `GetFloat`                     | `string key`                                                                                                                                           | `float`      |
| `GetString`                    | `string key`                                                                                                                                           | `string`     |
| `GetBool`                      | `string key`                                                                                                                                           | `bool`       |
| `GetImage`                     | `string key`                                                                                                                                           | `string`     |
| `GetFile`                      | `string key`                                                                                                                                           | `string`     |
| `IsPlatformVariablesAvailable` | void                                                                                                                                                   | `bool`       |
| `FetchPlatformVariables`       | `Dictionary<string, string> optionsDict, Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null` | void         |
| `FetchPlatformVariables`       | `Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null`                                         | void         |

### Actions

| Action name              | Return value                 |
| ------------------------ | ---------------------------- |
| `OnFetchSuccess`         | `List<VariablesData>`        |
| `OnFetchError`           | `TriggerData`                |
| `OnPlatformFetchSuccess` | `Dictionary<string, string>` |
| `OnPlatformFetchError`   | `string`                     |

### Data structures

```c
public class PlatformFetchVariables
{
    public string clientParams;
}
```

```c
public class VariablesData
{
    public string key;
    public string type;
    public string value;
}
```

## GP_Uniques

[Uniques variables documentation](https://docs.gamepush.com/docs/uniques/)

### Methods

| Method name | Method parameters                                                                                                 | Return value    |
| ----------- | ----------------------------------------------------------------------------------------------------------------- | --------------- |
| `Register`  | `string tag, string value, Action onUniqueValueRegister = null, Action<string> onUniqueValueRegisterError = null` | void            |
| `Get`       | `string tag`                                                                                                      | `string`        |
| `List`      | void                                                                                                              | `UniquesData[]` |
| `Check`     | `string tag, string value, Action onUniqueValueCheck = null, Action<string> onUniqueValueCheckError = null`       | void            |
| `Delete`    | `string tag, Action onUniqueValueCheck = null, Action<string> onUniqueValueCheckError = null`                     | void            |
| `GetString` | `string key`                                                                                                      | `string`        |
| `GetBool`   | `string key`                                                                                                      | `bool`          |
| `GetImage`  | `string key`                                                                                                      | `string`        |
| `GetFile`   | `string key`                                                                                                      | `string`        |

### Actions

| Action name                  | Return value |
| ---------------------------- | ------------ |
| `OnUniqueValueRegister`      | void         |
| `OnUniqueValueRegisterError` | `string`     |
| `OnUniqueValueCheck`         | void         |
| `OnUniqueValueCheckError`    | `string`     |
| `OnUniqueValueDelete`        | void         |
| `OnUniqueValueDeleteError`   | `string`     |

### Data structures

```c
public class UniquesData
{
    public string tag;
    public string value;
}
```

## GP_Storage

[Storage documentation](https://docs.gamepush.com/docs/storage/)

### Methods

| Method name  | Method parameters                                                  | Return value |
| ------------ | ------------------------------------------------------------------ | ------------ |
| `SetStorage` | `SaveStorageType storage`                                          | void         |
| `Get`        | `string key, Action<object> onGetValue`                            | void         |
| `Set`        | `string key, object value, Action<StorageField> onSetValue = null` | void         |
| `GetGlobal`  | `string key, Action<object> onGetValue`                            | void         |
| `SetGlobal`  | `string key, object value, Action<StorageField> onSetValue = null` | void         |

### Actions

| Action name        | Return value   |
| ------------------ | -------------- |
| `OnGetValue`       | `StorageField` |
| `OnSetValue`       | `StorageField` |
| `OnGetGlobalValue` | `StorageField` |
| `OnSetGlobalValue` | `StorageField` |

### Data structures

```c
public class StorageField
{
    public string key;
    public string value;
}
```
