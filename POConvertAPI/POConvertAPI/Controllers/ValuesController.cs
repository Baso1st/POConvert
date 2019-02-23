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
  public class ValuesController : ControllerBase
  {

    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
      Console.Write(CSharpToJson.CompileClasses(@"
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
  }"));
      return new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
      return "value";
    }

    public class myType
    {
      public int Id { get; set; }
      public string Value { get; set; }
    }

    // POST api/values
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
      return Ok(CSharpToJson.CompileClasses(value));
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
