using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.DapperRepository.Exception;
using Flex.Entity.Security;

namespace Flex.Entity.DapperRepository.Dapper
{
    public abstract class BaseRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ISecurityContextProvider _securityContextProvider;

        private  FlexSecurityContext _claims;

        protected BaseRepository( IDbConnectionFactory connectionFactory) : this(connectionFactory, null)
        {
        }

        protected BaseRepository(IDbConnectionFactory connectionFactory, ISecurityContextProvider securityContextProvider)
        {
            _connectionFactory = connectionFactory;
            _securityContextProvider = securityContextProvider;
           
        }


        protected async Task<T> WithConnection<T>(Func<DbConnection, Task<T>> getData, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = _connectionFactory.GetConnection())
                {
                    _claims = _securityContextProvider?.GetClaims();

                    // Asynchronously open a connection to the database
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    if (_claims != null)
                    {
                        await
                            connection.SetSecurityContextAsync(_claims.AssetRoot,
                                _claims.AssetPermission,
                                _claims.ServiceRoot,
                                _claims.ServicePermission,
                                _claims.EmailId).ConfigureAwait(false);
                    }
                    var result =  await getData(connection).ConfigureAwait(false);
                    connection.Close();
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                throw ExceptionMapper.Map(ex);
            }
        }

    }

 
}
