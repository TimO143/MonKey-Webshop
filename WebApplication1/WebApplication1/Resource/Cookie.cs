using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Resource
{
    public static class Cookie
    {
        public static List<shoppingCart_cookie> Cookiereader_shoppingcart(string cookieshoping)
        {
            List<shoppingCart_cookie> shoppingcartlist = new List<shoppingCart_cookie>();

            if (cookieshoping != null)
            {
                string[] shoppingCartCookies1 = Splitter.stringsplitter(cookieshoping, "_");

                foreach (string item in shoppingCartCookies1)
                {
                    string[] result = item.Split("-");
                    if (result.Length == 2)
                    {
                        shoppingcartlist.Add(new shoppingCart_cookie()
                            {ProductID = int.Parse(result[0]), Quantity = int.Parse(result[1])});
                    }
                }
            }

            return shoppingcartlist;
        }

        public static string CookieCreater_shoppingcart(List<shoppingCart_cookie> shoppingcartlist)
        {
            string cookieshoping_update = "";
            if (shoppingcartlist.Count > 0)
            {
                cookieshoping_update = shoppingcartlist[0].ProductID + "-" + shoppingcartlist[0].Quantity;

                for (int i = 1; i < shoppingcartlist.Count; i++)
                {
                    cookieshoping_update = cookieshoping_update + "_" + shoppingcartlist[i].ProductID + "-" +
                                           shoppingcartlist[i].Quantity;
                }
            }

            return cookieshoping_update;
        }

    }

    public class shoppingCart_cookie
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}