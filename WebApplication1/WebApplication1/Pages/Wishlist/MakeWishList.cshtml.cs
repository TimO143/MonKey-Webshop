using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;

namespace WebApplication1.Pages.Wishlist
{
    public class MakeWishListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public string test;
        public int testid;

        public MakeWishListModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public void OnGet()
        {
            test = "Post";
        }

        public void OnPost( int productid)
        {
            test = "Post";
            testid = productid;
            if (User.Identity.IsAuthenticated)
            {
                string id = _userManager.GetUserId(User);

                var query = from wishlist1 in _context.User_wishlist
                            where wishlist1.User_ID == id && wishlist1.Product_ID == productid
                            select wishlist1;

                User_Wishlist wishlist_check = query.FirstOrDefault();
                if (wishlist_check == null)
                {
                    Console.WriteLine("");
                    User_Wishlist wishlist = new User_Wishlist()
                    {
                        User_ID = id.ToString(),
                        Product_ID = productid
                    };                    
                    _context.User_wishlist.Add(wishlist);
                    _context.SaveChanges();
                }
                  
            }


            Response.Redirect("../Productinfo?id=" + productid + "&ProductAdded=FavoriteAdd");
        }
    }
}