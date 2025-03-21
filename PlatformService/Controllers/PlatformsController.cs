using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : Controller
{
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(IPlatformRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
        var platforms = _repository.GetAllPlatforms();
        var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

        return Ok(platformReadDtos);
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public IActionResult GetPlatformById([FromRoute] int id)
    {
        var platform = _repository.GetPlatformById(id);

        if (platform is null)
            return NotFound($"Platform with id = {id} not found");

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        return Ok(platformReadDto);
    }

    [HttpPost]
    public IActionResult CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
    {
        var platform = _mapper.Map<Platform>(platformCreateDto);
        var createdPlatform = _repository.CreatePlatform(platform);

        _repository.SaveChanges();

        var createdPlatformReadDto = _mapper.Map<PlatformReadDto>(createdPlatform);

        return CreatedAtRoute(nameof(GetPlatformById), new { id = createdPlatformReadDto.Id }, createdPlatformReadDto);
    }
}