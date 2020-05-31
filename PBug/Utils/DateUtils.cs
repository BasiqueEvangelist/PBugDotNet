using System.Globalization;
using System;
namespace PBug.Utils
{
    public static class DateUtils
    {
        public static string StringDate(DateTime? dt) => (dt?.ToLocalTime())?.ToString(CultureInfo.GetCultureInfo("ru-RU"));
    }
}