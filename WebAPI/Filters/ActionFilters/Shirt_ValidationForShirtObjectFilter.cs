using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.DTO.Repository;

namespace WebAPI.Filters.ActionFilters
{
    public class Shirt_ValidationForShirtObjectFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;

        public Shirt_ValidationForShirtObjectFilter(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirt = context.ActionArguments["shirt"] as Shirt;
            if (shirt == null)
            {
                context.ModelState.AddModelError("Shirt", "Shirt Cannot be null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = 400,
                    Title = "Invalid Shirt Object",
                    Detail = "The provided Shirt object is empty. Please correct the errors and try again."
                };
            }
            else 
            {
                var samreShirtExists = from s in _context.Shirts
                                       where s.Color.ToLower() == shirt.Color.ToLower()
                                       && s.Size.ToString().ToLower() == shirt.Size.ToString().ToLower()
                                       && s.Gender.ToLower() == shirt.Gender.ToLower()
                                       select s; 

                if (samreShirtExists.Any())
                {
                    context.ModelState.AddModelError("Shirt", "Same Shirt Already Exists");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = 409,
                        Title = "Duplicate Shirt",
                        Detail = "A shirt with the same Color, Size"
                    };

                    context.Result = new ConflictObjectResult(problemDetails);
                }

            }
        }
        }
}
