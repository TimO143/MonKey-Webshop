using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Services;

namespace WebApplication1.Pages.Admin
{
    public class AdminReadUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AdminReadUserModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public bool IsInRoleAdmin { get; set; }

        public ApplicationUser gebruiker { get; set; }

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

            [Display(Name = "Email address")]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Send Verification email")]
            public bool VerificationEmail { get; set; }

            [Required]
            [Display(Name = "Confirm user's e-mail")]
            public bool ConfirmEmail { get; set; }

            [Required]
            public string userid { get; set; }

            [Display(Name = "Does this user have an Admin Role?")]
            public bool AdminRole { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //[Required]
            //[Display(Name = "Birth Date")]
            //[DataType(DataType.Date)]
            //public DateTime DOB { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string UserID, string status)
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            ApplicationUser user;
            if (UserID != null)
            {
                user = (from u in _context.Users
                        where u.Id == UserID
                        select u).FirstOrDefault();
            }
            else
            {
                user = (from u in _context.Users
                        where u.Id == Input.userid
                        select u).FirstOrDefault();
            }

            gebruiker = user;
            IsInRoleAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user.");
            }

            if (status != null)
            {
                StatusMessage = status;
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
                Email = user.Email,
                //DOB = user.DOB,
                PhoneNumber = user.PhoneNumber,
                userid = user.Id
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = (from u in _context.Users
                                    where u.Id == Input.userid
                                    select u).FirstOrDefault();

            IsInRoleAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user.");
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
                IdentityResult setEmailResult;
                if (user.Email == null)
                {
                    setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                }
                else
                {
                    var emailtoken = await _userManager.GenerateChangeEmailTokenAsync(user, Input.Email);
                    setEmailResult = await _userManager.ChangeEmailAsync(user, Input.Email, emailtoken);
                }
                
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }*/

            if (Input.ConfirmEmail != user.EmailConfirmed)
            {
                if (Input.ConfirmEmail)
                {
                    var EmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var setEmailResult = await _userManager.ConfirmEmailAsync(user, EmailToken);
                    if (!setEmailResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    }
                }
                else
                {
                    user.EmailConfirmed = false;
                }

            }

            if (Input.AdminRole != IsInRoleAdmin)
            {
                if (Input.AdminRole)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            await _userManager.UpdateAsync(user);


            StatusMessage = "The profile has been updated";

            return Redirect("AdminEditUser?UserID=" + user.Id + "&status=" + StatusMessage);
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync(string UserID)
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            ApplicationUser user = (from u in _context.Users
                                    where u.Id == UserID
                                    select u).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{UserID}'.");
            }


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

            StatusMessage = "Verification email sent..";

            return Redirect("AdminEditUser?UserID=" + user.Id + "&status=" + StatusMessage);
        }
    }
}