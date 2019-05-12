using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoppingAreas.WebApi.Filters
{
	public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation error",
                    Errors = context.ModelState
                        .Where(m => m.Value.Errors.Any())
                        .ToDictionary(k => k.Key, v => v.Value.Errors.Select(e => e.ErrorMessage))
                });
            }
        }
    }
}
