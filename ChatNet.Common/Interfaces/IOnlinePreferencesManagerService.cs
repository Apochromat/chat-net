using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Online preferences manager
/// </summary>
public interface IOnlinePreferencesManagerService {
    /// <summary>
    /// Sets user's online preferences (only friends list)
    /// </summary>
    /// <param name="preferenceFriendsDto"></param>
    /// <returns></returns>
    Task SetPreferenceFriendsAsync(OnlinePreferenceFriendsDto preferenceFriendsDto);

    /// <summary>
    /// Sets user's online preferences (only type)
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="preferenceTypeDto"></param>
    /// <returns></returns>
    Task SetPreferenceTypeAsync(Guid userId, OnlinePreferenceTypeDto preferenceTypeDto);
    
    /// <summary>
    /// Gets user's online preferences
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<OnlinePreferenceFullDto?> GetPreferenceAsync(Guid userId);
}