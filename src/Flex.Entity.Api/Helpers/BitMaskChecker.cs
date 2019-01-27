using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Entity.Api.Helpers
{
   /// <summary>
   /// 
   /// </summary>
   public static class BitMaskChecker
    {
        /// <summary>
        /// Method 1 uses foreach loop to return the Enumerable
        /// </summary>
        /// <param name="mask"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> PermissionMasks<T>(Enum mask)
        {
            if (typeof(T).IsSubclassOf(typeof(Enum)) == false)
                throw new ArgumentException();

            List<T> toreturn = new List<T>(100);

            foreach (T curValueBit in Enum.GetValues(typeof(T)).Cast<T>())
            {
                Enum bit = (Enum)(object)curValueBit;

                if (mask.HasFlag(bit))
                    toreturn.Add(curValueBit);
            }

            return toreturn;
        }
        /// <summary>
        /// Method 2 seems to be more affective
        /// </summary>
        /// <param name="mask"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> PermissionMasksV2<T>(Enum mask)
        {
            if (typeof(T).IsSubclassOf(typeof(Enum)) == false)
                throw new ArgumentException();

            return Enum.GetValues(typeof(T))
                                 .Cast<Enum>()
                                 .Where(mask.HasFlag)
                                 .Cast<T>();
        }
    }
}
