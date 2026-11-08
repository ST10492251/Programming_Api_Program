using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ==========================================
        // PARTICIPANT AUTHENTICATION
        // ==========================================

        private bool IsParticipant()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            return userId != null && role == "Participant";
        }

        // Create an authenticated API client
        private HttpClient GetAuthenticatedApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            string? apiCookie =
                HttpContext.Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                client.DefaultRequestHeaders.Remove("Cookie");

                client.DefaultRequestHeaders.Add(
                    "Cookie", apiCookie);
            }

            return client;
        }

        // ==========================================
        // PARTICIPANT DASHBOARD
        // ==========================================

        // GET: Participant/Index
        public IActionResult Index()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // ==========================================
        // BROWSE EVENTS
        // ==========================================

        // GET: Participant/BrowseEvents
        public async Task<IActionResult> BrowseEvents()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client =
                    _httpClientFactory.CreateClient("RaceDayAPI");

                var response =
                    await client.GetAsync("api/Events/published");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage =
                        "Could not retrieve published events. API status: "
                        + (int)response.StatusCode;

                    return View(new List<PublishedEventViewModel>());
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                var events =
                    JsonSerializer.Deserialize<List<PublishedEventViewModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(
                    events ?? new List<PublishedEventViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =
                    "Could not connect to RaceDayAPI: " + ex.Message;

                return View(new List<PublishedEventViewModel>());
            }
        }

        // ==========================================
        // ENROL IN EVENT
        // ==========================================

        // POST: Participant/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(
            int eventId,
            string categoryName)
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetAuthenticatedApiClient();

                var enrollmentData = new
                {
                    EventId = eventId,
                    CategoryName = categoryName
                };

                var response = await client.PostAsJsonAsync(
                    "api/Enrollments", enrollmentData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "You have successfully enrolled in the event.";
                }
                else
                {
                    var errorMessage =
                        await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] =
                        "Enrolment failed. " + errorMessage;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Could not connect to the API: " + ex.Message;
            }

            return RedirectToAction("BrowseEvents");
        }

        // ==========================================
        // MY ENROLMENTS
        // ==========================================

        // GET: Participant/MyEnrolments
        public async Task<IActionResult> MyEnrolments()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetAuthenticatedApiClient();

                var response =
                    await client.GetAsync("api/Enrollments/mine");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage =
                        "Could not retrieve your enrolments. API status: "
                        + (int)response.StatusCode;

                    return View(new List<MyEnrollmentViewModel>());
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                var enrolments =
                    JsonSerializer.Deserialize<List<MyEnrollmentViewModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(
                    enrolments ?? new List<MyEnrollmentViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =
                    "Could not connect to RaceDayAPI: " + ex.Message;

                return View(new List<MyEnrollmentViewModel>());
            }
        }

        // ==========================================
        // MY RESULTS
        // ==========================================

        // GET: Participant/MyResults
        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetAuthenticatedApiClient();

                var response =
                    await client.GetAsync("api/Results/mine");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage =
                        "Could not retrieve your results. API status: "
                        + (int)response.StatusCode;

                    return View(new List<MyResultViewModel>());
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                var results =
                    JsonSerializer.Deserialize<List<MyResultViewModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(
                    results ?? new List<MyResultViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =
                    "Could not connect to RaceDayAPI: " + ex.Message;

                return View(new List<MyResultViewModel>());
            }
        }
    }

    // ==========================================
    // PUBLISHED EVENT VIEW MODEL
    // ==========================================

    public class PublishedEventViewModel
    {
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Status { get; set; }

        public List<PublishedCategoryViewModel> Categories { get; set; }
            = new List<PublishedCategoryViewModel>();
    }

    // ==========================================
    // PUBLISHED CATEGORY VIEW MODEL
    // ==========================================

    public class PublishedCategoryViewModel
    {
        public int EventCategoryID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public decimal EntryFee { get; set; }

        public int MaximumParticipants { get; set; }

        public int EnrolledCount { get; set; }
    }

    // ==========================================
    // PARTICIPANT ENROLMENT VIEW MODEL
    // ==========================================

    public class MyEnrollmentViewModel
    {
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public decimal EntryFee { get; set; }

        public DateTime? EnrolmentDate { get; set; }

        public string? Status { get; set; }
    }

    // ==========================================
    // PARTICIPANT RESULTS VIEW MODEL
    // ==========================================

    public class MyResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string ResultStatus { get; set; } = string.Empty;
    }
}
