using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flex.Entity.Api.Model;
using Flex.Entity.AzureSqlRepository.Exception;
using Flex.Entity.Repository;

namespace Flex.Entity.AzureSqlRepository.Repository
{
    public class EntityTypeRepository : IEntityTypeRepository, IDisposable
    {
        private readonly IDbContext _db;

        private readonly IMapper _mapper;

        public EntityTypeRepository(IDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task<int> CreateAsync(EntityType newType, CancellationToken cancellationToken)
        {
            try
            {
                _db.Set<DO.EntityType>().Add(_mapper.Map<DO.EntityType>(newType));
                return await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw ExceptionMapper.Map(ex);
            }
        }

        public async Task<int> DeleteAsync(string prefix, CancellationToken cancellationToken)
        {
            var entityType = await _db.Set<DO.EntityType>().SingleOrDefaultAsync(x => x.Prefix == prefix, cancellationToken).ConfigureAwait(false);
            if (entityType != null)
            {
                _db.Set<DO.EntityType>().Remove(entityType);
            }
            return await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<EntityType> GetAsync(string prefix, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                try
                {
                    var result = await
                        _db.Set<DO.EntityType>()
                            .SingleOrDefaultAsync(p => p.Prefix == prefix, cancellationToken).ConfigureAwait(false);
                    return _mapper.Map<EntityType>(result);
                }
                catch (System.Exception ex)
                {
                    throw ExceptionMapper.Map(ex);
                }
            }
            throw new ArgumentException("is null or empty", nameof(prefix));
        }


        public async Task<IEnumerable<EntityType>> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                return
                    await
                        _db.Set<DO.EntityType>()
                            .AsQueryable()
                            .ProjectToListAsync<EntityType>(_mapper.ConfigurationProvider)
                            .ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw ExceptionMapper.Map(ex);
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}