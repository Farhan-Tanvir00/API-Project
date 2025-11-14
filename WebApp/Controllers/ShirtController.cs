using Microsoft.AspNetCore.Mvc;
using WebAapp.Models.Repository;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ShirtController : Controller
    {
        private readonly IWebApiExecutor _apiExecutor;

        public ShirtController(IWebApiExecutor apiExecutor)
        {
            _apiExecutor = apiExecutor;
        }
        public async  Task<IActionResult> Index()
        {
            var shirts = await _apiExecutor.InvokeGet<List<Shirt>>("Home/getShirts");
            return View(shirts);
        }

        public IActionResult CreateShirt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var rssponse = await _apiExecutor.InvokePost("Home/CreateShirts", shirt);

                    if (rssponse != null)
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (ApiException ex)
                {
                    HandleApiException(ex);
                }
            }
            return View(shirt);
        }

        public async Task<IActionResult> UpdateShirt(int shirtId)
        {
            try
            {
                var shirt = await _apiExecutor.InvokeGet<Shirt>($"Home/getShirtById/{shirtId}");
                if (shirt != null)
                {
                    return View(shirt);
                }
            }
            catch (ApiException ex)
            {
                HandleApiException(ex);
                return View();
            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> UpdateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiExecutor.InvokePut($"Home/UpdateShirt/{shirt.ID}", shirt);
                    return RedirectToAction("Index");
                }
                catch (ApiException ex)
                {
                    HandleApiException(ex);
                }

            }
            return View(shirt);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShirt([FromForm]int shirtId)
        {
            try
            {
                var shirt = await _apiExecutor.InvokeDelete<Shirt>($"Home/RemoveShirt/{shirtId}");
                return RedirectToAction("Index");
            }
            catch (ApiException ex)
            {
                HandleApiException(ex);
                return View("Index", await _apiExecutor.InvokeGet<List<Shirt>>("Home/getShirts"));
            }
        }

        private void HandleApiException(ApiException ex)
        {
            if (ex.ErrorResponse != null
               && ex.ErrorResponse.Errors != null
               && ex.ErrorResponse.Errors.Count > 0)
            {
                foreach (var error in ex.ErrorResponse.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }
            }
        }
    }
}
