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
    }
}
