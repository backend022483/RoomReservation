using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomResertionApp.Data;
using RoomResertionApp.Models;
using System.Security.Claims;

namespace RoomResertionApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelDbContext _context;

        public AccountController(HotelDbContext context)
        {
            _context = context;
        }

        // Initialize database with seed data if empty
        private async Task SeedDatabaseIfNeeded()
        {
            if (!_context.Users.Any())
            {
                var seedUsers = new List<User>
                {
                    new User { Username = "guest", Password = PasswordHelper.HashPassword("Guest@123"), Email = "guest@hotel.com", FirstName = "John", LastName = "Guest", Role = UserRole.Guest, CreatedAt = DateTime.Now, IsActive = true, FailedLoginAttempts = 0 },
                    new User { Username = "receptionist", Password = PasswordHelper.HashPassword("Reception@123"), Email = "receptionist@hotel.com", FirstName = "Sarah", LastName = "Smith", Role = UserRole.Receptionist, CreatedAt = DateTime.Now, IsActive = true, FailedLoginAttempts = 0 },
                    new User { Username = "agent", Password = PasswordHelper.HashPassword("Agent@123"), Email = "agent@hotel.com", FirstName = "Mike", LastName = "Johnson", Role = UserRole.ReservationAgent, CreatedAt = DateTime.Now, IsActive = true, FailedLoginAttempts = 0 },
                    new User { Username = "manager", Password = PasswordHelper.HashPassword("Manager@123"), Email = "manager@hotel.com", FirstName = "David", LastName = "Williams", Role = UserRole.HotelManager, CreatedAt = DateTime.Now, IsActive = true, FailedLoginAttempts = 0 },
                    new User { Username = "regin", Password = PasswordHelper.HashPassword("Admin@2009"), Email = "admin@hotel.com", FirstName = "Regin", LastName = "Admin", Role = UserRole.Administrator, CreatedAt = DateTime.Now, IsActive = true, FailedLoginAttempts = 0 }
                };
                _context.Users.AddRange(seedUsers);
                await _context.SaveChangesAsync();
            }
        }

        public static List<LoginActivity> GetLoginActivities(HotelDbContext context)
        {
            return context.LoginActivities.OrderByDescending(l => l.Timestamp).ToList();
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            await SeedDatabaseIfNeeded();
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Check if user is locked out
            if (user != null && user.LockoutEndDate.HasValue && user.LockoutEndDate.Value > DateTime.Now)
            {
                var remainingTime = (user.LockoutEndDate.Value - DateTime.Now).TotalMinutes;
                ModelState.AddModelError("", $"Account is locked. Please try again in {Math.Ceiling(remainingTime)} minutes.");
                
                _context.LoginActivities.Add(new LoginActivity
                {
                    Username = model.Username,
                    Success = false,
                    ErrorMessage = "Account locked",
                    IpAddress = ipAddress,
                    Timestamp = DateTime.Now,
                    UserRole = user.Role.ToString()
                });
                await _context.SaveChangesAsync();
                
                return View(model);
            }

            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.Password))
            {
                // Log failed login attempt
                _context.LoginActivities.Add(new LoginActivity
                {
                    Username = model.Username,
                    Success = false,
                    ErrorMessage = "Invalid username or password",
                    IpAddress = ipAddress,
                    Timestamp = DateTime.Now,
                    UserRole = user?.Role.ToString()
                });
                
                // Increment failed login attempts if user exists
                if (user != null)
                {
                    user.FailedLoginAttempts++;
                    
                    // Lock account after 5 failed attempts
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.LockoutEndDate = DateTime.Now.AddMinutes(15);
                        user.IsActive = false;
                    }
                    
                    _context.Users.Update(user);
                }
                
                await _context.SaveChangesAsync();

                if (user != null && user.LockoutEndDate.HasValue)
                {
                    ModelState.AddModelError("", "Too many failed login attempts. Account has been locked for 15 minutes.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
                return View(model);
            }

            // Log successful login and reset failed attempts
            user.FailedLoginAttempts = 0;
            user.LockoutEndDate = null;
            user.IsActive = true;
            
            _context.LoginActivities.Add(new LoginActivity
            {
                Username = user.Username,
                Success = true,
                ErrorMessage = null,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now,
                UserRole = user.Role.ToString()
            });
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect administrators to Admin Dashboard
            if (user.Role == UserRole.Administrator)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            await SeedDatabaseIfNeeded();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            // Create new user
            var newUser = new User
            {
                Username = model.Username,
                Password = PasswordHelper.HashPassword(model.Password),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Guest, // Force Guest role for public registration
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Auto-login after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Username),
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.GivenName, newUser.FirstName),
                new Claim(ClaimTypes.Surname, newUser.LastName),
                new Claim(ClaimTypes.Role, newUser.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(model);
        }

        // POST: Account/Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            // Update user profile
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Update claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}
