using System.Collections.Generic;
using Flex.Entity.AzureSqlRepository.DO;

namespace Flex.Entity.AzureSqlRepository.Tests
{
    public static class FakeData
    {
        public static List<EntityType> GetEntityType()
        {
            return (new List<EntityType> {CreateEntityType(1,"Load","L")});
        }

        public static EntityType CreateEntityType(int id, string name, string prefix)
        {
            return new EntityType
            {
                EntityTypeId = id,
                Name = name,
                Prefix = prefix,
                IsAllowedAsAssetNode = true,
                IsAllowedSameDescendantNode = true,
                IsAllowedAsServiceNode = true
            };
        }
    }
}