using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Produces("application/json")]
    [Route("api/Points")]
    public class PointsController : Controller
    {
        public PointsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Main Functions
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // GET: api/Points
        [HttpGet()]
        public int Get()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser Gebruiker = _userManager.GetUserAsync(User).Result;
                return Gebruiker.TPunten;
            }
            // -1 is een error dit zijn niet het aantal punten.
            return -1;
        }
    }
}
