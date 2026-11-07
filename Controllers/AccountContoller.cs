
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using System.Net.Http.Json;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ==========================================
        // REGISTRATION
        // ==========================================

        // Display registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Send registration details to RaceDayAPI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Role != "Organiser" && model.Role != "Participant")
            {
                ModelState.AddModelError(
                    "Role", "Please select a valid role.");

                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var registrationData = new
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    Phone = model.Phone,
                    Role = model.Role
                };

                var response = await client.PostAsJsonAsync(
                    "api/auth/register", registrationData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Registration successful. Please log in.";

                    return RedirectToAction("Login");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. Please check your details. " + error);

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI: " + ex.Message);

                return View(model);
            }
        }

        // ==========================================
        // LOGIN
        // ==========================================

        // Display login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Send login details to RaceDayAPI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var loginData = new
                {
                    Email = model.Email,
                    Password = model.Password
                };

                var response = await client.PostAsJsonAsync(
                    "api/auth/login", loginData);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        string.Empty, "Invalid email or password.");

                    return View(model);
                }

                var result =
                    await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The API returned an invalid login response.");

                    return View(model);
                }

                // ==========================================
                // CAPTURE API AUTHENTICATION COOKIE
                // ==========================================

                string? authCookie = null;

                if (response.Headers.TryGetValues(
                    "Set-Cookie", out var cookies))
                {
                    authCookie = cookies.FirstOrDefault(c =>
                        c.StartsWith(
                            "RaceDay.Session.v2=",
                            StringComparison.OrdinalIgnoreCase));
                }

                if (authCookie == null)
                {
                    HttpContext.Session.Clear();

                    ModelState.AddModelError(
                        string.Empty,
                        "Login succeeded, but the API authentication cookie was not received. Check the API cookie configuration.");

                    return View(model);
                }

                // Keep only the cookie name and value.
                authCookie = authCookie.Split(';')[0];

                // Store API cookie in MVC Session.
                HttpContext.Session.SetString(
                    "ApiAuthCookie", authCookie);

                // ==========================================
                // STORE USER DETAILS IN MVC SESSION
                // ==========================================

                HttpContext.Session.SetInt32(
                    "UserID", result.UserID);

                HttpContext.Session.SetString(
                    "Email", result.Email ?? "");

                HttpContext.Session.SetString(
                    "Role", result.Role ?? "");

                // ==========================================
                // ROLE-BASED REDIRECTION
                // ==========================================

                TempData["SuccessMessage"] = "Welcome back!";

                if (result.Role == "Organiser")
                {
                    return RedirectToAction(
                        "Index", "Organiser");
                }

                if (result.Role == "Participant")
                {
                    return RedirectToAction(
                        "Index", "Participant");
                }

                HttpContext.Session.Clear();

                ModelState.AddModelError(
                    string.Empty, "Invalid account role.");

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Could not connect to RaceDayAPI: " + ex.Message);

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An unexpected error occurred: " + ex.Message);

                return View(model);
            }
        }

        // ==========================================
        // LOGOUT
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Retrieve API authentication cookie.
            var authCookie =
                HttpContext.Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrEmpty(authCookie))
            {
                client.DefaultRequestHeaders.Add(
                    "Cookie", authCookie);

                try
                {
                    // Sign out from RaceDayAPI.
                    await client.PostAsync(
                        "api/auth/logout", null);
                }
                catch (HttpRequestException)
                {
                    // Continue clearing MVC session.
                }
            }

            // Clear MVC session information.
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] =
                "You have logged out successfully.";

            return RedirectToAction("Index", "Home");
        }
    }

    // ==========================================
    // API LOGIN RESPONSE MODEL
    // ==========================================

    public class LoginResponse
    {
        public string? Message { get; set; }

        public int UserID { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
