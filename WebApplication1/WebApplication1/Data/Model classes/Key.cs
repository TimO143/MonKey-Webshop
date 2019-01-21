using System;
using System.Collections.Generic;

namespace WebApplication1.Data
{
    public class Key
    {
        public int ID { get; set; }
        public int TMPID { get; set; }
        public string License { get; set; }
        public bool Sold { get; set; }
        public int ProductID { get; set; }
        public string UserID { get; set; }
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set;  }
        public Product Products { get; set; }
        public Order Order { get; set; }
        public float Price { get; set; }
    }
}