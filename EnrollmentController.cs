using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using System.Net.Http.Json;

namespace RaceDayMVC.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnrollmentsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("UserID") != null
                && HttpContext.Session.GetString("Role") == "Organiser";
        }

        private HttpClient GetAuthenticatedApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrEmpty(apiCookie))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Cookie", apiCookie);
            }

            return client;
        }

        // GET: /Enrollments
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetAuthenticatedApiClient();

            var allEnrollments = new List<EnrollmentViewModel>();

            try
            {
                // Retrieve all events
                var eventsResponse = await client.GetAsync("api/Events");

                if (!eventsResponse.IsSuccessStatusCode)
                {
                    var error = await eventsResponse.Content.ReadAsStringAsync();

                    ViewBag.ErrorMessage =
                        $"Events API error: {(int)eventsResponse.StatusCode}. {error}";

                    return View(allEnrollments);
                }

                var events = await eventsResponse.Content
                    .ReadFromJsonAsync<List<EventViewModel>>();

                if (events == null || !events.Any())
                {
                    ViewBag.Message = "No events are currently available.";
                    return View(allEnrollments);
                }

                // Retrieve enrolments for each event
                foreach (var currentEvent in events)
                {
                    var response = await client.GetAsync(
                        $"api/Enrollments/events/{currentEvent.EventID}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();

                        ViewBag.ErrorMessage =
                            $"Enrolments API error: {(int)response.StatusCode}. {error}";

                        return View(allEnrollments);
                    }

                    var enrollments = await response.Content
                        .ReadFromJsonAsync<List<EnrollmentViewModel>>();

                    if (enrollments != null)
                    {
                        foreach (var enrollment in enrollments)
                        {
                            enrollment.EventName = currentEvent.EventName;
                        }

                        allEnrollments.AddRange(enrollments);
                    }
                }

                if (!allEnrollments.Any())
                {
                    ViewBag.Message =
                        "No enrolments have been recorded yet.";
                }

                return View(allEnrollments);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage =
                    "API connection error: " + ex.Message;

                return View(allEnrollments);
            }
        }
    }
}
