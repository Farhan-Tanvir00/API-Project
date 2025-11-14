using System.Text.Json;

namespace WebApp.Data
{
    public class ApiException: Exception
    {
        public ErrorResponse? ErrorResponse { get; }

        public ApiException(string errorMessageJson)
        {
            ErrorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorMessageJson);
        }
    }
}
