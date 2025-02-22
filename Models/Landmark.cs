using System;
using System.Collections.Generic;
using TripManager.Models;
using System.ComponentModel.DataAnnotations;

namespace TripManager.Models
{   

    public class Landmark
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }

}
