using Microsoft.Extensions.Options;

namespace Soda.Http.Extensions;

public class SodaHttpOption : IOptions<SodaHttpOption>
{
    public SodaHttpOption Value => this;
}