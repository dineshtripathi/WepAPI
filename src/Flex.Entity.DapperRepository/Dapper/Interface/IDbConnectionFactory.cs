using System.Data.Common;

namespace Flex.Entity.DapperRepository
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }



}