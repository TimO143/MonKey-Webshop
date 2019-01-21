using System.Collections.Generic;

namespace WebApplication1.Data
{
    public class Shopping_card
    {
        public int ID { get; set; }
        public string User_ID { get; set; }
        public ApplicationUser User { get; set; }
        public List<Shopping_card_Product> ShoppingCardProducts { get; set; }
    }
}