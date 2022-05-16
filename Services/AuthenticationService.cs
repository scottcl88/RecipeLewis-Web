using Microsoft.AspNetCore.Components;
using RecipeLewis.Models;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public interface IAuthenticationService
    {
        UserModel GetUser();
        Task Initialize();
        Task Login(string email, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        private UserModel _user;
        public UserModel GetUser()
        {
            if (_user == null)
            {
                //_user = _localStorageService.GetItem<UserModel>("user").Result;
            }
            return _user;
        }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            Console.WriteLine("AuthenticationService constructed");
        }

        public async Task Initialize()
        {
            _user = await _localStorageService.GetItem<UserModel>("user");
            Console.WriteLine("User set: {0}", _user);
        }

        public async Task Login(string email, string password)
        {
            _user = await _httpService.Post<UserModel>("users/authenticate", new { Email = email, Password = password });
            await _localStorageService.SetItem("user", _user);
        }

        public async Task Logout()
        {
            _user = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}