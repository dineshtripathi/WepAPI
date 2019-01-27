using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flex.Entity.AzureSqlRepository.Mapper;
using Flex.Entity.Repository;

namespace Flex.Entity.AzureSqlRepository
{
    public class RepositoryInitializer:IRepositoryInitializer
    {
        public void Initialize(object options = null)
        {
            //AutoMapperConfig.RegisterMappings();
            //using (var context = new MetaSchemaContext())
            //{
            //    context.Database.Initialize(force: false);
            //}
        }
    }
}
