using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Repository
{
    public interface IEntityTypeRepository
    {
        Task<int> CreateAsync(EntityType newType, CancellationToken cancellationToken);
        Task<int> DeleteAsync(string prefix, CancellationToken cancellationToken);
        Task<EntityType> GetAsync(string prefix, CancellationToken cancellationToken);
        Task<IEnumerable<EntityType>> GetAsync(CancellationToken cancellationToken);
    }
}