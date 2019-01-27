using System.Data.Entity.Core;
using System.Data.SqlClient;
using Flex.Entity.Repository;
using System.Data.Entity.Infrastructure;

namespace Flex.Entity.AzureSqlRepository.Exception
{
public static class ExceptionMapper
    {
        public static System.Exception Map(System.Exception ex)
        {
            if (ex is DbException)
            {
                return ex;
            }
            if ((ex is SqlException) || (ex is DbUpdateException && ex.InnerException is SqlException))
            {
                SqlException inner;
                if (ex is SqlException)
                {
                    inner = ex as SqlException;
                }
                else
                {
                    inner = ex.InnerException as SqlException;
                }
                switch (inner.Number)
                {
                    case 2627:
                        return new AzureSqlDbException(DbException.ErrorReason.ConstraintViolation, "Unique Key/Foreign Key Violation", inner);
                    case 547:
                        return new AzureSqlDbException(DbException.ErrorReason.ConstraintViolation, "Constraint Violation", inner);
                    case 17:
                    case 53:
                    case 4060:
                    case 18456:
                        return new AzureSqlDbException(DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.", inner);
					case 2706:
						return new AzureSqlDbException( DbException.ErrorReason.Configuration, "Unable to execute the command,Table does not exists.", inner );
					
                }

            }
            else if (ex is DbUpdateConcurrencyException)
            {
                return new AzureSqlDbException(DbException.ErrorReason.OptimisticConcurrency, "No rows were affected", ex);
            }
            else if (ex is OptimisticConcurrencyException)
            {
                return new AzureSqlDbException(DbException.ErrorReason.OptimisticConcurrency, "OptimisticConcurrencyException", ex);
            }
            else if (ex is UpdateException)
            {
                return new AzureSqlDbException(DbException.ErrorReason.InsertUpdateFailure, "UpdateException", ex);
            }
            else if (ex is EntityCommandExecutionException)
            {
                return new AzureSqlDbException(DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.", ex.InnerException);
            }
            else if (ex is EntityException)
            {
                return new AzureSqlDbException(DbException.ErrorReason.Connection, ex.Message, ex.InnerException);
            }
			
            else if (ex is System.InvalidOperationException)
            {
                if (ex.Message.Contains("timeout period elapsed prior to obtaining a connection from the pool"))
                {
                    return new AzureSqlDbException(DbException.ErrorReason.Timeout, "Unable to execute the command, DbServer transaction/security/connectivity failure.", ex);
                }
            }

            return ex;
        }
    }}
