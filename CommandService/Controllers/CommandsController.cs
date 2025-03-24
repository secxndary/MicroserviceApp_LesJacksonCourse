using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/platforms/{platformId:int}/[controller]")]
[ApiController]
public class CommandsController : Controller
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    [HttpGet]
    public IActionResult GetAllCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit GetAllCommandsForPlatform: platformId = {platformId}");

        if (!CheckIfPlatformExists(platformId))
            return NotFound($"There is no platforms with id = {platformId}");

        var commands = _repository.GetAllCommandsForPlatform(platformId);
        var commandReadDtos = _mapper.Map<IEnumerable<CommandReadDto>>(commands);

        return Ok(commandReadDtos);
    }

    [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
    public IActionResult GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit GetCommandForPlatform: platformId = {platformId}, commandId = {commandId}");

        if (!CheckIfPlatformExists(platformId))
            return NotFound($"There is no platforms with id = {platformId}");

        var command = _repository.GetCommand(platformId, commandId);

        if (command is null)
            return NotFound($"There is no commands with id = {commandId}");

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return Ok(commandReadDto);
    }

    [HttpPost]
    public IActionResult CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Hit CreateCommandForPlatform: platformId = {platformId}");

        if (!CheckIfPlatformExists(platformId))
            return NotFound($"There is no platforms with id = {platformId}");

        var commandModel = _mapper.Map<Command>(commandCreateDto);
        var createdCommand = _repository.CreateCommand(platformId, commandModel);

        _repository.SaveChanges();

        var createdCommandReadDto = _mapper.Map<PlatformReadDto>(createdCommand);

        return CreatedAtRoute(nameof(GetCommandForPlatform), 
            new { id = createdCommandReadDto.Id }, createdCommandReadDto);
    }


    private bool CheckIfPlatformExists(int platformId)
    {
        return _repository.PlatformExists(platformId);
    }
}