using System.Collections.Generic;
using System.Text;

namespace Soda.Http.Extensions;

public static class CommonExtensions
{
    public static Dictionary<string, object?> GetParameters(this object parameters)
    {
        var result = new Dictionary<string, object?>();

        var properties = parameters.GetType().GetProperties();

        foreach (var propertyInfo in properties)
        {
            result.TryAdd(propertyInfo.Name, propertyInfo.GetValue(parameters));
        }

        return result;
    }

    public static string ToUrl(this Dictionary<string, object?> dict, string? url)
    {
        var sb = new StringBuilder();
        foreach (var kvp in dict)
        {
            sb.Append($"&{kvp.Key}={kvp.Value}");
        }

        return $"{url}?{sb.ToString().TrimStart('&')}";
    }
}