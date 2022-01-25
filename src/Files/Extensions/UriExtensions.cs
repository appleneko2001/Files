using System;
using System.Collections.Generic;
using System.Web;

namespace Files.Extensions
{
    public static class UriExtensions
    {
        public static string[] GenerateSegments(this Uri uri)
        {
            var l = new List<string>
            {
                $"{uri.Scheme}{Uri.SchemeDelimiter}"
            };

            var hostPort = uri.GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped);
            if (!hostPort.EndsWith('/'))
                hostPort = $"{hostPort}/";
            
            l.Add(hostPort);

            var path = HttpUtility.UrlDecode(uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped));

            foreach (var part in path.Split('/'))
            {
                if(!string.IsNullOrWhiteSpace(part))
                    l.Add($"{part}/");
            }

            return l.ToArray();
        }
    }
}