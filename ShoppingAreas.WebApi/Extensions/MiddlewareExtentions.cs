using Microsoft.AspNetCore.Builder;
using ShoppingAreas.Web.Middleware;

namespace ShoppingAreas.Web.Extensions
{
	public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandler>();
        }
    }
}
