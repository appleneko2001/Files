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
            if(cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;

            if (v < BYTE_SIZE)
                return $"{v.ToString(cultureInfo)} Bytes";
            else if (v < KIB_SIZE)
                return $"{Math.Round(v / (float)BYTE_SIZE, 2).ToString(cultureInfo)} KiB";
            else if (v < MIB_SIZE)
                return $"{Math.Round(v / (float)KIB_SIZE, 2).ToString(cultureInfo)} MiB";
            else if (v < GIB_SIZE)
                return $"{Math.Round(v / (float)MIB_SIZE, 2).ToString(cultureInfo)} GiB";
            else
                return $"{Math.Round(v / (float)GIB_SIZE, 2).ToString(cultureInfo)} TiB";
        }
        
        public static string HumanizeSizeString(this long v, CultureInfo? cultureInfo = null)
        {
            if(cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;
            
            if (v < (long)BYTE_SIZE)
                return $"{v.ToString(cultureInfo)} Bytes";
            else if (v < (long)KIB_SIZE)
                return $"{Math.Round(v / (float)BYTE_SIZE, 2).ToString(cultureInfo)} KiB";
            else if (v < (long)MIB_SIZE)
                return $"{Math.Round(v / (float)KIB_SIZE, 2).ToString(cultureInfo)} MiB";
            else if (v < (long)GIB_SIZE)
                return $"{Math.Round(v / (float)MIB_SIZE, 2).ToString(cultureInfo)} GiB";
            else
                return $"{Math.Round(v / (float)GIB_SIZE, 2).ToString(cultureInfo)} TiB";
        }
    }
}