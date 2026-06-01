using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NBApp.Models;

// Add profile data for application users by adding properties to the NBAppUser class
public class NBAppUser : IdentityUser
{
    [RegularExpression(@"[a-zA-Z0-9]*$",ErrorMessage ="No spaces or special characters")]
    public required string DisplayName { get; set; }//the name that will be displayed for the user in the application, which can be different from their email.

    public string? ProfilePicturePath { get; set; } = "/profile/default.png";// profile picture is up to interpretation. no further explanation

    public List<Order> Orders { get; set; } = new();// creates a one-to-many relationship between NBAppUser and Order, where one user can have multiple orders.

    [NotMapped]
    public IList<string> Roles { get; set; } = null!;// this property is not mapped to the database, but it can be used to store the roles of the user in memory when needed.

}

