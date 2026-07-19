using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomResertionApp.Data;
using RoomResertionApp.Models;

namespace RoomResertionApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly HotelDbContext _context;

        public AdminController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoginActivity
        public async Task<IActionResult> LoginActivity()
        {
            var loginActivities = await _context.LoginActivities.OrderByDescending(l => l.Timestamp).ToListAsync();
            return View(loginActivities);
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var users = await _context.Users.ToListAsync();
            var dashboard = new
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                UsersByRole = users.GroupBy(u => u.Role).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                RecentUsers = users.OrderByDescending(u => u.CreatedAt).Take(5).ToList()
            };
            return View(dashboard);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Admin/CreateUser
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Password = PasswordHelper.HashPassword(model.Password),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User account '{model.Username}' created successfully.";
            return RedirectToAction("Users");
        }

        // GET: Admin/CreateAdminAccount
        public IActionResult CreateAdminAccount()
        {
            var model = new RegisterViewModel
            {
                Role = UserRole.Administrator
            };
            return View(model);
        }

        // POST: Admin/CreateAdminAccount
        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Password = PasswordHelper.HashPassword(model.Password),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Administrator account '{model.Username}' created successfully with full access to system functions.";
            return RedirectToAction("Users");
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };

            return View(model);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        public async Task<IActionResult> EditUser(int id, RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if username exists for another user
            if (await _context.Users.AnyAsync(u => u.Username == model.Username && u.Id != id))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            // Check if email exists for another user
            if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            user.Username = model.Username;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Role = model.Role;
            
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = PasswordHelper.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{model.Username}' updated successfully.";
            return RedirectToAction("Users");
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id, string confirm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting the last admin
            var adminCount = await _context.Users.CountAsync(u => u.Role == UserRole.Administrator);
            if (user.Role == UserRole.Administrator && adminCount <= 1)
            {
                ModelState.AddModelError("", "Cannot delete the last administrator");
                return View(user);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        // POST: Admin/ToggleUserStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        // GET: Admin/Rooms
        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        // GET: Admin/CreateRoom
        public IActionResult CreateRoom()
        {
            return View();
        }

        // POST: Admin/CreateRoom
        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = new Room
            {
                RoomNumber = model.RoomNumber,
                Type = model.Type,
                PricePerNight = model.PricePerNight,
                Capacity = model.Capacity,
                Description = model.Description,
                IsAvailable = model.IsAvailable,
                ImageUrl = model.ImageUrl,
                Location = model.Location,
                Amenities = model.Amenities
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Room '{model.RoomNumber}' created successfully.";
            return RedirectToAction("Rooms");
        }

        // GET: Admin/EditRoom/5
        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                Description = room.Description,
                IsAvailable = room.IsAvailable,
                ImageUrl = room.ImageUrl,
                Location = room.Location,
                Amenities = room.Amenities
            };

            return View(model);
        }

        // POST: Admin/EditRoom/5
        [HttpPost]
        public async Task<IActionResult> EditRoom(int id, RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            room.RoomNumber = model.RoomNumber;
            room.Type = model.Type;
            room.PricePerNight = model.PricePerNight;
            room.Capacity = model.Capacity;
            room.Description = model.Description;
            room.IsAvailable = model.IsAvailable;
            room.ImageUrl = model.ImageUrl;
            room.Location = model.Location;
            room.Amenities = model.Amenities;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Room '{model.RoomNumber}' updated successfully.";
            return RedirectToAction("Rooms");
        }

        // GET: Admin/ReservationStatistics
        public async Task<IActionResult> ReservationStatistics()
        {
            var totalReservations = await _context.Reservations.CountAsync();
            var confirmedReservations = await _context.Reservations.CountAsync(r => r.Status == "Confirmed");
            var pendingReservations = await _context.Reservations.CountAsync(r => r.Status == "Pending");
            var cancelledReservations = await _context.Reservations.CountAsync(r => r.Status == "Cancelled");

            var totalRevenue = await _context.Reservations
                .Where(r => r.Status == "Confirmed" || r.Status == "Pending")
                .SumAsync(r => r.TotalPrice);

            var reservationsByMonth = await _context.Reservations
                .Where(r => r.BookingDate >= DateTime.Now.AddMonths(-6))
                .GroupBy(r => new { r.BookingDate.Year, r.BookingDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Revenue = g.Sum(r => r.TotalPrice)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            var mostBookedRooms = await _context.Reservations
                .Where(r => r.Status == "Confirmed" || r.Status == "Pending")
                .GroupBy(r => r.RoomId)
                .Select(g => new
                {
                    RoomId = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(r => r.TotalPrice)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();

            var roomDetails = await _context.Rooms.ToListAsync();

            var model = new
            {
                TotalReservations = totalReservations,
                ConfirmedReservations = confirmedReservations,
                PendingReservations = pendingReservations,
                CancelledReservations = cancelledReservations,
                TotalRevenue = totalRevenue,
                ReservationsByMonth = reservationsByMonth,
                MostBookedRooms = mostBookedRooms,
                RoomDetails = roomDetails
            };

            return View(model);
        }

        // GET: Admin/DeleteRoom/5
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Admin/DeleteRoom/5
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id, string confirm)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction("Rooms");
        }
    }
}
