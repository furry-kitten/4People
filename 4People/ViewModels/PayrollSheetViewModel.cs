using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using _4People.Models;
using _4People.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels
{
    public class PayrollSheetViewModel : ReactiveObject
    {
        private readonly StorageFacade _facade = StorageFacade.Instance;

        public PayrollSheetViewModel()
        {
            Sheet.ObserveCollectionChanges()
                 .Throttle(TimeSpan.FromSeconds(1))
                 .Subscribe(p => ResetSheets());

            Task.Run(UpdateData);
        }

        [Reactive] public ObservableCollection<PayrollSheet> Sheet { get; set; } = new();
        [Reactive] public ObservableCollection<PayrollSheetRow> FullSheet { get; set; } = new();
        [Reactive] public ObservableCollection<TotalsRow> SubdivisionsTotals { get; set; } = new();
        [Reactive] public ObservableCollection<TotalsRow> CompaniesTotals { get; set; } = new();

        public async Task<List<PayrollSheet>> GetPayrollSheet()
        {
            List<PayrollSheet> sheets = new();
            var companiesPayrollSheet = _facade.CompanyWorker.GetFull();
            await foreach (var company in companiesPayrollSheet)
            {
                PayrollSheet sheet = new()
                {
                    Company = company,
                    Subdivisions = GetAllCompanySubdivisions(company),
                    Employees = GetAllCompanyEmployees(company)
                };

                sheets.Add(sheet);
            }

            return sheets;
        }

        public async Task UpdateData()
        {
            var payrollSheets = await GetPayrollSheet();
            payrollSheets.ForEach(sheet =>
            {
                sheet.Total = sheet.Employees.Sum(employee => employee.Salary);
                sheet.SubdivisionsResults = sheet.Employees
                                                 .GroupBy(employee =>
                                                     sheet.Subdivisions.First(subdivision =>
                                                         subdivision.Id == employee.SubdivisionId))
                                                 .ToDictionary(grouping => grouping.Key,
                                                     grouping => grouping.Sum(employee =>
                                                         employee.Salary));
            });

            Application.Current.Dispatcher.Invoke(() => Sheet.Add(payrollSheets));
        }

        private static List<Subdivision> GetAllCompanySubdivisions(Company company) =>
            company.Subdivisions?.ToList() ?? new List<Subdivision>();

        private static List<Employee> GetAllCompanyEmployees(Company company)
        {
            return company.Subdivisions
                          ?.Select(subdivision => subdivision.Employees ?? new List<Employee>())
                          .Where(employees => employees.Any())
                          .SelectMany(employees => employees)
                          .ToList() ??
                   new List<Employee>();
        }

        private void ResetSheets()
        {
            var items = Sheet.SelectMany(sheet => sheet.Employees)
                             .Select(employee => new PayrollSheetRow
                             {
                                 Company = employee.Subdivision.Company.Name,
                                 EmployeeFullName = $"{employee.Surname} {employee.Name} {employee.Patronymic}",
                                 SubdivisionName = employee.Subdivision.Name,
                                 Salary = employee.Salary
                             });

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FullSheet.Clear();
                FullSheet.Add(items);
            });

            var subdivisionsResults = Sheet.SelectMany(sheet => sheet.SubdivisionsResults)
                                           .Select(pair => new TotalsRow()
                                           {
                                               Name = pair.Key.Name,
                                               Total = pair.Value
                                           });

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                SubdivisionsTotals.Clear();
                SubdivisionsTotals.Add(subdivisionsResults);
            });

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CompaniesTotals.Clear();
                CompaniesTotals.Add(Sheet.Select(sheet => new TotalsRow()
                {
                    Name = sheet.Company.Name,
                    Total = sheet.Total,
                }));
            });
        }
    }
}