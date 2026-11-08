

using Microsoft.AspNetCore.Mvc;

namespace RaceDayMVC.Controllers
{
    public class ApiTestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiTestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try
            {
                var response = await client.GetAsync("api/Events");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ApiStatus = "Successfully connected to RaceDayAPI!";
                }
                else
                {
                    ViewBag.ApiStatus = "API responded with status: "
                        + (int)response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiStatus = "Connection failed: " + ex.Message;
            }

            return View();
        }
    }
}