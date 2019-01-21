using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class AdminModel : PageModel
    {       
        private readonly ApplicationDbContext _context;

        // all users
        public IList<ApplicationUser> Users { get; private set; }
        // all products
        public IList<Product> _Products { get; set; }

        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Message { get; set; }

        public async Task OnGetAsync(int productid)
        {
            //var query = (from alluser in _context.Users
            //             select alluser).ToList();
            //_users = query;

            Users = await _context.Users.AsNoTracking().ToListAsync();

            // get products ( test )
            var query = (from products in _context.Product
                        where products.ID > 13160
                        select products).ToList();
           
            _Products = query;

            Message = "Your application description page.";
        }

 
    }
}