using RecipeLewis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserModel> GetUser();
}

public class UserService : IUserService
{
    private IHttpService _httpService;

    public UserService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<UserModel> GetUser()
    {
        return await _httpService.Get<UserModel>("users/user");
    }
}