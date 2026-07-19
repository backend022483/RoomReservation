using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomResertionApp.Data;
using RoomResertionApp.Models;
using RoomResertionApp.Services;

namespace RoomResertionApp.Controllers
{
    [Authorize(Roles = "Receptionist,Administrator,HotelManager")]
    public class ReservationController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly IEmailService _emailService;

        public ReservationController(HotelDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Reservation/SelectRoom
        public async Task<IActionResult> SelectRoom(DateTime? checkIn, DateTime? checkOut, int? guests, string? location, string? amenities, int? minCapacity)
        {
            ViewBag.CheckIn = checkIn?.ToString("yyyy-MM-dd") ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.CheckOut = checkOut?.ToString("yyyy-MM-dd") ?? DateTime.Today.AddDays(2).ToString("yyyy-MM-dd");
            ViewBag.Guests = guests ?? 1;
            ViewBag.Location = location;
            ViewBag.Amenities = amenities;
            ViewBag.MinCapacity = minCapacity;

            var checkInDate = checkIn ?? DateTime.Today.AddDays(1);
            var checkOutDate = checkOut ?? DateTime.Today.AddDays(2);
            var guestCount = guests ?? 1;
            var minCap = minCapacity ?? 1;

            var allRooms = await _context.Rooms.ToListAsync();
            
            // Get all reservations for the date range
            var reservations = await _context.Reservations
                .Where(r => r.Status == "Confirmed" || r.Status == "Pending")
                .ToListAsync();

            // Filter rooms that don't have overlapping reservations
            var availableRooms = allRooms.Where(r => 
                r.IsAvailable && 
                r.Capacity >= guestCount &&
                r.Capacity >= minCap &&
                !reservations.Any(res => 
                    res.RoomId == r.Id &&
                    res.CheckInDate < checkOutDate && 
                    res.CheckOutDate > checkInDate
                )
            ).ToList();

            // Filter by location if provided
            if (!string.IsNullOrEmpty(location))
            {
                availableRooms = availableRooms.Where(r => 
                    !string.IsNullOrEmpty(r.Location) && 
                    r.Location.Contains(location, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Filter by amenities if provided
            if (!string.IsNullOrEmpty(amenities))
            {
                var amenityList = amenities.Split(',').Select(a => a.Trim().ToLower()).ToList();
                availableRooms = availableRooms.Where(r => 
                    !string.IsNullOrEmpty(r.Amenities) && 
                    amenityList.All(a => r.Amenities.ToLower().Contains(a))
                ).ToList();
            }

            return View(availableRooms);
        }

        // POST: Reservation/SelectRoom
        [HttpPost]
        public async Task<IActionResult> SelectRoom(DateTime checkIn, DateTime checkOut, int guests, string? location, string? amenities, int? minCapacity)
        {
            if (checkIn >= checkOut)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date");
                var rooms = await _context.Rooms.ToListAsync();
                var availableRooms = rooms.Where(r => r.IsAvailable && r.Capacity >= guests).ToList();
                ViewBag.CheckIn = checkIn.ToString("yyyy-MM-dd");
                ViewBag.CheckOut = checkOut.ToString("yyyy-MM-dd");
                ViewBag.Guests = guests;
                ViewBag.Location = location;
                ViewBag.Amenities = amenities;
                ViewBag.MinCapacity = minCapacity;
                return View(availableRooms);
            }

            if (checkIn < DateTime.Today)
            {
                ModelState.AddModelError("", "Check-in date cannot be in the past");
                var rooms = await _context.Rooms.ToListAsync();
                var availableRooms = rooms.Where(r => r.IsAvailable && r.Capacity >= guests).ToList();
                ViewBag.CheckIn = checkIn.ToString("yyyy-MM-dd");
                ViewBag.CheckOut = checkOut.ToString("yyyy-MM-dd");
                ViewBag.Guests = guests;
                ViewBag.Location = location;
                ViewBag.Amenities = amenities;
                ViewBag.MinCapacity = minCapacity;
                return View(availableRooms);
            }

            var allRooms = await _context.Rooms.ToListAsync();
            var minCap = minCapacity ?? 1;

            // Get all reservations for the date range
            var reservations = await _context.Reservations
                .Where(r => r.Status == "Confirmed" || r.Status == "Pending")
                .ToListAsync();

            // Filter rooms that don't have overlapping reservations
            var filteredRooms = allRooms.Where(r =>
                r.IsAvailable &&
                r.Capacity >= guests &&
                r.Capacity >= minCap &&
                !reservations.Any(res =>
                    res.RoomId == r.Id &&
                    res.CheckInDate < checkOut &&
                    res.CheckOutDate > checkIn
                )
            ).ToList();

            // Filter by location if provided
            if (!string.IsNullOrEmpty(location))
            {
                filteredRooms = filteredRooms.Where(r => 
                    !string.IsNullOrEmpty(r.Location) && 
                    r.Location.Contains(location, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Filter by amenities if provided
            if (!string.IsNullOrEmpty(amenities))
            {
                var amenityList = amenities.Split(',').Select(a => a.Trim().ToLower()).ToList();
                filteredRooms = filteredRooms.Where(r => 
                    !string.IsNullOrEmpty(r.Amenities) && 
                    amenityList.All(a => r.Amenities.ToLower().Contains(a))
                ).ToList();
            }

            ViewBag.CheckIn = checkIn.ToString("yyyy-MM-dd");
            ViewBag.CheckOut = checkOut.ToString("yyyy-MM-dd");
            ViewBag.Guests = guests;
            ViewBag.Location = location;
            ViewBag.Amenities = amenities;
            ViewBag.MinCapacity = minCapacity;
            return View(filteredRooms);
        }

        // GET: Reservation/GuestInfo
        public async Task<IActionResult> GuestInfo(int roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                return NotFound();
            }

            var reservation = new Reservation
            {
                RoomId = roomId,
                Room = room,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests
            };

            return View(reservation);
        }

        // POST: Reservation/GuestInfo
        [HttpPost]
        public async Task<IActionResult> GuestInfo(Reservation reservation)
        {
            Console.WriteLine($"=== Booking Process Started ===");
            Console.WriteLine($"RoomId: {reservation.RoomId}");
            Console.WriteLine($"CheckIn: {reservation.CheckInDate}");
            Console.WriteLine($"CheckOut: {reservation.CheckOutDate}");
            Console.WriteLine($"Guests: {reservation.NumberOfGuests}");
            Console.WriteLine($"Guest Name: {reservation.Guest?.FirstName} {reservation.Guest?.LastName}");
            Console.WriteLine($"Guest Email: {reservation.Guest?.Email}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine($"Validation failed: {string.Join(", ", errors)}");

                // Load room for display when validation fails
                if (reservation.Room == null && reservation.RoomId > 0)
                {
                    var roomForDisplay = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == reservation.RoomId);
                    if (roomForDisplay != null)
                    {
                        reservation.Room = roomForDisplay;
                    }
                }

                ModelState.AddModelError("", "Validation failed: " + string.Join(", ", errors));
                return View(reservation);
            }

            Console.WriteLine("Validation passed");

            // Load the room from database
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == reservation.RoomId);
            if (room == null)
            {
                Console.WriteLine("Room not found");
                ModelState.AddModelError("", "Room not found.");
                return View(reservation);
            }
            Console.WriteLine($"Room found: {room.RoomNumber} - {room.Type}");
            reservation.Room = room;

            // Calculate total price
            int nights = (int)(reservation.CheckOutDate - reservation.CheckInDate).TotalDays;
            reservation.TotalPrice = nights * reservation.Room.PricePerNight;
            reservation.BookingDate = DateTime.Now;
            reservation.Status = "Confirmed";
            reservation.ConfirmationNumber = GenerateConfirmationNumber();

            Console.WriteLine($"Total Price: {reservation.TotalPrice}");
            Console.WriteLine($"Confirmation Number: {reservation.ConfirmationNumber}");

            // Check for overlapping reservations before confirming
            var existingReservation = await _context.Reservations
                .Where(r => r.RoomId == reservation.RoomId &&
                            (r.Status == "Confirmed" || r.Status == "Pending") &&
                            r.CheckInDate < reservation.CheckOutDate &&
                            r.CheckOutDate > reservation.CheckInDate)
                .FirstOrDefaultAsync();

            if (existingReservation != null)
            {
                Console.WriteLine($"Room already booked: Reservation ID {existingReservation.Id}");
                ModelState.AddModelError("", "This room is already booked for the selected dates. Please choose different dates or another room.");
                return View(reservation);
            }

            Console.WriteLine("No overlapping reservations found");

            try
            {
                // Ensure Guest object is not null
                if (reservation.Guest == null)
                {
                    Console.WriteLine("Guest object is null, creating new Guest");
                    reservation.Guest = new Guest();
                }

                // Create guest if not exists
                var guest = await _context.Guests.FirstOrDefaultAsync(g =>
                    g.FirstName == reservation.Guest.FirstName &&
                    g.LastName == reservation.Guest.LastName &&
                    g.Email == reservation.Guest.Email);

                if (guest == null)
                {
                    Console.WriteLine("Creating new guest");
                    guest = reservation.Guest;
                    _context.Guests.Add(guest);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Guest created with ID: {guest.Id}");
                }
                else
                {
                    Console.WriteLine($"Using existing guest with ID: {guest.Id}");
                }

                reservation.GuestId = guest.Id;
                reservation.Guest = null;

                // Associate reservation with logged-in user if available and user exists
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        // Verify user exists in database
                        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                        if (userExists)
                        {
                            reservation.UserId = userId;
                            Console.WriteLine($"Associated with User ID: {reservation.UserId}");
                        }
                        else
                        {
                            Console.WriteLine($"User ID {userId} not found in database, setting UserId to null");
                            reservation.UserId = null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No user ID claim found, setting UserId to null");
                    reservation.UserId = null;
                }

                Console.WriteLine("Adding reservation to database");
                _context.Reservations.Add(reservation);
                var saveResult = await _context.SaveChangesAsync();
                Console.WriteLine($"Save result: {saveResult} rows affected");

                if (saveResult > 0)
                {
                    Console.WriteLine($"Reservation saved successfully with ID: {reservation.Id}");

                    // Send confirmation email (Future Release)
                    // await _emailService.SendReservationConfirmationAsync(
                    //     guest.Email,
                    //     $"{guest.FirstName} {guest.LastName}",
                    //     reservation.ConfirmationNumber,
                    //     room.RoomNumber,
                    //     reservation.CheckInDate,
                    //     reservation.CheckOutDate,
                    //     reservation.TotalPrice
                    // );

                    return RedirectToAction("Success", new { id = reservation.Id });
                }
                else
                {
                    Console.WriteLine("Failed to save - no rows affected");
                    ModelState.AddModelError("", "Failed to save reservation. No changes were made to the database.");
                    return View(reservation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while saving your reservation: " + ex.Message + " - " + ex.InnerException?.Message);
                return View(reservation);
            }
        }

        // GET: Reservation/Confirm
        public async Task<IActionResult> Confirm(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Restore guest info from TempData if available
            if (TempData["GuestFirstName"] != null)
            {
                reservation.Guest.FirstName = TempData["GuestFirstName"].ToString();
            }
            if (TempData["GuestLastName"] != null)
            {
                reservation.Guest.LastName = TempData["GuestLastName"].ToString();
            }
            if (TempData["GuestEmail"] != null)
            {
                reservation.Guest.Email = TempData["GuestEmail"].ToString();
            }
            if (TempData["GuestPhone"] != null)
            {
                reservation.Guest.PhoneNumber = TempData["GuestPhone"].ToString();
            }
            if (TempData["GuestAddress"] != null)
            {
                reservation.Guest.Address = TempData["GuestAddress"].ToString();
            }

            return View(reservation);
        }

        // POST: Reservation/Confirm
        [HttpPost]
        public async Task<IActionResult> Confirm(int id, string action)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (action == "confirm")
            {
                reservation.Status = "Confirmed";
                await _context.SaveChangesAsync();

                // Send confirmation email (Future Release)
                // await _emailService.SendReservationConfirmationAsync(
                //     reservation.Guest.Email,
                //     $"{reservation.Guest.FirstName} {reservation.Guest.LastName}",
                //     reservation.ConfirmationNumber,
                //     reservation.Room.RoomNumber,
                //     reservation.CheckInDate,
                //     reservation.CheckOutDate,
                //     reservation.TotalPrice
                // );

                return RedirectToAction("Success", new { id = reservation.Id });
            }
            else
            {
                // Cancel the pending reservation
                reservation.Status = "Cancelled";
                await _context.SaveChangesAsync();
                return RedirectToAction("SelectRoom");
            }
        }

        // GET: Reservation/Success
        public async Task<IActionResult> Success(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        private string GenerateConfirmationNumber()
        {
            return "RES-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
        }

        // GET: Reservation/MyReservations
        public async Task<IActionResult> MyReservations()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.BookingDate)
                .ToListAsync();

            return View(reservations);
        }

        // GET: Reservation/EditReservation/5
        public async Task<IActionResult> EditReservation(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            // Only allow editing if check-in date is in the future
            if (reservation.CheckInDate <= DateTime.Today)
            {
                ModelState.AddModelError("", "Cannot modify reservations that have already started or completed.");
                return RedirectToAction("MyReservations");
            }

            return View(reservation);
        }

        // POST: Reservation/EditReservation/5
        [HttpPost]
        public async Task<IActionResult> EditReservation(Reservation reservation)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var existingReservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == reservation.Id && r.UserId == userId);

            if (existingReservation == null)
            {
                return NotFound();
            }

            // Only allow editing if check-in date is in the future
            if (existingReservation.CheckInDate <= DateTime.Today)
            {
                ModelState.AddModelError("", "Cannot modify reservations that have already started or completed.");
                return RedirectToAction("MyReservations");
            }

            // Check for overlapping reservations with new dates
            var overlappingReservation = await _context.Reservations
                .Where(r => r.RoomId == existingReservation.RoomId &&
                            r.Id != existingReservation.Id &&
                            (r.Status == "Confirmed" || r.Status == "Pending") &&
                            r.CheckInDate < reservation.CheckOutDate &&
                            r.CheckOutDate > reservation.CheckInDate)
                .FirstOrDefaultAsync();

            if (overlappingReservation != null)
            {
                ModelState.AddModelError("", "The room is already booked for the selected dates. Please choose different dates.");
                return View(reservation);
            }

            // Update reservation
            existingReservation.CheckInDate = reservation.CheckInDate;
            existingReservation.CheckOutDate = reservation.CheckOutDate;
            existingReservation.NumberOfGuests = reservation.NumberOfGuests;

            // Recalculate total price
            int nights = (int)(existingReservation.CheckOutDate - existingReservation.CheckInDate).TotalDays;
            existingReservation.TotalPrice = nights * existingReservation.Room.PricePerNight;

            // Update guest info
            if (reservation.Guest != null)
            {
                existingReservation.Guest.FirstName = reservation.Guest.FirstName;
                existingReservation.Guest.LastName = reservation.Guest.LastName;
                existingReservation.Guest.Email = reservation.Guest.Email;
                existingReservation.Guest.PhoneNumber = reservation.Guest.PhoneNumber;
                existingReservation.Guest.Address = reservation.Guest.Address;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Reservation updated successfully.";

            // Send update email (Future Release)
            // await _emailService.SendReservationUpdateAsync(
            //     existingReservation.Guest.Email,
            //     $"{existingReservation.Guest.FirstName} {existingReservation.Guest.LastName}",
            //     existingReservation.ConfirmationNumber,
            //     existingReservation.Room.RoomNumber,
            //     existingReservation.CheckInDate,
            //     existingReservation.CheckOutDate,
            //     existingReservation.TotalPrice
            // );

            return RedirectToAction("MyReservations");
        }

        // GET: Reservation/CancelReservation/5
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            // Only allow cancellation if check-in date is in the future
            if (reservation.CheckInDate <= DateTime.Today)
            {
                ModelState.AddModelError("", "Cannot cancel reservations that have already started or completed.");
                return RedirectToAction("MyReservations");
            }

            return View(reservation);
        }

        // POST: Reservation/CancelReservation/5
        [HttpPost]
        public async Task<IActionResult> CancelReservation(int id, string confirm)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();

            // Send cancellation email (Future Release)
            // var reservationWithDetails = await _context.Reservations
            //     .Include(r => r.Room)
            //     .Include(r => r.Guest)
            //     .FirstOrDefaultAsync(r => r.Id == id);

            // if (reservationWithDetails != null)
            // {
            //     await _emailService.SendReservationCancellationAsync(
            //         reservationWithDetails.Guest.Email,
            //         $"{reservationWithDetails.Guest.FirstName} {reservationWithDetails.Guest.LastName}",
            //         reservationWithDetails.ConfirmationNumber,
            //         reservationWithDetails.Room.RoomNumber
            //     );
            // }

            TempData["SuccessMessage"] = "Reservation cancelled successfully.";
            return RedirectToAction("MyReservations");
        }

