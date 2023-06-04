namespace ChatNet.Common.Exceptions;

/// <summary>
/// Exception for method not allowed status 
/// </summary>
[Serializable]
public class MethodNotAllowedException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public MethodNotAllowedException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public MethodNotAllowedException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public MethodNotAllowedException(string message, Exception inner)
        : base(message, inner) {
    }
}