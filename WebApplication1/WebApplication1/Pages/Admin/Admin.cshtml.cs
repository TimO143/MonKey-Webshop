using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using System.Web;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Pages.Admin
{
    public class AdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // nodig voor create/update/delete algemeen scherm om naar toe te kunnen gaan.
        // Delete
        [TempData] public string StatusMessage1 { get; set; }
        // create
        [TempData] public string StatusMessage2 { get; set; }

        // all registered users
        public IList<ApplicationUser> RegisteredUsers { get; private set; }
        // all unregistered users
        public IList<ApplicationUser> UnregisteredUsers { get; private set; }
        // Alle orders die ooit zijn gemaakt in de database ( alle keys)
        public IList<Key> AllOrders { get; set; }
        public float SumOfOrders { get; set; }

        // Array met een list van keys in AllOrdersAll en prijs van de keys in AllOrdersSales voor alle maanden
        public List<Key>[] AllOrdersAll { get; set; }
        public List<float>[] AllOrdersSales { get; set; }

        // Array met een list van keys in AllOrdersAll en prijs van de keys in AllOrdersSales voor alle jaren
        public List<Key>[] AllOrdersYears { get; set; }
        public List<float>[] AllOrdersSalesYears { get; set; }

        // for Days filter this year
        public List<Key>[] AllOrdersDaysYear1Jan { get; set; }
        public List<float>[] AllOrdersSalesDaysYear1Jan { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Feb      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Feb { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Mar      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Mar { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Apr        { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Apr { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1May       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1May { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Jun      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Jun { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Jul      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Jul { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Aug       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Aug { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Sep      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Sep { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Okt      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Okt { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Nov      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Nov { get; set; }

        //public List<Key>[]   AllOrdersDaysYear1Dec      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear1Dec { get; set; }

        //// for Days filter last year
        //public List<Key>[]   AllOrdersDaysYear2Jan      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Jan { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Feb       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Feb { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Mar       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Mar { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Apr      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Apr { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2May      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2May { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Jun      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Jun { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Jul      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Jul { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Aug      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Aug { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Sep       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Sep { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Okt      { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Okt { get; set; }

        //public List<Key>[]   AllOrdersDaysYear2Nov       { get; set; }
        //public List<float>[] AllOrdersSalesDaysYear2Nov { get; set; }

        public List<Key>[] AllOrdersDaysYear2Dec { get; set; }
        public List<float>[] AllOrdersSalesDaysYear2Dec { get; set; }



        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        //[BindProperty] public InputModel Input { get; set; }
        //public class InputModel
        //{
        //    [Required]
        //    public int jaar { get; set; }
        //}

        public async Task OnGetAsync()
        {
            // Check if the user is logged in and authorised
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                // Zet het standaard jaar neer.
                //Input = new InputModel()
                //{
                //    jaar = DateTime.Now.Year
                //};

                // Haalt alle registered en niet registered users op voor grafiek UsersGraph
                RegisteredUsers = await _context.Users.AsNoTracking().Where(t => t.EmailConfirmed == true)
                    .ToListAsync();
                UnregisteredUsers = await _context.Users.AsNoTracking().Where(t => t.EmailConfirmed == false)
                    .ToListAsync();

                // Haal de totale ordered producten op uit de database 
                AllOrders = await _context.Key
                    .ToListAsync();


                // Orders en Sales per maand voor de laatste 3 jaren
                AllOrdersAll = new List<Key>[36];
                AllOrdersSales = new List<float>[36];

                // Dit zet de keys van dit jaar,vorig jaar, en 2 jaar geleden in 1 array die wordt gebruikt voor 3 grafieken
                for (int i = 0; i < 12; i++)
                {
                    AllOrdersAll[i] = (from key in _context.Key
                                       where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                             (key.OrderDate.Year == (DateTime.Now.Year)))
                                       select key).ToList();
                    AllOrdersAll[i + 12] = (from key in _context.Key
                                            where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                                  (key.OrderDate.Year == (DateTime.Now.AddYears(-1).Year)))
                                            select key).ToList();
                    AllOrdersAll[i + 24] = (from key in _context.Key
                                            where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                                  (key.OrderDate.Year == (DateTime.Now.AddYears(-2).Year)))
                                            select key).ToList();
                }
                // Dit zet de prijs van de keys van dit jaar,vorig jaar, en 2 jaar geleden in 1 array die wordt gebruikt voor 3 grafieken
                for (int i = 0; i < 12; i++)
                {
                    AllOrdersSales[i] = (from key in _context.Key
                                         where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                               (key.OrderDate.Year == (DateTime.Now.Year)))
                                         select key.Price).ToList();
                    AllOrdersSales[i + 12] = (from key in _context.Key
                                              where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                                    (key.OrderDate.Year == (DateTime.Now.AddYears(-1).Year)))
                                              select key.Price).ToList();
                    AllOrdersSales[i + 24] = (from key in _context.Key
                                              where (key.OrderDate.Month == (DateTime.Now.AddMonths(-i).Month) &&
                                                    (key.OrderDate.Year == (DateTime.Now.AddYears(-2).Year)))
                                              select key.Price).ToList();
                }

                // Orders en Sales per jaar voor de laatste 10 jaar
                AllOrdersYears = new List<Key>[11];
                AllOrdersSalesYears = new List<float>[11];

                for (int i = 0; i < 11; i++)
                {
                    AllOrdersYears[i] = (from key in _context.Key
                                         where key.OrderDate.Year == DateTime.Now.AddYears(-i).Year
                                         select key).ToList();
                    AllOrdersSalesYears[i] = (from key in _context.Key
                                              where key.OrderDate.Year == DateTime.Now.AddYears(-i).Year
                                              select key.Price).ToList();
                }

                // Dit zorgde ervoor dat het laden 1minuut duurde op laptop ookal was het 10s op mn desktop
                // Super lelijk maar het werkt voor de view. onthoud in de array [0,0,0] = 1 januari 2018 , [0,9,30] = 31 oktober 2018  en [1,1,1] = 2 februari 2019 enz
                //AllOrdersDays = new List<Key>[2,12,31];
                //AllOrdersSalesDays = new List<float>[2, 12, 31];
                //for (int year = 0; year < 2; year++)
                //{
                //    for (int month = 0; month < 12; month++)
                //    {
                //        for (int day = 0; day < 31; day++)
                //        {
                //            AllOrdersDays[year,month,day] = (from key in _context.Key
                //                                where key.OrderDate.Year == (year+2018) && key.OrderDate.Month == (month+1) && key.OrderDate.Day == (day+1)
                //                                select key).ToList();

                //            AllOrdersSalesDays[year, month, day] = (from key in _context.Key
                //                                               where key.OrderDate.Year == (year + 2018) && key.OrderDate.Month == (month + 1) && key.OrderDate.Day == (day + 1)
                //                                               select key.Price).ToList();
                //        }
                //    }
                //}

                //// poging om sneller te maken van de laad tijd
                AllOrdersDaysYear1Jan = new List<Key>[31];
                AllOrdersSalesDaysYear1Jan = new List<float>[31];
                //AllOrdersDaysYear1Feb       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Feb  = new List<float>[31];
                //AllOrdersDaysYear1Mar       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Mar  = new List<float>[31];
                //AllOrdersDaysYear1Apr        = new List<Key>[31];
                //AllOrdersSalesDaysYear1Apr  = new List<float>[31];
                //AllOrdersDaysYear1May        = new List<Key>[31];
                //AllOrdersSalesDaysYear1May  = new List<float>[31];
                //AllOrdersDaysYear1Jun        = new List<Key>[31];
                //AllOrdersSalesDaysYear1Jun  = new List<float>[31];
                //AllOrdersDaysYear1Jul       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Jul  = new List<float>[31];
                //AllOrdersDaysYear1Aug       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Aug  = new List<float>[31];
                //AllOrdersDaysYear1Sep       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Sep  = new List<float>[31];
                //AllOrdersDaysYear1Okt        = new List<Key>[31];
                //AllOrdersSalesDaysYear1Okt  = new List<float>[31];
                //AllOrdersDaysYear1Nov       = new List<Key>[31];
                //AllOrdersSalesDaysYear1Nov  = new List<float>[31];
                //AllOrdersDaysYear1Dec        = new List<Key>[31];
                //AllOrdersSalesDaysYear1Dec  = new List<float>[31];

                //AllOrdersDaysYear2Jan = new List<Key>[31];
                //AllOrdersSalesDaysYear2Jan = new List<float>[31];
                //AllOrdersDaysYear2Feb = new List<Key>[31];
                //AllOrdersSalesDaysYear2Feb = new List<float>[31];
                //AllOrdersDaysYear2Mar = new List<Key>[31];
                //AllOrdersSalesDaysYear2Mar = new List<float>[31];
                //AllOrdersDaysYear2Apr = new List<Key>[31];
                //AllOrdersSalesDaysYear2Apr = new List<float>[31];
                //AllOrdersDaysYear2May = new List<Key>[31];
                //AllOrdersSalesDaysYear2May = new List<float>[31];
                //AllOrdersDaysYear2Jun = new List<Key>[31];
                //AllOrdersSalesDaysYear2Jun = new List<float>[31];
                //AllOrdersDaysYear2Jul = new List<Key>[31];
                //AllOrdersSalesDaysYear2Jul = new List<float>[31];
                //AllOrdersDaysYear2Aug = new List<Key>[31];
                //AllOrdersSalesDaysYear2Aug = new List<float>[31];
                //AllOrdersDaysYear2Sep = new List<Key>[31];
                //AllOrdersSalesDaysYear2Sep = new List<float>[31];
                //AllOrdersDaysYear2Okt = new List<Key>[31];
                //AllOrdersSalesDaysYear2Okt = new List<float>[31];
                //AllOrdersDaysYear2Nov = new List<Key>[31];
                //AllOrdersSalesDaysYear2Nov = new List<float>[31];
                AllOrdersDaysYear2Dec = new List<Key>[31];
                AllOrdersSalesDaysYear2Dec = new List<float>[31];

                for (int day = 0; day < 31; day++)
                {
                    // January this year
                    AllOrdersDaysYear1Jan[day] = (from key in _context.Key
                                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 1 && key.OrderDate.Day == (day + 1)
                                                  select key).ToList();
                    AllOrdersSalesDaysYear1Jan[day] = (from key in _context.Key
                                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 1 && key.OrderDate.Day == (day + 1)
                                                       select key.Price).ToList();
                    //    // Februari this year
                    //    AllOrdersDaysYear1Feb[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 2 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Feb[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 2 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Maart this year
                    //    AllOrdersDaysYear1Mar[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 3 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Mar[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 3 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // April this year
                    //    AllOrdersDaysYear1Apr[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 4 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Apr[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 4 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Mei this year
                    //    AllOrdersDaysYear1May[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 5 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1May[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 5 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Juni this year
                    //    AllOrdersDaysYear1Jun[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 6 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Jun[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 6 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Juli this year
                    //    AllOrdersDaysYear1Jul[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 7 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Jul[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 7 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Augustus this year
                    //    AllOrdersDaysYear1Aug[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 8 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Aug[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 8 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // September this year
                    //    AllOrdersDaysYear1Sep[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 9 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Sep[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 9 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // Oktober this year
                    //    AllOrdersDaysYear1Okt[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 10 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Okt[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 10 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // November this year
                    //    AllOrdersDaysYear1Nov[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 11 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Nov[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 11 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    // December this year
                    //    AllOrdersDaysYear1Dec[day] = (from key in _context.Key
                    //                                  where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 12 && key.OrderDate.Day == (day + 1)
                    //                                  select key).ToList();
                    //    AllOrdersSalesDaysYear1Dec[day] = (from key in _context.Key
                    //                                       where key.OrderDate.Year == DateTime.Now.Year && key.OrderDate.Month == 12 && key.OrderDate.Day == (day + 1)
                    //                                       select key.Price).ToList();
                    //    //// January last year
                    //    //AllOrdersDaysYear2Jan[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 1 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Jan[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 1 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Februari last year
                    //    //AllOrdersDaysYear2Feb[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 2 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Feb[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 2 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Maart last year
                    //    //AllOrdersDaysYear2Mar[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 3 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Mar[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 3 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// April last year
                    //    //AllOrdersDaysYear2Apr[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 4 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Apr[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 4 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Mei last year
                    //    //AllOrdersDaysYear2May[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 5 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2May[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 5 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Juni last year
                    //    //AllOrdersDaysYear2Jun[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 6 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Jun[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 6 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Juli last year
                    //    //AllOrdersDaysYear2Jul[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 7 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Jul[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 7 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Augustus last year
                    //    //AllOrdersDaysYear2Aug[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 8 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Aug[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 8 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// September last year
                    //    //AllOrdersDaysYear2Sep[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 9 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Sep[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 9 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// Oktober last year
                    //    //AllOrdersDaysYear2Okt[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 10 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Okt[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 10 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    //    //// November last year
                    //    //AllOrdersDaysYear2Nov[day] = (from key in _context.Key
                    //    //                              where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 11 && key.OrderDate.Day == (day + 1)
                    //    //                              select key).ToList();
                    //    //AllOrdersSalesDaysYear2Nov[day] = (from key in _context.Key
                    //    //                                   where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 11 && key.OrderDate.Day == (day + 1)
                    //    //                                   select key.Price).ToList();
                    // December last year
                    AllOrdersDaysYear2Dec[day] = (from key in _context.Key
                                                  where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 12 && key.OrderDate.Day == (day + 1)
                                                  select key).ToList();
                    AllOrdersSalesDaysYear2Dec[day] = (from key in _context.Key
                                                       where key.OrderDate.Year == DateTime.Now.AddYears(-1).Year && key.OrderDate.Month == 12 && key.OrderDate.Day == (day + 1)
                                                       select key.Price).ToList();
                }
                // einde van de OnGet
            }
        }
    }
}