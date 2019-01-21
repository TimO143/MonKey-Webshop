using System;
using System.Collections;
using System.Collections.Generic;
using WebApplication1.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using WebApplication1.Resource.Pagination;
using WebApplication1.Resource.Option;

namespace WebApplication1.Searchengine
{
    /// <summary>
    /// Class die het zoeken via regelt op basis van punten.
    /// </summary>
    public class Searchbar
    {
        // Verbinding met database _context
        private readonly ApplicationDbContext _context;

        // Constructor die de verbinding ophaalt uit de database.
        public Searchbar(ApplicationDbContext context)
        {
            this._context = context;
        }

        private string[] stringsplitter(string searchquery) => searchquery.Split(' ');

        /// <summary>
        /// Deze functie haalt alle duplicate response name uit de lijst die getoond word.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool FilterDuplicates(List<Product_search> results, Product result)
        {
            bool check_item_exists = false;
            List<bool> check_items_exists = new List<bool>();
            if (results.Count != 0)
            {
                foreach (Product_search item in results)
                {
                    check_items_exists.Add(item.ResponseName == result.ResponseName);
                }

                if (!check_items_exists.Contains(true))
                {
                    check_item_exists = true;
                }
            }
            else
            {
                check_item_exists = true;
            }

            return check_item_exists;
        }

        /// <summary>
        /// Filter all the stop words out of the search filter.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private SearchQuery[] FilterStopWords(SearchQuery[] words)
        {
            List<SearchQuery> listWords = new List<SearchQuery>();

            foreach (SearchQuery word in words)
            {
                // Filteren van de stop woorden.
                int k = 0;

                // Filter alle nummers als niet querable.
                double num;
                if (double.TryParse(word.search_query, out num)) words[k].queryable = false;

                if (word.search_query == "the") words[k].search_query = "";
                if (word.search_query == "of") words[k].search_query = "";
                if (word.search_query == "de") words[k].search_query = "";
                if (word.search_query == "een") words[k].search_query = "";
                if (word.search_query == "het") words[k].search_query = "";
                if (word.search_query == "a") words[k].search_query = "";
                if (word.search_query == "an") words[k].search_query = "";
                if (word.search_query == "by") words[k].search_query = "";
                if (word.search_query == "to") words[k].search_query = "";
                if (word.search_query == "on") words[k].search_query = "";

                if (word.search_query != "")
                {
                    listWords.Add(word);
                }

                k++;
            }

            return listWords.ToArray();
        }

        /// <summary>
        /// Filter and order all the information
        /// </summary>
        /// <param name="descending"></param>
        /// <param name="results"></param>
        /// <param name="filter_by_selector"></param>
        /// <param name="order_by_selector"></param>
        private IOrderedEnumerable<Product_search> FilterAndOrderProductData(bool descending, List<Product_search> results, Func<Product_search, bool> filter_by_selector, Func<Product_search, object> order_by_selector)
        {
            IOrderedEnumerable<Product_search> array_results_tmp;
            // Zet de producten op volgorde van 
            if (descending)
            {
                array_results_tmp = results
                    .Where(filter_by_selector)
                    .OrderByDescending(order_by_selector);
            }
            else
            {
                array_results_tmp = results
                    .Where(filter_by_selector)
                    .OrderBy(order_by_selector);
            }

            return array_results_tmp;
        }

