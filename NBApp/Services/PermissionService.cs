using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using System.Security.Claims;

namespace NBApp.Services
{
    public interface IPermissionService
    {
        Task<bool> IsAllowedAsync(string featureKey, ClaimsPrincipal user);
        Task<List<string>> GetAllowedRolesAsync(string featureKey);
    }

    public class PermissionService(NBAppContext context) : IPermissionService
    {
        private readonly NBAppContext _context = context;

        public async Task<bool> IsAllowedAsync(string featureKey, ClaimsPrincipal user)
        {
            var allowedRoles = await GetAllowedRolesAsync(featureKey);

            if (!allowedRoles.Any())
            {
                return false; // no roles configured for this feature = locked to everyone
            }

            return allowedRoles.Any(role => user.IsInRole(role));
        }

        public async Task<List<string>> GetAllowedRolesAsync(string featureKey)
        {
            return await _context.FeaturePermissions
                .Where(p => p.FeatureKey == featureKey)
                .Select(p => p.RoleName)
                .ToListAsync();
        }
    }
}