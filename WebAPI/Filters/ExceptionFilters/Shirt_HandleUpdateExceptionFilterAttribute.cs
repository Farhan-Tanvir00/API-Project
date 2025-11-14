using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Data;
using WebAPI.DTO.Repository;

namespace WebAPI.Filters.ExceptionFilters
{
    public class Shirt_HandleUpdateExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ApplicationDbContext _context;

        public Shirt_HandleUpdateExceptionFilterAttribute(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var strShirtId = context.RouteData.Values["id"] as string;
            if(int.TryParse(strShirtId, out int shirtId))
            {
                if (_context.Shirts.FirstOrDefault(x => x.ID == shirtId) == null)
                {
                    context.ModelState.AddModelError("ID", "Shirt with the specified ID does not exist anymore.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = 404,
                        Title = "Shirt Not Found",
                        Detail = "No shirt found with the provided ID."
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                }
            }

        }
    }
}
