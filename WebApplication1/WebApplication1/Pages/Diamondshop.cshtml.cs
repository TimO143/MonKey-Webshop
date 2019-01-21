using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Resource;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class DiamondshopModel : PageModel
    {
        public FirstnameLastnameEmail EmailKey;
        private readonly WebApplication1.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ApplicationUser _User;

        public DiamondshopModel(WebApplication1.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;

        }

        public void OnGet(string FirstName, string LastName, string Email)
        {
            EmailKey = new FirstnameLastnameEmail() { Email = "mehdiiseenbaas@gmail.com", Firstname = "ja", Lastname = "jaaa" };
            _User = _userManager.GetUserAsync(User).Result;
        }

        

        [BindProperty]
        public Key Key { get; set; }

        public async Task<IActionResult> OnPostAsync(string email, string firstname, string lastname, int? PointsSpend, int productid)
        {


            var id = _userManager.GetUserId(User);
            ApplicationUser gebruiker = await _userManager.GetUserAsync(User);

            // Select an new highest TMPID.
            int currentTMPID = 0;
            bool UniqueTMPCheck = true;
            Random rnd = new Random();
            while (UniqueTMPCheck)
            {
                currentTMPID = rnd.Next(1, 2000000000);
                UniqueTMPCheck = _context.Key.Any(item => item.TMPID == currentTMPID);
            }
             
            if (PointsSpend > gebruiker.TPunten || PointsSpend == null)
            {
                PointsSpend = 0;
            }
            if (gebruiker.TPunten < 250)
            {
                return RedirectToPage();
            }
            else
            {
                Order Order = new Order()
                {
                    User_ID = id,
                    PointsGain = 0,
                    Paid = 0,
                    PointsSpend = (int)PointsSpend,
                    OrderDate = DateTime.Now,
                    Keys = new List<Key>(),
                };


                if (productid != 0)
                {

                    Order.Keys.Add(new Key()
                    {
                        UserID = id,
                        TMPID = currentTMPID,
                        License = Guid.NewGuid().ToString(),
                        ProductID = productid,
                        Price = 0,
                        OrderDate = DateTime.Now
                    });

                    PointsSpend = PointsSpend + (int)250;
                    Order.PointsSpend = (int)PointsSpend;
                }

                gebruiker.TPunten = gebruiker.TPunten - (int)PointsSpend;

                _context.Users.Update(gebruiker);
                _context.Order.Add(Order);
            }

            // From List keys to only key array
            //EmailKeyArray[] EmailKey = new EmailKeyArray[Order.Keys.Count];

            //for (int j = 0; j < Order.Keys.Count; j++)
            //{
            //    EmailKey[j] = new EmailKeyArray();
            //    EmailKey[j].Key = Order.Keys[j].License;
            //    EmailKey[j].ProductName = (from p in _context.Product
            //                               where p.ID == Order.Keys[j].ProductID
            //                               select p.ResponseName).FirstOrDefault();
            //}

            await _context.SaveChangesAsync();
            return Redirect("FinishCheckout?id=" + currentTMPID);
        }




        }


}