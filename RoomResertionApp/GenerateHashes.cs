// This file is disabled to avoid conflict with Program.cs
// using System;
// using System.Security.Cryptography;
// using System.Text;
//
// class Program
// {
//     static void Main()
//     {
//         string[] passwords = { "guest123", "rec123", "agent123", "manager123", "M0ig3l0L@g1n2009" };
//         
//         foreach (string password in passwords)
//         {
//             string hash = HashPassword(password);
//             Console.WriteLine($"'{password}' => '{hash}'");
//         }
//     }
//
//     static string HashPassword(string password)
//     {
//         using (var sha256 = SHA256.Create())
//         {
//             var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//             return Convert.ToBase64String(hashedBytes);
//         }
//     }
// }
