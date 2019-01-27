using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Flex.Entity.AzureSqlRepository
{
    public class MetaSchemaDbConfiguration : DbConfiguration
    {
        public MetaSchemaDbConfiguration()
        {
            this.SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
            //"System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer
            this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());
            //this.SetExecutionStrategy("System.Data.SqlClient", ()=> new SqlAzureExecutionStrategy());
            this.SetDatabaseInitializer<MetaSchemaContext>(new MetaSchemaDbInitializer());

        }
    }
}