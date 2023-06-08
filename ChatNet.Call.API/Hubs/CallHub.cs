using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatNet.Call.API.Hubs; 

/// <summary>
/// Call hub
/// </summary>
[Authorize(AuthenticationSchemes = "Bearer")]
public class CallHub : Hub {
    private readonly ICallService _callService;
    private readonly ILogger<CallHub> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="callService"></param>
    public CallHub(ILogger<CallHub> logger, ICallService callService) {
        _logger = logger;
        _callService = callService;
    }

    /// <summary>
    /// 
    /// </summary>
    public override async Task OnConnectedAsync() {
        _logger.LogInformation("Client connected");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override Task OnDisconnectedAsync(Exception? exception) {
        _logger.LogInformation("Client disconnected");
        return base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public async Task SendMessage(object stream) {
        await this.Clients.Others.SendAsync("message", stream);
    }
    
    /// <summary>
    /// Join to call
    /// </summary>
    /// <param name="callId"></param>
    public async Task Join(string callId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, callId);
        await Clients.Caller.SendAsync("joined", callId);
        await Clients.Group(callId).SendAsync("ready");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiverId"></param>
    public async Task CallSomebody(string receiverId) {
        var callId = await _callService.CallSomebody(new Guid(Context.UserIdentifier), new Guid(receiverId));
        await Groups.AddToGroupAsync(Context.ConnectionId, callId.ToString());
        await Clients.User(Context.UserIdentifier).SendAsync("created", callId);
    }
}