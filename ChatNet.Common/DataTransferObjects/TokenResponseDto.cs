using System.ComponentModel;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Token response DTO
/// </summary>
public class TokenResponseDto {
    /// <summary>
    /// Access token
    /// </summary>
    [DisplayName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [DisplayName("refresh_token")]
    public required string RefreshToken { get; set; }
}