        // GET: Reservation/RoomOccupancy
        public async Task<IActionResult> RoomOccupancy()
        {
            var today = DateTime.Today;

            // Arrivals - Check-in today
            var arrivals = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Where(r => r.CheckInDate.Date == today && (r.Status == "Confirmed" || r.Status == "Pending"))
                .ToListAsync();

            // Departures - Check-out today
            var departures = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Where(r => r.CheckOutDate.Date == today && (r.Status == "Confirmed" || r.Status == "Pending"))
                .ToListAsync();

            // In-house - Currently staying (checked in before today, checking out after today)
            var inHouse = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Where(r => r.CheckInDate.Date < today && r.CheckOutDate.Date > today && (r.Status == "Confirmed" || r.Status == "Pending"))
                .ToListAsync();

            // Available rooms - Not booked for today
            var bookedRoomIds = await _context.Reservations
                .Where(r => (r.Status == "Confirmed" || r.Status == "Pending") &&
                            r.CheckInDate.Date <= today &&
                            r.CheckOutDate.Date > today)
                .Select(r => r.RoomId)
                .ToListAsync();

            var availableRooms = await _context.Rooms
                .Where(r => r.IsAvailable && !bookedRoomIds.Contains(r.Id))
                .ToListAsync();

            var model = new
            {
                Arrivals = arrivals,
                Departures = departures,
                InHouse = inHouse,
                AvailableRooms = availableRooms,
                Today = today
            };

            return View(model);
        }
    }
}
