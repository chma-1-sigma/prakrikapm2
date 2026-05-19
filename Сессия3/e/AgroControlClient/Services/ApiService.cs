using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AgroControlClient.Models;

namespace AgroControlClient.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _authToken = string.Empty;
        private readonly string _baseUrl;

        public ApiService(string baseUrl = "https://localhost:5080")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public void SetAuthToken(string token)
        {
            _authToken = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<(bool Success, string Message, string Token)> Login(string username, string password)
        {
            try
            {
                var request = new { username, password };
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Auth/login", content);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        return (true, "Вход выполнен", result.Token);
                    }
                }
                return (false, "Неверный логин или пароль", null);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка подключения: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> Register(RegisterRequest request)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Auth/register", content);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Регистрация успешна");
                }
                return (false, json);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}