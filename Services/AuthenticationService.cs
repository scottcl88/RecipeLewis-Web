using Microsoft.AspNetCore.Components;
using Models.Results;
using Newtonsoft.Json;
using RecipeLewis.Models;
using RecipeLewis.Models.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
                    await RefreshToken();
                    var getExpiryTime = GetTokenExpirationTime(_authUser.User.Token);
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(getExpiryTime);
                    var timeUTC = DateTime.UtcNow;
                    var diff = expTime - timeUTC;
                    diff = diff.Subtract(TimeSpan.FromMinutes(2));
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
            var refreshContent = await refreshResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthenticateResponse>(refreshContent);
            _authUser.User.RefreshToken = result.RefreshToken;
            _authUser.User.Token = result.Token;
            await _localStorageService.SetItem("user", _authUser.User);
        }

        private static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }
    }
}