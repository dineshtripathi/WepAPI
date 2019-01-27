using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Flex.Entity.Api.Model;
using Flex.Entity.DapperRepository.Dapper;
using Flex.Entity.Repository;
using Flex.Entity.Security;

namespace Flex.Entity.DapperRepository.Repository
{
    public class TagTypeRepository : BaseRepository, ITagTypeRepository
    {
        private readonly IMapper _mapper;
        public TagTypeRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper) : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        public TagTypeRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper, ISecurityContextProvider securityContextProvider) : base(dbConnectionFactory, securityContextProvider)
        {
            _mapper = mapper;

        }
        public async Task<int> CreateAsync(TagType newType, CancellationToken cancellationToken)
        {
            if (newType == null)
                throw new ArgumentNullException(nameof(newType));
            var sql = @"INSERT  INTO Meta.TagType ([key]) VALUES (@key);";
            
            var p = new DynamicParameters();
            p.Add("key", newType.key, DbType.AnsiString);
            return await WithConnection(async (dbConnection) =>
            {
                return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);

            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteAsync(string key, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var sql = @"DELETE From Meta.TagType WHERE [key] = @key;";
                var p = new DynamicParameters();
                p.Add("key", key, DbType.AnsiString);
                return await WithConnection(async (dbConnection) =>
                {
                    return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);

                }, cancellationToken).ConfigureAwait(false);
            }
            throw new ArgumentException("is null or empty", nameof(key));

        }

        public async Task<TagType> GetAsync(string key, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var sql = @"SELECT [key] FROM  Meta.TagType WHERE [key] = @key;";
                var p = new DynamicParameters();
                p.Add("key", key, DbType.AnsiString);
                return await WithConnection(async (dbConnection) =>
                {
                    var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.None, cancellationToken);
                    var result = await dbConnection.QueryAsync<string>(cd).ConfigureAwait(false);
                    return _mapper.Map<TagType>(result.FirstOrDefault());
                }, cancellationToken).ConfigureAwait(false);
            }
            throw new ArgumentException("is null or empty", nameof(key));
        }

        public async Task<IEnumerable<TagType>> GetAsync(CancellationToken cancellationToken)
        {
            var sql = @"SELECT [key] FROM  Meta.TagType;";
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, null, null, null, null, CommandFlags.None, cancellationToken);
                var result = await dbConnection.QueryAsync<string>(cd).ConfigureAwait(false);
                return result.Select(_mapper.Map<TagType>).ToList();
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
