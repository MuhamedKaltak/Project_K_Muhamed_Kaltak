﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Project_K.Utilities;
using Project_K.Model;
using System.Text.RegularExpressions;

namespace Project_K.Services
{
    public class EmailService
    {
        public async Task SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("muhamedkaltakprojekt@outlook.com", "MuhamedKaltak_Projekt"),
                    EnableSsl = true,
                };

                var message = new MailMessage
                {
                    From = new MailAddress("muhamedkaltakprojekt@outlook.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                message.To.Add(recipientEmail);

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                await UINotification.DisplayAlertMessage("ERROR", $"Could not send email, exception : {ex.Message}", "OK");
            }
        }

        public async Task<bool> CheckEmailFormat(string email)
        {
            return await Task.Run(() =>
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            });
        }

    }
}
