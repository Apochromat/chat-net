using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects;

/// <summary>
/// DTO for file information
/// </summary>
public class FileInfoDto {
    /// <summary>
    /// File identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the file
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// File size
    /// </summary>
    public required FileSizeDto Size { get; set; }

    /// <summary>
    /// File type
    /// </summary>
    public required FileType Type { get; set; }

    /// <summary>
    /// Identifier of the user who uploaded the file
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// List of users who can view the file
    /// </summary>
    public List<Guid> Viewers { get; set; } = new();

    /// <summary>
    /// Date and time when the file was uploaded
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the file was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}