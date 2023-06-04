namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for conflict HTTP status code
/// </summary>
[Serializable]
public class ConflictException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public ConflictException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ConflictException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ConflictException(string message, Exception inner)
        : base(message, inner) {
    }
}