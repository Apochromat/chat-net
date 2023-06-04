using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Token request DTO
/// </summary>
public class TokenRequestDto {
    /// <summary>
    /// Expired access token
    /// </summary>
    [Required]
    [DisplayName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    [DisplayName("refresh_token")]
    public required string RefreshToken { get; set; } 
}