﻿using ChatNet.Call.DAL;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Call.BLL.Services; 

/// <inheritdoc cref="ICallService"/>
public class CallService : ICallService {
    private readonly CallDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public CallService(CallDbContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc cref="ICallService.GetCurrentCalls"/>
    public async Task<List<CallDto>> GetCurrentCalls(Guid userId) {
        var calls = await _dbContext.Calls
            .Where(c => 
                c.CallerId == userId || c.ReceiverId == userId
                && (c.State == CallState.Created || c.State == CallState.Accepted || c.State == CallState.Established))
            .Select(c => new CallDto {
                Id = c.Id,
                CallerId = c.CallerId,
                ReceiverId = c.ReceiverId,
                State = c.State,
                CreatedAt = c.CreatedAt,
                IsInitiator = c.CallerId == userId
            })
            .ToListAsync();
        
        return calls;
    }

    /// <inheritdoc cref="ICallService.GetCall"/>
    public async Task<CallDto> GetCall(Guid userId, Guid callId) {
        var call = await _dbContext.Calls
            .Where(c => 
                c.CallerId == userId || c.ReceiverId == userId
                && c.Id == callId)
            .Select(c => new CallDto {
                Id = c.Id,
                CallerId = c.CallerId,
                ReceiverId = c.ReceiverId,
                State = c.State,
                CreatedAt = c.CreatedAt,
                IsInitiator = c.CallerId == userId
            })
            .FirstOrDefaultAsync();

        if (call == null) {
            throw new NotFoundException("Call not found");
        }

        return call;
    }

    /// <inheritdoc cref="ICallService.CallSomebody"/>
    public async Task<Guid> CallSomebody(Guid callerId, Guid receiverId) {
        var existingCall = await _dbContext.Calls
            .FirstOrDefaultAsync(c => (c.CallerId == callerId || c.ReceiverId == callerId)
                                      && (c.ReceiverId == receiverId || c.CallerId == receiverId)
                                      && (c.State == CallState.Created || c.State == CallState.Established));
        if (existingCall != null) {
            throw new ConflictException($"There is already a call between {callerId} and {receiverId}. CallId: {existingCall.Id}");
        }
        
        var call = new DAL.Entities.Call {
            Id = Guid.NewGuid(),
            CallerId = callerId,
            ReceiverId = receiverId,
            State = CallState.Created,
            CreatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Calls.AddAsync(call);
        await _dbContext.SaveChangesAsync();
        
        return call.Id;
    }

    /// <inheritdoc cref="ICallService.ConnectToCall"/>
    public async Task ConnectToCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId 
            && (c.ReceiverId == userId || c.CallerId == userId)
            && c.State == CallState.Accepted);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not part of the call");
        }

        if (call.ReceiverId == userId) {
            call.IsReceiverConnected = true;
        }
        else {
            call.IsCallerConnected = true;
        }
        
        if (call is { IsReceiverConnected: true, IsCallerConnected: true }) {
            call.State = CallState.Established;
            await _dbContext.SaveChangesAsync();
        }
        
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="ICallService.DisconnectFromCall"/>
    public async Task DisconnectFromCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId 
            && (c.ReceiverId == userId || c.CallerId == userId));
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not part of the call");
        }
        
        call.IsReceiverConnected = false;
        call.IsCallerConnected = false;

        call.EndedAt = DateTime.UtcNow;
        call.State = CallState.Ended;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="ICallService.IsReadyToStart"/>
    public async Task<bool> IsReadyToStart(Guid callId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not part of the call");
        }

        return call.State == CallState.Established;
    }

    /// <inheritdoc cref="ICallService.AcceptCall"/>
    public async Task AcceptCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId 
            && c.ReceiverId == userId
            && c.State == CallState.Created);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not receiver of the call");
        }
        
        call.State = CallState.Accepted;
        call.StartedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="ICallService.RejectCall"/>
    public async Task RejectCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId 
            && c.ReceiverId == userId
            && c.State == CallState.Created);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not receiver of the call");
        }
        
        call.State = CallState.Rejected;
        call.EndedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="ICallService.HangUpCall"/>
    public async Task HangUpCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId 
            && (c.CallerId == userId || c.ReceiverId == userId)
            && c.State == CallState.Established);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not part of the call");
        }
        
        call.State = CallState.Ended;
        call.EndedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="ICallService.CancelCall"/>
    public async Task CancelCall(Guid callId, Guid userId) {
        var call = await _dbContext.Calls.FirstOrDefaultAsync(c => 
            c.Id == callId
            && c.CallerId == userId
            && c.State == CallState.Created);
        if (call == null) {
            throw new NotFoundException($"Call with id {callId} not found or you are not caller of the call");
        }
        
        call.State = CallState.Cancelled;
        call.EndedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}