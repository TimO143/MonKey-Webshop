using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Pages.Account;
using WebApplication1.Services;

namespace WebApplication1.Pages.Admin
{
    public class AdminCreateUserModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;

        public AdminCreateUserModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Name *")]
            [RegularExpression(@"^[A-Za-z�-�]+$", ErrorMessage = "Please only enter letters")]
            public string Name { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Last name *")]
            [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Please only enter letters")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Country *")]
            public string Country { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "City *")]
            [RegularExpression(@"^[A-Za-z�-�]+$", ErrorMessage = "Please only enter letters")]
            public string City { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Street *")]
            [RegularExpression(@"^[A-Za-z�-� ]+$", ErrorMessage = "Please only enter letters")]
            public string Street { get; set; }

            [StringLength(8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [DataType(DataType.Text)]
            [Display(Name = "Zip code")]
            //[RegularExpression(@"[0-9]{4} [A-Z]{2}", ErrorMessage = "Please enter a valid zip for this country")]
            public string Zip { get; set; }

            [Required]
            [StringLength(4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [DataType(DataType.Text)]
            [Display(Name = "House number *")]
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid house number")]
            public string HouseNumber { get; set; }

            [Required]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email *")]
            public string Email { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password *")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{6,50}$", ErrorMessage = "Check 'Requirements for a valid password' below")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password *")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Display(Name = "Confirm the email of the user")]
            public bool ConfirmEmail { get; set; }

            [Display(Name = "Does this user have an Admin Role?")]
            public bool AdminRole { get; set; }
        }



        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    LastName = Input.LastName,
                    Country = Input.Country,
                    City = Input.City,
                    Street = Input.Street,
                    Zip = Input.Zip,
                    HouseNumber = Input.HouseNumber,
                    DOB = Input.DOB
                };
                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
                IdentityResult EmailConfirmResult = null;

                if (Input.ConfirmEmail)
                {
                    string EmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    EmailConfirmResult = await _userManager.ConfirmEmailAsync(user, EmailToken);
                }

                if (Input.AdminRole)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                if (result.Succeeded || result.Succeeded && EmailConfirmResult.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(Url.GetLocalUrl(returnUrl));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}