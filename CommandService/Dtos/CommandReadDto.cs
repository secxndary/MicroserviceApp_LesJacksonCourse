namespace CommandService.Dtos;

public sealed record CommandReadDto
{
    public int Id { get; init; }
    public string HowTo { get; init; }
    public string CommandLine { get; init; }
    public int PlatformId { get; init; }
}