using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Soda.Http.Extensions;

public static class SodaHttpExtensions
{
    public static IServiceCollection AddSodaHttp(this IServiceCollection services, Action<SodaHttpOption> options)
    {
        var option = new SodaHttpOption();

        options.Invoke(option);

        return services;
    }
}

public class SodaLocator
{
    public static void Instance()
    {
    }
}

public class SodaHttpOption : IOptions<SodaHttpOption>
{
    public SodaHttpOption Value => this;
}