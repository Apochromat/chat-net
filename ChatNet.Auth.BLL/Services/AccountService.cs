using ChatNet.Auth.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatNet.Auth.BLL.Services; 

/// <inheritdoc cref="IAccountService"/>
public class AccountService: IAccountService {
    
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    public AccountService(UserManager<User> userManager) {
        _userManager = userManager;
    }
    
    /// <inheritdoc cref="IAccountService.GetProfileAsync"/>
    public async Task<ProfileFullDto> GetProfileAsync(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileFullDto {
            Id = user.Id,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            FullName = user.FullName,
            BirthDate = user.BirthDate,
            JoinedAt = user.JoinedAt,
            //TODO 
            //PhotoId = 
            IsBanned = await _userManager.IsLockedOutAsync(user)
        };
        return profile;
    }

    /// <inheritdoc cref="IAccountService.EditProfileAsync"/>
    public async Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        user.FullName = accountProfileEditDto.FullName;
        user.BirthDate = accountProfileEditDto.BirthDate;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) {
            throw new InvalidOperationException("User update failed");
        } 
    }

    /// <inheritdoc cref="IAccountService.GetShortProfileAsync"/>
    public async Task<ProfileShortDto> GetShortProfileAsync(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileShortDto {
            PhotoId = Guid.NewGuid(),
            FullName = user.FullName
        };
        return profile;    
    }
}