using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using TripManager.Models;

public class User : IdentityUser
{
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
