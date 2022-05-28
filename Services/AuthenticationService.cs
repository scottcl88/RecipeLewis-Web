using Microsoft.AspNetCore.Components;
using Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
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
        public UserModel? User => _authUser?.User;

        public AuthenticationService(
            IHttpService httpService,
            IUserService userService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthUserSingleton authUser
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _authUser = authUser;
            _localStorageService = localStorageService;
            _userService = userService;
        }

        public async Task Initialize()
        {
            _authUser.User = await _localStorageService.GetItem<UserModel>("user");
        }

        public async Task Login(string email, string password)
        {
            _authUser.User = await _httpService.Post<UserModel>("users/authenticate", new { Email = email, Password = password });
            await _localStorageService.SetItem("user", _authUser.User);
        }
        public async Task ResetLocalUser()
        {
            _authUser.User = await _userService.GetUser();
            await _localStorageService.SetItem("user", _authUser.User);
        }
        public async Task Logout()
        {
            _authUser.User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}