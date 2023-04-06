using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using _4People.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace _4People.Extensions
{
    internal static class EfContextExtensions
    {
        public static DbContext IsModified<TEntity>(this DbContext context,
            TEntity entity,
            params Expression<Func<TEntity, object>>[] predicates)
            where TEntity : BaseDbModel
        {
            foreach (var predicate in predicates)
            {
                context.Entry(entity).Property(predicate).IsModified = true;
            }

            return context;
        }

        public static void DetachAllEntries<T>(this DbContext context,
            IEnumerable<T>? attachedObjects)
            where T : BaseDbModel
        {
            if (attachedObjects == null)
            {
                return;
            }

            foreach (var entry in attachedObjects)
            {
                context.Entry(entry!).State = EntityState.Detached;
            }
        }

        public static void Detach<T>(this DbContext context, T? entry)
            where T : BaseDbModel
        {
            if (entry == null)
            {
                return;
            }

            context.Entry(entry).State = EntityState.Detached;
        }
    }
}