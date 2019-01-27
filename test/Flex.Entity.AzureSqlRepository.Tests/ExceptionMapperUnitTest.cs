using System;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Flex.Entity.AzureSqlRepository.Exception;
using Flex.Entity.Repository;
using Moq;
using NUnit.Framework;

namespace Flex.Entity.AzureSqlRepository.Tests
{
    /// <summary>
    /// Summary description for ExceptionMapperUnitTest
    /// </summary>
    [TestFixture]
    public class ExceptionMapperUnitTest
    {
        [Test]
        public void ExceptionMapper_DbException_Map_Test()
        {
            var dbExceptionMock = new Mock<DbException>(null, null);
            var actual = ExceptionMapper.Map(dbExceptionMock.Object);
            Assert.AreEqual(actual, dbExceptionMock.Object);
        }

        [Test]
        public void ExceptionMapper_SqlException_UniqueKey_ForeignKey_Violation_Map_Test()
        {
            ValidateMapAzureSqlDbException(2627, DbException.ErrorReason.ConstraintViolation, "Unique Key/Foreign Key Violation");
        }

        [Test]
        public void ExceptionMapper_SqlException_Constraint_Violation_Map_Test()
        {
            ValidateMapAzureSqlDbException(547, DbException.ErrorReason.ConstraintViolation, "Constraint Violation");
        }

        [Test]
        public void ExceptionMapper_SqlException_Connection_Failure_Map_Test()
        {
            ValidateMapAzureSqlDbException(17, DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
            ValidateMapAzureSqlDbException(4060, DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
            ValidateMapAzureSqlDbException(53, DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
            ValidateMapAzureSqlDbException(18456, DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
        }

        [Test]
        public void ExceptionMapper_SqlException_Configuration_Map_Test()
        {
            ValidateMapAzureSqlDbException(2706, DbException.ErrorReason.Configuration, "Unable to execute the command,Table does not exists.");
        }

        [Test]
        public void ExceptionMapper_DbUpdateConcurrencyException_Map_Test()
        {
            var exception = new DbUpdateConcurrencyException();
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.OptimisticConcurrency, "No rows were affected");
        }

        [Test]
        public void ExceptionMapper_OptimisticConcurrencyException_Map_Test()
        {
            var exception = new OptimisticConcurrencyException();
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.OptimisticConcurrency, "OptimisticConcurrencyException");
        }

        [Test]
        public void ExceptionMapper_UpdateException_Map_Test()
        {
            var exception = new UpdateException();
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.InsertUpdateFailure, "UpdateException");
        }

        [Test]
        public void ExceptionMapper_EntityCommandExecutionException_Map_Test()
        {
            var exception = new EntityCommandExecutionException();
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.Connection, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
        }

        [Test]
        public void ExceptionMapper_EntityException_Map_Test()
        {
            var exception = new EntityException("Error message from exception mock.");
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.Connection, exception.Message);
        }

        [Test]
        public void ExceptionMapper_InvalidOperationException_Map_Test()
        {
            var exception = new InvalidOperationException("timeout period elapsed prior to obtaining a connection from the pool");
            ValidateMapAzureSqlDbException(exception, DbException.ErrorReason.Timeout, "Unable to execute the command, DbServer transaction/security/connectivity failure.");
        }

        private void ValidateMapAzureSqlDbException(int errorCode, DbException.ErrorReason errorReason, string errorMessage)
        {
            var sqlException = new SqlExceptionBuilder().WithErrorNumber(errorCode).WithErrorMessage("sqlException").Build();
            var azureSqlDbException = new AzureSqlDbException(errorReason, errorMessage, sqlException);
            var actual = ExceptionMapper.Map(sqlException) as AzureSqlDbException;
            Assert.IsNotNull(actual);
            Assert.AreEqual(azureSqlDbException.Message, actual.Message);
            Assert.AreEqual(azureSqlDbException.Reason, actual.Reason);
        }

        private void ValidateMapAzureSqlDbException(System.Exception exception, DbException.ErrorReason errorReason, string errorMessage)
        {
            var azureSqlDbException = new AzureSqlDbException(errorReason, errorMessage, exception);
            var actual = ExceptionMapper.Map(exception) as AzureSqlDbException;
            Assert.IsNotNull(actual);
            Assert.AreEqual(azureSqlDbException.Message, actual.Message);
            Assert.AreEqual(azureSqlDbException.Reason, actual.Reason);
        }


    }
}
