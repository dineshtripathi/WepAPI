using System.Collections.Generic;
using System.Data.Entity;

namespace Flex.Entity.AzureSqlRepository
{
    public class MetaSchemaDbInitializer : NullDatabaseInitializer<MetaSchemaContext>
    {
        public override void InitializeDatabase(MetaSchemaContext context)
        {
//            context.Database.Create();
            base.InitializeDatabase(context);
           
        }

        //protected override void Seed(MetaSchemaContext context)
        //{
        //    //IList<EntityType> defaultEntityTypes = new List<EntityType>()
        //    //{
        //    //    new EntityType()
        //    //    {
        //    //        EntityTypeId = 1,
        //    //        Prefix = "C",
        //    //        Name = "Client",
        //    //        IsAllowedAsAssetNode = true,
        //    //        IsAllowedAsServiceNode = false,
        //    //        IsAllowedAsOwnChildNode = false,
        //    //        IsAllowedToDelete = false
        //    //    },
        //    //    new EntityType()
        //    //    {
        //    //        EntityTypeId = 1,
        //    //        Prefix = "C",
        //    //        Name = "Client",
        //    //        IsAllowedAsAssetNode = true,
        //    //        IsAllowedAsServiceNode = false,
        //    //        IsAllowedAsOwnChildNode = false,
        //    //        IsAllowedToDelete = false
        //    //    },
        //    //    new EntityType()
        //    //    {
        //    //        EntityTypeId = 1,
        //    //        Prefix = "C",
        //    //        Name = "Client",
        //    //        IsAllowedAsAssetNode = true,
        //    //        IsAllowedAsServiceNode = false,
        //    //        IsAllowedAsOwnChildNode = false,
        //    //        IsAllowedToDelete = false
        //    //    },
        //    //    new EntityType()
        //    //    {
        //    //        EntityTypeId = 1,
        //    //        Prefix = "C",
        //    //        Name = "Client",
        //    //        IsAllowedAsAssetNode = true,
        //    //        IsAllowedAsServiceNode = false,
        //    //        IsAllowedAsOwnChildNode = false,
        //    //        IsAllowedToDelete = false
        //    //    },
        //    //    new EntityType()
        //    //    {
        //    //        EntityTypeId = 1,
        //    //        Prefix = "C",
        //    //        Name = "Client",
        //    //        IsAllowedAsAssetNode = true,
        //    //        IsAllowedAsServiceNode = false,
        //    //        IsAllowedAsOwnChildNode = false,
        //    //        IsAllowedToDelete = false
        //    //    }

        //    //};

        //    //foreach (EntityType type in defaultEntityTypes)
        //    //    context.EntityTypes.Add(type);

        //    base.Seed(context);
        //}
    }
}