namespace Files.Adb.Extensions
{
    public static class StringExtensions
    {
        public static string AppendParam(this string s, string k, string v) => s.Replace("{" + k +"}", v);
    }
}