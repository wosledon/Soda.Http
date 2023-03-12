using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace Soda.Http.Extensions;

public class SodaHttpOption : IOptions<SodaHttpOption>
{
    public AuthenticationHeaderValue? AuthenticationHeaderValue { get; set; }

    public bool EnableCompress { get; set; } = false;

    public string[]? Accept { get; set; } =
    {
        "application/json",
        "text/plain",
        "*/*"
    };

    public (string, string)[]? Headers { get; set; }

    public SodaHttpOption Value => this;
}