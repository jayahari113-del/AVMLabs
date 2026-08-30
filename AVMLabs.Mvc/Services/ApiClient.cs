using System.Net.Http.Json;
using System.Text.Json;

namespace AVMLabs.Mvc.Services
{
    // Thin wrapper around HttpClient for talking to the AVMLabs.Api project.
    public class ApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<T>(JsonOpts);
        }

        public async Task<(bool Success, string? Error, T? Data)> PostAsync<T>(string url, object payload)
        {
            var response = await _http.PostAsJsonAsync(url, payload);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(JsonOpts);
                return (true, null, data);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            return (false, errorBody, default);
        }
    }
}
