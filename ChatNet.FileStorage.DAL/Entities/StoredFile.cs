using ChatNet.Common.Enumerations;

namespace ChatNet.FileStorage.DAL.Entities; 

/// <summary>
/// Entity that represents a file in the database.
/// </summary>
public class StoredFile {
    /// <summary>
    /// File identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the file
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Content type of the file in MIME format
    /// </summary>
    public required string ContentType { get; set; }
    
    /// <summary>
    /// File type
    /// </summary>
    public required FileType Type { get; set; }

    /// <summary>
    /// Is the file public
    /// </summary>
    public required bool IsPublic { get; set; } = false;
    
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the file was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the file was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;
    
    /// <summary>
    /// File content
    /// </summary>
    public required byte[] Content { get; set; }
}