using System;

namespace Flex.Entity.JWTSecurity.JWTModel.Claims
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum PermissionMask

    {

        /// <summary>
        /// 
        /// </summary>
        Execute=0x01,
        /// /// <summary>
        /// 
        /// </summary>
        Update = 0x02,
        /// /// <summary>
        /// 
        /// </summary>
        Create=0x04,
        /// <summary>
        /// 
        /// </summary>
        Read=0x08,
        /// <summary>
        /// 
        /// </summary>
        FullAccess=0x20
   
            
    }
}