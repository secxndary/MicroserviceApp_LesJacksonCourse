using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }


    public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platformReadDto),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            _configuration["CommandServiceUrl"], 
            httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "--> Sync POST to CommandService was OK!"
            : "--> Sync POST to CommandService was NOT OK!");
    }
}