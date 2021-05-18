namespace Avalier.Busk
{
    public static class Url
    {
        public static string Combine(string endpoint, string virtualPath)
        {
            var url = endpoint;
            if (!url.EndsWith("/")) url += "/";
            url += virtualPath;
            return url;
        }
    }
}