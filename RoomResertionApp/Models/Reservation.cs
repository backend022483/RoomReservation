using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomResertionApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        [BindNever]
        public Room? Room { get; set; }
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }
        public int? UserId { get; set; }
        [BindNever]
        public User? User { get; set; }
        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; }
        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string? ConfirmationNumber { get; set; }
    }
}
