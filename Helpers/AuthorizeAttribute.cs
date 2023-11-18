using RecipeLewis.Models;

namespace BlazorApp.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthorizeAttribute : Attribute
{
    private readonly IList<Role> _roles;

    public CustomAuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? [];
    }

    public IList<Role> Roles => _roles;
}