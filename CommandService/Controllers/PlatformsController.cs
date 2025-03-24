using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : Controller
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandService");

        var platforms = _repository.GetAllPlatforms();
        var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

        return Ok(platformReadDtos);
    }


    [HttpPost]
    public IActionResult InboundRequestTest()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound test from Platforms Controller");
    }
}