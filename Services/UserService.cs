using Models.Results;
using RecipeLewis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserModel> GetUser();
    Task<GenericResult> RequestEditAccess();
}

public class UserService : IUserService
{
    private readonly IHttpService _httpService;

    public UserService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<UserModel> GetUser()
    {
        return await _httpService.Get<UserModel>("users/user");
    }
    public async Task<GenericResult> RequestEditAccess()
    {
        return await _httpService.Get<GenericResult>("users/request-edit-access");
    }
}