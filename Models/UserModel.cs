using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace RecipeLewis.Models;
public enum Role
{
    Unknown,
    User,
    Editor,
    Admin
}
public record class UserId(int Value);

public class UserModel : EntityDataModel
{
    public UserId? UserId { get; set; }
    public Guid? UserGUID { get; set; }
    public string? LastIPAddress { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? LastLogout { get; set; }
    public string? TimeZone { get; set; }
    public int? UtcOffset { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public Role Role { get; set; }
    public bool RequestedAccess { get; set; }
    public DateTime? RequestedAccessExpires { get; set; }
    public string? Token { get; set; }
    public bool IsVerified { get; set; }
}
