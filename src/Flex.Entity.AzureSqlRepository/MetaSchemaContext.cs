using System.Data.Entity;
using Flex.Entity.AzureSqlRepository.DO;

namespace Flex.Entity.AzureSqlRepository
{
    [DbConfigurationType(typeof(MetaSchemaDbConfiguration))]
    public class MetaSchemaContext: DbContext, IDbContext
    {
        public MetaSchemaContext(): base("name=MetaSchemaRepositoryContext")
        {
            Database.CommandTimeout = 60;
        }

        public new IDbSetWrapper<TEntity> Set<TEntity>() where TEntity : class
        {
            var set = base.Set<TEntity>();
            return new DbSetWrapper<TEntity>(set);
        }

        //public IDbSet<Entity.Repository.DO.EntityType> EntityTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           modelBuilder.Entity<EntityType>();
        }

    }
}
