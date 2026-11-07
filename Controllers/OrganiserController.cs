
using Microsoft.AspNetCore.Mvc;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : Controller
    {
        // Display the Organiser Dashboard
        public IActionResult Index()
        {
            // Check whether the user is logged in
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check whether the user is an Organiser
            string? role = HttpContext.Session.GetString("Role");

            if (role != "Organiser")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
