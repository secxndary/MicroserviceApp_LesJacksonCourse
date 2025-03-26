using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }


    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            case EventType.Undetermined:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event...");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform_Published Event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine($"--> Could not determine event type. Message: {notificationMessage}");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using var scope = _scopeFactory.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishMessage);

        try
        {
            var platformModel = _mapper.Map<Platform>(platformPublishDto);

            if (repository.ExternalPlatformExists(platformModel.ExternalId))
            {
                Console.WriteLine($"--> Platform with id = {platformModel.ExternalId} already exists");
            }
            else
            {
                repository.CreatePlatform(platformModel);
                repository.SaveChanges();

                Console.WriteLine("--> Platform added!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not add platform to DB: {ex.Message}");
        }
    }
}