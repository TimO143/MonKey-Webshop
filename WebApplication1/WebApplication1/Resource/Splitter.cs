using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;

namespace WebApplication1.Resource
{
    public static class Splitter
    {
        /// <summary>
        /// Can split an string into mulltiple strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitcharacter"></param>
        /// <returns>String[]</returns>
        public static string[] stringsplitter(this string text, string splitcharacter)
        {
            return text.Split(splitcharacter);
        } 
    }
}