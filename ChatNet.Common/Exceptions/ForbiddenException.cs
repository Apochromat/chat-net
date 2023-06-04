namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for forbidden status 
/// </summary>
[Serializable]
public class ForbiddenException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public ForbiddenException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ForbiddenException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ForbiddenException(string message, Exception inner)
        : base(message, inner) {
    }
}