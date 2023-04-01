using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Soda.Http.SamplesWebApi.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class ApiController : ControllerBase
    {
        [NonAction]
        public IActionResult Success(object? obj = null)
        {
            return Ok(new
            {
                Code = 200,
                Data = obj
            });
        }

        [NonAction]
        public IActionResult Fail(object? obj = null)
        {
            return Ok(new
            {
                Code = 400,
                Data = obj
            });
        }
    }

    public class TestController : ApiController
    {
        [HttpGet]
        public IActionResult Get(string id)
        {
            return Success($"获取数据成功, Id 为{id}");
        }

        [HttpGet]
        public IActionResult GetResult([FromQuery] TestParameter parameters)
        {
            return Success(parameters);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TestParameter parameters)
        {
            return Success(parameters);
        }

        [HttpPost]
        public IActionResult PostResult([FromQuery] TestParameter query, [FromBody] TestParameter body)
        {
            return Success(new
            {
                query,
                body
            });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            return Success(id);
        }

        [HttpDelete]
        public IActionResult DeleteResult([FromQuery] TestParameter parameters)
        {
            return Success(parameters);
        }

        [HttpPut]
        public IActionResult Put(string id)
        {
            return Success(id);
        }

        [HttpPut]
        public IActionResult PutResult([FromBody] TestParameter parameters)
        {
            return Success(parameters);
        }

        [HttpPatch]
        public IActionResult Patch(string id)
        {
            return Success(id);
        }

        [HttpPatch]
        public IActionResult PatchResult([FromBody] TestParameter parameters)
        {
            return Success(parameters);
        }
    }

    public class TestParameter
    {
        public string? Id { get; set; }
        public IEnumerable<string>? Ids { get; set; }
    }
}