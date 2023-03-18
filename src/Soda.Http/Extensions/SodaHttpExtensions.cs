using System;
using Microsoft.Extensions.DependencyInjection;

namespace Soda.Http.Extensions;

public static class SodaHttpExtensions
{
    public static IServiceCollection AddSodaHttp(this IServiceCollection services, Action<SodaHttpOption> options)
    {
        SodaHttpOption? option = new();

        options.Invoke(option);

        SodaLocator.Instance(option);

        return services;
    }
}