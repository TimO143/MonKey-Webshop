using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Remotion.Linq.Clauses;
using WebApplication1.Data;
using WebApplication1.Resource;

namespace WebApplication1.Pages
{
    public class ShoppingCartControllerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartControllerModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public void OnGet()
        {

        }

        public string cookie;

        public void OnPost(/*[FromBody]*/ int productid, string WichPage = "True")
        {
            // Controlleert of de gebruiker is ingelogd.
            if (User.Identity.IsAuthenticated) // Wordt uitgevoerd als de gebruiker is ingelogd.
            {
                string id = _userManager.GetUserId(User);

                var query = from shop in _context.Shopping_card
                    where shop.User_ID == id
                    select shop;

                Shopping_card shoping_check = query.FirstOrDefault();

                // Controlleert of de gebruiker al een shopping cart heeft.
                if (shoping_check == null) // Wordt uitgevoerd als de gebruiker nog geen shopping cart heeft.
                {
                    Console.WriteLine("");
                    Shopping_card shoppingCard = new Shopping_card()
                    {
                        User_ID = id.ToString(),
                        ShoppingCardProducts = new List<Shopping_card_Product>()
                        {
                            new Shopping_card_Product()
                            {
                                Product_ID = productid
                            }
                        }

                    };
                    _context.Shopping_card.Add(shoppingCard);
                    _context.SaveChanges();
                }
                else // Wordt uitgevoerd als gebruiker uit de shoppingcard al bestaat
                {
                    var query3 = (from shoping in _context.Shopping_card
                            from shopingProduct in _context.Shopping_Card_Products
                            where shopingProduct.Shopping_card_ID == shoping.ID && shoping.User_ID == id
                        select shopingProduct).ToList();

                    List<Shopping_card_Product> shoppingCardProducts = query3;
                    bool checkProductExists = false;

                    foreach (var item in shoppingCardProducts)
                    {
                        if (item.Product_ID == productid)
                        {
                            checkProductExists = true;
                        }
                    }

                    var query2 = from shopingCard in _context.Shopping_card
                        where shopingCard.User_ID == id
                        select shopingCard;

                    Shopping_card shoppingCard = query2.FirstOrDefault();

                    if (!checkProductExists) // Wordt uitgevoerd als het product nog niet in de shopping cart zit.
                    {
                        Shopping_card_Product shoppingCardProduct = new Shopping_card_Product()
                        {
                            Shopping_card_ID = shoppingCard.ID,
                            Product_ID = productid
                        };
                        _context.Shopping_Card_Products.Add(shoppingCardProduct);
                        _context.SaveChanges();
                    }
                    else // Wordt uitgevoerd als het product al wel in de shopping cart zit.
                    {
                        var queryShoppingCardId = (from shoppingCard_I in _context.Shopping_card
                            where shoppingCard_I.User_ID == id
                            select shoppingCard).FirstOrDefault();
                        Shopping_card_Product shoppingCardProduct = _context.Shopping_Card_Products.SingleOrDefault(b => b.Product_ID == productid && b.Shopping_card_ID == queryShoppingCardId.ID);
                        if (shoppingCardProduct != null)
                        {
                            shoppingCardProduct.quantity = shoppingCardProduct.quantity + 1;
                            _context.SaveChanges();
                        }
                    }
                }

            }
            else // Wordt uitgevoerd als de gebruiker niet is ingelogd. (Wordt dus gebruikt voor de cookies).
            {
                // 
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

                if (shoppingcartlist.Count == 0)
                {
                    Response.Cookies.Append("ShoppingCart", productid + "-" + 1, cookieOptions);
                }
                else
                {
                    // Controlleer of product al bestaat en zo je increase the count bij 1.
                    bool Productexists = false;
                    int count = 0;
                    foreach (shoppingCart_cookie item in shoppingcartlist)
                    {
                        if (item.ProductID == productid) // Wordt uigevoerd als item al in shoppingcard zit en verhoogd quantity met 1
                        {
                            Productexists = true;
                            shoppingcartlist[count].Quantity = shoppingcartlist[count].Quantity + 1;
                        }

                        count++;
                    }

                    if (!Productexists) // Als product nog niet bestaat wordt deze toegevoegd.
                    {
                        Response.Cookies.Append("ShoppingCart", cookieshoping + "_" + productid + "-" + 1, cookieOptions);
                    }
                    else // Als product all wel bestaat moet de cookie geupdate worden met een verhoogde quantity.
                    {
                        string cookieshoping_update = shoppingcartlist[0].ProductID + "-" + shoppingcartlist[0].Quantity;

                        for (int i = 1; i < shoppingcartlist.Count; i++)
                        {
                            cookieshoping_update = cookieshoping_update + "_" + shoppingcartlist[i].ProductID + "-" +
                                                   shoppingcartlist[i].Quantity;
                        }
                        Response.Cookies.Append("ShoppingCart", cookieshoping_update, cookieOptions);
                    }
                }
            }

            if (WichPage == "True")
            {
                // Redirect back to Product info
                Response.Redirect("ProductInfo?id=" + productid + "&ProductAdded=ShoppingCard");
            }
            // Redirect to shoppingcard
            Response.Redirect("ShoppingCart");
        }
    }
}