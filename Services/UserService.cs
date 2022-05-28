using Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserModel> GetUser();
    Task<List<UserModel>> GetAll();
    Task<GenericResult> RequestEditAccess();
    Task<GenericResult> VerifyEmail(VerifyEmailRequest request);
    Task<GenericResult> ForgotPassword(ForgotPasswordRequest request);
    Task<GenericResult> ResetPassword(ResetPasswordRequest request);
    Task<GenericResult> ValidateResetToken(ValidateResetTokenRequest request);
    Task<GenericResult> Register(RegisterRequest request);
    Task<UserModel> Promote(UserId userId);
    Task<UserModel> Demote(UserId userId);
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
    public async Task<List<UserModel>> GetAll()
    {
        return await _httpService.Get<List<UserModel>>("users/get-all");
    }
    public async Task<GenericResult> RequestEditAccess()
    {
        return await _httpService.Post<GenericResult>("users/request-edit-access");
    }
    public async Task<GenericResult> VerifyEmail(VerifyEmailRequest request)
    {
        return await _httpService.Post<GenericResult>("users/verify-email", request);
    }
    public async Task<GenericResult> ValidateResetToken(ValidateResetTokenRequest request)
    {
        return await _httpService.Post<GenericResult>("users/validate-reset-token", request);
    }
    public async Task<GenericResult> ForgotPassword(ForgotPasswordRequest request)
    {
        return await _httpService.Post<GenericResult>("users/forgot-password", request);
    }
    public async Task<GenericResult> ResetPassword(ResetPasswordRequest request)
    {
        return await _httpService.Post<GenericResult>("users/reset-password", request);
    }
    public async Task<GenericResult> Register(RegisterRequest request)
    {
        return await _httpService.Post<GenericResult>("users/register", request);
    }
    public async Task<UserModel> Promote(UserId userId)
    {
        return await _httpService.Post<UserModel>($"users/promote/{userId.Value}");
    }
    public async Task<UserModel> Demote(UserId userId)
    {
        return await _httpService.Post<UserModel>($"users/demote/{userId.Value}");
    }
}