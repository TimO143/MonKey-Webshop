using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin
{
    public class 
        AdminEditProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;


        public AdminEditProductModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            // Queryname belangrijk voor search, zorg ervoor dat het alleen lowercase toe laat
            public string QueryName { get; set; }
            //public int QueryID { get; set; }
            public int ID { get; set; }

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
            [Range(0, 9999, ErrorMessage = "Price has to be between 0 and 9.999")]
            public float PriceFinal { get; set; }


            public bool GenreIsAdventure { get; set; }
            public bool GenreIsRacing { get; set; }
            public bool GenreIsAction { get; set; }
            public bool GenreIsMassivelyMultiplayer { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? productid)
        {
            // Check if the user is logged in and authorised
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Page();
            }

            if (productid == null)
            {
                return NotFound();
            }
            
            Product = await _context.Product.SingleOrDefaultAsync(m => m.ID == productid);
            if (Product == null)
            {
                return NotFound();
            }
            Input = new InputModel
            {
                ID = Product.ID,
                QueryName = Product.QueryName,
                AboutText = Product.AboutText,
                HeaderImage = Product.HeaderImage,
                ResponseName = Product.ResponseName,
                PriceFinal = Product.PriceFinal,
                GenreIsAction = Product.GenreIsAction,
                GenreIsAdventure = Product.GenreIsAdventure,
                GenreIsMassivelyMultiplayer = Product.GenreIsMassivelyMultiplayer,
                GenreIsRacing = Product.GenreIsRacing,
            };
            StatusMessage = null;
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
           
            if ( Input.AboutText != Product.AboutText)
            {
                Product.AboutText = Input.AboutText;
            }

            if (Input.HeaderImage != Product.HeaderImage)
            {
                Product.HeaderImage = Input.HeaderImage;
            }

            if (Input.ResponseName != Product.ResponseName)
            {
                Product.ResponseName = Input.ResponseName;
                Product.QueryName = Input.ResponseName.ToLower();
            }
            if (Input.PriceFinal != Product.PriceFinal)
            {
                Product.PriceFinal = Input.PriceFinal;
            }
            if (Input.GenreIsAction != Product.GenreIsAction)
            {
                Product.GenreIsAction = Input.GenreIsAction;
            }
            if (Input.GenreIsAdventure != Product.GenreIsAdventure)
            {
                Product.GenreIsAdventure = Input.GenreIsAdventure;
            }
            if (Input.GenreIsMassivelyMultiplayer != Product.GenreIsMassivelyMultiplayer)
            {
                Product.GenreIsMassivelyMultiplayer = Input.GenreIsMassivelyMultiplayer;
            }
            if (Input.GenreIsRacing != Product.GenreIsRacing)
            {
                Product.GenreIsRacing = Input.GenreIsRacing;
            }

            Input.QueryName = Input.ResponseName.ToLower();
            _context.Product.Attach(Product).State = EntityState.Modified;
            StatusMessage = $"You have updated this product: {Input.QueryName} !";
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            
            return Page();
            //return RedirectToPage("Account/Manage/AdminEditProduct");
            //return RedirectToPage("/Admin/Admin");
        }

        private bool ProductExists(int productid)
        {
            return _context.Product.Any(e => e.ID == productid);
        }
    }
}
