using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Resource.Pagination;
using WebApplication1.Resource.Option;
using WebApplication1.Searchengine;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Pages
{
    public class ShoppingModel : PageModel
    {
        // database context
        public readonly ApplicationDbContext _context;
        public int page_size;

        //check if user is admin
        public bool IsAdmin { get; set; }

        public ShoppingModel(ApplicationDbContext context)
        {
            _context = context;
            page_size = 50;
        }

        // Variablen
        public int _Min { get; set; }
        public int _Max { get; set; }
        public string _Adventure { get; set; }
        public string _Racing { get; set; }
        public string _actie { get; set; }
        public string _Multiplayer { get; set; }
        public Option<Page<Product>> Products_page { get; set; }
        public bool show_Pagination { get; set; }
        public string _order { get; set; }

        // Define input requirements
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Search")]
        public string Search { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Price { get; set; }
                
        public void OnGet()
        {
            // check if user is an admin ( if not then Admin = false)
            var Admin = User.IsInRole("Admin");
            IsAdmin = Admin;

            int page_index = 0;
            int page_size = 50;
            Products_page = _context.Product.GetPage(page_index, page_size, a => a.PriceFinal, P => true, true);
        }

        public void OnGetPage(int page_index, int page_size, int? min, int? max, string Adventure, string Racing, string actie, string Multiplayer, string order)
        {
            var Admin = User.IsInRole("Admin");
            IsAdmin = Admin;

            bool descending = true;
            _order = order;

            Func<Product, bool> filter;
            Func<Product, bool> filterMinMax = p => true;
            Func<Product, bool> filterAdventure = p => true;
            Func<Product, bool> filterRacing = p => true;
            Func<Product, bool> filterShooter = p => true;
            Func<Product, bool> filterMultiplayer = p => true;
            Func<Product, object> filterorder = p => p.PriceFinal;

            if (min != null && max != null && min != 0 && max != 0)
            {
                _Min = min.GetValueOrDefault();
                _Max = max.GetValueOrDefault();
                filterMinMax = P => min <= P.PriceFinal && max >= P.PriceFinal;
            }

            if (Adventure == "1")
            {
                _Adventure = Adventure;
                filterAdventure = P => P.GenreIsAdventure;
            }

            if (Racing == "1")
            {
                _Racing = Racing;
                filterRacing = P => P.GenreIsRacing;
            }

            if (actie == "1")
            {
                _actie = actie;
                filterShooter = p => p.GenreIsAction;
            }

            if (Multiplayer == "1")
            {
                _Multiplayer = Multiplayer;
                filterMultiplayer = p => p.GenreIsMassivelyMultiplayer;
            }

            filter = p => filterAdventure(p) && filterMinMax(p) && filterRacing(p) && filterShooter(p) && filterMultiplayer(p);

            if (order == "Price (Low to High)")
            {
                filterorder = p => p.PriceFinal;
                descending = false;
            }

            Products_page = _context.Product.GetPage(page_index, page_size, filterorder, filter, descending);

        }
    }
}