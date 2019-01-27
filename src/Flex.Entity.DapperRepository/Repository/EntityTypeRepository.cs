
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Dapper;
using Flex.Entity.DapperRepository.Dapper;
using Flex.Entity.Repository;
using Flex.Entity.Security;
using EntityType = Flex.Entity.Api.Model.EntityType;

namespace Flex.Entity.DapperRepository.Repository
{
    public class EntityTypeRepository : BaseRepository, IEntityTypeRepository
    {
        private readonly IMapper _mapper;
        public EntityTypeRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper) : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        public EntityTypeRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper, ISecurityContextProvider securityContextProvider) : base(dbConnectionFactory,securityContextProvider)
        {
            _mapper = mapper;

        }
        public async Task<int> CreateAsync(EntityType newType, CancellationToken cancellationToken)
        {
            if (newType == null)
                throw new ArgumentNullException(nameof(newType));
            var sql =@"INSERT  INTO Meta.EntityType
                            (Prefix,
                              Name,
                              IsAllowedAsAssetNode,
                              IsAllowedAsServiceNode,
                              IsAllowedSameDescendantNode
                            )
                        VALUES(@Prefix,
                                    @Name,
                                    @IsAllowedAsAssetNode,
                                    @IsAllowedAsServiceNode,
                                    @IsAllowedSameDescendantNode
                                );";
            var parameters = _mapper.Map<DO.EntityType>(newType);
            var p = new DynamicParameters(parameters);

            return await WithConnection(async (dbConnection) =>
            {
               return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);

            }, cancellationToken).ConfigureAwait(false);
        }

        public async  Task<int> DeleteAsync(string prefix, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var sql = @"DELETE From Meta.EntityType WHERE Prefix = @prefix;";
                var p = new DynamicParameters();
                p.Add("prefix", prefix, DbType.AnsiString);
                return await WithConnection(async (dbConnection) =>
                {
                    return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);

                }, cancellationToken).ConfigureAwait(false);
            }
            throw new ArgumentException("is null or empty", nameof(prefix));

        }

        public async Task<EntityType> GetAsync(string prefix, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("is null or empty", nameof(prefix));

            var sql = @"SELECT EntityTypeId
                                ,Prefix
                                ,Name
                                ,IsAllowedAsAssetNode
                                ,IsAllowedAsServiceNode
                                ,IsAllowedSameDescendantNode
                            FROM Meta.EntityType WHERE Prefix = @Prefix;";
                var p = new DynamicParameters();
                p.Add("Prefix", prefix, DbType.AnsiString);
                return await WithConnection(async (dbConnection) =>
                {
                    var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.None,
                        cancellationToken);
                    var result = await dbConnection.QueryAsync<DO.EntityType>(cd).ConfigureAwait(false);
                    return _mapper.Map<EntityType>(result.FirstOrDefault());
                }, cancellationToken).ConfigureAwait(false);

        }

        public async Task<IEnumerable<EntityType>> GetAsync(CancellationToken cancellationToken)
        {
            var sql = @"SELECT EntityTypeId
                                ,Prefix
                                ,Name
                                ,IsAllowedAsAssetNode
                                ,IsAllowedAsServiceNode
                                ,IsAllowedSameDescendantNode
                            FROM Meta.EntityType;";
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, null, null, null, null, CommandFlags.None, cancellationToken);
                var result = await dbConnection.QueryAsync<DO.EntityType>(cd).ConfigureAwait(false);
                return result.Select(_mapper.Map<EntityType>).ToList();
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
