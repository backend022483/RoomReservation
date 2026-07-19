using System.ComponentModel.DataAnnotations;

namespace RoomResertionApp.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }
        public string? Amenities { get; set; }
    }

    public class RoomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Price per night is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }

        public string? Location { get; set; }

        public string? Amenities { get; set; }
    }
}
