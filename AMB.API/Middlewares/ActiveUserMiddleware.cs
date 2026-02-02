using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMB.API.Middlewares
{
    public class ActiveUserMiddleware
    {
        private readonly RequestDelegate _next;

        public ActiveUserMiddleware(RequestDelegate next) 
        { 
            _next = next; 
        }

        public async Task InvokeAsync(HttpContext context, AMBContext dbContext)
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var auth0UserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(auth0UserId))
            {
                var employee = await dbContext.Employees
                    .AsNoTracking()
                    .Select(e => new { e.UserId, e.Status })
                    .FirstOrDefaultAsync(e => e.UserId == auth0UserId);

                if (employee == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Employee not found");
                    return;
                }

                if (employee.Status == (int)EntityStatus.Inactive)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Your account has been deactivated.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
