using System;
using System.Globalization;

namespace Files.Extensions
{
    public static class Int64Extensions
    {
        public const ulong BYTE_SIZE = 1024;
        public const ulong KIB_SIZE = BYTE_SIZE * BYTE_SIZE;
        public const ulong MIB_SIZE = BYTE_SIZE * BYTE_SIZE * BYTE_SIZE;
        public const ulong GIB_SIZE = BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE;
        public const ulong TIB_SIZE = BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE;
        public const ulong PIB_SIZE = BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE * BYTE_SIZE;
        
        public static string HumanizeSizeString(this ulong v, CultureInfo? cultureInfo = null)
        {
            cultureInfo ??= CultureInfo.InvariantCulture;

            return v switch
            {
                < BYTE_SIZE => $"{v.ToString(cultureInfo)} Bytes",
                < KIB_SIZE => $"{Math.Round(v / (float) BYTE_SIZE, 2).ToString(cultureInfo)} KiB",
                < MIB_SIZE => $"{Math.Round(v / (float) KIB_SIZE, 2).ToString(cultureInfo)} MiB",
                < GIB_SIZE => $"{Math.Round(v / (float) MIB_SIZE, 2).ToString(cultureInfo)} GiB",
                < TIB_SIZE => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} TiB",
                < PIB_SIZE => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} PiB",
                _ => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} EiB"
            };
        }
        
        public static string HumanizeSizeString(this long v, CultureInfo? cultureInfo = null)
        {
            cultureInfo ??= CultureInfo.InvariantCulture;

            return v switch
            {
                < (long) BYTE_SIZE => $"{v.ToString(cultureInfo)} Bytes",
                < (long) KIB_SIZE => $"{Math.Round(v / (float) BYTE_SIZE, 2).ToString(cultureInfo)} KiB",
                < (long) MIB_SIZE => $"{Math.Round(v / (float) KIB_SIZE, 2).ToString(cultureInfo)} MiB",
                < (long) GIB_SIZE => $"{Math.Round(v / (float) MIB_SIZE, 2).ToString(cultureInfo)} GiB",
                < (long) TIB_SIZE => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} TiB",
                < (long) PIB_SIZE => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} PiB",
                _ => $"{Math.Round(v / (float) GIB_SIZE, 2).ToString(cultureInfo)} EiB"
            };
        }
    }
}