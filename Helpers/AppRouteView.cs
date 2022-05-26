using BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Net;

namespace BlazorApp.Helpers;

public class AppRouteView : RouteView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IAuthenticationService AuthenticationService { get; set; }

    protected override void Render(RenderTreeBuilder builder)
    {
        CustomAuthorizeAttribute? attribute = (CustomAuthorizeAttribute?)Attribute.GetCustomAttribute(RouteData.PageType, typeof(CustomAuthorizeAttribute));
        if (attribute != null && AuthenticationService.User == null)
        {
            Console.WriteLine("User not authenticated, going to login");
            var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
            NavigationManager.NavigateTo($"login?returnUrl={returnUrl}");
            return;
        }
        else if (attribute != null && !attribute.Roles.Contains(AuthenticationService.User.Role))
        {
            Console.WriteLine("User does not have permission, going to index");
            NavigationManager.NavigateTo($"/");
            return;
        }
        base.Render(builder);
    }
}