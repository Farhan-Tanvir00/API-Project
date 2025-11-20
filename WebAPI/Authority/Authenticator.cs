using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Authority
{
    public static class Authenticator
    {
       
        public static bool Authenticate(string clientId, string secret)
        {
            var application = AppRepository.GetApplicationByClientId(clientId);
            if (application == null)
            {
                return false;
            }
            return (application.ClientId == clientId && application.Secret == secret);
        }

        public static string CreateToken(string clientId, DateTime expiresAt, string secterKey)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secterKey)),
                SecurityAlgorithms.HmacSha256Signature);


            var application = AppRepository.GetApplicationByClientId(clientId);
            var claimsDictionary = new Dictionary<string, object>
            {
                { "AppName", application.ApplicationName ?? string.Empty},        
            };

            var scopes = application?.Scopes?.Split(",") ?? Array.Empty<string>();
            if (scopes.Length > 0) {
                foreach (var scope in scopes) 
                {
                    claimsDictionary.Add(scope.Trim().ToLower(), "true");
                }
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