        /// <summary>
        /// De main zoek functie.
        /// </summary>
        /// <param name="searchquery"></param>
        /// <param name="page_size"></param>
        /// <param name="Page_index"></param>
        /// <returns></returns>
        public Option<Search_Page<Product_search>> search(string searchquery, int page_size, int Page_index, Func<Product_search, object> order_by_selector, Func<Product_search, bool> filter_by_selector, bool descending)
        {
            if (searchquery == null || searchquery == "")
            {
                return new Empty<Search_Page<Product_search>>();
            }
            // Maak de zoek item tekst kleiner.
            searchquery = searchquery.ToLower();

            // Split de zoek term op in woorden.
            string[] words_count = this.stringsplitter(searchquery);
            SearchQuery[] words = new SearchQuery[words_count.Length];

            for (int i = 0; i < words_count.Length; i++)
            {
                words[i] = new SearchQuery();
                words[i].search_query = words_count[i];
            }


            // Filteren van de stop woorden.
            words = FilterStopWords(words);

            // Zoek alle producten op die in een word bevat dat in de title staat.
            List<IQueryable> result_query_list = new List<IQueryable>();
            List<Product> results_query = new List<Product> { };
            foreach (SearchQuery word in words)
            {
                // Als de word veld niet leeg is of queryable is dan wordt er gezocht op dat word.
                if (word.queryable)
                {
                    var query = from p in _context.Product
                        where p.QueryName.Contains(word.search_query)
                        select p;

                    result_query_list.Add(query);
                }
            }

            // IQueryable to list
            foreach (IQueryable query in result_query_list)
            {
                foreach (Product item in query)
                {
                    if (words.All(word => item.QueryName.Contains(word.search_query)))
                    {
                        results_query.Add(item);
                    }
                }
            }

            // Maak de punten telling voor array.
            List<Product_search> results = new List<Product_search>();
            foreach (Product result in results_query)
            {
                if (FilterDuplicates(results, result))
                {

                    Product_search final_result = new Product_search(result);
                    final_result.points = 5;
                    string[] queryname = this.stringsplitter(final_result.QueryName);

                    // Controleert hoevaak een wordt voorkomt in een zin.
                    var matchQuery = from word in words
                        from name in queryname
                        where name == word.search_query
                        select word;

                    // Bepaalt het aantal punten opbasis van het aantal overeenkomende worden.
                    final_result.points = final_result.points + matchQuery.Count() * 3000;

                    // Bepaalt het aantal punten op basis van het aantal recommendations.
                    final_result.points = final_result.points + result.RecommendationCount / 1000;

                    if (result.PlatformLinux) final_result.points = final_result.points + 15;
                    if (result.PlatformMac) final_result.points = final_result.points + 15;
                    if (result.PlatformWindows) final_result.points = final_result.points + 50;

                    results.Add(final_result);
                }
            }

            // Order and filter all the list.
            IOrderedEnumerable<Product_search> array_results_tmp = FilterAndOrderProductData(descending, results, filter_by_selector, order_by_selector);

            Product_search[] array_results = array_results_tmp
                                                .Skip(Page_index * page_size)
                                                .Take(page_size)
                                                .ToArray();

            var tot_items = array_results_tmp.Count();
            var tot_pages = tot_items / page_size;


            // Return een pagina met een array van product_search
            if (array_results.Length <= 0)
            {
                return new Empty<Search_Page<Product_search>>();
            }
            return new Some<Search_Page<Product_search>>() { data = new Search_Page<Product_search>() {Index = Page_index, Items = array_results, TotalPages = tot_pages, SearchQuery = searchquery}};
        }
    }

    /// <summary>
    /// Class die de zoek worden bijhoud en bijhoud of deze opgezocht mogen worden of niet.
    /// </summary>
    public class SearchQuery
    {
        public SearchQuery()
        {
            queryable = true;
        }
        public string search_query { get; set; }
        public bool queryable { get; set; }
    }

    /// <summary>
    /// Class die extends van page
    /// Deze onthoud ook nog de searchquery die is gebruikt om tot deze pagina te komen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Search_Page<T> : Page<T>
    {
        public string SearchQuery { get; set; }
    }

