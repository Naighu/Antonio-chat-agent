namespace Antonio.Core.Exceptions;

public class LlmException(string message, Exception? inner = null) : Exception(message, inner);

public sealed class LlmAuthException(string message) : LlmException(message);

public sealed class LlmRateLimitedException(string message) : LlmException(message);

public sealed class LlmUnavailableException(string message, Exception? inner = null) : LlmException(message, inner);