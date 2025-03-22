using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : Controller
{
    public PlatformsController()
    {
        
    }


    [HttpPost]
    public IActionResult InboundRequestTest()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound test from Platforms Controller");
    }
}