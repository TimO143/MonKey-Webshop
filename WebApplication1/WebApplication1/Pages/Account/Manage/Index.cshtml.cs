using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Services;

namespace WebApplication1.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]
            public string Name { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Last name")]
            [RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Country")]
            [RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]
            public string Country { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "City")]
            [RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]
            public string City { get; set; }

            [StringLength(8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [DataType(DataType.Text)]
            [Display(Name = "Zip")]
            //[RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]
            public string Zip { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Street")]
            [RegularExpression(@"^[A-Za-zÀ-ÿ ]+$", ErrorMessage = "Please only enter letters")]

            public string Street { get; set; }

            [Required]
            [StringLength(4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [DataType(DataType.Text)]
            [Display(Name = "Housenumber")]
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid house number")]
            public string HouseNumber { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //[Required]
            //[Display(Name = "Birth Date")]
            //[DataType(DataType.Date)]
            //public DateTime DOB { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Name = user.Name,
                LastName = user.LastName,
                Country = user.Country,
                City = user.City,
                Street = user.Street,
                Zip = user.Zip,
                HouseNumber = user.HouseNumber,
                //DOB = user.DOB,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }


            if (Input.LastName != user.LastName)
            {  
                user.LastName = Input.LastName;
            }


            if (Input.Country != user.Country)
            {
                user.Country = Input.Country;
            }


            if (Input.City != user.City)
            {
                user.City = Input.City;
            }


            if (Input.Street != user.Street)
            {
                user.Street = Input.Street;
            }


            if (Input.Zip != user.Zip)
            {
                user.Zip = Input.Zip;
            }

            if (Input.HouseNumber != user.HouseNumber)
            {
                user.HouseNumber = Input.HouseNumber;
            }

            //if (Input.DOB != user.DOB)
            //{
            //    user.DOB = Input.DOB;
            //}

            /*if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }*/

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            await _userManager.UpdateAsync(user);


            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await _userManager.UpdateAsync(user);


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
