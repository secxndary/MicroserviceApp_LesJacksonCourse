using System.ComponentModel.DataAnnotations;

namespace CommandService.Dtos;

public sealed record CommandCreateDto
{
    [Required]
    public string HowTo { get; init; }

    [Required]
    public string CommandLine { get; init; }
}