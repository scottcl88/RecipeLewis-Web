using Microsoft.AspNetCore.Components;
using Models.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using RecipeLewis.Models.Results;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public sealed class AuthUserSingleton
    {
        public UserModel? User { get; set; }
    }
    public interface IAuthenticationService
    {
        UserModel User { get; }
        Task Initialize();
        Task Login(string email, string password);
        Task Logout();
        Task ResetLocalUser();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly IUserService _userService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthUserSingleton _authUser;
        private readonly HttpClient _httpClient;
        private bool _refreshTokenLoopStarted;
        private CancellationTokenSource _cts;
        public UserModel? User => _authUser?.User;

        public AuthenticationService(
            IHttpService httpService,
            IUserService userService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthUserSingleton authUser,
            HttpClient httpClient
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _authUser = authUser;
            _localStorageService = localStorageService;
            _userService = userService;
            _httpClient = httpClient;
        }

        public async Task Initialize()
        {
            _authUser.User = await _localStorageService.GetItem<UserModel>("user");
            //await this.ResetLocalUser();
            Console.WriteLine("AuthService Initialize called. " + _authUser?.User?.Token);
            if (_authUser?.User != null && !string.IsNullOrEmpty(_authUser.User.Token) && !_refreshTokenLoopStarted)
            {
                _cts = new CancellationTokenSource();
                RefreshTokenLoop(_cts.Token);
            }
        }

        public async Task Login(string email, string password)
        {
            _authUser.User = await _httpService.Post<UserModel>("users/authenticate", new { Email = email, Password = password });
            await _localStorageService.SetItem("user", _authUser.User);
            Console.WriteLine("Logged in with Refresh Token: " + _authUser.User.Token);
            if (_authUser?.User != null && !string.IsNullOrEmpty(_authUser.User.Token) && !_refreshTokenLoopStarted)
            {
                _cts = new CancellationTokenSource();
                RefreshTokenLoop(_cts.Token);
            }
        }
        private void RefreshTokenLoop(CancellationToken token)
        {
            _refreshTokenLoopStarted = true;
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Attempting RefreshToken");
                    await RefreshToken();
                    var getExpiryTime = GetTokenExpirationTime(_authUser.User.Token);
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(getExpiryTime);
                    var timeUTC = DateTime.UtcNow;
                    var diff = expTime - timeUTC;
                    diff = diff.Subtract(TimeSpan.FromMinutes(2));
                    Console.WriteLine("Scheduling RefreshToken. Next one: " + diff.ToString());
                    await Task.Delay(diff, token);
                }
            }, token);
        }
        public async Task ResetLocalUser()
        {
            var token = _authUser.User?.Token;
            var refreshToken = _authUser.User?.RefreshToken;
            _authUser.User = await _userService.GetUser();
            _authUser.User.Token = token;
            _authUser.User.RefreshToken = refreshToken;
            await _localStorageService.SetItem("user", _authUser.User);
            Console.WriteLine("ResetLocalUser has set: " + _authUser?.User?.Token);
        }
        public async Task Logout()
        {
            _authUser.User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
            _cts?.Cancel();
        }
        public async Task<GenericResult> RevokeToken(RevokeTokenRequest request)
        {
            return await _httpService.Post<GenericResult>($"users/revoke-token", request);
        }

        public async Task RefreshToken()
        {
            Console.WriteLine("RefreshToken called");
            var user = await _localStorageService.GetItem<UserModel>("user");
            if (user == null)
            {
                Console.WriteLine("User is null, cannot refresh token");
                return;
            }

            var refreshRequest = new RefreshTokenRequest();
            refreshRequest.Token = user.RefreshToken;

            var request = new HttpRequestMessage(HttpMethod.Post, "users/refresh-token");
            request.Content = new StringContent(JsonConvert.SerializeObject(refreshRequest), Encoding.UTF8, "application/json");

            var refreshResult = await _httpClient.SendAsync(request);
            if (!refreshResult.IsSuccessStatusCode)
            {
                Console.WriteLine("Could not refresh token, logging out");
                await _localStorageService.RemoveItem("user");
                _navigationManager.NavigateTo("login");
                _cts.Cancel();
                return;
            }
            Console.WriteLine("Done with refresh token request");
            var refreshContent = await refreshResult.Content.ReadAsStringAsync();
            Console.WriteLine("Done with refresh token request 2 " + refreshContent);
            var result = JsonConvert.DeserializeObject<AuthenticateResponse>(refreshContent);
            Console.WriteLine("Refresh token responded successfully. " + result.RefreshToken);
            _authUser.User.RefreshToken = result.RefreshToken;
            _authUser.User.Token = result.Token;
            await _localStorageService.SetItem("user", _authUser.User);
            Console.WriteLine("RefreshToken updated = " + result.RefreshToken);
        }
        //private async Task TryRefreshToken()
        //{
        //    var user = await _localStorageService.GetItem<UserModel>("user");
        //    if (user == null || string.IsNullOrEmpty(user.Token)) return string.Empty;
        //    var expiryTime = GetTokenExpirationTime(user.Token);
        //    var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiryTime));
        //    var timeUTC = DateTime.UtcNow;
        //    var diff = expTime - timeUTC;
        //    Console.WriteLine("TryRefreshToken: " + diff);
        //    if (diff.TotalMinutes <= 2)
        //    {
        //        var authResponse = await RefreshToken();
        //        return authResponse?.Token;
        //    }
        //}

        private static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            Console.WriteLine("tokenExp: " + tokenExp + " ticks: " + ticks);
            return ticks;
        }

        //private static bool CheckTokenIsValid(string token)
        //{
        //    var tokenTicks = GetTokenExpirationTime(token);
        //    var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

        //    var now = DateTime.Now.ToUniversalTime();

        //    var valid = tokenDate >= now;

        //    return valid;
        //}
    }
}