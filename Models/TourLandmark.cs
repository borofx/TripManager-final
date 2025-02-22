namespace TripManager.Models
{
    public class TourLandmark
    {
        public int TourId { get; set; }
        public int LandmarkId { get; set; }

        public Tour Tour { get; set; } = null!;
        public Landmark Landmark { get; set; } = null!;
    }
}
