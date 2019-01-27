using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flex.Entity.AzureSqlRepository.DO;

namespace Flex.Entity.AzureSqlRepository.Mapper
{
    public class EntityTypeMapper : Profile
    {
        public EntityTypeMapper()
        {
            CreateMap<EntityType, Api.Model.EntityType>()
                .ForMember(s => s.prefix, m => m.MapFrom(d => d.Prefix))
                .ForMember(s => s.name, m => m.MapFrom(d => d.Name))
                .ForMember(s => s.allow_in_asset_hierarchy, m => m.MapFrom(d => d.IsAllowedAsAssetNode))
                .ForMember(s => s.allow_in_service_hierarchy, m => m.MapFrom(d => d.IsAllowedAsServiceNode))
                .ForMember(s => s.allow_same_type_descendant, m => m.MapFrom(d => d.IsAllowedSameDescendantNode))

                .ReverseMap()
                .ForMember(s => s.Prefix, m => m.MapFrom(d => d.prefix))
                .ForMember(s => s.Name, m => m.MapFrom(d => d.name))
                .ForMember(s => s.IsAllowedAsAssetNode, m => m.MapFrom(d => d.allow_in_asset_hierarchy))
                .ForMember(s => s.IsAllowedAsServiceNode, m => m.MapFrom(d => d.allow_in_service_hierarchy))
                .ForMember(s => s.IsAllowedSameDescendantNode, m => m.MapFrom(d => d.allow_same_type_descendant));



        }
        public override string ProfileName => this.GetType().Name;
    }
}
