using Newtonsoft.Json;
using System.Text;

namespace MinutesOfMeeting
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://mom-webapi.onrender.com/api/");
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            // API expects query parameters, not JSON body
            string url = $"Auth/login?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

            var response = await _httpClient.PostAsync(url, null);

            return await response.Content.ReadAsStringAsync();
        }

        //public async Task<string?> AuthenticateAsync(string email, string password)
        //{
        //    var requestData = new
        //    {
        //        Email = email,      // note the casing
        //        Password = password
        //    };

        //    var content = new StringContent(
        //        JsonConvert.SerializeObject(requestData),
        //        Encoding.UTF8,
        //        "application/json");

        //    var response = await _httpClient.PostAsync("Auth/Login", content);

        //    // Always return what API sends back
        //    return await response.Content.ReadAsStringAsync();
        //}


    }
}
