namespace AspNet.Identity.PostgreSQL
{
    public static class StringExtension
    {
        public static string Quoted(this string str)
        {
            return "\"" + str + "\"";
        }
    }
}
