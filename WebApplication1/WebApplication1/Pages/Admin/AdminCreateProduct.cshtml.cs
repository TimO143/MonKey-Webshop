using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin
{
    public class AdminCreateProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public InputModel Input { get; set; }

        //public string ReturnUrl { get; set; }
        [TempData]
        public string StatusMessage2 { get; set; }

        public AdminCreateProductModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class InputModel
        {
            // Queryname belangrijk voor search, zorg ervoor dat het alleen lowercase toe laat
            //public string QueryName { get; set; }
            //public int QueryID { get; set; }

            // nodig voor ProductInfo pagina
            //HeaderImage  , ResponseName , AboutText , PriceFinal 
            [Url]
            [Display(Name = "Image")]
            public string HeaderImage { get; set; }
            //[Url]
            //public string Background { get; set; }
            [Required]
            [Display(Name = "Name of the Game *")]
            public string ResponseName { get; set; }
            [Required]
            [Display(Name = "Put Information about product here *")]
            public string AboutText { get; set; }
            [Required]
            [Display(Name = "Price of the product *")]
            //[RegularExpression(@"^[1-9]+$", ErrorMessage ="No negative numbers")]
            [Range(0,9999,ErrorMessage ="Price has to be between 0 and 9.999")]
            public float PriceFinal  { get; set; }
            

            public bool GenreIsAdventure { get; set; }
            public bool GenreIsRacing { get; set; }
            public bool GenreIsAction { get; set; }
            public bool GenreIsMassivelyMultiplayer { get; set; }
        }

        public IActionResult  OnGet()
        {
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            if (ModelState.IsValid)
            {
                Product product = new Product()
                {
                    QueryName = Input.ResponseName.ToLower(),
                    //QueryID = ,
                    HeaderImage = Input.HeaderImage,
                    ResponseName = Input.ResponseName,
                    AboutText = Input.AboutText,
                    PriceFinal = Input.PriceFinal,
                    GenreIsAction = Input.GenreIsAction,
                    GenreIsAdventure = Input.GenreIsAdventure,
                    GenreIsMassivelyMultiplayer = Input.GenreIsMassivelyMultiplayer,
                    GenreIsRacing = Input.GenreIsRacing,
                };
                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                StatusMessage2 = $"{product.QueryName} has been added to the database!";
                return RedirectToPage("/Admin/Admin");
            }
            return Page();
        }
    }
}