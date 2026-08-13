using Microsoft.AspNetCore.Identity;
using NBApp.Models;

namespace NBApp.ViewModels
{
    public class FeaturePermissionRow
    {
        public string FeatureKey { get; set; } = string.Empty;
        public List<string> AllowedRoles { get; set; } = new();
    }

    public class UserViewModel
    {
        public IEnumerable<NBAppUser> Users { get; set; } = null!;
        public IEnumerable<IdentityRole> Roles { get; set; } = null!;
        public List<FeaturePermissionRow> Features { get; set; } = new();
    }
}