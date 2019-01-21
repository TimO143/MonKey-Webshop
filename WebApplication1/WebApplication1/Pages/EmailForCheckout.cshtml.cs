using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class EmailForCheckoutModel : PageModel
    {
        public EmailForCheckoutModel(UserManager<ApplicationUser> userManager)
        {
            Input = new InputModel();
            _userManager = userManager;
        }

        private UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "First name *")]
            [RegularExpression(@"^[A-Za-z�-�]+$", ErrorMessage = "Please only enter letters")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name *")]
            [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Please only enter letters")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email *")]
            public string Email { get; set; }
        }
        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User).Result;
                Input.Email = user.Email;
                Input.FirstName = user.Name;
                Input.LastName = user.LastName;
            }
        }

        public IActionResult OnPost()
        {
            return Redirect("Checkout?FirstName=" + Input.FirstName + "&LastName=" + Input.LastName + "&Email=" +
                            Input.Email);
        }
    }
}