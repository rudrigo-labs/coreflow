using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Extensions
{
    public static class UriExtensions
    {
        public static Uri? ConvertToUri(this string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : null;
        }

        public static bool IsValidUri(this string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}

