using ChatNet.Auth.DAL;
using ChatNet.Auth.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Auth.BLL.Services; 

/// <inheritdoc cref="IAccountService"/>
public class AccountService: IAccountService {
    
    private readonly UserManager<User> _userManager;
    private readonly AuthDbContext _authDb;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="authDb"></param>
    public AccountService(UserManager<User> userManager, AuthDbContext authDb) {
        _userManager = userManager;
        _authDb = authDb;
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
            PhotoId = user.PhotoId
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
        user.PhotoId = accountProfileEditDto.PhotoId;

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
            Id = user.Id,
            PhotoId = user.PhotoId,
            FullName = user.FullName
        };
        return profile;    
    }
    
    /// <summary>
    /// Get users
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="searchString"></param>
    /// <returns></returns>
    public async Task<List<ProfileShortDto>> GetUsersByList(List<Guid> userIds, string? searchString) {
        var users = await _userManager.Users
            .Where(u =>
                (searchString == null || u.FullName.Contains(searchString))
                && userIds.Contains(u.Id))
            .Select(u=> new ProfileShortDto {
                Id = u.Id,
                PhotoId = u.PhotoId,
                FullName = u.FullName
            })
            .OrderBy(u=>u.FullName)
            .ToListAsync();
       
        return users;    
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="searchString"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async Task<Pagination<ProfileShortDto>> SearchUsersAsync(Guid userId, string? searchString, int page , int pageSize) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }
        var users = await _authDb.Users
            .Where(u => u.Id != userId
                && (searchString == null 
                || u.FullName.Contains(searchString)))
            .OrderBy(u=>u.FullName)
            .Select(u => new ProfileShortDto {
                Id = u.Id,
                PhotoId = u.PhotoId,
                FullName = u.FullName
            })
            .ToListAsync();
        var pagesAmount = (int)Math.Ceiling((double)users.Count / pageSize);
            
        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<ProfileShortDto>(users
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList(), 
            page, pageSize, pagesAmount);
    }
}