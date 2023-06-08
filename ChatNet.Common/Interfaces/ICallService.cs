using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Service for call management
/// </summary>
public interface ICallService {
    /// <summary>
    /// Get current calls
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<CallDto>> GetCurrentCalls(Guid userId);
    
    /// <summary>
    /// Call somebody
    /// </summary>
    /// <param name="callerId"></param>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    Task<Guid> CallSomebody(Guid callerId, Guid receiverId);
    
    /// <summary>
    /// Accept call
    /// </summary>
    /// <param name="callId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AcceptCall(Guid callId, Guid userId);
    
    /// <summary>
    /// Reject call
    /// </summary>
    /// <param name="callId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RejectCall(Guid callId, Guid userId);
    
    /// <summary>
    /// Hang up call
    /// </summary>
    /// <param name="callId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task HangUpCall(Guid callId, Guid userId);
    
    /// <summary>
    /// Cancel call
    /// </summary>
    /// <param name="callId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CancelCall(Guid callId, Guid userId);
}