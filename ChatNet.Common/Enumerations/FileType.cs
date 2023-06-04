namespace ChatNet.Common.Enumerations; 

/// <summary>
/// Enumeration that represents the type of a file.
/// </summary>
public enum FileType {
    /// <summary>
    /// Unknown file type
    /// </summary>
    Unknown,
    /// <summary>
    /// Text file as .txt, .doc, .docx, .pdf, etc.
    /// </summary>
    Text,
    /// <summary>
    /// Image file as .jpg, .png, .gif, etc.
    /// </summary>
    Image,
    /// <summary>
    /// Audio file as .mp3, .wav, etc.
    /// </summary>
    Audio,
    /// <summary>
    /// Video file as .mp4, .avi, etc.
    /// </summary>
    Video,
    /// <summary>
    /// Application file as .exe, .msi, etc.
    /// </summary>
    Application,
    /// <summary>
    /// Archive file as .zip, .rar, etc.
    /// </summary>
    Archive
}