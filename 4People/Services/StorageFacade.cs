using System.Threading.Tasks;
using _4People.Database.Models;

namespace _4People.Services
{
    public class StorageFacade
    {
        private StorageFacade() { }

        public static StorageFacade Instance { get; } = new();

        public CompanyWorker CompanyWorker { get; private set; } = null!;
        public SubdivisionWorker SubdivisionWorker { get; private set; } = null!;
        public EmployeeWorker EmployeeWorker { get; private set; } = null!;
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