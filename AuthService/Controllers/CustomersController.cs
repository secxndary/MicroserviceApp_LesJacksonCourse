using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace AuthService.Controllers;

[Route("api/a/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly List<string> _customers = new() { "John Doe", "Jane Doe" };

    [HttpGet]
    [Authorize(Roles = GlobalConstants.ManagerRole)]
    public IActionResult GetCustomers()
    {
        return Ok(_customers);
    }
}