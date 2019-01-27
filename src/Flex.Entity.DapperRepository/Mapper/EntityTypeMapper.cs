using AutoMapper;
using Flex.Entity.DapperRepository.DO;

namespace Flex.Entity.DapperRepository.Mapper
{
    public class EntityTypeMapper : Profile
    {
        public EntityTypeMapper()
        {
            CreateMap<EntityType, Api.Model.EntityType>()
                .ForMember(d => d.prefix, m => m.MapFrom(s => s.Prefix))
                .ForMember(d => d.name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.allow_in_asset_hierarchy, m => m.MapFrom(s => s.IsAllowedAsAssetNode))
                .ForMember(d => d.allow_in_service_hierarchy, m => m.MapFrom(s => s.IsAllowedAsServiceNode))
                .ForMember(d => d.allow_same_type_descendant, m => m.MapFrom(s => s.IsAllowedSameDescendantNode))

                .ReverseMap()
                .ForMember(d => d.Prefix, m => m.MapFrom(s => s.prefix))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.name))
                .ForMember(d => d.IsAllowedAsAssetNode, m => m.MapFrom(s => s.allow_in_asset_hierarchy))
                .ForMember(d => d.IsAllowedAsServiceNode, m => m.MapFrom(s => s.allow_in_service_hierarchy))
                .ForMember(d => d.IsAllowedSameDescendantNode, m => m.MapFrom(s => s.allow_same_type_descendant));
        }
        public override string ProfileName => this.GetType().Name;
    }
}
