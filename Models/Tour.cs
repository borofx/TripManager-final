using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TripManager.Models;

namespace TripManager.Models
{
    public class Tour
    {
        public Tour()
        {
            TourLandmarks = new List<TourLandmark>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;


        public string UserId { get; set; } = string.Empty;

        public User? User { get; set; } = null!;

        public ICollection<TourLandmark> TourLandmarks { get; set; }
    }
}
