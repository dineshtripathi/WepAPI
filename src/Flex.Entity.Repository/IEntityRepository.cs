using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Repository
{
    public interface IEntityRepository
    {
        Task<Api.Model.Entity> CreateAsync(Api.Model.EntityRequest newEntity, CancellationToken cancellationToken);
        Task<Api.Model.Entity> UpdateAsync(string code, EntityPatchRequest updatedEntity, CancellationToken cancellationToken);
        Task<int> DeleteAsync(string code, bool deleteDescendants,CancellationToken cancellationToken);
        Task<Api.Model.EntityAt> GetAsync(string code, CancellationToken cancellationToken, DateTime? at = null);
        Task<IEnumerable<Api.Model.EntityAt>> GetAsync(string code, CancellationToken cancellationToken, string hierarchy, DateTime? at = null);

        Task<Entities> GetAsync(CancellationToken cancellationToken, int top = 1000, int skip = 0, string name = null, string serviceDescendant = null, string assetDescendant = null, string serviceChild = null, string assetChild = null, string hasTag = null, Tag matchesTag = null, DateTime? at = null);


    }
}
