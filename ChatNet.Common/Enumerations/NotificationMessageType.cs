namespace ChatNet.Common.Enumerations; 

/// <summary>
/// Notification message types.
/// </summary>
public enum NotificationMessageType {
    /// <summary>
    /// Sent when a new message was sent.
    /// </summary>
    NewMessage,
    /// <summary>
    /// Sent when a new reaction was added to a message.
    /// </summary>
    NewReaction,
    /// <summary>
    /// Sent when a user becomes online.
    /// </summary>
    UserOnline,
    /// <summary>
    /// Sent when a user becomes offline.
    /// </summary>
    UserOffline,
    /// <summary>
    /// Sent when a new friendship request was sent.
    /// </summary>
    NewFriendshipRequest,
    /// <summary>
    /// Sent when a friendship request was accepted.
    /// </summary>
    FriendshipAccepting,
    /// <summary>
    /// Sent when a friendship request was rejected.
    /// </summary>
    FriendshipRejecting,
    /// <summary>
    /// Sent when a new chat was created.
    /// </summary>
    ChatUpdate,
    /// <summary>
    /// Sent when somebody calls the user.
    /// </summary>
    NewAudioCall,
    /// <summary>
    /// Sent when somebody calls the user with video.
    /// </summary>
    NewVideoCall,
    /// <summary>
    /// Sent when a call was rejected.
    /// </summary>
    CallRejecting
}