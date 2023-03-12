namespace Soda.Http.Extensions;

internal class SodaLocator
{
    private static SodaHttpOption? _option;

    internal static void Instance(SodaHttpOption option)
    {
        _option = option;
    }
}