using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.CustomMiddlewares
{
    public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _default = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context,
            AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden)
            {
                var problem = new ProblemDetails
                {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this API resource."
                };

                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }

            await _default.HandleAsync(next, context, policy, authorizeResult);
        }
    }

}
