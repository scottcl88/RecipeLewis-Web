using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace RecipeLewis.Models;
public enum Role
{
    Admin,
    User
}
public record class UserId(int Value);

public class UserModel : EntityDataModel
{
    public UserId? UserId { get; set; }
    public Guid? UserGUID { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
    public bool IsVerified { get; set; }
}
