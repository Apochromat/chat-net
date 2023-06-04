namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO for device
/// </summary>
public class DeviceDto {
    /// <summary>
    /// Device identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public required string UserAgent { get; set; }
    
    /// <summary>
    /// IP address
    /// </summary>
    public required string IpAddress { get; set; }

    /// <summary>
    /// Device name
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Last activity
    /// </summary>
    public DateTime LastActivity { get; set; }
}