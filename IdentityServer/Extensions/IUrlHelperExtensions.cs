using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Extensions
{
    public static class IUrlHelperExtensions
    {
        public static string? AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object? routeValues = null)
        {
            var scheme = url.ActionContext.HttpContext.Request.Scheme;

            return url.Action(actionName, controllerName, routeValues, scheme);
        }
    }
}
