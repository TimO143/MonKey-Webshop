using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages.Wishlist
{
    public class WishlistModel : PageModel
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;
        public WishlistModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public List<User_Wishlist> Wishlistitems { get; set; }
        public List<Product> productitems { get; set; }
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {

                string id = _userManager.GetUserId(User);
                List<User_Wishlist> GetWishListItems = (from wish in _context.User_wishlist
                                        where wish.User_ID == id 
                                        select wish).ToList();

                Wishlistitems = GetWishListItems;

                foreach(User_Wishlist item in GetWishListItems)
                {
                    item.Product = (from product in _context.Product
                                    where product.ID == item.Product_ID
                                    select product).FirstOrDefault();
                            
                }
                /*var GetProducts = (from product in _context.Product
                                   from wishlist in _context.User_wishlist
                                   where product.ID == wishlist.Product_ID
                                   select product).ToList();

                productitems = GetProducts;*/
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }


        }
    }
}