namespace PlatformService.Dtos;

public sealed record PlatformReadDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Publisher { get; init; }
    public string Cost { get; init; }
}