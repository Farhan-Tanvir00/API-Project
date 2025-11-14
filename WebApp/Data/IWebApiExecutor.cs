
namespace WebApp.Data
{
    public interface IWebApiExecutor
    {
        Task<T?> InvokeDelete<T>(string relativeUrl);
        Task<T?> InvokeGet<T>(string relativeUrl);
        Task<T?> InvokePost<T>(string relativeUrl, T data);
        Task InvokePut<T>(string relativeUrl, T data);
    }
}