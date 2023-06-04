namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for not found HTTP status code
/// </summary>
[Serializable]
public class NotFoundException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public NotFoundException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public NotFoundException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public NotFoundException(string message, Exception inner)
        : base(message, inner) {
    }
}