using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Soda.Http.Exceptions;
using Soda.Http.Extensions;

namespace Soda.Http.Core
{
    public partial class SodaHttp
    {
        private static readonly HttpClientHandler HttpClientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            MaxAutomaticRedirections = 5,
        };

        private static readonly HttpClient HttpClientInner = new HttpClient(HttpClientHandler);

        private static readonly MediaTypeHeaderValue DefaultMediaType = new MediaTypeHeaderValue("application/json");

        private static readonly string[] DefaultAccept =
        {
            "application/json",
            "text/plain",
            "*/*"
        };

        private static SodaHttpOption? Option => SodaLocator.GetOption()?.Value;

        private static readonly string[] DefaultAcceptEncoding = { "gzip", "deflate" };

        static SodaHttp()
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 200;
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                HttpClientInner.Timeout = TimeSpan.FromSeconds(60);
                _ = HttpClientInner.GetAsync("http://127.0.0.1").Result;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 启用请求内容gzip压缩 自动使用gzip压缩body并设置Content-Encoding为gzip
        /// </summary>
        public static bool EnableCompress { get; set; } = false;

        private string? _url;
        private HttpContent? _httpContent;
        private NameValueCollection? _headers;
        private MediaTypeHeaderValue _mediaType;
        private string[] _accept;
        private AuthenticationHeaderValue? _authenticationHeaderValue;

        private Dictionary<string, object?>? _params;

        protected SodaHttp()
        {
            _mediaType = DefaultMediaType;
            _accept = DefaultAccept;
        }

        public static SodaHttp Create()
        {
            return new SodaHttp();
        }

        protected SodaHttp Authentication(AuthenticationHeaderValue? authentication)
        {
            _authenticationHeaderValue = authentication;
            return this;
        }

        public SodaHttp Authentication(string scheme, string parameter)
        {
            _authenticationHeaderValue = new AuthenticationHeaderValue(scheme, parameter);
            return this;
        }

        /// <summary>
        /// 默认为 application/json, text/plain, */*
        /// </summary>
        /// <param name="accept"> </param>
        /// <returns> </returns>
        public SodaHttp Accept(string[] accept)
        {
            _accept = accept;
            return this;
        }

        public SodaHttp ContentType(string mediaType, string? charSet = null)
        {
            _mediaType = new MediaTypeHeaderValue(mediaType);
            if (!string.IsNullOrEmpty(charSet))
            {
                _mediaType.CharSet = charSet;
            }

            return this;
        }

        /// <summary>
        /// 默认为 gzip, deflate
        /// </summary>
        /// <returns> </returns>
        /// public HttpClientWrapper AcceptEncoding(string[] acceptEncoding) { //_acceptEncoding =
        /// acceptEncoding; return this; }
        public SodaHttp Url(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            _url = url;
            return this;
        }

        public SodaHttp Params(object parameters)
        {
            _params = parameters.GetParameters();

            return this;
        }

        public SodaHttp Body(object body)
        {
            return Body(Newtonsoft.Json.JsonConvert.SerializeObject(body));
        }

        public SodaHttp Body(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return this;
            }

            var sc = new StringContent(body);
            if ((Option?.EnableCompress ?? false) || EnableCompress)
            {
                _httpContent = new CompressedContent(sc, CompressedContent.CompressionMethod.GZip);
                sc.Headers.ContentEncoding.Add("gzip");
            }
            else
            {
                _httpContent = sc;
            }

            //sc.Headers.ContentLength = sc..Length;
            sc.Headers.ContentType = _mediaType;

