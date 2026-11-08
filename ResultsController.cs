using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using System.Net.Http.Json;

namespace RaceDayMVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Check organiser login
        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("UserID") != null
                && HttpContext.Session.GetString("Role") == "Organiser";
        }

        // Connect to API using the logged-in user's cookie
        private HttpClient GetAuthenticatedApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                client.DefaultRequestHeaders.Remove("Cookie");
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Cookie", apiCookie);
            }

            return client;
        }

        // GET: /Results
        // Display participants enrolled in the organiser's events.
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetAuthenticatedApiClient();

            var resultList = new List<ManageResultViewModel>();

            try
            {
                // Get all events
                var eventsResponse = await client.GetAsync("api/Events");

                if (!eventsResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage =
                        "Could not retrieve events from the API.";

                    return View(resultList);
                }

                var events = await eventsResponse.Content
                    .ReadFromJsonAsync<List<ResultEventViewModel>>();

                if (events == null || !events.Any())
                {
                    ViewBag.Message = "No events are currently available.";
                    return View(resultList);
                }

                // Get enrolled participants for each event
                foreach (var currentEvent in events)
                {
                    var response = await client.GetAsync(
                        $"api/Enrollments/events/{currentEvent.EventID}");

                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var enrollments = await response.Content
                        .ReadFromJsonAsync<List<EnrollmentViewModel>>();

                    if (enrollments == null)
                    {
                        continue;
                    }

                    foreach (var enrollment in enrollments)
                    {
                        resultList.Add(new ManageResultViewModel
                        {
                            EnrolmentID = enrollment.ID,
                            EventID = currentEvent.EventID,
                            EventName = currentEvent.EventName,
                            EventDate = currentEvent.EventDate,
                            CategoryName = enrollment.CategoryName,

                            ParticipantID = enrollment.Participant.UserID,
                            ParticipantName =
                                enrollment.Participant.FirstName + " " +
                                enrollment.Participant.LastName,

                            Email = enrollment.Participant.Email,
                            Phone = enrollment.Participant.Phone
                        });
                    }
                }

                if (!resultList.Any())
                {
                    ViewBag.Message =
                        "No participants have enrolled in your events yet.";
                }

                return View(resultList);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage =
                    "API connection error: " + ex.Message;

                return View(resultList);
            }
        }

        // POST: /Results/Save
        // Save or update a participant's result.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int enrolmentId,
            string? finishTime,
            int? position,
            string resultStatus)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(resultStatus))
            {
                TempData["ErrorMessage"] =
                    "Please select a result status.";

                return RedirectToAction(nameof(Index));
            }

            TimeSpan? parsedFinishTime = null;

            if (!string.IsNullOrWhiteSpace(finishTime))
            {
                if (!TimeSpan.TryParse(finishTime, out TimeSpan time))
                {
                    TempData["ErrorMessage"] =
                        "Please enter a valid finish time.";

                    return RedirectToAction(nameof(Index));
                }

                parsedFinishTime = time;
            }

            var client = GetAuthenticatedApiClient();

            var resultData = new SaveResultViewModel
            {
                EnrolmentID = enrolmentId,
                FinishTime = parsedFinishTime,
                Position = position,
                ResultStatus = resultStatus
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/Results", resultData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Participant result saved successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] =
                        $"Could not save result. API response: " +
                        $"{(int)response.StatusCode} {error}";
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] =
                    "API connection error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }

    // Event information received from the API
    public class ResultEventViewModel
    {
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }
    }

    // Participant information displayed on the Manage Results page
    public class ManageResultViewModel
    {
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int ParticipantID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    // Data sent to the Results API
    public class SaveResultViewModel
    {
        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string ResultStatus { get; set; } = "finished";
    }
}
