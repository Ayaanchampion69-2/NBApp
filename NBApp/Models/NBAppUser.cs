using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NBApp.Models;

// Add profile data for application users by adding properties to the NBAppUser class
public class NBAppUser : IdentityUser
{
    public List<Order> Orders { get; set; } = new();
    
}

