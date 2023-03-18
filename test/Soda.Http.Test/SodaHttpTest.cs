using System.Net.Http.Headers;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;
using Soda.Http.Core;
using Soda.Http.Extensions;
using Xunit.Abstractions;

namespace Soda.Http.Test
{
    public class SodaHttpTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SodaHttpTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test1()
        {
            var result = await QSodaHttp.Url("https://www.baidu.com/").GetAsync<string>();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Test2()
        {
            var services = new ServiceCollection();
            services.AddSodaHttp(opts =>
            {
                opts.AuthenticationHeaderValue = new AuthenticationHeaderValue("Bearer", "AuthKey");
                opts.EnableCompress = false;
                opts.BaseUrl = "https://www.baidu.com/";
            });

            var result = await QSodaHttp.Uri("/api/test")
                .Params(new
                {
                    Id = "123456",
                    Values = new[]
                    {
                        "111","222"
                    }
                })
                .GetAsync<string>();

            _testOutputHelper.WriteLine(result);

            Assert.NotNull(result);
        }
    }
}