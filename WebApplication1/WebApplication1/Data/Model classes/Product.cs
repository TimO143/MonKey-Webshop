﻿using System.Collections.Generic;

namespace WebApplication1.Data
{
    public class Product
    {
        public int ID { get; set; }
        public int QueryID { get; set; }
        public int ResponseID { get; set; }
        public string QueryName { get; set; }
        public string ResponseName { get; set; }
        public string ReleaseDate { get; set; }
        public int RequiredAge { get; set; }
        public int DemoCount { get; set; }
        public int DeveloperCount { get; set; }
        public int DLCCount { get; set; }
        public int Metacritic { get; set; }
        public int MovieCount { get; set; }
        public int PackageCount { get; set; }
        public int RecommendationCount { get; set; }
        public int PublisherCount { get; set; }
        public int ScreenshotCount { get; set; }
        public int SteamSpyOwners { get; set; }
        public int SteamSpyOwnersVariance { get; set; }
        public int SteamSpyPlayersEstimate { get; set; }
        public int SteamSpyPlayersVariance { get; set; }
        public int AchievementCount { get; set; }
        public int AchievementHighlightedCount { get; set; }
        public bool ControllerSupport { get; set; }
        public bool IsFree { get; set; }
        public bool FreeVerAvail { get; set; }
        public bool PurchaseAvail { get; set; }
        public bool SubscriptionAvail { get; set; }
        public bool PlatformWindows { get; set; }
        public bool PlatformLinux { get; set; }
        public bool PlatformMac { get; set; }
        public bool PCReqsHaveMin { get; set; }
        public bool PCReqsHaveRec { get; set; }
        public bool LinuxReqsHaveMin { get; set; }
        public bool LinuxReqsHaveRec { get; set; }
        public bool MacReqsHaveMin { get; set; }
        public bool MacReqsHaveRec { get; set; }
        public bool CategorySinglePlayer { get; set; }
        public bool CategoryMultiplayer { get; set; }
        public bool CategoryCoop { get; set; }
        public bool CategoryMMO { get; set; }
        public bool CategoryInAppPurchase { get; set; }
        public bool CategoryIncludeSrcSDK { get; set; }
        public bool CategoryIncludeLevelEditor { get; set; }
        public bool CategoryVRSupport { get; set; }
        public bool GenreIsNonGame { get; set; }
        public bool GenreIsIndie { get; set; }
        public bool GenreIsAction { get; set; }
        public bool GenreIsAdventure { get; set; }
        public bool GenreIsCasual { get; set; }
        public bool GenreIsStrategy { get; set; }
        public bool GenreIsRPG { get; set; }
        public bool GenreIsSimulation { get; set; }
        public bool GenreIsEarlyAccess { get; set; }
        public bool GenreIsFreeToPlay { get; set; }
        public bool GenreIsSports { get; set; }
        public bool GenreIsRacing { get; set; }
        public bool GenreIsMassivelyMultiplayer { get; set; }
        public string PriceCurrency { get; set; }
        public float PriceInitial { get; set; }
        public float PriceFinal { get; set; }
        public string SupportEmail { get; set; }
        public string SupportURL { get; set; }
        public string AboutText { get; set; }
        public string Background { get; set; }
        public string ShortDescrip { get; set; }
        public string DetailedDescrip { get; set; }
        public string DRMNotice { get; set; }
        public string ExtUserAcctNotice { get; set; }
        public string HeaderImage { get; set; }
        public string LegalNotice { get; set; }
        public string Reviews { get; set; }
        public string SupportedLanguages { get; set; }
        public string Website { get; set; }
        public string PCMinReqsText { get; set; }
        public string PCRecReqsText { get; set; }
        public string LinuxMinReqsText { get; set; }
        public string LinuxRecReqsText { get; set; }
        public string MacMinReqsText { get; set; }
        public string MacRecReqsText { get; set; }
        public List<Key> Keys { get; set; }
        public List<User_Wishlist> Wishlists { get; set; }
        public List<Shopping_card_Product> ShoppingCardProducts { get; set; }
        public List<Shopping_card> ShoppingCart { get; set; }
    }
}