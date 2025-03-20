using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepository
{
    IEnumerable<Platform> GetAllPlatforms();
    Platform GetPlatformById(int id);
    void CreatePlatform(Platform platform);
    bool SaveChanges();
}
