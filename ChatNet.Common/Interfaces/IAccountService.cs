using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces;

/// <summary>
/// Account service
/// </summary>
public interface IAccountService{
    /// <summary>
    /// Get user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ProfileFullDto> GetProfileAsync(Guid userId);
    
    /// <summary>
    /// Edit user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountProfileEditDto"></param>
    /// <returns></returns>
    Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto);
    
    /// <summary>
    /// Get short user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ProfileShortDto> GetShortProfileAsync(Guid userId);

    /// <summary>
    /// Search users
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="searchString"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<Pagination<ProfileShortDto>> SearchUsersAsync(Guid userId, string? searchString, int page , int pageSize);
}