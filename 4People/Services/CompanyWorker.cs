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
    public class CompanyWorker : BaseDbWorker<Company>
    {
        internal CompanyWorker(EfContext context) : base(context) { }

        public async Task<bool> UpdateAsync(Company company)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    await UpdateEntityAsync(company);
                    Context.DetachAllEntries(company.Subdivisions);
                    Context.DetachAllEntries(
                        company.Subdivisions?.SelectMany(subdivision => subdivision.Employees));

                    Context.DetachAllEntries(
                        company.Subdivisions?.Select(subdivision => subdivision.Leader));
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AddAsync(Company company)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    company = await AddEntityAsync(company);
                    await Context.SaveChangesAsync();
                    Context.DetachAllEntries(company.Subdivisions);
                    Context.DetachAllEntries(
                        company.Subdivisions?.SelectMany(subdivision => subdivision.Employees));
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public override IEnumerable<Company> GetFull(Expression<Func<Company, bool>>? predicate)
        {
            var collection = Entity.Include(company => company.Subdivisions)
                                   .ThenInclude(subdivision => subdivision.Employees)
                                   .Include(company => company.Subdivisions)
                                   .ThenInclude(subdivision => subdivision.Leader)
                                   .AsNoTracking();

            if (predicate != null)
            {
                collection = collection.Where(predicate);
            }

            return collection.AsEnumerable();
        }

        public IAsyncEnumerable<Company> GetFull()
        {
            var collection = Entity.Include(company => company.Subdivisions)
                                   .ThenInclude(subdivision => subdivision.Employees)
                                   .AsNoTracking();

            return collection.AsAsyncEnumerable();
        }
    }
}