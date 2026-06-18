using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Common.Exceptions;

namespace ProductManagement.Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblem(context, validationException),
            NotFoundException => CreateProblem(context, StatusCodes.Status404NotFound, "Resource not found", exception.Message),
            ConflictException => CreateProblem(context, StatusCodes.Status409Conflict, "Business conflict", exception.Message),
            _ => CreateProblem(context, StatusCodes.Status500InternalServerError, "Unexpected error", "An unexpected error occurred.")
        };

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static ProblemDetails CreateProblem(HttpContext context, int statusCode, string title, string detail)
    {
        return new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private static ValidationProblemDetails CreateValidationProblem(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Type = "https://httpstatuses.com/400",
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };
    }
}
