using System;
using System.Collections.Specialized;
using Soda.Http.Core;

namespace Soda.Http;

public class Http
{
    public static SodaHttp Url(string url)
    {
        return SodaHttp.Create();
    }
}