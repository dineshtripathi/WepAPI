using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.Model;
using Flex.Entity.DapperRepository.Dapper;
using Flex.Entity.Repository;
using Flex.Entity.Security;
using AutoMapper;
using Dapper;

namespace Flex.Entity.DapperRepository.Repository
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        private readonly IMapper _mapper;
        public TagRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper, ISecurityContextProvider securityContextProvider) : base(dbConnectionFactory, securityContextProvider)
        {
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(string entityCode, string key, TagRequest tag,  CancellationToken cancellationToken)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            var sql = @" Declare @entityId int; 
                          SELECT @entityId = entityId FROM Meta.Entity WHERE Code =@entityCode;
                          INSERT INTO Meta.Tag (EntityId, [key], [value]) VALUES (@entityId, @key, @value);";
            
            var p = new DynamicParameters();
            p.Add("key", key, DbType.AnsiString);
            p.Add("value", tag.value, DbType.AnsiString);
            p.Add("entityCode", entityCode, DbType.AnsiString);
            return await WithConnection(async (dbConnection) =>
            {
                return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(string entityCode, string key, TagRequest tag, CancellationToken cancellationToken)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            var sql = @"UPDATE T SET value=@value
                          FROM Meta.Tag T 
                         INNER JOIN Meta.Entity E on T.EntityId = E.EntityId
                         WHERE E.Code=@entityCode AND T.[Key]=@key;";

            var p = new DynamicParameters();
            p.Add("key", key, DbType.AnsiString);
            p.Add("value", tag.value, DbType.AnsiString);
            p.Add("entityCode", entityCode, DbType.AnsiString);

            return await WithConnection(async (dbConnection) =>
            {
                return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteAsync(string entityCode, string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(entityCode)) throw new ArgumentException("is null or empty", nameof(entityCode));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("is null or empty", nameof(key));

            const string sql = @"DELETE T 
                                   FROM Meta.Tag T 
                                  INNER JOIN Meta.Entity E ON  E.EntityId = T.EntityId
                                  WHERE E.Code =@entityCode AND T.[key] = @key;";

            var p = new DynamicParameters();
            p.Add("key", key, DbType.AnsiString);
            p.Add("entityCode", entityCode, DbType.AnsiString);

            return await WithConnection(async (dbConnection) =>
            {
                return await dbConnection.ExecuteAsync(sql, p).ConfigureAwait(false);

            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TagAt> GetAsync(string entityCode, string key, DateTime? at, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(entityCode)) throw new ArgumentException("is null or empty", nameof(entityCode));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("is null or empty", nameof(key));

            var dateTimeAt = at == null ? string.Empty : "FOR SYSTEM_TIME AS OF @dateTime";
            var sql = $@"SELECT T.TagId, T.EntityId, T.[Key], T.[Value], T.ValidFrom, T.ValidTo, T.RowVersion 
                            FROM Meta.Tag { dateTimeAt } AS T
                            INNER JOIN Meta.Entity E ON E.EntityId = T.EntityId
                            WHERE E.Code=@entityCode AND T.[Key] =@key 
                            ";

            var p = new DynamicParameters();
            p.Add("key", key, DbType.AnsiString);
            p.Add("entityCode", entityCode, DbType.AnsiString);
            if (at != null)
            {
                p.Add("@dateTime", at, DbType.DateTime2);    
            }

            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.None, cancellationToken);
                var result = await dbConnection.QueryAsync<DO.Tag>(cd).ConfigureAwait(false);
                return _mapper.Map<TagAt>(result.FirstOrDefault());
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TagAt>> GetAsync(string entityCode, DateTime? at, CancellationToken cancellationToken)
        {
            var dateTimeAt = at == null ? string.Empty : "FOR SYSTEM_TIME AS OF @dateTime";
            var sql = $@"SELECT T.TagId, T.EntityId, T.[Key], T.[Value], T.ValidFrom, T.ValidTo, T.RowVersion 
                          FROM Meta.Tag  { dateTimeAt } AS T
                         INNER JOIN Meta.Entity E ON E.EntityId = T.EntityId
                         WHERE E.Code=@entityCode;";

            var p = new DynamicParameters();
            p.Add("entityCode", entityCode, DbType.AnsiString);
            if (at != null)
            {
                p.Add("@dateTime", at, DbType.DateTime2);
            }
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.None, cancellationToken);
                var result = await dbConnection.QueryAsync<DO.Tag>(cd).ConfigureAwait(false);
                
                return result.Select(_mapper.Map<TagAt>).ToList();
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
