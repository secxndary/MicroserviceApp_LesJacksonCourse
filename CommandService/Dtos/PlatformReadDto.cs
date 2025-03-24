namespace CommandService.Dtos;

public sealed record PlatformReadDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}