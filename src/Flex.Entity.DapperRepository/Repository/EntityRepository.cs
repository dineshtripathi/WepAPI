using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    public class EntityRepository : BaseRepository, IEntityRepository
    {
        private readonly IMapper _mapper;

        public EntityRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public EntityRepository(IMapper mapper, IDbConnectionFactory connectionFactory, ISecurityContextProvider securityContextProvider) : base(connectionFactory, securityContextProvider)
        {
            _mapper = mapper;
        }

        public async Task<Api.Model.Entity> CreateAsync(EntityRequest newEntity, CancellationToken cancellationToken)
        {
            if (newEntity == null)
                throw new ArgumentNullException(nameof(newEntity));
            Api.Model.Entity retEntity = _mapper.Map<Api.Model.Entity>(newEntity);
            var p = new DynamicParameters();
            p.Add("@TypePrefix", newEntity.typePrefix, DbType.AnsiString, direction: ParameterDirection.Input, size: 2);
            p.Add("@Name", newEntity.name, DbType.AnsiString, direction: ParameterDirection.Input, size: 255);
            p.Add("@AssetParentCode", string.IsNullOrWhiteSpace(newEntity.asset_parent)?null:newEntity.asset_parent, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("@ServiceParentCode", string.IsNullOrWhiteSpace(newEntity.service_parent) ? null : newEntity.service_parent, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("@RC", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            p.Add("@Code", dbType: DbType.AnsiString,direction: ParameterDirection.Output, size: 10);
            p.Add("@TypeName", dbType: DbType.AnsiString, direction: ParameterDirection.Output, size: 255);
            p.Add("@EntityId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            //p.Add("@AssetNodeId", dbType: DbType.Object, direction: ParameterDirection.Output);
            //p.Add("@ServiceNodeId", dbType: DbType.Object, direction: ParameterDirection.Output);
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition("[Meta].[uspCreateEntity]", p, null, null, CommandType.StoredProcedure, CommandFlags.None, cancellationToken);
                var result =  await dbConnection.ExecuteAsync(cd).ConfigureAwait(false);
                var rc = p.Get<int>("RC");
                if (rc == 1)
                {
                    retEntity.code = p.Get<string>("Code");
                    retEntity.type = p.Get<string>("TypeName");
                    return retEntity;
                }
                return null;
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Api.Model.Entity> UpdateAsync(string code, EntityPatchRequest updatedEntity, CancellationToken cancellationToken)
        {

            if (updatedEntity == null)
                throw new ArgumentNullException(nameof(updatedEntity));
            Api.Model.Entity retEntity = _mapper.Map<Api.Model.Entity>(updatedEntity);
            var p = new DynamicParameters();
            p.Add("Code", code, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("NewName", updatedEntity.name, DbType.AnsiString, direction: ParameterDirection.Input, size: 255);
            p.Add("NewAssetParentCode", updatedEntity.asset_parent, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("NewServiceParentCode", updatedEntity.service_parent, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("@RC", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition("[Meta].[uspUpdateEntity]", p, null, null, CommandType.StoredProcedure, CommandFlags.None, cancellationToken);
                var result = await dbConnection.ExecuteAsync(cd).ConfigureAwait(false);
                var rc = p.Get<int>("RC");
                if (rc == 1)
                {
                    retEntity.code = code;
                    return retEntity;
                }
                return null;
            }, cancellationToken).ConfigureAwait(false);
            throw new NotImplementedException();
        }

        public async Task<int> DeleteAsync(string code, bool deleteDescendants, CancellationToken cancellationToken)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            var p = new DynamicParameters();
            p.Add("Code", code, DbType.AnsiString, direction: ParameterDirection.Input, size: 10);
            p.Add("DeleteDescendants", deleteDescendants, DbType.Boolean, direction: ParameterDirection.Input);
            p.Add("RC", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition("[Meta].[uspDeleteEntity]", p, null, null, CommandType.StoredProcedure, CommandFlags.None, cancellationToken);
                var result = await dbConnection.ExecuteAsync(cd).ConfigureAwait(false);
                var rc = p.Get<int>("RC");
                return rc;
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<EntityAt> GetAsync(string code, CancellationToken cancellationToken, DateTime? at = null)
        {

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("is null or empty", nameof(code));

            var dateTimeAt = at == null ? string.Empty : "FOR SYSTEM_TIME AS OF @AtTime";
            var sql = $@"SELECT 
                        E.EntityId As EntityId
                        ,ET.EntityTypeId As EntityTypeId
		                ,ET.Prefix As TypePrefix
		                ,ET.Name As Type
                        ,E.Code As Code
		                ,E.Name As Name
		                ,APE.Code AS AssetParentCode
		                ,SPE.Code AS ServiceParentCode
                        ,E.ValidFrom As ValidFrom
                        ,E.ValidTo As ValidTo
                        FROM Meta.vwEntity {dateTimeAt} AS E 
                            INNER JOIN Meta.EntityType ET ON E.EntityTypeId = ET.EntityTypeId
		                LEFT JOIN Meta.vwEntity {dateTimeAt} AS APE ON E.AssetParentId = APE.AssetNodeId
		                LEFT JOIN Meta.vwEntity {dateTimeAt} AS SPE ON E.ServiceParentId = SPE.ServiceNodeId
		                WHERE E.Code=@Code;";
            var p = new DynamicParameters();
            p.Add("Code", code, DbType.AnsiString);
            if (at != null)
                p.Add("AtTime", (DateTime) at, DbType.DateTime2);
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.None,cancellationToken);
                var resultDo =  await dbConnection.QueryAsync<DO.Entity>(cd).ConfigureAwait(false);
                return _mapper.Map<EntityAt>(resultDo.SingleOrDefault());
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EntityAt>> GetAsync(string code, CancellationToken cancellationToken, string hierarchy, DateTime? at = null)
        {
            
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("is null or empty", nameof(code));
            if (!hierarchy.Equals("asset",StringComparison.OrdinalIgnoreCase) && !hierarchy.Equals("service", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("invalid hierarchy", hierarchy);
            var dateTimeAt = at == null ? string.Empty : "FOR SYSTEM_TIME AS OF @AtTime";
            var sqlService = @"E.ServiceNodeId.IsDescendantOf(EntityCTE.ServiceNodeId)";

            var sqlAsset = @"E.AssetNodeId.IsDescendantOf(EntityCTE.AssetNodeId)";

            var sqlAssetOrService = (hierarchy.Equals("asset", StringComparison.OrdinalIgnoreCase)) ? sqlAsset : sqlService;

            var sql = $@"WITH EntityCTE (EntityId, AssetNodeId, ServiceNodeId)
                                AS
                                (
                                    SELECT EntityId, AssetNodeId, ServiceNodeId
                                    FROM Meta.vwEntity {dateTimeAt}
                                    WHERE Code = @Code
                                )
                                SELECT 
                                                        E.EntityId As EntityId
                                                        ,ET.EntityTypeId As EntityTypeId
		                                                ,ET.Prefix As TypePrefix
		                                                ,ET.Name As Type
                                                        ,E.Code As Code
		                                                ,E.Name As Name
		                                                ,APE.Code AS AssetParentCode
		                                                ,SPE.Code AS ServiceParentCode
                                                        ,E.ValidFrom As ValidFrom
                                                        ,E.ValidTo As ValidTo
                                                        FROM Meta.vwEntity {dateTimeAt} AS E 
						                                INNER  JOIN EntityCTE ON {sqlAssetOrService} = 1
						                                INNER JOIN Meta.EntityType ET ON E.EntityTypeId = ET.EntityTypeId
		                                                LEFT JOIN Meta.vwEntity {dateTimeAt} AS APE ON E.AssetParentId = APE.AssetNodeId
		                                                LEFT JOIN Meta.vwEntity {dateTimeAt} AS SPE ON E.ServiceParentId = SPE.ServiceNodeId
						                                WHERE E.EntityId <> (EntityCTE.EntityId);";


            var p = new DynamicParameters();
            p.Add("Code", code, DbType.AnsiString);
            if (at != null)
                p.Add("AtTime", (DateTime)at, DbType.DateTime2);
            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.Buffered, cancellationToken);
                var resultDo = await dbConnection.QueryAsync<DO.Entity>(cd).ConfigureAwait(false);
                return resultDo.Select(_mapper.Map<EntityAt>);
            }, cancellationToken).ConfigureAwait(false);
        }


        public async Task<Entities> GetAsync(CancellationToken cancellationToken, int top = 1000, int skip = 0, string name = null, string serviceDescendant = null, string assetDescendant = null, string serviceChild = null, string assetChild = null, string hasTag = null, Tag matchesTag = null, DateTime? at = null)

        {

            var p = new DynamicParameters();
            p.Add("Top", top);
            p.Add("Skip", skip);

            if (at != null)
                p.Add("AtTime", (DateTime)at, DbType.DateTime2);
            var dateTimeAt = at == null ? string.Empty : "FOR SYSTEM_TIME AS OF @AtTime";

            var sqlPreQuery = (!string.IsNullOrWhiteSpace(serviceDescendant) || !string.IsNullOrWhiteSpace(assetDescendant))?
                                $@"DECLARE @ServiceAncestorHId HIERARCHYID;
                                DECLARE @AssetAncestorHId HIERARCHYID;
                                Select @ServiceAncestorHId = ServiceNodeId FROM Meta.vwEntity {dateTimeAt} E WHERE Code = @ServiceAncestorCode
                                Select @AssetAncestorHId = AssetNodeId FROM Meta.vwEntity {dateTimeAt} E WHERE Code = @AssetAncestorCode
                                ;" : string.Empty;
            p.Add("ServiceAncestorCode", !string.IsNullOrWhiteSpace(serviceDescendant) ? serviceDescendant : "OE1", DbType.AnsiString);
            p.Add("AssetAncestorCode", !string.IsNullOrWhiteSpace(assetDescendant) ? assetDescendant : "OE1", DbType.AnsiString);

            var sqlBase =
                $@"{sqlPreQuery} 
                    WITH EntityCTE (EntityId,EntityTypeId, TypePrefix, Type, Code, Name, AssetParentCode,ServiceParentCode,ValidFrom,ValidTo, AssetNodeId, ServiceNodeId)
                            AS
                            (SELECT 
                                                    E.EntityId As EntityId
                                                    ,ET.EntityTypeId As EntityTypeId
		                                              ,ET.Prefix As TypePrefix
		                                              ,ET.Name As Type
                                                    ,E.Code As Code
		                                              ,E.Name As Name
		                                              ,APE.Code AS AssetParentCode
		                                              ,SPE.Code AS ServiceParentCode
                                                    ,E.ValidFrom As ValidFrom
                                                    ,E.ValidTo As ValidTo
						                              ,E.AssetNodeId
						                              ,E.ServiceNodeId
                                                    FROM Meta.vwEntity {dateTimeAt} AS E INNER JOIN Meta.EntityType ET ON E.EntityTypeId = ET.EntityTypeId
		                                              LEFT JOIN Meta.vwEntity {dateTimeAt} AS APE ON E.AssetParentId = APE.AssetNodeId
		                                              LEFT JOIN Meta.vwEntity {dateTimeAt} AS SPE ON E.ServiceParentId = SPE.ServiceNodeId) ";
            var sqlSelect = " SELECT * FROM EntityCTE AS EV ";
            var sqlCount = " SELECT COUNT(1) AS Count FROM EntityCTE AS EV ";
            var sqlTopSkip = @"ORDER BY EV.EntityId
                               OFFSET @Skip ROWS
                               FETCH NEXT @Top ROWS ONLY ";
            var sqlWhereKeyword = (     string.IsNullOrWhiteSpace(name)
                                        && string.IsNullOrWhiteSpace(serviceDescendant)
                                        && string.IsNullOrWhiteSpace(assetDescendant)
                                        && string.IsNullOrWhiteSpace(serviceChild)
                                        && string.IsNullOrWhiteSpace(assetChild)
                                        && string.IsNullOrWhiteSpace(hasTag)
                                        && matchesTag == null
                                    ) ? string.Empty:" WHERE ";
            var sqlJoins = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append(" EV.Name LIKE @Name ");
                p.Add("Name", name, DbType.AnsiString);
            }

            if (!string.IsNullOrWhiteSpace(serviceChild))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append(" EV.ServiceParentCode = @ServiceChild ");
                p.Add("ServiceChild", serviceChild, DbType.AnsiString);
            }

            if (!string.IsNullOrWhiteSpace(assetChild))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append(" EV.AssetParentCode = @AssetChild ");
                p.Add("AssetChild", assetChild, DbType.AnsiString);

            }

            if (!string.IsNullOrWhiteSpace(hasTag))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append($" EXISTS (SELECT TOP 1 EntityId FROM Meta.Tag {dateTimeAt} T WHERE T.EntityId = EV.EntityId AND T.Value IS NOT NULL AND T.[Key] = @HasTag)  ");
                p.Add("HasTag", hasTag, DbType.AnsiString);
            }

            if (matchesTag != null)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                p.Add("HasTag", matchesTag.key, DbType.AnsiString);
                if (matchesTag.value != null)
                {
                    p.Add("Value", matchesTag.value, DbType.AnsiString);
                    whereBuilder.Append(
                        " EXISTS (SELECT TOP 1 EntityId FROM Meta.Tag T WHERE T.EntityId = EV.EntityId AND T.Value = @Value AND T.[Key] = @HasTag)  ");
                }
                else
                {
                    whereBuilder.Append(" EXISTS (SELECT TOP 1 EntityId FROM Meta.Tag T WHERE T.EntityId = EV.EntityId AND T.Value IS NULL AND T.[Key] = @HasTag)  ");
                }
            }


            if (!string.IsNullOrWhiteSpace(serviceDescendant))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append(" EV.AssetNodeId.IsDescendantOf(@ServiceAncestorHId) = 1 ");
            }

            if (!string.IsNullOrWhiteSpace(assetDescendant))
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" AND ");
                whereBuilder.Append(" EV.AssetNodeId.IsDescendantOf(@AssetAncestorHId) = 1 ");
            }


            


            var sqlWhere = whereBuilder.ToString();

            var sql = $@"{sqlBase}
                         {sqlSelect}
                          {sqlJoins}
                          {sqlWhereKeyword}
                          {sqlWhere}
                          {sqlTopSkip};";

            var sqlToGetCount = $@"{sqlBase}
                                {sqlCount}
                                {sqlJoins}
                                {sqlWhereKeyword}
                                {sqlWhere}; ";

            return await WithConnection(async (dbConnection) =>
            {
                var cd = new CommandDefinition(sqlToGetCount, p, null, null, null, CommandFlags.Buffered, cancellationToken);
                var resultCount = await dbConnection.QuerySingleAsync(cd).ConfigureAwait(false);
                int count = resultCount.Count;
                if (count > 0)
                {
                    cd = new CommandDefinition(sql, p, null, null, null, CommandFlags.Buffered, cancellationToken);
                    var resultDo = await dbConnection.QueryAsync<DO.Entity>(cd).ConfigureAwait(false);
                    return new Entities() {count = count, entities = resultDo.Select(_mapper.Map<EntityDetail>)};
                }
                return new Entities() {count = count};
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
