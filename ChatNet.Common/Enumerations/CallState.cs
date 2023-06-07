namespace ChatNet.Common.Enumerations; 

/// <summary>
/// Enumerates the possible states of a call.
/// </summary>
public enum CallState {
    /// <summary>
    /// Set when the call is created.
    /// </summary>
    Created,
    /// <summary>
    /// Set when the call is rejected by receiver.
    /// </summary>
    Rejected,
    /// <summary>
    /// Set when the call is cancelled by the caller.
    /// </summary>
    Cancelled,
    /// <summary>
    /// Set when the call is accepted by the receiver.
    /// </summary>
    Established,
    /// <summary>
    /// Set when the call is ended by the caller or receiver.
    /// </summary>
    Ended
}