using Microsoft.AspNetCore.Components;
using RecipeLewis.Models;
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
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthUserSingleton _authUser;
        public UserModel? User => _authUser?.User;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthUserSingleton authUser
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _authUser = authUser;
            _localStorageService = localStorageService;
            Console.WriteLine("AuthenticationService constructed");
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

        public async Task Logout()
        {
            _authUser.User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}