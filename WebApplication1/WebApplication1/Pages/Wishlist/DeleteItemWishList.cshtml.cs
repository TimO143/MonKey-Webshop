using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Wishlist
{
    public class DeleteItemWishListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteItemWishListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User_Wishlist User_Wishlist { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User_Wishlist = await _context.User_wishlist
                .Include(u => u.Product)
                .Include(u => u.User).SingleOrDefaultAsync(m => m.User_ID == id);

            if (User_Wishlist == null)
            {
                return NotFound();
            }
            return Page();
        }
        // delete vanuit wishlist
        public async Task<IActionResult> OnPostAsync(string id, int wish)
        {
            if (id == null)
            {
                return NotFound();
            }

            User_Wishlist = await _context.User_wishlist.FindAsync(id,wish);

            if (User_Wishlist != null)
            {
                _context.User_wishlist.Remove(User_Wishlist);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Wishlist/Wishlist");
        }

        // delete vanuit productinfo
        public async Task<IActionResult> OnPostDeleteAsync(string id, int wish)
        {
            if (id == null)
            {
                return NotFound();
            }

            User_Wishlist = await _context.User_wishlist.FindAsync(id, wish);

            if (User_Wishlist != null)
            {
                _context.User_wishlist.Remove(User_Wishlist);
                await _context.SaveChangesAsync();
            }

            return Redirect("../Productinfo?id=" + wish + "&ProductAdded=FavoriteDelete");
        }
    }
}
