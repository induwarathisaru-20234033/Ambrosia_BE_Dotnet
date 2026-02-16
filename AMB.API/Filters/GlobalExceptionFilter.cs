using AMB.Application.Dtos;
using AMB.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;

namespace AMB.API.Filters
{
    public sealed class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var (statusCode, message, errors) = MapException(context.Exception);

            var response = new BaseResponseDto<object>(message, errors);

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }

        private static (int statusCode, string message, List<string> errors) MapException(Exception exception)
        {
            switch (exception)
            {
                case ValidationException validationException:
                    return (StatusCodes.Status400BadRequest,
                        "Validation failed.",
                        validationException.Errors.Select(error => error.ErrorMessage).ToList());

                case Auth0Exception auth0Exception:
                    var auth0StatusCode = auth0Exception.StatusCode ?? StatusCodes.Status400BadRequest;
                    var auth0Errors = new List<string>();
                    
                    if (!string.IsNullOrWhiteSpace(auth0Exception.Auth0Error))
                    {
                        auth0Errors.Add(auth0Exception.Auth0Error);
                    }

                    if (auth0Errors.Count == 0)
                    {
                        auth0Errors.Add(auth0Exception.Message);
                    }

                    return (auth0StatusCode, auth0Exception.Message, auth0Errors);

                case UnauthorizedAccessException unauthorizedAccessException:
                    return (StatusCodes.Status401Unauthorized,
                        unauthorizedAccessException.Message,
                        new List<string> { unauthorizedAccessException.Message });

                case KeyNotFoundException keyNotFoundException:
                    return (StatusCodes.Status404NotFound,
                        keyNotFoundException.Message,
                        new List<string> { keyNotFoundException.Message });

                case ArgumentException argumentException:
                    return (StatusCodes.Status400BadRequest,
                        argumentException.Message,
                        new List<string> { argumentException.Message });

                case BaseException baseException:
                    return (StatusCodes.Status400BadRequest,
                        baseException.Message,
                        string.IsNullOrWhiteSpace(baseException.ErrorDescription)
                            ? new List<string> { baseException.Message }
                            : new List<string> { baseException.Message, baseException.ErrorDescription });

                case DbUpdateConcurrencyException concurrencyException:
                    return (StatusCodes.Status409Conflict,
                        "A concurrency conflict occurred.",
                        new List<string> { concurrencyException.Message });

                case DbUpdateException dbUpdateException:
                    var uniqueViolation = TryMapUniqueConstraint(dbUpdateException);
                    if (uniqueViolation != null)
                    {
                        return uniqueViolation.Value;
                    }

                    return (StatusCodes.Status500InternalServerError,
                        "A database update error occurred.",
                        new List<string> { dbUpdateException.Message });

                case SqlException sqlException:
                    return (StatusCodes.Status500InternalServerError,
                        "A SQL database error occurred.",
                        new List<string> { sqlException.Message });

                case DbException dbException:
                    return (StatusCodes.Status500InternalServerError,
                        "A database error occurred.",
                        new List<string> { dbException.Message });

                default:
                    return (StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred.",
                        new List<string> { exception.Message });
            }
        }

        private static (int statusCode, string message, List<string> errors)? TryMapUniqueConstraint(DbUpdateException exception)
        {
            if (exception.InnerException is not SqlException sqlException)
            {
                return null;
            }

            if (sqlException.Number is not (2601 or 2627))
            {
                return null;
            }

            var errorText = sqlException.Message ?? string.Empty;
            var errors = new List<string>();

            if (errorText.Contains("UX_Employees_EmployeeId_Active", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Employee ID is already in use by an active employee.");
            }

            if (errorText.Contains("UX_Employees_MobileNumber_Active", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Mobile number is already in use by an active employee.");
            }

            if (errorText.Contains("UX_Employees_Username_Active", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Username is already in use by an active employee.");
            }

            if (errorText.Contains("UX_Employees_UserId_Active", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Auth user ID is already linked to an active employee.");
            }

            if (errors.Count == 0)
            {
                return null;
            }

            return (StatusCodes.Status409Conflict, "Duplicate employee information.", errors);
        }
    }
}
