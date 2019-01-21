using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AddKeyModel : PageModel
    {
        private readonly WebApplication1.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public string mehdiid { get; set; }
        //public IList<Shopping_card_Product> proptabtab { get; set; }
        public List<ResponseShopingCart> proptabtab { get; set; }

        public AddKeyModel(WebApplication1.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;

        }

        public IActionResult OnGet()
        {
                ViewData["ProductID"] = new SelectList(_context.Product, "ID", "ID");
            return Page();


            }

        [BindProperty]
        public Key Key { get; set; }

        public async Task<IActionResult> OnPostAsync(string email, string firstname, string lastname, int? PointsSpend)
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

            mehdiid = id;
            if (User.Identity.IsAuthenticated)
            {
                var fullcart = (from cartprd in _context.Shopping_Card_Products
                                from cartid in _context.Shopping_card
                                where cartprd.Shopping_card_ID == cartid.ID && cartid.User_ID == id
                                select cartprd);
                //proptabtab = fullcart.ToList();

                var query = from shopping in _context.Shopping_card
                            where shopping.User_ID == id
                            let shoppingProducts = (
                                from shoppingProdutstable in _context.Shopping_Card_Products
                                from Products in _context.Product
                                where shoppingProdutstable.Shopping_card_ID == shopping.ID &&
                                      shoppingProdutstable.Product_ID == Products.ID
                                select new ResponseShopingCart() { product = Products, quantity = shoppingProdutstable.quantity + 1 }
                            ).ToList()
                            select shoppingProducts;
                proptabtab = query.FirstOrDefault();

                if (PointsSpend > gebruiker.TPunten || PointsSpend == null)
                {
                    PointsSpend = 0;
                }

                Order Order = new Order()
                {
                    User_ID = id,
                    PointsGain = 0,
                    Paid = 0,
                    PointsSpend = (int)PointsSpend,
                    OrderDate = DateTime.Now,
                    Keys = new List<Key>(),
                };

                float TotalPrice = 0;

                foreach (var item in proptabtab)
                {
                    int i = 0;
                    while (i < item.quantity)
                    {
                        TotalPrice = TotalPrice + item.product.PriceFinal;
                        Order.Keys.Add(new Key()
                        {
                            UserID = id,
                            TMPID = currentTMPID,
                            License = Guid.NewGuid().ToString(),
                            ProductID = item.product.ID,
                            Price = item.product.PriceFinal,
                            OrderDate = DateTime.Now
                        });
                        i = i + 1;
                    }
                }

                if (PointsSpend != null && PointsSpend != 0)
                {
                    double Discount = Math.Round((Math.Pow(((int)PointsSpend + TotalPrice), 1.8) / 1000) * 100) / 100;
                    TotalPrice = (float) Math.Round((TotalPrice - Discount) * 100) / 100;
                }

                Order.PointsGain = (int)Math.Round(TotalPrice);
                Order.Paid = TotalPrice;
                gebruiker.TPunten = gebruiker.TPunten + (int)Math.Round(TotalPrice) - (int)PointsSpend;

                _context.Users.Update(gebruiker);
                _context.Order.Add(Order);

                // From List keys to only key array
                EmailKeyArray[] EmailKey = new EmailKeyArray[Order.Keys.Count];

                for (int j = 0; j < Order.Keys.Count; j++)
                {
                    EmailKey[j] = new EmailKeyArray();
                    EmailKey[j].Key = Order.Keys[j].License;
                    EmailKey[j].ProductName = (from p in _context.Product
                        where p.ID == Order.Keys[j].ProductID
                        select p.ResponseName).FirstOrDefault();
                }

                // Send the E-mail
                _emailSender.SendKeysToEmailAsync(email, EmailKey, firstname, lastname);

                _context.Shopping_Card_Products.RemoveRange(fullcart);

            }
            else // Voor cookie shoppingCard
            {
                string cookieshoping = Request.Cookies["ShoppingCart"];
                List<shoppingCart_cookie> shoppingcartlist = Cookie.Cookiereader_shoppingcart(cookieshoping);
                List<Key> keys = new List<Key>();

                Order Order = new Order()
                {
                    PointsGain = 0,
                    Paid = 0,
                    PointsSpend = 0,
                    OrderDate = DateTime.Now,
                    Keys = new List<Key>(),
                };

                float TotalPrice = 0;

                foreach (shoppingCart_cookie item in shoppingcartlist)
                {
                    int i = 0;
                    while (i < item.Quantity)
                    {
                        TotalPrice = TotalPrice + _context.Product.Where(x => x.ID == item.ProductID).Select(x => x.PriceFinal).FirstOrDefault();
                        Order.Keys.Add(new Key()
                        {
                            TMPID = currentTMPID,
                            License = Guid.NewGuid().ToString(),
                            ProductID = item.ProductID,
                            Price = _context.Product.Where(x => x.ID == item.ProductID).Select(x => x.PriceFinal).FirstOrDefault(),
                            OrderDate = DateTime.Now
                        });
                        i = i + 1;
                    }
                }

                Order.Paid = TotalPrice;
                _context.Order.Add(Order);

                // From List keys to only key array
                EmailKeyArray[] EmailKey = new EmailKeyArray[Order.Keys.Count];

                for (int j = 0; j < Order.Keys.Count; j++)
                {
                    EmailKey[j] = new EmailKeyArray();
                    EmailKey[j].Key = Order.Keys[j].License;
                    EmailKey[j].ProductName = (from p in _context.Product
                                                where p.ID == Order.Keys[j].ProductID
                                                select p.ResponseName).FirstOrDefault();
                }

                // Send the E-mail
                _emailSender.SendKeysToEmailAsync(email, EmailKey, firstname, lastname);

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
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.SaveChangesAsync();
            return Redirect("FinishCheckout?id=" + currentTMPID);
        }
    }

    public class EmailKeyArray
    {
        public string Key { get; set; }
        public string ProductName { get; set; }
    }
}