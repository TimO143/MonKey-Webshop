using System;
using System.Collections.Generic;

namespace WebApplication1.Data
{
    public class Order
    {
        public int ID { get; set; }
        public string User_ID { get; set; }
        public int PointsSpend { get; set; }
        public int PointsGain { get; set; }
        public float Paid { get; set; }
        public DateTime OrderDate { get; set; }
        public ApplicationUser User { get; set; }
        public List<Key> Keys { get; set; }
    }
}