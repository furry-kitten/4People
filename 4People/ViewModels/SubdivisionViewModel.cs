using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels
{
    public class SubdivisionViewModel : BaseDbModelViewModel
    {
        public SubdivisionViewModel()
        {
            Name = "11111111111111111";
            Leader = "2212121121212";
            Subdivision = new Subdivision();

            InitSubscribes();
        }

        public SubdivisionViewModel(Subdivision subdivision) : base(subdivision)
        {
            Subdivision = subdivision;
            Name = subdivision.Name;
            Leader = subdivision.Leader?.Name ?? string.Empty;
            if (subdivision.Employees?.Any() == true)
            {
                Employees.Add(
                    subdivision.Employees.Select(employee => new EmployeeViewModel(employee)));
            }

            InitSubscribes();
        }

        [Reactive] public Subdivision Subdivision { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public string Leader { get; set; }

        public ObservableCollection<EmployeeViewModel> Employees { get; set; } = new();

        public override Unit Save()
        {
            PrepareToSave();
            if (Subdivision.Id == default)
            {
                Task.Run(Add).ConfigureAwait(false);
            }
            else
            {
                Task.Run(Update).ConfigureAwait(false);
            }

            return Unit.Default;
        }

        public override void PrepareToSave()
        {
            Subdivision.Name = Name;
            Subdivision.LeaderId = Subdivision.LeaderId;
            IsChanged = true;
            foreach (var model in Employees)
            {
                model.PrepareToSave();
            }

            Subdivision.Employees = Subdivision.Id == default ?
                Employees.Select(model => model.Employee).ToList() :
                Employees.Where(model => model.IsChanged).Select(model => model.Employee).ToList();

            Subdivision.Leader = null;
        }

        public override void Remove()
        {
            Subdivision.Company = null;
            Task.Run(async () => await Facade.SubdivisionWorker.RemoveEntity(new Subdivision(){Id = Subdivision.Id}))
                .ConfigureAwait(false);
        }

        public void AddEmployee()
        {
            Employee employee = new()
            {
                EmploymentDate = DateTime.Now,
                SubdivisionId = Subdivision.Id,
                Rank = string.Empty,
                Name = string.Empty,
                Patronymic = string.Empty,
                Surname = string.Empty
            };

            var item = new EmployeeViewModel(employee)
            {
                IsChanged = true
            };

            Employees.Add(item);

            IsChanged = true;
        }

        private async Task Update()
        {
            var result = await Facade.SubdivisionWorker.UpdateAsync(Subdivision);
            Sync(result);
        }

        private async Task Add()
        {
            var result = await Facade.SubdivisionWorker.AddAsync(Subdivision);
            Sync(result);
        }

        private void Sync(bool result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsChanged = !result;
                foreach (var changedModel in Employees.Where(model => model.IsChanged))
                {
                    changedModel.IsChanged = !result;
                }
            });
        }

        private void InitSubscribes()
        {
            this.WhenAnyPropertyChanged(nameof(Name), nameof(Leader))
                .WhereNotNull()
                .Subscribe(model => model.PrepareToSave());

            foreach (var employee in Employees)
            {
                employee.WhenAnyValue(model => model.IsChanged)
                        .Subscribe(result =>
                        {
                            IsChanged = result;
                            if (result)
                            {
                                PrepareToSave();
                            }
                        });
            }

            Employees.ObserveCollectionChanges()
                     .Skip(1)
                     .Subscribe(_ =>
                     {
                         Subdivision.Employees = Subdivision.Id == default ?
                             Employees.Select(model => model.Employee).ToList() :
                             Employees.Where(model => model.IsChanged)
                                      .Select(model => model.Employee)
                                      .ToList();

                         foreach (var model in Employees.Where(model => model.IsChanged))
                         {
                             model.WhenAnyValue(viewModel => viewModel.IsChanged)
                                  .Subscribe(result =>
                                  {
                                      IsChanged = result;
                                      if (result)
                                      {
                                          PrepareToSave();
                                      }
                                  });
                         }

                         IsChanged = true;
                     });
        }
    }
}