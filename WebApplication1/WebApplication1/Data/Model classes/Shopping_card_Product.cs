namespace WebApplication1.Data
{
    public class Shopping_card_Product
    {
        public int Shopping_card_ID { get; set; }
        public int Product_ID { get; set; }
        public int quantity { get; set; }
        public Shopping_card ShoppingCard { get; set; }
        public Product Product { get; set; }
    }
}