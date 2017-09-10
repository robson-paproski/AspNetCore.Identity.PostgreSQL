using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.PostgreSQL
{
    public static class StringExtension
    {
        public static string Quoted(this string str)
        {
            return "\"" + str + "\"";
        }
    }
}
