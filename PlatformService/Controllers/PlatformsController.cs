using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : Controller
{
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
    }


    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
        var platforms = _repository.GetAllPlatforms();
        var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

        return Ok(platformReadDtos);
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public IActionResult GetPlatformById(int id)
    {
        var platform = _repository.GetPlatformById(id);

        if (platform is null)
            return NotFound($"Platform with id = {id} not found");

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        return Ok(platformReadDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        var createdPlatform = _repository.CreatePlatform(platformModel);

        _repository.SaveChanges();

        var createdPlatformReadDto = _mapper.Map<PlatformReadDto>(createdPlatform);

        try
        {
            await _commandDataClient.SendPlatformToCommand(createdPlatformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { id = createdPlatformReadDto.Id }, createdPlatformReadDto);
    }
}