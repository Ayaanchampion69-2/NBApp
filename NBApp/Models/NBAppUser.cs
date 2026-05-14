using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NBApp.Models;

// Add profile data for application users by adding properties to the NBAppUser class
public class NBAppUser : IdentityUser
{
    public required string DisplayName { get; set; }//the name that will be displayed for the user in the application, which can be different from their email.
    public string? ProfilePicturePath { get; set; } = "/profile/default.png";// profile picture is up to interpretation. no further explanation
    public List<Order> Orders { get; set; } = new();// creates a one-to-many relationship between NBAppUser and Order, where one user can have multiple orders.

}

