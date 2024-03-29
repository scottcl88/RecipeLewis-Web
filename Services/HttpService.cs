using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RecipeLewis.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace BlazorApp.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri);

        Task<T> Post<T>(string uri, object? value = null);

        Task<T> PostForm<T>(string uri, MultipartFormDataContent formData);

        Task<T> Put<T>(string uri, object value);

        Task<T> Delete<T>(string uri);

        string GetBaseAddress();
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public string GetBaseAddress()
        {
            return _httpClient.BaseAddress?.AbsoluteUri ?? "";
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            if (value != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
            }
            return await sendRequest<T>(request);
        }

        public async Task<T> PostForm<T>(string uri, MultipartFormDataContent formData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = formData;
            Console.WriteLine("PostForm content set");
            return await sendRequest<T>(request);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }

        public async Task<T> Delete<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await sendRequest<T>(request);
        }

        // helper methods

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var user = await _localStorageService.GetItem<UserModel>("user");
            bool isApiUrl = !request.RequestUri?.IsAbsoluteUri ?? false;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItem("user");
                _navigationManager.NavigateTo("login");
                return default;
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
            var content = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<T>(content);
            return deserializedObject;
        }
    }
}