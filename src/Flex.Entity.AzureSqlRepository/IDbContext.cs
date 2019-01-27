﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Flex.Entity.AzureSqlRepository
{
    public interface IDbContext
    {

        IDbSetWrapper<T> Set<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbEntityEntry Entry(object o);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        void Dispose();
    }

   
}