using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;

namespace WebApplication1.Data
{
    public class User_Wishlist
    {
        public string User_ID { get; set; }
        public int Product_ID { get; set; }
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}