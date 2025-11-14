using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Data;
using WebAPI.DTO.Repository;

namespace WebAPI.Filters.ActionFilters
{
    public class Shirt_ValidateShirtIdFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        public Shirt_ValidateShirtIdFilter(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var id = context.ActionArguments["id"] as int?;

            if (id.HasValue)
            {
                if (id.Value <= 0) 
                { 
                    context.ModelState.AddModelError("ID", "ID must be greater than zero.");

                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = 400,
                        Title = "Invalid Shirt ID",
                        Detail = "The provided Shirt ID is not valid. Please correct the errors and try again."
                    };

                    context.Result = new BadRequestObjectResult(problemDetails);
                }
                else {

                    var shirt = _context.Shirts.Find(id.Value);

                    if (shirt == null)
                    {
                        context.ModelState.AddModelError("ID", "Shirt with the specified ID does not exist.");

                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = 404,
                            Title = "Shirt Not Found",
                            Detail = "No shirt found with the provided ID."
                        };

                        context.Result = new NotFoundObjectResult(problemDetails);
                    }
                    else
                    {
                        context.HttpContext.Items["shirt"] = shirt; //Passing the shirt to the controller through HttpContext.Items
                    }
                }
            }
        }
    }
}
