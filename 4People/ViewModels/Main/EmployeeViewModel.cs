using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using _4People.Services;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels.Main
{
    public class EmployeeViewModel : BaseDbModelViewModel
    {
        public EmployeeViewModel()
        {
            Surname = nameof(Surname);
            Name = nameof(Name);
            Patronymic = nameof(Patronymic);
            BirthDate = DateTime.Now;
            EmploymentDate = DateTime.UtcNow;
            Rank = nameof(Rank);
            Salary = 5000000.500m;
            Employee = new Employee();

            InitSubscribes();
        }

        public EmployeeViewModel(Employee employee) : base(employee)
        {
            Employee = employee;
            Surname = employee.Surname;
            Name = employee.Name;
            Patronymic = employee.Patronymic;
            BirthDate = employee.BirthDate;
            EmploymentDate = employee.EmploymentDate;
            Rank = employee.Rank;
            Salary = employee.Salary;

            InitSubscribes();
        }

        [Reactive] public Employee Employee { get; set; }
        [Reactive] public string Surname { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public string Patronymic { get; set; }
        [Reactive] public DateTime BirthDate { get; set; }
        [Reactive] public DateTime EmploymentDate { get; set; }
        [Reactive] public string Rank { get; set; }
        [Reactive] public decimal Salary { get; set; }

        public override void Save()
        {
            PrepareToSave();
            if (Employee.Id == default)
            {
                Task.Run(Add).ConfigureAwait(false);
            }
            else
            {
                Task.Run(Update).ConfigureAwait(false);
            }
        }

        public override void PrepareToSave()
        {
            Employee.Surname = Surname;
            Employee.Name = Name;
            Employee.Patronymic = Patronymic;
            Employee.BirthDate = BirthDate;
            Employee.EmploymentDate = EmploymentDate;
            Employee.Rank = Rank;
            Employee.Salary = Salary;
            Employee.Subdivision = null;
            IsChanged = true;
        }

        public override void Remove()
        {
            var employee = new Employee
            {
                Id = Employee.Id
            };

            Task.Run(async () => await Facade.EmployeeWorker.RemoveEntity(employee));
        }

        private async Task Update()
        {
            var result = await Facade.EmployeeWorker.UpdateAsync(Employee);
            Sync(result);
        }

        private async Task Add()
        {
            var result = await StorageFacade.Instance.EmployeeWorker.AddAsync(Employee);
            Sync(result);
        }

        private void Sync(bool result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsChanged = !result;
            });
        }

        private void InitSubscribes()
        {
            this.WhenAnyPropertyChanged(nameof(Name), nameof(Surname), nameof(Patronymic),
                    nameof(BirthDate), nameof(EmploymentDate), nameof(Rank), nameof(Salary))
                .WhereNotNull()
                .Subscribe(model => model.PrepareToSave());
        }
    }
}