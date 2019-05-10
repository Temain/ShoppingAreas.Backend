using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShoppingAreas.Web.Middleware
{
	public class CustomExceptionHandler
	{
		private	readonly RequestDelegate _next;

		public CustomExceptionHandler(RequestDelegate next)
		{
			_next =	next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context,	ex);
			}
		}

		private	async Task HandleExceptionAsync(HttpContext	context, Exception exception)
		{
			var	response = context.Response;
			response.ContentType = "application/json";
			response.StatusCode	= (int)HttpStatusCode.InternalServerError;
			await response.WriteAsync(JsonConvert.SerializeObject(new
			{
				message	= exception.Message,
				stackTrace = exception.StackTrace
			}));
		}
	}
}
