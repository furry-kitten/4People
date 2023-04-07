using System.Collections.Generic;
using System.Linq;
using _4People.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace _4People.Extensions
{
    internal static class EfContextExtensions
    {
        public static void DetachAllEntries<T>(this DbContext context,
            IEnumerable<T?>? attachedObjects)
            where T : BaseDbModel
        {
            if (attachedObjects == null || attachedObjects.All(model => model == null))
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