using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Items
    {
        public int ID { get; set; }
        public string QueryName { get; set; }
        public string HeaderImage { get; set; }
    }
}
