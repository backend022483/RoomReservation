using Microsoft.AspNetCore.Identity;

Console.WriteLine("Generating password hashes...");

var passwordHasher = new PasswordHasher<object>();

var passwords = new[]
{
    "Admin@2009",
    "Guest@123",
    "Reception@123",
    "Agent@123",
    "Manager@123"
};

foreach (var password in passwords)
{
    var hashedPassword = passwordHasher.HashPassword(null, password);
    Console.WriteLine($"Password: {password}");
    Console.WriteLine($"Hash: {hashedPassword}");
    Console.WriteLine();
}
