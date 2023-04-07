using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using _4People.Enums;
using _4People.Models;
using _4People.Services;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels.Report
{
    public class EmployeeListViewModel : ReactiveObject
    {
        private const int RetirementAge = 65;
        private const int YearOfMajority = 18;
        private readonly StorageFacade _facade = StorageFacade.Instance;

        public EmployeeListViewModel()
        {
            InitSubscribes();
            GetFilterData();
        }

        public ObservableCollection<int> YearFilter { get; set; } = new();
        public ObservableCollection<int> AgeFilter { get; set; } = new();
        public ObservableCollection<EmployeeInfo> Employees { get; set; } = new();

        [Reactive] public int? SelectedFilter { get; set; }
        [Reactive] public FilterType SelectedFilterType { get; set; }
        [Reactive] public bool IsSearching { get; set; }

        public Expression<Func<Employee, bool>>? GetFilter(int? filterValue) =>
            filterValue is not null ?
                SelectedFilterType is FilterType.ByAge ?
                    employee => DateTime.Today.Year - employee.BirthDate.Year == filterValue :
                    employee => employee.BirthDate.Year == filterValue :
                null;

        private void InitSubscribes()
        {
            this.WhenAnyValue(model => model.SelectedFilter)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(async filterValue => await GetEmployees(filterValue));

            this.WhenAnyValue(model => model.SelectedFilterType)
                .Throttle(TimeSpan.FromSeconds(1))
                .Where(type => type is FilterType.None)
                .Subscribe(async _ => await GetEmployees());
        }

        private async Task GetEmployees(int? filterValue = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsSearching = true;
            });

            var employees = _facade.EmployeeWorker.GetFull(GetFilter(filterValue));
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Employees.Clear();
                Employees.Add(employees.Select(employee => new EmployeeInfo
                {
                    SubdivisionName = employee.Subdivision!.Name,
                    CompanyName = employee.Subdivision.Company!.Name,
                    FulName = $"{employee.Surname} {employee.Name} {employee.Patronymic}",
                    Age = DateTime.Today.Year - employee.BirthDate.Year,
                    YearsInCompany = DateTime.Today.Year - employee.EmploymentDate.Year
                }));

                IsSearching = false;
            });
        }

        private void GetFilterData()
        {
            for (var i = YearOfMajority; i <= RetirementAge; i++)
            {
                AgeFilter.Add(i);
                YearFilter.Add(DateTime.Now.Year - i);
            }
        }
    }
}