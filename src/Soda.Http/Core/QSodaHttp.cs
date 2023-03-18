using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soda.Http.Extensions;

namespace Soda.Http.Core
{
    public class QSodaHttp
    {
        public static SodaHttp Url(string url)
        {
            return SodaHttp.Create().Url(url);
        }

        public static SodaHttp Uri(string uri)
        {
            var baseUrl = SodaLocator.GetOption()?.Value?.BaseUrl;
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("没有配置 BaseUrl 不能使用 Uri模式");
            }

            return SodaHttp.Create().Url($"{baseUrl}{(baseUrl.EndsWith("/") || uri.StartsWith("/") ? "" : "/")}{uri}");
        }

        public static void InitAuthentication(string key, string value)
        {
            SodaLocator.InitAuthentication(key, value);
        }

        public static void AddSodaHttp(Action<SodaHttpOption?> options)
        {
            SodaLocator.AddSodaHttp(options);
        }

        public static void InitHeader(string key, string value)
        {
            SodaLocator.InitHeader(key, value);
        }
    }
}