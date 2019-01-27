using System;
using System.Data;
using System.Data.SqlClient;
using Flex.Entity.Repository;

namespace Flex.Entity.DapperRepository.Exception
{
    public static class ExceptionMapper
    {
        public static System.Exception Map(System.Exception ex)
        {
            if (ex is DbException)
            {
                return ex;
            }
            if ((ex is SqlException) || (ex is DataException && ex.InnerException is SqlException))
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
                    case 2601:
                        return new DapperDbException(DbException.ErrorReason.ConstraintViolation, "Unique Index Violation", inner);
                    case 2627:
                        return new DapperDbException(DbException.ErrorReason.ConstraintViolation, "Unique Key/Foreign Key Violation", inner);
                    case 547:
                    case 515:
                        return new DapperDbException(DbException.ErrorReason.ConstraintViolation, "Constraint Violation", inner);
                    case 17:
                    case 53:
                    case 4060:
                    case 18456:
                        return new DapperDbException(DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.", inner);
                    case 2706:
                        return new DapperDbException(DbException.ErrorReason.Configuration, "Unable to execute the command,Table does not exists.", inner);
                    //oe custom numbers
                    case 60000:
                    case 60001:
                        return new DapperDbException(DbException.ErrorReason.ConstraintViolation, ex.Message, inner);
                    case 70000:
                        return new DapperDbException(DbException.ErrorReason.SecurityViolation, ex.Message, inner);
                    default:
                        return new DapperDbException(DbException.ErrorReason.ExtendedReason, ex.Message, inner, inner.Number);


                }

            }
            else if (ex is DataException)
            {
                return new DapperDbException(DbException.ErrorReason.OptimisticConcurrency, "No rows were affected", ex);
            }
            //else if (ex is UpdateException)
            //{
            //    return new DapperDbException(DbException.ErrorReason.OptimisticConcurrency, "OptimisticConcurrencyException", ex);
            //}
            //else if (ex is UpdateException)
            //{
            //    return new DapperDbException(DbException.ErrorReason.InsertUpdateFailure, "UpdateException", ex);
            //}
            //else if (ex is EntityCommandExecutionException)
            //{
            //    return new DapperDbException(DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.", ex.InnerException);
            //}
            //else if (ex is EntityException)
            //{
            //    return new DapperDbException(DbException.ErrorReason.Connection, ex.Message, ex.InnerException);
            //}

            else if (ex is System.InvalidOperationException)
            {
                if (ex.Message.Contains("timeout period elapsed prior to obtaining a connection from the pool"))
                {
                    return new DapperDbException(DbException.ErrorReason.Timeout, "Unable to execute the command, DbServer transaction/security/connectivity failure.", ex);
                }
            }
            else if(ex is TimeoutException)
            {
                return new DapperDbException(DbException.ErrorReason.Timeout, "Unable to execute the command, DbServer transaction/security/connectivity failure.", ex);
            }

            return ex;
        }
    }
}