            return this;
        }

        /// <summary>
        /// 仅支持POST和PUT
        /// </summary>
        /// <param name="path">     </param>
        /// <param name="name">     </param>
        /// <param name="filename"> </param>
        /// <returns> </returns>
        public SodaHttp File(string path, string name, string filename)
        {
            _httpContent ??= new MultipartFormDataContent();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var bac = new StreamContent(stream);
            ((MultipartFormDataContent)_httpContent).Add(bac, name, filename);

            return this;
        }

        /// <summary>
        /// 仅支持POST和PUT
        /// </summary>
        /// <param name="content"> </param>
        /// <param name="name">    </param>
        /// <returns> </returns>
        public SodaHttp File(string content, string name)
        {
            _httpContent ??= new MultipartFormDataContent();

            ((MultipartFormDataContent)_httpContent).Add(new StringContent(content), name);
            return this;
        }

        public SodaHttp Form(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            _httpContent ??= new FormUrlEncodedContent(nameValueCollection);

            return this;
        }

        public SodaHttp Headers(NameValueCollection headers)
        {
            CheckHeaderIsNull();

            foreach (string key in headers.Keys)
            {
                _headers?.Add(key, headers.Get(key));
            }

            return this;
        }

        private void CheckHeaderIsNull()
        {
            _headers ??= new NameValueCollection();
        }

        public SodaHttp Header(string key, string value)
        {
            CheckHeaderIsNull();
            _headers?.Add(key, value);
            return this;
        }

        public T GetData<T>(HttpResponseMessage res)
        {
            string? str = null;

            Task.Run(async () => str = await res.Content.ReadAsStringAsync()).Wait();

            if (IsStringOrDecimalOrPrimitiveType(typeof(T)))
            {
                return (T)Convert.ChangeType(str, typeof(T));
            }

            if (string.IsNullOrEmpty(str)) throw new InvalidOperationException("请求异常");

            return str.ToObject<T>()!;
        }

        protected async Task<T> RequestAsync<T>(HttpMethod method)
        {
            using var res = await GetOriginHttpResponse(method);
            return GetData<T>(res);
        }

        private bool IsStringOrDecimalOrPrimitiveType(Type t)
        {
            var typename = t.Name;
            return t.IsPrimitive || typename is "String" or "Decimal";
        }

        protected virtual async Task<HttpResponseMessage> GetOriginHttpResponse(HttpMethod method)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
            var sw = new Stopwatch();

            var realUrl = _params is null ? _url : _params.ToUrl(_url);
            try
            {
                using var requestMessage = new HttpRequestMessage(method, _url)!;
                switch (method.Method)
                {
                    case "PUT":
                    case "POST":
                        if (_httpContent != null)
                        {
                            requestMessage.Content = _httpContent;
                        }
                        break;
                }

                foreach (var acc in (Option?.Accept ?? _accept))
                {
                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acc));
                }

                foreach (var accenc in DefaultAcceptEncoding)
                {
                    requestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(accenc));
                }

                if (_authenticationHeaderValue != null)
                {
                    if (Option is not null && Option.AuthenticationHeaderValue is not null)
                    {
                        requestMessage.Headers.Authorization = Option.AuthenticationHeaderValue;
                    }

                    requestMessage.Headers.Authorization = _authenticationHeaderValue;
                }

                if (_headers != null)
                {
                    if (Option?.Headers is null)
                    {
                        foreach (var header in _headers.AllKeys)
                        {
                            requestMessage.Headers.Add(header, _headers.Get(header));
                        }
                    }
                    else
                    {
                        foreach (var header in Option.Headers)
                        {
                            requestMessage.Headers.Add(header.Item1, header.Item2);
                        }
                    }
                }

                sw.Start();
                HttpResponseMessage? res = await HttpClientInner.SendAsync(requestMessage);
                httpStatusCode = res!.StatusCode;
                sw.Stop();
                return res;
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                throw new SodaHttpException(e.Message);
            }
            finally
            {
                //Logger.Info($"{method.Method} {_url} {httpStatusCode} {sw.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public async Task<HttpResponseMessage> GetHttpResponseMessageAsync()
        {
            return await GetOriginHttpResponse(HttpMethod.Get);
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public HttpResponseMessage PostHttpResponseMessage()
        {
            return GetOriginHttpResponse(HttpMethod.Post).Result;
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public async Task<HttpResponseMessage> PostHttpResponseMessageAsync()
        {
            return await GetOriginHttpResponse(HttpMethod.Post);
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public HttpResponseMessage PutHttpResponseMessage()
        {
            return GetOriginHttpResponse(HttpMethod.Put).Result;
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public async Task<HttpResponseMessage> PutHttpResponseMessageAsync()
        {
            return await GetOriginHttpResponse(HttpMethod.Put);
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public HttpResponseMessage DeleteHttpResponseMessage()
        {
            return GetOriginHttpResponse(HttpMethod.Delete).Result;
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public async Task<HttpResponseMessage> DeleteHttpResponseMessageAsync()
        {
            return await GetOriginHttpResponse(HttpMethod.Delete);
        }

        /// <summary>
        /// 获取请求的 <see cref="HttpResponseMessage" /> 结果
        /// </summary>
        /// <returns> </returns>
        public HttpResponseMessage GetHttpResponseMessage()
        {
            return GetOriginHttpResponse(HttpMethod.Get).Result;
        }

        /// <summary>
        /// 自动将请求返回值反序列化为 <typeparam name="T"> </typeparam> 类型 若返回响应体无法反序列化则抛出异常
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public async Task<T> GetAsync<T>()
        {
            return await RequestAsync<T>(HttpMethod.Get);
        }

        [Obsolete("建议使用异步方法")]
        public T Get<T>()
        {
            return GetAsync<T>().Result;
        }

        public Task<T> PostAsync<T>()
        {
            return RequestAsync<T>(HttpMethod.Post);
        }

        [Obsolete("建议使用异步方法")]
        public T Post<T>()
        {
            return PostAsync<T>().Result;
        }

        public Task<T> PutAsync<T>()
        {
            return RequestAsync<T>(HttpMethod.Put);
        }

        [Obsolete("建议使用异步方法")]
        public T Put<T>()
        {
            return PutAsync<T>().Result;
        }

        public Task<T> DeleteAsync<T>()
        {
            return RequestAsync<T>(HttpMethod.Delete);
        }

        [Obsolete("建议使用异步方法")]
        public T Delete<T>()
        {
            return DeleteAsync<T>().Result;
        }

        public override string ToString()
        {
            return $"{_url}";
        }
    }
}