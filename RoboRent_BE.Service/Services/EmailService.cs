using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Service.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RoboRent_BE.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.TryParse(_configuration["Smtp:Port"], out var p) ? p : 587;
            var user = _configuration["Smtp:User"];
            var pass = _configuration["Smtp:Pass"];
            var from = _configuration["Smtp:From"] ?? user;

            // Log configuration for debugging (without password)
            _logger.LogInformation("SMTP Configuration - Host: {Host}, Port: {Port}, User: {User}, From: {From}", 
                host, port, user, from);

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                _logger.LogError("SMTP configuration is incomplete. Host: {Host}, User: {User}, Pass: {PassSet}", 
                    host, user, !string.IsNullOrEmpty(pass) ? "Set" : "Not Set");
                throw new InvalidOperationException("SMTP configuration is incomplete");
            }

            using var message = new MailMessage();
            message.From = new MailAddress(from!);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 5000
            };

            try
            {
                _logger.LogInformation("Attempting to send email to {ToEmail} with subject: {Subject}", toEmail, subject);
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP Error sending email to {Email}. StatusCode: {StatusCode}, Message: {Message}", 
                    toEmail, smtpEx.StatusCode, smtpEx.Message);
                throw new InvalidOperationException($"SMTP Error: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Error: {Error}", toEmail, ex.Message);
                throw;
            }
        }
    }
}



