using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels.Report
{
    public class ReportViewModel : ReactiveObject
    {
        [Reactive] public PayrollSheetViewModel PayrollSheet { get; set; } = new();
        [Reactive] public EmployeeListViewModel EmployeeList { get; set; } = new();
    }
}
