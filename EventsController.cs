
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using System.Net.Http.Json;

namespace RaceDayMVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Check whether the logged-in user is an Organiser.
        private bool IsOrganiser()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            return userId != null && role == "Organiser";
        }

        // Create an API client with the Organiser's authentication cookie.
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

        // ==========================================
        // DISPLAY ALL EVENTS
        // ==========================================

        // GET: /Events
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetAuthenticatedApiClient();

            try
            {
                var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                    "api/Events");

                return View(events ?? new List<EventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Could not connect to RaceDayAPI. Please try again.";

                return View(new List<EventViewModel>());
            }
        }

        // ==========================================
        // DISPLAY CREATE EVENT FORM
        // ==========================================

        // GET: /Events/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new EventViewModel());
        }

        // ==========================================
        // CREATE NEW EVENT
        // ==========================================

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = GetAuthenticatedApiClient();

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/Events", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event created successfully!";

                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Could not create the event. API response: " + error);

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI. Please try again.");

                return View(model);
            }
        }

        // ==========================================
        // DISPLAY EDIT EVENT FORM
        // ==========================================

        // GET: /Events/Edit/2
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetAuthenticatedApiClient();

            try
            {
                var eventItem = await client.GetFromJsonAsync<EventViewModel>(
                    $"api/Events/{id}");

                if (eventItem == null)
                {
                    TempData["ErrorMessage"] = "Event not found.";
                    return RedirectToAction("Index");
                }

                return View("~/Views/Events/Edit.cshtml", eventItem);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Could not retrieve the event from RaceDayAPI.";

                return RedirectToAction("Index");
            }
        }

        // ==========================================
        // SAVE UPDATED EVENT
        // ==========================================

        // POST: /Events/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.EventID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Events/Edit.cshtml", model);
            }

            var client = GetAuthenticatedApiClient();

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"api/Events/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event updated successfully!";

                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Could not update the event. API response: " + error);

                return View("~/Views/Events/Edit.cshtml", model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI. Please try again.");

                return View("~/Views/Events/Edit.cshtml", model);
            }
        }


        // POST: Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = GetAuthenticatedApiClient();

            try
            {
                var response = await client.DeleteAsync($"api/Events/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] =
                        "Could not delete the event. API response: " + error;
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