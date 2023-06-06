using ChatNet.Backend.DAL;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.BLL.Services; 

public class FriendService:IFriendService {
    private readonly BackendDbContext _dbContext;

    public FriendService(BackendDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task AddFriend(Guid userId, Guid friendId) {
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
        user.Friends.Add(friend);
        await _dbContext.SaveChangesAsync();
        //TODO : _notificationService.SendFriends(user.Friends.Select(u => u.Id).ToList());
    }

    public async Task<Pagination<Guid>> GetFriends(Guid userId, int page, int pageSize) {
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
            user.Friends.Select(u => u.Id).ToList(),
            page, pageSize, pagesAmount);
    }

    public async Task DeleteFriend(Guid userId, Guid friendId) {
        var user = await _dbContext.Users
            .Include(u=>u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id does not found");
        var friend = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null) 
            throw new NotFoundException("Friend with this id does not found");
        if (!user.Friends.Contains(friend))
            throw new ConflictException("He is not your friend");
        user.Friends.Remove(friend);
        await _dbContext.SaveChangesAsync();
    }
}