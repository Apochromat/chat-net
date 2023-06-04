namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for bad request HTTP status code
/// </summary>
[Serializable]
public class BadRequestException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public BadRequestException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public BadRequestException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public BadRequestException(string message, Exception inner)
        : base(message, inner) {
    }
}