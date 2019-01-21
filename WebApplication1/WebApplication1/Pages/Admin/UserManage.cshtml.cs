using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin
{
    public class UserManageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // all users
        public IList<ApplicationUser> Users { get; private set; }

        public UserManageModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            Users = _context.Users.AsNoTracking().ToList();
        }
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Your application description page.";
        }

        public void OnGetDelete(string UserID)
        {

            // Check if the user is logged in and authorised
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {

                ApplicationUser user = _userManager.FindByIdAsync(UserID).Result;
                Shopping_card Shopping_card = _context.Shopping_card.Where(shoping => shoping.User_ID == UserID)
                    .FirstOrDefault();
                if (Shopping_card != null)
                {
                    List<Shopping_card_Product> Shopping_card_Products = _context.Shopping_Card_Products
                        .Where(product => product.Shopping_card_ID == Shopping_card.ID)
                        .ToList();

                    if (Shopping_card_Products.Count() > 0)
                    {
                        // Remove all items from the users shoppingcard.
                        foreach (var item in Shopping_card_Products)
                        {
                            _context.Shopping_Card_Products.Remove(item);
                        }
                    }

                    // Remove the shoppingcard of the user.
                    _context.Shopping_card.Remove(Shopping_card);
                }


                // Remove the user from database.
                var task = Task.Run(async () => { await _userManager.DeleteAsync(user); });
                task.Wait();

                Users = _context.Users.AsNoTracking().ToList();
            }
        }
    }
}