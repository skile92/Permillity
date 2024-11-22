using Microsoft.AspNetCore.Mvc;
using Permillity;

namespace PermillityTest.Controllers;

[ApiController]
public class EmployeeController : ControllerBase
{
    [HttpGet]
    [Route("api/HumanResources/Employee")]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        var rnd = new Random();

        await Task.Delay(rnd.Next(100));

        return new string[] { "value1", "value2" };
    }

    [HttpGet]
    [Route("api/HumanResources/EmployeeCustom")]
    [PermillityAvoid]
    public ActionResult<IEnumerable<string>> GetCustom([FromQuery] string name, [FromQuery] string surname)
    {
        return new string[] { "value1", "value2" };
    }

    [HttpGet]
    [Route("api/HumanResources/Employee/{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpGet]
    [Route("api/HumanResources/EmployeeByName/{name}")]
    public string GetByName(string name)
    {
        return "value";
    }

    [HttpPost]
    [Route("api/HumanResources/Employee")]
    public void Post([FromBody] string value)
    {
    }

    [HttpPut]
    [Route("api/HumanResources/Employee/{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete]
    [Route("api/HumanResources/Employee/{id}")]
    public void Delete(int id, [FromBody] string test)
    {
    }
}
