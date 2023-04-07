using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using _4People.Database;
using _4People.Database.Models;
using _4People.Extensions;
using Microsoft.EntityFrameworkCore;

namespace _4People.Services
{
    public abstract class BaseDbWorker<TEntity>
        where TEntity : BaseDbModel
    {
        protected static readonly SemaphoreSlim DbLocker = new(1);
        protected EfContext Context;
        protected DbSet<TEntity> Entity;

        protected BaseDbWorker(EfContext context)
        {
            Context = context;
            Entity = context.Set<TEntity>();
        }

        public static async Task<EfContext?> InitAsync()
        {
            EfContext? context = null;
            await DbLocker.WaitHandleAsync(async () =>
                context = await EfContext.CreateInstanceAsync().ConfigureAwait(false));

            return context!;
        }

        public abstract IEnumerable<TEntity> GetFull(Expression<Func<TEntity, bool>>? predicate);

        public async Task RemoveEntity(TEntity entity)
        {
            await DbLocker.WaitHandleAsync(async () =>
            {
                Entity.Remove(entity);
                await Context.SaveChangesAsync();
            });
        }

        protected async Task UpdateEntityAsync(TEntity entity)
        {
            Context.Attach(entity);
            var entityEntry = Context.Entry(entity);
            entityEntry.State = EntityState.Modified;
            Context.Update(entity);
            await Context.SaveChangesAsync();
            Context.Detach(entity);
        }

        protected async Task<TEntity> AddEntityAsync(TEntity entity) =>
            (await Context.AddAsync(entity)).Entity;

        internal async Task<TEntity?> Get(Guid id) => await Entity.FindAsync(id);
    }
}