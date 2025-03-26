using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;

namespace CommandService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }


    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        var grpcEndpoint = _configuration["GrpcPlatform"];

        Console.WriteLine($"--> Calling gRPC Service: {grpcEndpoint}");

        var channel = GrpcChannel.ForAddress(grpcEndpoint);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);

            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldn't call gRPC Server: {ex.Message}");

            return null;
        }
    }
}