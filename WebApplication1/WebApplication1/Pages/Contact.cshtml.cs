using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class ContactModel : PageModel
    {
        public ContactModel(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        private IEmailSender _emailSender;

        public string Message { get; set; }

        [BindProperty]
        public inputclass Input { get; set; }

        public class inputclass
        {
            [DataType(DataType.Text)]
            [Display(Name = "Message")]
            public string Message { get; set; }
            [Display(Name = "Email address")]
            [EmailAddress]
            public string emailaddress { get; set; }
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            //_emailSender.SendContactEmailAsync(Input.Message, Input.emailaddress);

            Message = "Your message has been send!";

            Input.Message = "";
            Input.emailaddress = "";
        }
    }
}
