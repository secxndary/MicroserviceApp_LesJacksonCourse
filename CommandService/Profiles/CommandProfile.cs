using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<Platform, PlatformReadDto>();

        CreateMap<PlatformPublishDto, Platform>()
            .ForMember(platform => platform.Id,
                opts => opts.MapFrom(platformPublish => platformPublish.Id))
            .ForMember(platform => platform.ExternalId,
            opts => opts.MapFrom(platformPublish => platformPublish.Id));

        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(grpcPlatformModel => grpcPlatformModel.Id,
                opts => opts.MapFrom(platform => platform.PlatformId))
            .ForMember(grpcPlatformModel => grpcPlatformModel.ExternalId,
                opts => opts.MapFrom(platform => platform.PlatformId))
            .ForMember(grpcPlatformModel => grpcPlatformModel.Commands,
                opts => opts.Ignore());
    }
}