using System.Text.Json.Serialization;

namespace RecipeLewis.Models.Results;
public class AuthenticateResponse
{
    public UserId UserId { get; set; }
    public bool IsVerified { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}