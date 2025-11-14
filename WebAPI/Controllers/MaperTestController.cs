using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;
using WebAPI.DTO.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MaperTestController : ControllerBase
    {
        private readonly IMapper _mapper;
        public MaperTestController(IMapper mapper)
        {
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            Shirt? shirt = ShirtsRepo.GetShirtByID(1);

            ShirtDTO shirtDto = _mapper.Map<ShirtDTO>(shirt);
            return Ok(shirtDto);
        }
    }
}
