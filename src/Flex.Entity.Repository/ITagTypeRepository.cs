using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Repository
{
    public interface ITagTypeRepository
    {
        Task<int> CreateAsync(TagType tagType, CancellationToken cancellationToken);
        Task<int> DeleteAsync(string key, CancellationToken cancellationToken);
        Task<TagType> GetAsync(string key, CancellationToken cancellationToken);
        Task<IEnumerable<TagType>> GetAsync(CancellationToken cancellationToken);
    }
}