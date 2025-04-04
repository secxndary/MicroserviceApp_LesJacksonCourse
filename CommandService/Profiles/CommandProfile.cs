using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<PlatformPublishDto, Platform>()
            .ForMember(platform => platform.ExternalId,
                opts => opts.MapFrom(platformPublish => platformPublish.Id));
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(grpcPlatformModel => grpcPlatformModel.ExternalId,
                opts => opts.MapFrom(platform => platform.PlatformId))
            .ForMember(grpcPlatformModel => grpcPlatformModel.Commands,
                opts => opts.Ignore());
    }
}