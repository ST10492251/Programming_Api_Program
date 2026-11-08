
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using System.Net.Http.Json;

namespace RaceDayMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsOrganiser()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            return userId != null && role == "Organiser";
        }

        private HttpClient GetAuthenticatedApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            string? authCookie =
                HttpContext.Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrEmpty(authCookie))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Cookie", authCookie);
            }

            return client;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedApiClient();

            try
            {
                var categories =
                    await client.GetFromJsonAsync<List<CategoryViewModel>>(
                        "api/Categories");

                return View(categories ?? new List<CategoryViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Could not connect to RaceDayAPI.";

                return View(new List<CategoryViewModel>());
            }
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            return View(new CategoryViewModel());
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = GetAuthenticatedApiClient();

            try
            {
                var response =
                    await client.PostAsJsonAsync("api/Categories", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Category created successfully!";

                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Could not create category. API response: " + error);

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI.");

                return View(model);
            }
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedApiClient();

            try
            {
                var category =
                    await client.GetFromJsonAsync<CategoryViewModel>(
                        $"api/Categories/{id}");

                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Could not retrieve the category.";

                return RedirectToAction("Index");
            }
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (id != model.CategoryID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var client = GetAuthenticatedApiClient();

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"api/Categories/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Category updated successfully!";

                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Could not update category. API response: " + error);

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI.");

                return View(model);
            }
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedApiClient();

            try
            {
                var response =
                    await client.DeleteAsync($"api/Categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Category deleted successfully!";
                }
                else
                {
                    var error =
                        await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] =
                        "Could not delete category. " + error;
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Could not connect to RaceDayAPI.";
            }

            return RedirectToAction("Index");
        }
    }
}
