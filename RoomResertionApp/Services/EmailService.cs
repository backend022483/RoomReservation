using System.Net;
using System.Net.Mail;

namespace RoomResertionApp.Services
{
    public interface IEmailService
    {
        Task SendReservationConfirmationAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber, DateTime checkIn, DateTime checkOut, decimal totalPrice);
        Task SendReservationUpdateAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber, DateTime checkIn, DateTime checkOut, decimal totalPrice);
        Task SendReservationCancellationAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendReservationConfirmationAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber, DateTime checkIn, DateTime checkOut, decimal totalPrice)
        {
            try
            {
                var subject = $"Reservation Confirmed - {confirmationNumber}";
                var body = $@"
Dear {guestName},

Your reservation has been successfully confirmed!

Reservation Details:
- Confirmation Number: {confirmationNumber}
- Room: {roomNumber}
- Check-in Date: {checkIn:MMMM dd, yyyy}
- Check-out Date: {checkOut:MMMM dd, yyyy}
- Total Price: {totalPrice:C}

Thank you for choosing our hotel. We look forward to welcoming you!

Best regards,
Hotel Reservation System
";

                await SendEmailAsync(toEmail, subject, body);
                _logger.LogInformation($"Reservation confirmation email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending reservation confirmation email: {ex.Message}");
            }
        }

        public async Task SendReservationUpdateAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber, DateTime checkIn, DateTime checkOut, decimal totalPrice)
        {
            try
            {
                var subject = $"Reservation Updated - {confirmationNumber}";
                var body = $@"
Dear {guestName},

Your reservation has been successfully updated!

Updated Reservation Details:
- Confirmation Number: {confirmationNumber}
- Room: {roomNumber}
- Check-in Date: {checkIn:MMMM dd, yyyy}
- Check-out Date: {checkOut:MMMM dd, yyyy}
- Total Price: {totalPrice:C}

If you did not make this change, please contact us immediately.

Best regards,
Hotel Reservation System
";

                await SendEmailAsync(toEmail, subject, body);
                _logger.LogInformation($"Reservation update email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending reservation update email: {ex.Message}");
            }
        }

        public async Task SendReservationCancellationAsync(string toEmail, string guestName, string confirmationNumber, string roomNumber)
        {
            try
            {
                var subject = $"Reservation Cancelled - {confirmationNumber}";
                var body = $@"
Dear {guestName},

Your reservation has been successfully cancelled.

Cancelled Reservation Details:
- Confirmation Number: {confirmationNumber}
- Room: {roomNumber}

We hope to serve you again in the future.

Best regards,
Hotel Reservation System
";

                await SendEmailAsync(toEmail, subject, body);
                _logger.LogInformation($"Reservation cancellation email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending reservation cancellation email: {ex.Message}");
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
