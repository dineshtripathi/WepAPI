using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flex.Entity.Repository;

namespace Flex.Entity.DapperRepository
{
    class DapperRepositoryInitializer: IRepositoryInitializer
    {
        public void Initialize(object options = null)
        {
           // if(options != null && options is string)
               // SqlServerTypes.Utilities.LoadNativeAssemblies((string)options);
        }
    }
}
