using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepository
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    Platform CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);

    IEnumerable<Command> GetAllCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    Command CreateCommand(int platformId, Command command);
}