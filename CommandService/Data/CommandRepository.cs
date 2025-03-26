using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _context;

    public CommandRepository(AppDbContext context)
    {
        _context = context;
    }


    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Platform CreatePlatform(Platform platform)
    {
        if (platform is null)
            throw new ArgumentNullException(nameof(platform));

        _context.Platforms.Add(platform);

        return platform;
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }


    public IEnumerable<Command> GetAllCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public Command CreateCommand(int platformId, Command command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        command.PlatformId = platformId;
        _context.Commands.Add(command);

        return command;
    }


    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
