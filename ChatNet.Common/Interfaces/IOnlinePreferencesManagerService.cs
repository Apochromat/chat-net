using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces; 

/// <summary>
/// Online preferences manager
/// </summary>
public interface IOnlinePreferencesManagerService {
    /// <summary>
    /// Sets user's online preferences
    /// </summary>
    /// <param name="preferenceDto"></param>
    /// <returns></returns>
    Task SetPreferenceAsync(OnlinePreferenceDto preferenceDto);
    
    /// <summary>
    /// Gets user's online preferences
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<OnlinePreferenceDto?> GetPreferenceAsync(Guid userId);
}