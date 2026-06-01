using Microsoft.AspNetCore.Identity;
using NBApp.Models;

namespace NBApp.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<NBAppUser> Users { get; set; } = null!;
        public IEnumerable<IdentityRole> Roles { get; set; } = null!;
    }
}
