using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
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

    /// <inheritdoc cref="IOnlinePreferencesManagerService.SetPreferenceFriendsAsync"/>
    public async Task SetPreferenceFriendsAsync(OnlinePreferenceFriendsDto preferenceFriendsDto) {
        var userPreference = await _dbContext.OnlinePreferences.Where(
                o => o.UserId == preferenceFriendsDto.UserId)
            .FirstOrDefaultAsync();
        if (userPreference == null) {
            var preference = new OnlinePreference() {
                Id = Guid.NewGuid(),
                UserId = preferenceFriendsDto.UserId,
                Type = OnlinePreferenceType.Nobody,
                Friends = preferenceFriendsDto.Friends
            };

            await _dbContext.OnlinePreferences.AddAsync(preference);
        }
        else {
            userPreference.Friends = preferenceFriendsDto.Friends;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="IOnlinePreferencesManagerService.SetPreferenceTypeAsync"/>
    public async Task SetPreferenceTypeAsync(Guid userId, OnlinePreferenceTypeDto preferenceTypeDto) {
        var userPreference = await _dbContext.OnlinePreferences.Where(
                o => o.UserId == userId)
            .FirstOrDefaultAsync();
        if (userPreference == null) {
            var preference = new OnlinePreference() {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = OnlinePreferenceType.Nobody,
                Friends = new List<Guid>()
            };

            await _dbContext.OnlinePreferences.AddAsync(preference);
        }
        else {
            userPreference.Type = preferenceTypeDto.Type;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc cref="IOnlinePreferencesManagerService.GetPreferenceAsync"/>
    public async Task<OnlinePreferenceFullDto?> GetPreferenceAsync(Guid userId) {
        var userPreference = await _dbContext.OnlinePreferences.Where(
                o => o.UserId == userId)
            .FirstOrDefaultAsync();

        if (userPreference == null) {
            return null;
        }

        return new OnlinePreferenceFullDto() {
            UserId = userPreference.UserId,
            Type = userPreference.Type,
            Friends = userPreference.Friends
        };
    }
}