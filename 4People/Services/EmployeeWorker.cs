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
    public class EmployeeWorker : BaseDbWorker<Employee>
    {
        public EmployeeWorker(EfContext context) : base(context) { }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    await UpdateEntityAsync(employee);
                    Context.Detach(employee.Subdivision);
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AddAsync(Employee employee)
        {
            try
            {
                await DbLocker.WaitHandleAsync(async () =>
                {
                    employee = await AddEntityAsync(employee);
                    await Context.SaveChangesAsync();
                    Context.Detach(employee.Subdivision);
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public override IEnumerable<Employee> GetFull(Expression<Func<Employee, bool>>? predicate)
        {
            var collection = Entity.Include(employee => employee.Subdivision)
                                   .ThenInclude(subdivision => subdivision.Company)
                                   .AsNoTracking();

            if (predicate != null)
            {
                collection = collection.Where(predicate);
            }

            return collection.AsEnumerable();
        }
    }
}