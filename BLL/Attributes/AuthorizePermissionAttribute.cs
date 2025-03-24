using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Attributes;

public class AuthorizePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly ActionPermission _action;

    private readonly string _module;

    public AuthorizePermissionAttribute(string module, ActionPermission action)
    {
        _module = module;
        _action = action;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        bool hasPermission = authService.HasPermission(_module, _action);

        if (!hasPermission)
        {
            // context.Result = new ForbidResult();
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Auth" },  // Change to your actual controller name
                { "action", "AccessDenied" } // Change to your actual action name
            });
        }
    }
}

