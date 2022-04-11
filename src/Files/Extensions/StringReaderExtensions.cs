using System.IO;

namespace Files.Extensions
{
    public static class StringReaderExtensions
    {
        public static bool GetNextChar(this StringReader reader, out char? c)
        {
            c = null;
                    
            var read = reader.Read();
            if (read == -1)
                return false;

            c = (char) read;
            return true;
        }
    }
}