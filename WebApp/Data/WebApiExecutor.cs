using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp.Data
{
    public class WebApiExecutor : IWebApiExecutor
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public WebApiExecutor(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            //return await httpClient.GetFromJsonAsync<T>(relativeUrl);

            await AddJwtHeader(httpClient);

            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);

            await HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> InvokePost<T>(string relativeUrl, T data)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            await AddJwtHeader(httpClient);

            var response = await httpClient.PostAsJsonAsync(relativeUrl, data);

            await HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePut<T>(string relativeUrl, T data)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            await AddJwtHeader(httpClient);

            var response = await httpClient.PutAsJsonAsync(relativeUrl, data);
            await HandleErrorResponse(response);

        }

        public async Task<T?> InvokeDelete<T>(string relativeUrl)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            await AddJwtHeader(httpClient);
            var response = await httpClient.DeleteAsync(relativeUrl);

            await HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new ApiException(message);
            }
        }

        private async Task AddJwtHeader(HttpClient httpClient)
        {
            JwtToken? token = null;
            string? strToken = _contextAccessor.HttpContext?.Session.GetString("access_token");

            if (!string.IsNullOrWhiteSpace(strToken))
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);
            }

            if (token == null || token.ExpiresAt <= DateTime.UtcNow)
            {
                var clientId = _configuration.GetValue<string>("ClientId");
                var secret = _configuration.GetValue<string>("Secret");

                var authClient = _clientFactory.CreateClient("Authorization");
                var response = await authClient.PostAsJsonAsync("auth", new Appcredentials
                {
                    ClientId = clientId,
                    Secret = secret
                });

                response.EnsureSuccessStatusCode();

                strToken = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);

                _contextAccessor.HttpContext?.Session.SetString("access_token", strToken);
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
        }
    }
}
