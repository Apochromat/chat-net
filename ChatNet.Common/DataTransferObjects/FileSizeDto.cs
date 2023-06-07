namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// DTO for file size
/// </summary>
public class FileSizeDto {
    /// <summary>
    /// File size in bytes
    /// </summary>
    private readonly int _sizeInBytes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sizeInBytes"></param>
    public FileSizeDto(int sizeInBytes) {
        _sizeInBytes = sizeInBytes;
    }

    /// <summary>
    /// Size in kilobytes
    /// </summary>
    public decimal SizeInKb => Math.Round((decimal)_sizeInBytes / 1024, 2);
    
    /// <summary>
    /// Size in megabytes
    /// </summary>
    public decimal SizeInMb => Math.Round((decimal)_sizeInBytes / 1048576, 2);
    
    /// <summary>
    /// Size in gigabytes
    /// </summary>
    public decimal SizeInGb => Math.Round((decimal)_sizeInBytes / 1073741824, 2);
}