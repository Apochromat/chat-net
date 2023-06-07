using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatNet.Auth.DAL;
using ChatNet.Auth.DAL.Entities;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ChatNet.Auth.BLL.Services; 

/// <summary>
/// Service for authentication and authorization
/// </summary>
public class AuthService : IAuthService {
    private readonly ILogger<AuthService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly AuthDbContext _authDbContext;
    private readonly IConfiguration _configuration;
    private readonly IMessageSenderService _messageSenderService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="authDbContext"></param>
    /// <param name="configuration"></param>
    /// <param name="messageSenderService"></param>

    public AuthService(ILogger<AuthService> logger, UserManager<User> userManager, SignInManager<User> signInManager,
        AuthDbContext authDbContext,IConfiguration configuration, IMessageSenderService messageSenderService) {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _authDbContext = authDbContext;
        _configuration = configuration;
        _messageSenderService = messageSenderService;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="accountRegisterDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> RegisterAsync(AccountRegisterDto accountRegisterDto, HttpContext httpContext) {
        if (accountRegisterDto.Email == null) {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Email is empty");
        }

        if (accountRegisterDto.Password == null) {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Password is empty");
        }

        if (await _userManager.FindByEmailAsync(accountRegisterDto.Email) != null) {
            throw new ConflictException("User with this email already exists");
        }

        var user = new User {
            Email = accountRegisterDto.Email,
            UserName = accountRegisterDto.Email,
            FullName = accountRegisterDto.FullName,
            BirthDate = accountRegisterDto.BirthDate,
            PhoneNumber = accountRegisterDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, accountRegisterDto.Password);

        if (result.Succeeded) {
            _logger.LogInformation("Successful register");
            await _messageSenderService.SendNotification(new NotificationMessageDto {
                Title = "request on create user in backend",
                NewUserId = user.Id
            });

            return await LoginAsync(new AccountLoginDto()
                { Email = accountRegisterDto.Email, Password = accountRegisterDto.Password }, httpContext);
        }

        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        throw new BadRequestException(errors);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="accountLoginDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> LoginAsync(AccountLoginDto accountLoginDto, HttpContext httpContext) {
        var identity = await GetIdentity(accountLoginDto.Email.ToLower(), accountLoginDto.Password);
        if (identity == null) {
            throw new BadRequestException("Incorrect username or password");
        }

        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(x => x.Email == accountLoginDto.Email);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        
        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress || x.UserAgent == userAgent);

        if (device == null) {
            device = new Device() {
                User = user,
                RefreshToken = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
                UserAgent = userAgent,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };
            await _authDbContext.Devices.AddAsync(device);
        }

        device.LastActivity = DateTime.UtcNow;
        device.ExpirationDate = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt")
            .GetValue<int>("RefreshTokenLifetimeInMonths"));
        
        await _authDbContext.SaveChangesAsync();

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: _configuration.GetSection("Jwt")["Audience"],
            notBefore: DateTime.UtcNow,
            claims: identity.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        _logger.LogInformation("Successful login");

        return new TokenResponseDto() {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            RefreshToken = device.RefreshToken
        };
    }

    /// <summary>
    /// Logout user by deleting his current device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task LogoutAsync(Guid userId, HttpContext httpContext) {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(x => x.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var device = user.Devices.FirstOrDefault(x => x.UserAgent == httpContext.Request.Headers["User-Agent"]);

        if (device == null) {
            throw new MethodNotAllowedException("You can`t logout from this device");
        }

        _authDbContext.Devices.Remove(device);
        await _authDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="tokenRequestDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto, HttpContext httpContext) {
        tokenRequestDto.AccessToken = tokenRequestDto.AccessToken.Replace("Bearer ", "");
        var principal = GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
        if (principal.Identity == null) {
            throw new BadRequestException("Invalid jwt token");
        }

        var user = _userManager.Users.Include(x => x.Devices)
            .FirstOrDefault(x => x.Id.ToString() == principal.Identity.Name);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        if (await _userManager.IsLockedOutAsync(user)) {
            throw new UnauthorizedException("User is banned");
        }

        var device =
            user.Devices.FirstOrDefault(x => x.UserAgent == httpContext.Request.Headers["User-Agent"]);

        if (device == null) {
            throw new MethodNotAllowedException("You can't refresh token from another device. Re-login needed");
        }

        if (device.RefreshToken != tokenRequestDto.RefreshToken) {
            throw new BadRequestException("Refresh token is invalid");
        }

        if (device.ExpirationDate < DateTime.UtcNow) {
            throw new UnauthorizedException("Refresh token is expired. Re-login needed");
        }

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: null,
            notBefore: DateTime.UtcNow,
            claims: principal.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        device.LastActivity = DateTime.UtcNow;
        await _authDbContext.SaveChangesAsync();

        return new TokenResponseDto() {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            RefreshToken = device.RefreshToken
        };
    }

    /// <summary>
    /// Get user devices
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<DeviceDto>> GetDevicesAsync(Guid userId) {
        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        return Task.FromResult(user.Devices.Select(d => new DeviceDto {
            DeviceName = d.DeviceName,
            IpAddress = d.IpAddress,
            UserAgent = d.UserAgent,
            LastActivity = d.LastActivity,
            Id = d.Id,
        }).ToList());
    }

    /// <summary>
    /// Rename device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="deviceRenameDto"></param>
    /// <returns></returns>
    public async Task RenameDeviceAsync(Guid userId, Guid deviceId, DeviceRenameDto deviceRenameDto) {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var device = user.Devices.FirstOrDefault(d => d.Id == deviceId);
        if (device == null) {
            throw new NotFoundException("Device not found");
        }

        device.DeviceName = deviceRenameDto.DeviceName;
        await _authDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Delete device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    public async Task DeleteDeviceAsync(Guid userId, Guid deviceId) {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var device = _authDbContext.Devices.FirstOrDefault(d => d.User == user);
        if (device == null) {
            throw new NotFoundException("Device not found");
        }

        _authDbContext.Devices.Remove(device);
        await _authDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="changePasswordDto"></param>
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto) {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded) {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string jwtToken) {
        var key = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty));

        var validationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _configuration.GetSection("Jwt")["Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration.GetSection("Jwt")["Audience"],
            ValidateLifetime = false
        };

        ClaimsPrincipal principal;
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
        }
        catch (ArgumentException ex) {
            throw new BadRequestException("Invalid jwt token", ex);
        }

        return principal;
    }

    private async Task<ClaimsIdentity?> GetIdentity(string email, string password) {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return null;

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Id.ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user)) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims, "Token", ClaimTypes.Name, ClaimTypes.Role);
    }
}