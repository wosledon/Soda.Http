using System;

namespace Soda.Http.Extensions;

internal class SodaLocator
{
    private static SodaHttpOption? _option;

    internal static void Instance(SodaHttpOption option)
    {
        _option = option;
    }

    internal static void InitAuthentication(string key, string value)
    {
        if (_option is null) _option = new();

        _option.AuthenticationHeaderValue = new System.Net.Http.Headers.AuthenticationHeaderValue(key, value);
    }

    internal static void AddSodaHttp(Action<SodaHttpOption?> options)
    {
        SodaHttpOption? option = null;

        options.Invoke(option);

        if (option is not null) SodaLocator.Instance(option);
    }

    internal static SodaHttpOption? GetOption()
    {
        return _option;
    }
}