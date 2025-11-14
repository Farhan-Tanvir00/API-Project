using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.DTO;

namespace WebAPI.Filters.ActionFilters
{
    public class Shirt_ValidateUpdateShirtFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirt = context.ActionArguments["shirt"] as Shirt;
            var shirtId = context.ActionArguments["id"] as int?;

            if(shirt is not null && shirtId.HasValue && shirt.ID != shirtId)
            {
                context.ModelState.AddModelError("ID", "Shirt ID in the body does not match the ID in the URL.");

                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = 400,
                    Title = "Mismatched Shirt ID",
                    Detail = "The Shirt ID provided in the request body does not match the ID specified in the URL. Please correct the errors and try again."
                };

                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
