
using System.Runtime.Serialization;
using Flex.Entity.Repository;

namespace Flex.Entity.DapperRepository.Exception
{
    public class DapperDbException : DbException
    {
        public DapperDbException(ErrorReason errorReason, int extendedReason = 0) : base(errorReason, extendedReason)
        {
        }

        public DapperDbException(ErrorReason errorReason, string message, int extendedReason = 0) : base(errorReason, message, extendedReason)
        {
        }

        public DapperDbException(ErrorReason errorReason, SerializationInfo info, StreamingContext context, int extendedReason = 0) : base(errorReason, info, context, extendedReason)
        {
        }

        public DapperDbException(ErrorReason errorReason, string message, System.Exception exception, int extendedReason = 0) : base(errorReason, message, exception, extendedReason)
        {
        }
    }
}
