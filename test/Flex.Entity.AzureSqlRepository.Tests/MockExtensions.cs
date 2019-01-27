using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Flex.Entity.AzureSqlRepository.Tests
{
    public static class MockExtensions
    {
        public enum ChangesOperations { Add, Update, Delete }
        public static void SetupIQueryable<T>(this Mock<T> mock, IQueryable queryable)
            where T : class, IQueryable
        {
            mock.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
            mock.Setup(r => r.Provider).Returns(queryable.Provider);
            mock.Setup(r => r.ElementType).Returns(queryable.ElementType);
            mock.Setup(r => r.Expression).Returns(queryable.Expression);
        }

        public static void SetupDbSetAsync<T>(this Mock<IDbSetWrapper<T>> mockSet, Mock<IDbContext> mockContext, List<T> database) where T:class
        {
            mockContext.Setup(x => x.Set<T>()).Returns(mockSet.Object);
            mockSet.As<IDbAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<T>(database.GetEnumerator()));
            mockSet.As<IQueryable<T>>().SetupIQueryableAsync(database.AsQueryable());
        }

        public static void SetupAddAsync<T>(this Mock<IDbSetWrapper<T>> mockSet, Mock<IDbContext> mockContext, List<T> buffer, List<ChangesOperations> operations) where T : class
        {
            mockSet.As<IDbSet<T>>().Setup(x => x.Add(It.IsAny<T>())).Returns<T>(et =>
            {
                buffer.Add(et);
                operations.Add(ChangesOperations.Add);
                return et;
            });
        }

        public static void SetupDeleteAsync<T>(this Mock<IDbSetWrapper<T>> mockSet, Mock<IDbContext> mockContext, List<T> buffer, List<ChangesOperations> operations) where T : class
        {
            mockSet.As<IDbSet<T>>().Setup(x => x.Remove(It.IsAny<T>())).Returns<T>(et =>
            {
                buffer.Add(et);
                operations.Add(ChangesOperations.Delete);
                return et;
            });
        }


        //public static void SetupSaveChangesAsync<T>(this Mock<IDbContext> mockContext, List<T> database, List<T> buffer, List<ChangesOperations> operations) where T : class 
        //{
        //    mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct =>
        //    {
        //        GetCallbackOperations(database, buffer, operations);
        //    }).Returns(Task.FromResult(GetCallbackOperations(database, buffer, operations)));
        //}

        public static void SetupSaveChangesAsync<T>(this Mock<IDbContext> mockContext, List<T> database, List<T> buffer, List<ChangesOperations> operations) where T : class
        {
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns( ()=> Task.FromResult(GetCallbackOperations(database, buffer, operations)));
        }

        private static int GetCallbackOperations<T>(List<T> database, List<T> buffer, List<ChangesOperations> operations) where T : class
        {
            int count = 0;
            
            for (var i = 0; i < operations.Count; i++)
            {
                var o = operations[i];
                T b = buffer[i];
                switch (o)
                {
                    case ChangesOperations.Add:
                        count++;
                        if (IsDuplicated(database, b)) throw new System.Exception("Unique Key Constratint!");
                        database.Add(b);
                        break;
                    case ChangesOperations.Update:
                        count++;
                        var index = database.FindIndex(x => x.Equals(buffer));
                        database[index] = b;
                        break;
                    case ChangesOperations.Delete:
                        count++;
                        database.Remove(b);
                        break;
                }
            }
            buffer.Clear();
            return count;
        }

        private static bool IsDuplicated<T>(List<T> database, T buffer) where T :class
        {
            return database.Any(d => IsMatched(d, buffer));
        }

        private static bool IsMatched<T>( T database, T buffer) where T: class
        {
            var properties = GetUniqueKeyProperties<T>();
            
            foreach (var p in properties)
            {
               var dbValue = GetInstanceField(typeof(T), database, p.Name);
               var bufferValue = GetInstanceField(typeof(T), buffer, p.Name);
                if (dbValue == bufferValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetupIQueryableAsync<T>(this Mock<T> mock, IQueryable queryable) where T : class, IQueryable
        {
            mock.Setup(r => r.Provider).Returns(new MockDbAsyncQueryProvider<T>(queryable.Provider));
            mock.Setup(r => r.ElementType).Returns(queryable.ElementType);
            mock.Setup(r => r.Expression).Returns(queryable.Expression);
            mock.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
        }

        public static object GetInstanceField(Type type, object instance, string properityName)
        {
            var property = instance.GetType().GetProperty(properityName);
            return property != null ? property.GetValue(instance) : null;
        }

        private static List<PropertyInfo> GetUniqueKeyProperties<T>() where T : class
        {
            var properityInfo = new List<PropertyInfo>();
            foreach (var prop in typeof(T).GetProperties())
            {
                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    var indexAttribute = attr as IndexAttribute;
                    if (indexAttribute == null) continue;
                    if (indexAttribute.IsUnique)
                    {
                        properityInfo.Add(prop);
                    }
                }
            }
            return properityInfo;
            
        }

        private static string GetIns<T>() where T : class
        {
            string propertieNameWithKey = string.Empty;

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    var keyAttribute = attr as KeyAttribute;
                    if (keyAttribute != null)
                    {
                        propertieNameWithKey = prop.Name;
                        break;
                    }
                }
            }
            return propertieNameWithKey;
        }
    }
}