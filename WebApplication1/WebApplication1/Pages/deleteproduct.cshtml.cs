using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Resource;

namespace WebApplication1.Pages
{
    public class deleteproductModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public deleteproductModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Shopping_card_Product Shopping_card_Product { get; set; }
        public Shopping_card Shopping_card { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Shopping_card_Product = await _context.Shopping_Card_Products
                .Include(s => s.ShoppingCard).SingleOrDefaultAsync(m => m.Shopping_card_ID == id);

            if (Shopping_card_Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, int? cartid)
        {
            if (User.Identity.IsAuthenticated) // Wordt uitgevoerd als de gebuiker is ingelogd.
            {
                if (cartid == null)
                {
                    return NotFound();
                }

                Shopping_card_Product = await _context.Shopping_Card_Products.FindAsync(cartid, id);
                if(Shopping_card_Product == null)
                {
                    return RedirectToPage("/ShoppingCart");
                }
                if (Shopping_card_Product.quantity != -1)
                {
                    Shopping_card_Product.quantity -= 1;
                    await _context.SaveChangesAsync();

                    if (Shopping_card_Product.quantity <= -1)
                    {
                        _context.Shopping_Card_Products.Remove(Shopping_card_Product);
                        await _context.SaveChangesAsync();
                    }
                }
                
            }
            else // Wordt uitgevoerd als de gebruik niet is ingelogd.
            {
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(14),
                    HttpOnly = true
                };

                // Split characters:
                // - between productID and quantity
                // + between Produts in shopping card.
                // Tuple<ProductID, Quantity

                string cookieshoping = Request.Cookies["ShoppingCart"];
                List<shoppingCart_cookie> shoppingcartlist = Cookie.Cookiereader_shoppingcart(cookieshoping);
                List<shoppingCart_cookie> shoppingcartlist_new = new List<shoppingCart_cookie>();

                foreach (var item in shoppingcartlist)
                {
                    if (item.ProductID != id)
                    {
                        shoppingcartlist_new.Add(new shoppingCart_cookie() {ProductID = item.ProductID, Quantity = item.Quantity});
                    }
                    else
                    {
                        if (item.Quantity > 1)
                        {
                            shoppingcartlist_new.Add(new shoppingCart_cookie() { ProductID = item.ProductID, Quantity = item.Quantity - 1 });
                        }
                    }
                }

                Response.Cookies.Append("ShoppingCart", Cookie.CookieCreater_shoppingcart(shoppingcartlist_new), cookieOptions);

            }
            return RedirectToPage("./ShoppingCart");
        }

        public async Task<IActionResult> OnPostDeleteitemAsync(int id1, int? cartid1)
        {
            if (User.Identity.IsAuthenticated) // Wordt uitgevoerd als de gebuiker is ingelogd.
            {
                if (cartid1 == null)
                {
                    return NotFound();
                }


                Shopping_card_Product = await _context.Shopping_Card_Products.FindAsync(cartid1, id1);
                _context.Shopping_Card_Products.Remove(Shopping_card_Product);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Cookie instellingen.
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(14),
                    HttpOnly = true
                };

                // Split characters:
                // - between productID and quantity
                // + between Produts in shopping card.
                // Tuple<ProductID, Quantity

                string cookieshoping = Request.Cookies["ShoppingCart"];
                List<shoppingCart_cookie> shoppingcartlist = Cookie.Cookiereader_shoppingcart(cookieshoping);
                List<shoppingCart_cookie> shoppingcartlist_new = new List<shoppingCart_cookie>();

                foreach (var item in shoppingcartlist)
                {
                    if (item.ProductID != id1)
                    {
                        shoppingcartlist_new.Add(new shoppingCart_cookie() { ProductID = item.ProductID, Quantity = item.Quantity });
                    }
                }
                Response.Cookies.Append("ShoppingCart", Cookie.CookieCreater_shoppingcart(shoppingcartlist_new), cookieOptions);
            }

            return RedirectToPage("./ShoppingCart");
        }

        public async Task<IActionResult> OnPostDeletecartAsync()
        {
            if (User.Identity.IsAuthenticated) // Wordt uitgevoerd als de gebuiker is ingelogd.
            {
                var id = _userManager.GetUserId(User);
                var fullcart = from cartprd in _context.Shopping_Card_Products
                    from cartid in _context.Shopping_card
                    where cartprd.Shopping_card_ID == cartid.ID && cartid.User_ID == id
                    select cartprd;

                //_context.Shopping_Card_Products.Where(s => s.Shopping_card_ID == Shopping_card.ID);
                _context.Shopping_Card_Products.RemoveRange(fullcart);
                await _context.SaveChangesAsync();
                return RedirectToPage("./ShoppingCart");
            }
            else
            {
                // Cookie instellingen.
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(14),
                    HttpOnly = true
                };

                // Split characters:
                // - between productID and quantity
                // + between Produts in shopping card.
                // Tuple<ProductID, Quantity

                Response.Cookies.Append("ShoppingCart", "", cookieOptions);

                return RedirectToPage("./ShoppingCart");
            }
        }
    }
}
