using AutoMapper;
using Flex.Entity.Api.Model;
using Flex.Entity.DapperRepository.DO;

namespace Flex.Entity.DapperRepository.Mapper
{
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            CreateMap<Api.Model.Entity, Api.Model.EntityPatchRequest>().ReverseMap();
            CreateMap<Api.Model.EntityRequest, Api.Model.EntityPatchRequest>().ReverseMap();
            CreateMap<Api.Model.Entity, Api.Model.EntityPatchRequest>().ReverseMap();
            CreateMap<Api.Model.EntityAt, Api.Model.Entity>()
                .IncludeBase<Api.Model.Entity, EntityPatchRequest>()
                .ReverseMap()
                .IncludeBase<Api.Model.EntityPatchRequest, Api.Model.Entity>();

            CreateMap<Api.Model.EntityAt, Api.Model.EntityDetail>()
                .IncludeBase<Api.Model.EntityAt, Api.Model.Entity>()
                .IncludeBase<Api.Model.Entity, EntityPatchRequest>()
                .ReverseMap()
                .IncludeBase<Api.Model.Entity,Api.Model.EntityAt>()
                .IncludeBase<Api.Model.EntityPatchRequest, Api.Model.Entity>();

            CreateMap<DO.Entity, Api.Model.EntityAt>()
                .ForMember(d => d.code, m => m.MapFrom(s => s.Code))
                .ForMember(d => d.name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.asset_parent, m => m.MapFrom(s => s.AssetParentCode))
                .ForMember(d => d.service_parent, m => m.MapFrom(s => s.ServiceParentCode))
                //.ForMember(d => d.typePrefix, m => m.MapFrom(s => s.TypePrefix))
                .ForMember(d => d.updated_at, m => m.MapFrom(s => s.ValidFrom))
                .ReverseMap()
                .ForMember(d => d.Code, m => m.MapFrom(s => s.code))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.name))
                .ForMember(d => d.AssetParentCode, m => m.MapFrom(s => s.asset_parent))
                .ForMember(d => d.ServiceParentCode, m => m.MapFrom(s => s.service_parent))
                //.ForMember(d => d.TypePrefix, m => m.MapFrom(s => s.typePrefix))
                .ForMember(d => d.EntityId, m => m.Ignore())
                .ForMember(d => d.ValidFrom, m => m.Ignore())
                .ForMember(d => d.ValidTo, m => m.Ignore());

            CreateMap<DO.Entity, Api.Model.EntityDetail>()
                .IncludeBase<DO.Entity, Api.Model.EntityAt>()
                .ReverseMap()
                .IncludeBase<Api.Model.EntityAt,DO.Entity>();



        }
        public override string ProfileName => this.GetType().Name;
    }
}
