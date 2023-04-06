using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels
{
    public class ReportViewModel : ReactiveObject
    {
        [Reactive] public PayrollSheetViewModel PayrollSheet { get; set; } = new();
        [Reactive] public EmployeeListViewModel EmployeeList { get; set; } = new();
    }
}
