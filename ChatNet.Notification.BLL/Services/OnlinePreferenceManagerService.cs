using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Interfaces;
using ChatNet.Notification.DAL;
using ChatNet.Notification.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Notification.BLL.Services; 

/// <inheritdoc cref="IOnlinePreferencesManagerService"/>
public class OnlinePreferenceManagerService : IOnlinePreferencesManagerService {
    private readonly NotificationDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public OnlinePreferenceManagerService(NotificationDbContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc cref="IOnlinePreferencesManagerService.SetPreferenceAsync"/>
    public async Task SetPreferenceAsync(OnlinePreferenceDto preferenceDto) {
        var userPreference = await _dbContext.OnlinePreferences.Where(
            o=> o.UserId == preferenceDto.UserId)
            .FirstOrDefaultAsync();
        if (userPreference == null) {
            var preference = new OnlinePreference() {
                Id = Guid.NewGuid(),
                UserId = preferenceDto.UserId,
                Type = preferenceDto.Type,
                Friends = preferenceDto.Friends
            };
            
            await _dbContext.OnlinePreferences.AddAsync(preference);
        }
        else {
            userPreference.Type = preferenceDto.Type;
            userPreference.Friends = preferenceDto.Friends;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="IOnlinePreferencesManagerService.GetPreferenceAsync"/>
    public async Task<OnlinePreferenceDto?> GetPreferenceAsync(Guid userId) {
        var userPreference = await _dbContext.OnlinePreferences.Where(
            o=> o.UserId == userId)
            .FirstOrDefaultAsync();
        
        if (userPreference == null) {
            return null;
        }

        return new OnlinePreferenceDto() {
            UserId = userPreference.UserId,
            Type = userPreference.Type,
            Friends = userPreference.Friends
        };
    }
}