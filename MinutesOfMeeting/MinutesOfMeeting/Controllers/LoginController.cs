using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MinutesOfMeeting.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(AuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var jsonData = await _authService.AuthenticateAsync(email, password);

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                ViewBag.Error = "API did not respond.";
                return View();
            }

            try
            {
                dynamic obj = JsonConvert.DeserializeObject(jsonData);
                string token = obj?.token;

                if (string.IsNullOrEmpty(token))
                {
                    ViewBag.Error = "Invalid credentials.";
                    return View();
                }

                HttpContext.Session.SetString("Token", token);
                return RedirectToAction("GetAllDepartment", "Department");
            }
            catch
            {
                ViewBag.Error = "Invalid response from API.";
                return View();
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> Login(string email, string password)
        //{
        //    var jsonData = await _authService.AuthenticateAsync(email, password);

        //    ViewBag.Raw = jsonData;

        //    if (string.IsNullOrWhiteSpace(jsonData))
        //    {
        //        ViewBag.Error = "API returned empty response.";
        //        return View();
        //    }

        //    try
        //    {
        //        dynamic obj = JsonConvert.DeserializeObject(jsonData);
        //        string token = obj?.data?.token;

        //        if (string.IsNullOrEmpty(token))
        //        {
        //            ViewBag.Error = "Login failed. API response shown below.";
        //            return View();
        //        }

        //        HttpContext.Session.SetString("Token", token);
        //        return RedirectToAction("GetAllDepartment", "Department");
        //    }
        //    catch
        //    {
        //        ViewBag.Error = "Invalid JSON from API.";
        //        return View();
        //    }
        //}




        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
