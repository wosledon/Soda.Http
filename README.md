# Soda.Http

基于`HttpClient`封装的 Http 请求库。

## 用法

### 1 预载

预载配置并不是必须的，但是有助于我们进行一些通用基础设置，例如Headers、Accept、BaseUrl等等。

在`AspNetCore`中：

```csharp
services.AddSodaHttp(opts =>
{
    opts.BaseUrl = "http://localhost:8080/";
    opts.Accept = new[]
    {
        "application/json",
        "text/plain",
        "*/*"
    };
    opts.EnableCompress = false;
    opts.Headers = new[]{
        ("X-Ca-Test", "key")
    };
});
```

较为通用的写法，程序构建时：

```csharp
QSodaHttp.AddSodaHttp(opts =>
{
    opts.BaseUrl = "http://localhost:8080/";
    opts.Accept = new[]
    {
        "application/json",
        "text/plain",
        "*/*"
    };
    opts.EnableCompress = false;
    opts.Headers = new[]{
        ("X-Ca-Test", "key")
    };
})
```

### 2 全局配置 Authentication

有时需要全局配置 Authentication，如果在代码中请求中独立配置了 Authentication 则会覆盖全局 Authentication

```csharp
QSodaHttp.InitAuthentication("Bearer", "Values");
```

如果你是塞到 Header 里的这种做法

```csharp
QSodaHttp.InitHeader("X-Ca-Key", "Values");
```

### 3 Http 请求

#### 3.1 QSodaHttp

```csharp
var result = await QSodaHttp.Url($"http://localhost:8080/api/persion").GetAsync<object>();
```
