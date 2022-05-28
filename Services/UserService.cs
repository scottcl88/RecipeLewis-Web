using Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserModel> GetUser();
    Task<GenericResult> RequestEditAccess();
    Task<GenericResult> VerifyEmail(VerifyEmailRequest request);
    Task<GenericResult> ForgotPassword(ForgotPasswordRequest request);
    Task<GenericResult> ResetPassword(ResetPasswordRequest request);
    Task<GenericResult> ValidateResetToken(ValidateResetTokenRequest request);
    Task<GenericResult> Register(RegisterRequest request);
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
}