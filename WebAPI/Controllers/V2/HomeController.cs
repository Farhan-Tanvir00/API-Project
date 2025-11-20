using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.DTO.Repository;
using WebAPI.Filters.ActionFilters;
using WebAPI.Filters.ExceptionFilters;

namespace WebAPI.Controllers.V2
{
    [ApiVersion("2.0")]
    //[ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    [Route("[controller]/[Action]")]
    
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        //[Authorize("Read")]
        public IActionResult getShirts()
        {
            Console.WriteLine("Version 2 Apis");
            return Ok(_context.Shirts.ToList());
            //return Ok(ShirtsRepo.Shirts);
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilter))] //if the filter has constructor parameters.
        [Authorize("Read")]
        public IActionResult getShirtById(int id)
        {
            return Ok(HttpContext.Items["shirt"]); //Value from the Action Filter
        }

        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidationForShirtObjectFilter))]
        [Authorize("Write")]
        public IActionResult CreateShirts([FromBody] Shirt shirt)
        {
            //ShirtsRepo.AddNewShirt(shirt);

            _context.Shirts.Add(shirt);
            _context.SaveChanges();

            //convention to return the newly created object with its location
            return CreatedAtAction(nameof(getShirtById), new { id = shirt.ID }, shirt);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilter))] //if the filter has constructor parameters.
        [Shirt_ValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionFilterAttribute))]
        [Authorize("Write")]
        public IActionResult UpdateShirt(int id, [FromBody] Shirt shirt)
        {
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt; //Value from the Action Filter

            shirtToUpdate.Color = shirt.Color;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.Gender = shirt.Gender;
            shirtToUpdate.Price = shirt.Price;

            _context.SaveChanges();
            //ShirtsRepo.UpdateShirt(shirt);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilter))] //if the filter has constructor parameters.
        [Authorize("Delete")]
        public IActionResult RemoveShirt(int id)
        {
            //var shirt = ShirtsRepo.GetShirtByID(id);
            //ShirtsRepo.RemoveShirt(id);

            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

            _context.Shirts.Remove(shirtToDelete);
            _context.SaveChanges();

            return Ok(shirtToDelete);
        }
    }
}
