namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for unauthorized HTTP status code
/// </summary>
[Serializable]
public class UnauthorizedException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public UnauthorizedException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public UnauthorizedException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public UnauthorizedException(string message, Exception inner)
        : base(message, inner) {
    }
}