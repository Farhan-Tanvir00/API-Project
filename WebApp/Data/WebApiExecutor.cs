using System.Text.Json;

namespace WebApp.Data
{
    public class WebApiExecutor : IWebApiExecutor
    {
        private readonly IHttpClientFactory _clientFactory;

        public WebApiExecutor(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            //return await httpClient.GetFromJsonAsync<T>(relativeUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);

            await HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> InvokePost<T>(string relativeUrl, T data)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
            var response = await httpClient.PostAsJsonAsync(relativeUrl, data);

            await HandleErrorResponse(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePut<T>(string relativeUrl, T data)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");

            var response = await httpClient.PutAsJsonAsync(relativeUrl, data);
            await HandleErrorResponse(response);

        }
        
        public async Task<T?> InvokeDelete<T>(string relativeUrl)
        {
            var httpClient = _clientFactory.CreateClient("ShirtsAPI");
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
    }
}
