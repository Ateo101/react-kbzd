using System.Globalization;

namespace CrimeEmpire.Economy
{
    public static class NumberFormat
    {
        static readonly string[] Suffixes = { "", "k", "M", "B", "T" };

        /// <summary>1070 -> "1070", 15500 -> "15.5k", 123500 -> "123.5k", 2000000 -> "2M".</summary>
        public static string Short(long value)
        {
            if (value < 0) return "-" + Short(-value);
            if (value < 10_000) return value.ToString(CultureInfo.InvariantCulture);

            long divisor = 1;
            int suffix = 0;
            while (suffix < Suffixes.Length - 1 && value / divisor >= 1000)
            {
                divisor *= 1000;
                suffix++;
            }

            // Integer math, truncated: 15_599 shows 15.5k, never more than the player has.
            long tenths = value / (divisor / 10);
            long whole = tenths / 10;
            long fraction = tenths % 10;
            return fraction == 0
                ? $"{whole.ToString(CultureInfo.InvariantCulture)}{Suffixes[suffix]}"
                : $"{whole.ToString(CultureInfo.InvariantCulture)}.{fraction}{Suffixes[suffix]}";
        }
    }
}
