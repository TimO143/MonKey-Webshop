using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Searchengine;
using WebApplication1.Resource.Pagination;
using WebApplication1.Resource.Option;

namespace WebApplication1.Pages
{
    public class SearchModel : PageModel
    {
        // database context
        public readonly ApplicationDbContext _context;
        public int page_size;

        //check if user is admin
        public bool IsAdmin { get; set; }

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
            page_size = 50;
        }

        // Variablen 
        public Option<Search_Page<Product_search>> Products_page { get; set; }
        public int _Min { get; set; }
        public int _Max { get; set; }
        public string _Adventure { get; set; }
        public string _Racing { get; set; }
        public string _actie { get; set; }
        public string _Multiplayer { get; set; }
        public string _searchquery { get; set; }
        public string _order { get; set; }

        public void OnPost(string Search, int page_index, int page_size, int? min, int? max, string Adventure, string Racing, string actie, string Multiplayer, string order)
        {
            // check if user is an admin ( if not then Admin = false)
            var Admin = User.IsInRole("Admin");
            IsAdmin = Admin;

            bool descending = false;
            _order = order;

            Func<Product_search, bool> filter = p => true;
            Func<Product_search, bool> filterMinMax = p => true;
            Func<Product_search, bool> filterAdventure = p => true;
            Func<Product_search, bool> filterRacing = p => true;
            Func<Product_search, bool> filterShooter = p => true;
            Func<Product_search, bool> filterMultiplayer = p => true;
            Func<Product_search, object> filterorder = p => p.points;
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
            Searchbar Searchbar = new Searchbar(_context);

            if (order == "Price (High to low)")
            {

                descending = true;
                filterorder = p => p.PriceFinal;
            }
            else if(order == "Price (Low to High)")
            {
                filterorder = p => p.PriceFinal;
            }
            else
            {
                descending = true;
            }
            Products_page = Searchbar.search(Search, page_size, page_index, filterorder, filter, descending);
            _searchquery = Search;
        }

        public void OnGet(string Search, int page_index, int page_size, int? min, int? max, string Adventure, string Racing, string actie, string Multiplayer, string order)
        {
            // check if user is an admin ( if not then Admin = false)
            var Admin = User.IsInRole("Admin");
            IsAdmin = Admin;

            bool descending = false;
            _order = order;

            Func<Product_search, bool> filter = p => true;
            Func<Product_search, bool> filterMinMax = p => true;
            Func<Product_search, bool> filterAdventure = p => true;
            Func<Product_search, bool> filterRacing = p => true;
            Func<Product_search, bool> filterShooter = p => true;
            Func<Product_search, bool> filterMultiplayer = p => true;
            Func<Product_search, object> filterorder = p => p.points;
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
            Searchbar Searchbar = new Searchbar(_context);

            if (order == "Price (High to low)")
            {

                descending = true;
                filterorder = p => p.PriceFinal;
            }
            else if (order == "Price (Low to High)")
            {
                filterorder = p => p.PriceFinal;
            }
            else
            {
                descending = true;
            }

            Products_page = Searchbar.search(Search, page_size, page_index, filterorder, filter, descending);
            _searchquery = Search;
        }
    }
}