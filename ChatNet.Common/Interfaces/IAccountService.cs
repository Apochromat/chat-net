using ChatNet.Common.DataTransferObjects;

namespace ChatNet.Common.Interfaces;

public interface IAccountService{
    Task<ProfileFullDto> GetProfileAsync(Guid userId);
    Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto);
    Task<ProfileShortDto> GetShortProfileAsync(Guid userId);
}