using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.Model;


namespace Flex.Entity.Repository
{
    public interface ITagRepository
    {
        Task<int> CreateAsync(string entityCode,  string key, TagRequest tag, CancellationToken cancellationToken);
        Task<int> UpdateAsync(string entityCode, string key, TagRequest tag, CancellationToken cancellationToken);
        Task<int> DeleteAsync(string entityCode, string key, CancellationToken cancellationToken);
        Task<TagAt> GetAsync(string entityCode, string key, DateTime? at, CancellationToken cancellationToken);
        Task<IEnumerable<TagAt>> GetAsync(string entityCode, DateTime? at, CancellationToken cancellationToken);
    }
}