using System;

namespace FoilEngine
{
    /// <summary>Base exception for all Foil Engine SDK errors.</summary>
    public class FoilEngineException : Exception
    {
        public int StatusCode { get; }

        public FoilEngineException(string message, int statusCode = 0)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>Raised when the API key is missing or invalid (401).</summary>
    public class AuthenticationException : FoilEngineException
    {
        public AuthenticationException(string message = "Invalid or missing API key")
            : base(message, 401) { }
    }

    /// <summary>Raised when access is denied (403).</summary>
    public class ForbiddenException : FoilEngineException
    {
        public ForbiddenException(string message = "Access denied")
            : base(message, 403) { }
    }

    /// <summary>Raised when a resource is not found (404).</summary>
    public class NotFoundException : FoilEngineException
    {
        public NotFoundException(string message = "Resource not found")
            : base(message, 404) { }
    }

    /// <summary>Raised for invalid requests (400).</summary>
    public class BadRequestException : FoilEngineException
    {
        public BadRequestException(string message = "Bad request")
            : base(message, 400) { }
    }

    /// <summary>Raised when rate limited (429).</summary>
    public class RateLimitException : FoilEngineException
    {
        public float? RetryAfter { get; }

        public RateLimitException(string message = "Rate limit exceeded", float? retryAfter = null)
            : base(message, 429)
        {
            RetryAfter = retryAfter;
        }
    }

    /// <summary>Raised for server-side errors (5xx).</summary>
    public class ServerException : FoilEngineException
    {
        public ServerException(string message = "Internal server error")
            : base(message, 500) { }
    }
}
