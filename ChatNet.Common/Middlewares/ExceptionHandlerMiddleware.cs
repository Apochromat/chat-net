using System.Diagnostics;
using System.Net;
using ChatNet.Common.ErrorModels;
using ChatNet.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChatNet.Common.Middlewares; 

/// <summary>
/// Exception handler middleware
/// </summary>
public class ExceptionHandlerMiddleware {
     private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Handle exception
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (NotFoundException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (ConflictException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (ForbiddenException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (BadRequestException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (MethodNotAllowedException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.MethodNotAllowed,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (UnauthorizedException ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = ex.Message,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }

        catch (Exception ex) {
            var errorDetails = new ErrorDetails {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Something went wrong",
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };
            _logger.LogError(ex, errorDetails.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}

/// <summary>
/// Exception handler middleware extension
/// </summary>
public static class ExceptionHandlerMiddlewareExtension {
    /// <summary>
    /// Extension method
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseErrorHandleMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}