    /// <summary>
    /// Class extends van van product.
    /// Heeft als extra attribute points voor het odreren van de zoek query.
    /// </summary>
    public class Product_search : Product
    {
        public Product_search(Product product)
        {
            ID = product.ID;
            QueryID = product.QueryID;
            ResponseID = product.ResponseID;
            QueryName = product.QueryName;
            ResponseName = product.ResponseName;
            ReleaseDate = product.ReleaseDate;
            RequiredAge = product.RequiredAge;
            DemoCount = product.DemoCount;
            DeveloperCount = product.DeveloperCount;
            DLCCount = product.DLCCount;
            Metacritic = product.Metacritic;
            MovieCount = product.MovieCount;
            PackageCount = product.PackageCount;
            RecommendationCount = product.RecommendationCount;
            PublisherCount = product.PublisherCount;
            ScreenshotCount = product.ScreenshotCount;
            SteamSpyOwners = product.SteamSpyOwners;
            SteamSpyOwnersVariance = product.SteamSpyOwnersVariance;
            SteamSpyPlayersEstimate = product.SteamSpyPlayersEstimate;
            SteamSpyPlayersVariance = product.SteamSpyPlayersVariance;
            AchievementCount = product.AchievementCount;
            AchievementHighlightedCount = product.AchievementHighlightedCount;
            ControllerSupport = product.ControllerSupport;
            IsFree = product.IsFree;
            FreeVerAvail = product.FreeVerAvail;
            PurchaseAvail = product.PurchaseAvail;
            SubscriptionAvail = product.SubscriptionAvail;
            PlatformWindows = product.PlatformLinux;
            PlatformLinux = product.PlatformLinux;
            PlatformMac = product.PlatformMac;
            PCReqsHaveMin = product.PCReqsHaveMin;
            PCReqsHaveRec = product.PCReqsHaveRec;
            LinuxReqsHaveMin = product.LinuxReqsHaveMin;
            LinuxReqsHaveRec = product.LinuxReqsHaveRec;
            MacReqsHaveMin = product.MacReqsHaveMin;
            MacReqsHaveRec = product.MacReqsHaveRec;
            CategorySinglePlayer = product.CategorySinglePlayer;
            CategoryMultiplayer = product.CategoryMultiplayer;
            CategoryCoop = product.CategoryCoop;
            CategoryMMO = product.CategoryMMO;
            CategoryInAppPurchase = product.CategoryInAppPurchase;
            CategoryIncludeSrcSDK = product.CategoryIncludeSrcSDK;
            CategoryIncludeLevelEditor = product.CategoryIncludeLevelEditor;
            CategoryVRSupport = product.CategoryVRSupport;
            GenreIsNonGame = product.GenreIsNonGame;
            GenreIsIndie = product.GenreIsIndie;
            GenreIsAction = product.GenreIsAction;
            GenreIsAdventure = product.GenreIsAdventure;
            GenreIsCasual = product.GenreIsCasual;
            GenreIsStrategy = product.GenreIsStrategy;
            GenreIsRPG = product.GenreIsRPG;
            GenreIsSimulation = product.GenreIsSimulation;
            GenreIsEarlyAccess = product.GenreIsEarlyAccess;
            GenreIsFreeToPlay = product.GenreIsFreeToPlay;
            GenreIsSports = product.GenreIsSports;
            GenreIsRacing = product.GenreIsRacing;
            GenreIsMassivelyMultiplayer = product.GenreIsMassivelyMultiplayer;
            PriceCurrency = product.PriceCurrency;
            PriceInitial = product.PriceInitial;
            PriceFinal = product.PriceFinal;
            SupportEmail = product.SupportEmail;
            SupportURL = product.SupportURL;
            AboutText = product.AboutText;
            Background = product.Background;
            ShortDescrip = product.ShortDescrip;
            DetailedDescrip = product.DetailedDescrip;
            DRMNotice = product.DRMNotice;
            ExtUserAcctNotice = product.ExtUserAcctNotice;
            HeaderImage = product.HeaderImage;
            LegalNotice = product.LegalNotice;
            Reviews = product.Reviews;
            SupportedLanguages = product.SupportedLanguages;
            Website = product.Website;
            PCMinReqsText = product.PCMinReqsText;
            PCRecReqsText = product.PCRecReqsText;
            LinuxMinReqsText = product.LinuxMinReqsText;
            LinuxRecReqsText = product.LinuxRecReqsText;
            MacRecReqsText = product.MacRecReqsText;
            MacMinReqsText = product.MacMinReqsText;
        }

        // Bewaart het aantal punten van de producten
        public int points;
    }
}