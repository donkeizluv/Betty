using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Betty.Helper
{
    public static class StringHelper
    {
        public static bool TryParseDivideOperator(string s, out decimal f)
        {
            f = default(decimal);
            if(string.IsNullOrEmpty(s)) return false;
            var split = RemoveTwoContinousSplace(s.Trim()).Split(' ');
            if(split.Count() == 2)
            {
                if(!TryDivideOperator(split.First().Trim(), out var f1))
                    return false;
                if(!TryDivideOperator(split.Last().Trim(), out var f2))
                    return false;                    
                f = f1 + f2;
                return true;
            }
            return TryDivideOperator(s, out f);
        }
        public static bool TryDivideOperator(string s, out decimal f)
        {
            f = default(decimal);
            if(string.IsNullOrEmpty(s)) return false;
            var ss = s.Replace(" ", string.Empty);
            if(ss.Contains("/"))
            {
                var split = ss.Split('/');
                if(split.Count() != 2) return false;
                if(!decimal.TryParse(split.First(), out var f1))
                    return false;
                if(!decimal.TryParse(split.Last(), out var f2))
                    return false;
                f = f1 / f2;
                return true;
            }
            return decimal.TryParse(ss, out f);
        }
        public static string RemoveTwoContinousSplace(string text)
        {
            if (text == null) return null;
            return Regex.Replace(text, @"[ \t]{2,}", string.Empty);
        }
        public static bool Isdecimal(string s)
        {
            if(string.IsNullOrEmpty(s)) return false;
            if(decimal.TryParse(CleanInnerText(s), out var f))
                return true;
            return false;
        }
        public static string CleanInnerText(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            return s.Replace("&nbsp;", " ").
                Replace("\r\n", string.Empty).
                Replace("\r", string.Empty).
                Replace("\n", string.Empty).Trim();
        }
    }
}
