using Microsoft.AspNetCore.Mvc;
using MinutesOfMeeting.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MinutesOfMeeting.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://mom-webapi.onrender.com/api/");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                RedirectToAction("Login", "Login");
                return;
            }

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // ================= LIST =================
        public async Task<IActionResult> GetAllDepartment()
        {
            try
            {
                AddJwtToken();

                var response = await _client.GetAsync("Department/GetAll");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                var json = await response.Content.ReadAsStringAsync();

                dynamic obj = JsonConvert.DeserializeObject(json);
                var list = JsonConvert.DeserializeObject<List<DepartmentModel>>(obj.data.ToString());

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<DepartmentModel>());
            }
        }

        // GET: AddEdit
        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                AddJwtToken();

                // If Add
                if (id == null)
                    return View(new DepartmentModel());

                // If Edit
                var response = await _client.GetAsync($"Department/GetById/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // API returns { success, message, data }
                dynamic obj = JsonConvert.DeserializeObject(json);
                var model = JsonConvert.DeserializeObject<DepartmentModel>(obj.data.ToString());

                return View(model);
            }
            catch
            {
                TempData["Error"] = "Failed to load department.";
                return RedirectToAction(nameof(GetAllDepartment));
            }
        }


        // POST: Save (Add or Update)
        [HttpPost]
        public async Task<IActionResult> Save(DepartmentModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEdit", model);

            try
            {
                AddJwtToken();

                var content = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response;

                if (model.departmentID == null || model.departmentID == 0)
                {
                    // Add
                    response = await _client.PostAsync("Department/Create", content);
                }
                else
                {
                    // Update
                    response = await _client.PutAsync($"Department/Update/{model.departmentID}", content);
                }

                response.EnsureSuccessStatusCode();

                TempData["Success"] = (model.departmentID == null || model.departmentID == 0)
                    ? "Department added successfully."
                    : "Department updated successfully.";
            }
            catch
            {
                TempData["Error"] = "Failed to save department.";
            }

            return RedirectToAction(nameof(GetAllDepartment));
        }


        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                AddJwtToken();

                var response = await _client.DeleteAsync($"Department/Delete/{id}");
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetAllDepartment");
            }
            catch
            {
                return RedirectToAction("GetAllDepartment");
            }
        }
    }
}
