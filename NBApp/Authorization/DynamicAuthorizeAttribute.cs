using Microsoft.AspNetCore.Mvc.Filters;
using NBApp.Services;

namespace NBApp.Authorization
{
    // Usage: [DynamicAuthorize("Product.Create")] on top of an [Authorize] controller/action.
    // Checks the FeaturePermissions table at runtime instead of a hardcoded [Authorize(Roles="...")].
    public class DynamicAuthorizeAttribute(string featureKey) : Attribute, IAsyncActionFilter
    {
        private readonly string _featureKey = featureKey;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ChallengeResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            var allowed = await permissionService.IsAllowedAsync(_featureKey, context.HttpContext.User);

            if (!allowed)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
                return;
            }

            await next();
        }
    }
}