using System;
using System.Linq;
using System.Security.Claims;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;
using Flex.Entity.JWTSecurity.JWTModel.Claims;
using Flex.Entity.Security;
using Newtonsoft.Json;

namespace Flex.Entity.JWTSecurity
{
    /// <summary>
    /// </summary>
    public static class TokenParser
    {
        /// <summary>
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static UserMetadata GetUserMetadata(ClaimsIdentity claims)
        {
            try
            {
                var userMetadataClaim = claims.Claims.Single(c => c.Type == "user_metadata").Value;
                if (userMetadataClaim != null)
                {
                    var userMetadata = JsonConvert.DeserializeObject<UserMetadata>(userMetadataClaim);
                    return userMetadata;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static AppMetadata GetAppMetadata(ClaimsIdentity claims)
        {
            try
            {
                var appMetadataClaim = claims.Claims.Single(c => c.Type == "app_metadata").Value;
                if (appMetadataClaim != null)
                {
                    var appMetadata = JsonConvert.DeserializeObject<AppMetadata>(appMetadataClaim);
                    if(appMetadata != null  && appMetadata.asset != null)
                        appMetadata.asset.permission = appMetadata.asset.permission ?? (int)PermissionMask.FullAccess;
                    if (appMetadata != null && appMetadata.service != null)
                        appMetadata.service.permission = appMetadata.service.permission ?? (int)PermissionMask.FullAccess;
                    return appMetadata;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static AppMetadata GetAppMetadata(TokenModel rootTokenModel)
        {
          
            if (rootTokenModel.app_metadata != null)
            {
                AppMetadata metadata = new AppMetadata();
                if (rootTokenModel.app_metadata.asset != null) metadata.asset = rootTokenModel.app_metadata.asset; else return null;
                if (rootTokenModel.app_metadata.service != null) metadata.service = rootTokenModel.app_metadata.service;
                return metadata;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static TokenModel GetClaimsRootValue(ClaimsIdentity claims)
        {
            try
            {
                var rootObject = new TokenModel();

                    rootObject.picture = claims.Claims.SingleOrDefault(c => c.Type == "picture")?.Value??string.Empty;
                    rootObject.nickname = claims.Claims.SingleOrDefault(c => c.Type == "nickname")?.Value??string.Empty;
                    rootObject.user_id = (claims.Claims.SingleOrDefault(c => c.Type == "user_id")?.Value??string.Empty);
              
                rootObject.user_metadata =
                    JsonConvert.DeserializeObject<UserMetadata>(
                        claims.Claims.SingleOrDefault(c => c.Type == "user_metadata")?.Value ?? string.Empty
                        );
                rootObject.email= claims.Claims.SingleOrDefault(c => c.Type == "email")?.Value ?? string.Empty;

                rootObject.email_verified =
                    Convert.ToBoolean(claims.Claims.SingleOrDefault(c => c.Type == "email_verified")?.Value??false.ToString());
               
                    var optionalIdentity =
                        JsonConvert.DeserializeObject<Identity>(claims.Claims.SingleOrDefault(c => c.Type == "identities")?.Value??string.Empty);
                    if (optionalIdentity != null) rootObject.Identitiy = optionalIdentity;
             
                    //do nothing as the idendities may or may not be present in the claims /token and might not be valid as this point
               

              
                    rootObject.updated_at = (claims.Claims.SingleOrDefault(c => c.Type == "updated_at")?.Value??string.Empty);
                    rootObject.created_at = (claims.Claims.SingleOrDefault(c => c.Type == "created_at")?.Value??string.Empty);
                    rootObject.global_client_id = (claims.Claims.SingleOrDefault(c => c.Type == "global_client_id")?.Value??string.Empty);
               

                return rootObject;
            }
            catch (Exception)
            {
                //convert exception to a null value
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>

        public static FlexSecurityContext GetSecurityContext(ClaimsIdentity claims)
        {
            try
            {

                FlexSecurityContext tokenInfoForRespository = new FlexSecurityContext();
                tokenInfoForRespository.UserName = claims.Claims.SingleOrDefault(c => c.Type == "name")?.Value??string.Empty;
                 tokenInfoForRespository.UserId = claims.Claims.SingleOrDefault(c => c.Type == "user_id")?.Value??string.Empty;
                    tokenInfoForRespository.NickName = claims.Claims.SingleOrDefault(c => c.Type == "nickname")?.Value??string.Empty;
               
                  
                    tokenInfoForRespository.EmailId = claims.Claims.Single(c => c.Type == "email").Value;
                    if (string.IsNullOrWhiteSpace(tokenInfoForRespository.EmailId))
                        return null;

                // tokenInfoForRespository.TokenRepository= JsonConvert.DeserializeObject<AppMetadata>(claims.Claims.Single(c => c.Type == "app_metadata").Value);
               AppMetadata metadata = JsonConvert.DeserializeObject<AppMetadata>(claims.Claims.Single(c => c.Type == "app_metadata").Value);
                tokenInfoForRespository.AssetPermission = metadata.asset.permission ?? (int)PermissionMask.FullAccess;
                tokenInfoForRespository.AssetRoot = metadata.asset.root;
                tokenInfoForRespository.ServicePermission = metadata.service.permission ?? (int)PermissionMask.FullAccess;
                tokenInfoForRespository.ServiceRoot = metadata.service.root;
               
            
                    return tokenInfoForRespository;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static FlexSecurityContext GetSecurityContext(string key,ClaimsIdentity claims)
        {
            FlexSecurityContext flexSecurityContext = JsonConvert.DeserializeObject<FlexSecurityContext>(claims.Claims.Single(c => c.Type == key).Value);

            return flexSecurityContext;
        }
    }
}