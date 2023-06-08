using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Data transfer object for call
/// </summary>
public class CallDto {
    /// <summary>
    /// Call identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id of the user who is calling
    /// </summary>
    public Guid CallerId { get; set; }
    
    /// <summary>
    /// Id of the user who is being called
    /// </summary>
    public Guid ReceiverId { get; set; }
    
    /// <summary>
    /// Indicates if the user is initiator of the call
    /// </summary>
    public bool IsInitiator { get; set; }
    
    /// <summary>
    /// State of the call
    /// </summary>
    public CallState State { get; set; }
    
    /// <summary>
    /// Time when the call was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}