using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin
{
    public class AdminDeleteProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDeleteProductModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [TempData]
        public string StatusMessage1 { get; set; }

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
            StatusMessage1 = null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? productid)
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

            Product = await _context.Product.FindAsync(productid);

            if (Product != null)
            {
                _context.Product.Remove(Product);
                await _context.SaveChangesAsync();
            }

            StatusMessage1 = $"You permanently deleted {Product.QueryName} from the database";
            //return RedirectToPage();
            return RedirectToPage("/Admin/Admin");
            
        }
    }
}
