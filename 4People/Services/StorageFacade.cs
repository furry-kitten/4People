using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _4People.Database.Models;
using _4People.Models;

namespace _4People.Services
{
    public class StorageFacade
    {
        private StorageFacade() { }

        public static StorageFacade Instance { get; } = new();

        public CompanyWorker CompanyWorker { get; private set; }
        public SubdivisionWorker SubdivisionWorker { get; private set; }
        public EmployeeWorker EmployeeWorker { get; private set; }
        public bool IsInited { get; set; }

        public async Task Init()
        {
            var context = await BaseDbWorker<Company>.InitAsync();
            IsInited = context != null;
            if (!IsInited)
            {
                return;
            }

            CompanyWorker = new CompanyWorker(context!);
            SubdivisionWorker = new SubdivisionWorker(context!);
            EmployeeWorker = new EmployeeWorker(context!);
        }
    }
}