using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using WebApplication1.Pages;
using WebApplication1.Services;

namespace WebApplication1.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        public static Task SendKeysToEmailAsync(this IEmailSender emailSender, string email, EmailKeyArray[] EmailKey, string firstname, string lastname)
        {
            string listofKeys = "";

            foreach (var Key in EmailKey)
            {
                listofKeys = listofKeys + Key.Key + Key.ProductName + "<br>";
            }

            return emailSender.SendEmailAsync(email, "Your purchase",
                "Thank you for your purchase by Monkey.<br>" +
                "Your keys will be listed below:<br>" +
                listofKeys);
        }

        public static Task SendContactEmailAsync(this IEmailSender emailSender, string Message, string email)
        {

            return emailSender.SendEmailAsync(">>>enter email-address here<<<<", "Contact bericht",
                "Beste admin er is een bericht voor je.<br>" +
                "Het email van de afzender is: " + email + "<br>" +
                "Het bericht staat hieronder:<br>" +
                Message
                );
        }
    }
}
