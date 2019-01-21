using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string HouseNumber { get; set; }
        public int TPunten { get; set; }
        public DateTime DOB { get; set; }

        public List<User_Wishlist> Wishlists { get; set; }
        public Shopping_card ShoppingCard { get; set; }
        public List<Order> Orders { get; set; }
    }
}
