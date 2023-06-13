using ChatNet.Backend.DAL;
using ChatNet.Backend.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class FriendService:IFriendService {
    private readonly BackendDbContext _dbContext;
    private readonly INotificationQueueService _notificationQueueService;
    public FriendService(BackendDbContext dbContext, INotificationQueueService notificationQueueService) {
        _dbContext = dbContext;
        _notificationQueueService = notificationQueueService;
    }

    public async Task AcceptFriendshipRequest(Guid userId, Guid friendId) {
        var user = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var friend = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null) 
            throw new NotFoundException("User with this id does not found");
        var friendship = await _dbContext.FriendShipRequests
            .Include(r => r.RequestFrom)
            .Include(r => r.RequestTo)
            .FirstOrDefaultAsync(u =>
                u.RequestFrom == friend && u.RequestTo == user);
        if (friendship == null) 
            throw new NotFoundException("Friendship request not found");
        if (friendship.RequestTo != user)
            throw new MethodNotAllowedException("You can not accept this request");
        user.Friends.Add(friendship.RequestFrom);
        friend.Friends.Add(friendship.RequestTo);
        _dbContext.Remove(friendship);
        await _dbContext.SaveChangesAsync();
        await _notificationQueueService.SendOnlinePreferenceAsync(new OnlinePreferenceFriendsDto {
            UserId = user.Id,
            Friends = user.Friends.Select(u => u.Id).ToList()
        });
        await _notificationQueueService.SendOnlinePreferenceAsync(new OnlinePreferenceFriendsDto {
            UserId = friend.Id,
            Friends = friend.Friends.Select(u => u.Id).ToList()
        });
        await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
            Type = NotificationMessageType.FriendshipAccepting,
            Title = "Your friendship request accepted",
            Text = "Congratulations!",
            ReceiverId = friendship.RequestFrom.Id,
            SenderId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task RejectFriendshipRequest(Guid userId, Guid friendId) {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var friend = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null) 
            throw new NotFoundException("User with this id does not found");
        var friendship = await _dbContext.FriendShipRequests
            .Include(r=>r.RequestFrom)
            .Include(r=>r.RequestTo)
            .FirstOrDefaultAsync(u =>
                u.RequestFrom == user && u.RequestTo == friend
                || u.RequestFrom == friend && u.RequestTo == user);
        if (friendship == null) 
            throw new NotFoundException("You don't have this friendship request");
        if (friendship.RequestTo != user && friendship.RequestFrom != user)
            throw new MethodNotAllowedException("You can not reject this request");
        _dbContext.Remove(friendship);
        await _dbContext.SaveChangesAsync();
        if (friendship.RequestFrom != user) 
            await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
            Type = NotificationMessageType.FriendshipRejecting,
            Title = "Your friendship rejected",
            Text = "You rejected",
            ReceiverId = friendship.RequestFrom.Id,
            SenderId = user.Id,
            CreatedAt = DateTime.UtcNow 
            });
    }

    public async Task<Pagination<Guid>> GetUserFriendShipRequests(Guid userId, bool myRequests,
    int page , int pageSize) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var requests = await _dbContext.FriendShipRequests
            .Where(r => myRequests && r.RequestFrom == user
                        || (!myRequests && r.RequestTo == user))
            .OrderBy(u=>u.RequestFrom)
            .Select(r=> myRequests? r.RequestTo.Id : r.RequestFrom.Id)
            .ToListAsync();
        var pagesAmount = (int)Math.Ceiling((double)requests.Count / pageSize);
            
        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<Guid>(
            requests
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList(),
            page, pageSize, pagesAmount);
    }

    public async Task SendFriendshipRequest(Guid userId, Guid friendId) {
        if (userId == friendId)
            throw new ConflictException("it's you");
        var user = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var friend = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null) throw new NotFoundException("Friend with this id does not found");
        if (user.Friends.Contains(friend))
            throw new ConflictException("He is already your friend");
        if (await _dbContext.FriendShipRequests.AnyAsync(
                f => f.RequestFrom == user
                     && f.RequestTo == friend
                     || f.RequestTo == user
                     && f.RequestFrom == friend))
            throw new ConflictException("This friendship request already exist");
        await _dbContext.FriendShipRequests.AddAsync(new FriendShipRequest {
            RequestFrom = user,
            RequestTo = friend
        });
        await _dbContext.SaveChangesAsync();
        await _notificationQueueService.SendNotificationAsync(new NotificationMessageDto {
            Type = NotificationMessageType.NewFriendshipRequest,
            Title = "You have new friendship request",
            Text = "Check your friendship requests",
            ReceiverId = friend.Id,
            SenderId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<Pagination<Guid>> GetFriends(Guid userId, int page, int pageSize) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }
        var user = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        
        var pagesAmount = (int)Math.Ceiling((double)user.Friends.Count / pageSize);
            
        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<Guid>(
            user.Friends
                .OrderBy(u=>u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => u.Id).ToList(),
            page, pageSize, pagesAmount);
    }

    public async Task DeleteFriend(Guid userId, Guid friendId) {
        var user = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var friend = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null) 
            throw new NotFoundException("Friend with this id does not found");
        if (!user.Friends.Contains(friend))
            throw new ConflictException("He is not your friend");
        user.Friends.Remove(friend);
        friend.Friends.Remove(user);
        await _dbContext.SaveChangesAsync();
        await _notificationQueueService.SendOnlinePreferenceAsync(new OnlinePreferenceFriendsDto {
            UserId = user.Id,
            Friends = user.Friends.Select(u => u.Id).ToList()
        });
        await _notificationQueueService.SendOnlinePreferenceAsync(new OnlinePreferenceFriendsDto {
            UserId = friend.Id,
            Friends = friend.Friends.Select(u => u.Id).ToList()
        });
    }

}