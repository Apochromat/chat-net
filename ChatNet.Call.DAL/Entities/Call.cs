using ChatNet.Common.Enumerations;

namespace ChatNet.Call.DAL.Entities; 

/// <summary>
/// Call entity
/// </summary>
public class Call {
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
    /// State of the call
    /// </summary>
    public CallState State { get; set; }
    
    /// <summary>
    /// Time when the call was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Time when the call was accepted
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// Time when the call was ended/rejected/cancelled
    /// </summary>
    public DateTime? EndedAt { get; set; }
}