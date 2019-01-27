using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace Flex.Entity.DapperRepository
{
  
        /// <summary>
        /// Additional extensions for Dapper
        /// </summary>
        public static class AdditionalDapperExtensions
    {
            #region Trim Results

            public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            {
                var dapperResult = SqlMapper.Query<T>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
                var result = TrimStrings(dapperResult.ToList());
                return result;
            }

            public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                var dapperResult = SqlMapper.Query<TFirst, TSecond, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
                var result = TrimStrings(dapperResult.ToList());
                return result;
            }

        #endregion Trim Results

        public static Task<int> SetSecurityContextAsync(this IDbConnection cnn, string assetRootValue, int assetPermissionValue, string serviceRootValue, int servicePermissionValue, string userValue )
        {
            var sql =$@"INSERT INTO [Security].[vwContextInfo] ([KEY],[VALUE]) 
                                VALUES('{ClaimKeyEnum.AR}', '{assetRootValue}')
	                                   ,('{ClaimKeyEnum.AP}', '{assetPermissionValue}')
	                                   ,('{ClaimKeyEnum.SR}', '{serviceRootValue}')
	                                   ,('{ClaimKeyEnum.SP}', '{servicePermissionValue}')
	                                   ,('{ClaimKeyEnum.U}', '{userValue}');";
            return cnn.ExecuteAsync(sql);
        }

        #region Private Methods

        private static IEnumerable<T> TrimStrings<T>(IList<T> objects)
            {
                var publicInstanceStringProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite);

                foreach (var prop in publicInstanceStringProperties)
                {
                    foreach (var obj in objects)
                    {
                        var value = (string)prop.GetValue(obj, null);
                        var trimmedValue = value.SafeTrim();
                        prop.SetValue(obj, trimmedValue, null);
                    }
                }

                return objects;
            }

            private static string SafeTrim(this string source)
            {
                if (source == null)
                {
                    return null;
                }
                return source.Trim();
            }

            #endregion Private Methods
        }
}


