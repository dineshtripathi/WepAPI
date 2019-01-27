using System.Runtime.Serialization;
using Flex.Entity.Repository;

namespace Flex.Entity.AzureSqlRepository.Exception
{
    public class AzureSqlDbException: DbException
    {
        public AzureSqlDbException(ErrorReason errorReason, int extendedReason = 0) : base(errorReason,extendedReason)
        {
        }

        public AzureSqlDbException(ErrorReason errorReason, string message, int extenedReason = 0) : base(errorReason, message, extenedReason)
        {
        }

        public AzureSqlDbException(ErrorReason errorReason, SerializationInfo info, StreamingContext context, int extenedReason = 0) : base(errorReason, info, context, extenedReason)
        {
        }

        public AzureSqlDbException(ErrorReason errorReason, string message, System.Exception exception, int extenedReason = 0) : base(errorReason, message, exception, extenedReason)
        {
        }
    }
}
