using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using _4People.Database;
using _4People.Database.Models;
using _4People.Extensions;
using Microsoft.EntityFrameworkCore;

namespace _4People.Services
{
    public class SubdivisionWorker : BaseDbWorker<Subdivision>
    {
        internal SubdivisionWorker(EfContext context) : base(context) { }

        public async Task<bool> UpdateAsync(Subdivision subdivision)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    await UpdateEntityAsync(subdivision);
                    await Context.SaveChangesAsync();
                    Context.DetachAllEntries(subdivision.Employees);
                    Context.Detach(subdivision.Leader);
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AddAsync(Subdivision subdivision)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    subdivision = await AddEntityAsync(subdivision);
                    var saveChangesAsync = await Context.SaveChangesAsync();
                    Context.DetachAllEntries(subdivision.Employees);
                    Context.Detach(subdivision.Leader);
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public override IEnumerable<Subdivision> GetFull(
            Expression<Func<Subdivision, bool>>? predicate)
        {
            var collection = Entity.Include(subdivision => subdivision.Employees)
                                   .Include(subdivision => subdivision.Company)
                                   .Include(subdivision => subdivision.Leader)
                                   .AsNoTracking();

            if (predicate != null)
            {
                collection = collection.Where(predicate);
            }

            return collection.AsEnumerable();
        }
    }
}