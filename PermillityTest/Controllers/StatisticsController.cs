using Microsoft.AspNetCore.Mvc;
using Permillity.Dashboard;

namespace PermillityTest.Controllers;

[Route("Dashboard")]
public class StatisticsController : Controller
{
    private readonly IPermillityService _permillityService;

    public StatisticsController(IPermillityService permillityService)
    {
        _permillityService = permillityService;
    }

    [HttpGet("GetDashboard")]
    public async Task<ActionResult> GetDashboard()
    {
        return Content(await _permillityService.GetDashboardAsync(), "text/html");
    }
}
