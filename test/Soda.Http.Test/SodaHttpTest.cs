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

            var services = new ServiceCollection();
            services.AddSodaHttp(opts =>
            {
                opts.EnableCompress = false;
                opts.BaseUrl = "http://localhost:5050/Test";
            });
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
            var result = await QSodaHttp.Url("https://www.baidu.com/")
                .Header("X-Ca-Key", "XXX")
                .Authentication("Bearer", "XXX")
                .Params(new { Id = "123456" })
                .Body(new { })
                // .Form(...)
                // .File(...)
                .PostAsync<string>();

            Assert.NotNull(result);
        }

        [Fact]
        public async void Get()
        {
            var res = await QSodaHttp.Uri("Get").Params(new { Id = "123456" }).GetAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void GetResult()
        {
            var res = await QSodaHttp.Uri("GetResult").Params(new { Id = "123456", Ids = new[] { "123", "456" } }).GetAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void Post()
        {
            var res = await QSodaHttp.Uri("Post").Body(new { Id = "123456", Ids = new[] { "123", "456" } }).PostAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void PostResult()
        {
            var res = await QSodaHttp.Uri("PostResult")
                .Params(new { Id = "123456", Ids = new[] { "123", "456" } })
                .Body(new { Id = "123456", Ids = new[] { "123", "456" } })
                .PostAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void DeleteTest()
        {
            var res = await QSodaHttp.Uri("Delete").Params(new { Id = "123456" }).DeleteAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void DeleteResult()
        {
            var res = await QSodaHttp.Uri("DeleteResult").Params(new { Id = "123456", Ids = new[] { "123", "456" } }).DeleteAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void Put()
        {
            var res = await QSodaHttp.Uri("Put").Params(new { Id = "123456" }).PutAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void PutResult()
        {
            var res = await QSodaHttp.Uri("PutResult")
                .Body(new { Id = "123456", Ids = new[] { "123", "456" } })
                .PutAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void Patch()
        {
            var res = await QSodaHttp.Uri("Patch").Params(new { Id = "123456" }).PatchAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }

        [Fact]
        public async void PatchResult()
        {
            var res = await QSodaHttp.Uri("PatchResult")
                .Body(new { Id = "123456", Ids = new[] { "123", "456" } })
                .PatchAsync<object>();

            _testOutputHelper.WriteLine(res?.ToJson());
        }
    }
}