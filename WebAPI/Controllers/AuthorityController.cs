using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Authority;

namespace WebAPI.Controllers
{
    [ApiVersion("1.0")]
    //[ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    
    public class AuthorityController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthorityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AppCredential credential)
        {

            if(credential.ClientId == null || credential.Secret == null)
            {
                ModelState.AddModelError("ID", "Nothing is provided");

                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = 400,
                    Title = "Unauthorized",
                    Detail = "Please Provide Credentials"
                };

                return new BadRequestObjectResult(problemDetails);
            }

            if (Authenticator.Authenticate(credential.ClientId, credential.Secret))
            {
                var expires_at = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = Authenticator.CreateToken(credential.ClientId, expires_at, _configuration["SecurityKey"] ?? string.Empty),
                });
            }
            else
            {
               ModelState.AddModelError("Unauthorized", "You are not Authorized");

                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = 401,
                };

                return new UnauthorizedObjectResult(problemDetails);
            }
        }

    }
}

     


   