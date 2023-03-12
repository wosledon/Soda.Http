using System;
using Microsoft.Extensions.DependencyInjection;

namespace Soda.Http.Extensions;

public static class SodaHttpExtensions
{
    public static IServiceCollection AddSodaHttp(this IServiceCollection services, Action<SodaHttpOption?> options)
    {
        SodaHttpOption? option = null;

        options.Invoke(option);

        if (option is not null) SodaLocator.Instance(option);

        return services;
    }
}