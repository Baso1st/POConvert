using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using POConvertAPI.Services;

namespace POConvertAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSharpToJsonController : ControllerBase
    {
        CSharpToJson cSharpToJson;
        public CSharpToJsonController(CSharpToJson cSharpToJson)
        {
            this.cSharpToJson = cSharpToJson;
        }
        public class MyType
        {
            public string Value { get; set; }
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to POConvert");
        }

        [HttpPost]
        public IActionResult PostString([FromBody] string cSharp)
        {
            return Ok(cSharpToJson.CompileClasses(cSharp));
        }
    }
}
