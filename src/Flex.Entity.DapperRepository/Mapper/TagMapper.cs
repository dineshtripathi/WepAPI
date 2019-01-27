using System.Runtime.Remoting.Channels;
using AutoMapper;
using Flex.Entity.DapperRepository.DO;

namespace Flex.Entity.DapperRepository.Mapper
{
    public class TagMapper : Profile
    {
        public TagMapper()
        {
            CreateMap<string, Api.Model.TagType>()
                .ForMember(d => d.key, m => m.MapFrom(s => s)).ReverseMap();

            CreateMap<Api.Model.TagAt, DO.Tag>()
                .ForMember(d => d.Key, m => m.MapFrom(s => s.key))
                .ForMember(d => d.Value, m => m.MapFrom(s => s.value))
                .ForMember(d => d.ValidFrom, m => m.MapFrom(s => s.updated_at))
                .ReverseMap()
                .ForMember(d => d.key, m => m.MapFrom(s => s.Key))
                .ForMember(d => d.value, m => m.MapFrom(s => s.Value))
                .ForMember(d => d.updated_at, m => m.MapFrom(s => s.ValidFrom));

            CreateMap<Api.Model.Tag, DO.Tag>()
                .ForMember(d => d.Key, m => m.MapFrom(s => s.key))
                .ForMember(d => d.Value, m => m.MapFrom(s => s.value))
                .ReverseMap()
                .ForMember(d => d.key, m => m.MapFrom(s => s.Key))
                .ForMember(d => d.value, m => m.MapFrom(s => s.Value));

        }
        public override string ProfileName => this.GetType().Name;
    }
}
