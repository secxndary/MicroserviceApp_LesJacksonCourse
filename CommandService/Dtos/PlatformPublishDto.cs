namespace CommandService.Dtos;

public sealed record PlatformPublishDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Event { get; set; }
}