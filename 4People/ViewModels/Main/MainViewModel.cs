using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using _4People.Services;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels.Main
{
    public class MainViewModel : ReactiveObject
    {
        private readonly StorageFacade _storageFacade = StorageFacade.Instance;

        public MainViewModel()
        {
            this.WhenAnyValue(model => model.SelectedItem).Skip(1).Subscribe(OnSelectedItemChanged);
        }

        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new();

        [Reactive] public BaseDbModelViewModel? SelectedItem { get; set; }
        [Reactive] public bool IsDatabaseLoading { get; set; }
        [Reactive] public bool IsCompanySelected { get; set; }
        [Reactive] public bool IsSubdivisionSelected { get; set; }
        [Reactive] public bool IsEmployeeSelected { get; set; }
        [Reactive] public bool IsButtonsVisible { get; set; }

        public async Task FillFields()
        {
            var companies = _storageFacade.CompanyWorker.GetFull(null).ToList();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Companies.Add(companies.Select(company => new CompanyViewModel(company)));
            });
        }

        public void AddCompany()
        {
            var company = new Company
            {
                CreationDate = DateTime.Now
            };

            var item = new CompanyViewModel(company)
            {
                IsChanged = true
            };

            Companies.Add(item);
            SelectedItem = Companies.Last();
        }

        public void Save() => SelectedItem!.Save();

        public void Remove()
        {
            SelectedItem!.Remove();
            if (IsCompanySelected)
            {
                Companies.Remove((SelectedItem as CompanyViewModel)!);
            }
            else if (IsSubdivisionSelected)
            {
                var selectedItem = (SelectedItem as SubdivisionViewModel)!;
                Companies.First(model => model.Subdivisions.Any(subdivisionModel =>
                             subdivisionModel.Subdivision.Id == selectedItem.Subdivision.Id))
                         .Subdivisions.Remove(selectedItem);
            }
            else if (IsEmployeeSelected)
            {
                var selectedItem = (SelectedItem as EmployeeViewModel)!;
                Companies.Select(model => model.Subdivisions.FirstOrDefault(subdivisionModel =>
                             subdivisionModel.Employees.Any(employeeModel =>
                                 employeeModel.Employee.Id == selectedItem.Employee.Id)))
                         .First(model => model != null)!.Employees.Remove(selectedItem);
            }

            Save();
        }

        private void OnSelectedItemChanged(BaseDbModelViewModel? obj)
        {
            IsCompanySelected = obj is CompanyViewModel;
            IsSubdivisionSelected = obj is SubdivisionViewModel;
            IsEmployeeSelected = obj is EmployeeViewModel;
            IsButtonsVisible = IsCompanySelected || IsSubdivisionSelected || IsEmployeeSelected;
        }
    }
}