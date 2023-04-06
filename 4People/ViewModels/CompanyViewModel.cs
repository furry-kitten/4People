using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using _4People.Database.Models;
using _4People.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels
{
    public class CompanyViewModel : BaseDbModelViewModel
    {
        public CompanyViewModel()
        {
            Company = new Company
            {
                Subdivisions = new List<Subdivision>()
            };

            Name = nameof(Name);
            CreationDate = DateTime.Now;
            Address = nameof(Address);
            Subdivisions.Add(Company.Subdivisions.Select(subdivision =>
                new SubdivisionViewModel(subdivision)));

            InitSubscribes();
        }

        public CompanyViewModel(Company company) : base(company)
        {
            Company = company;
            Name = company.Name;
            CreationDate = company.CreationDate;
            Address = company.Address;
            if (company.Subdivisions?.Any() == true)
            {
                Subdivisions.Add(company.Subdivisions.Select(subdivision =>
                    new SubdivisionViewModel(subdivision)));
            }

            InitSubscribes();
        }

        [Reactive] public Company Company { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public DateTime CreationDate { get; set; }
        [Reactive] public string Address { get; set; }

        public ObservableCollection<SubdivisionViewModel> Subdivisions { get; set; } = new();

        public override Unit Save()
        {
            PrepareToSave();
            if (Company.Id == default)
            {
                Task.Run(Add).ConfigureAwait(false);
            }
            else
            {
                Task.Run(Update).ConfigureAwait(false);
            }

            foreach (var employee in Subdivisions.Where(model => model.IsChanged).SelectMany(model => model.Employees.Where(employee => employee.IsChanged)))
            {
                employee.IsChanged = false;
            }

            return Unit.Default;
        }

        public override void PrepareToSave()
        {
            Company.Name = string.IsNullOrWhiteSpace(Name) ? Company.Name : Name;
            Company.CreationDate = CreationDate;
            Company.Address = string.IsNullOrWhiteSpace(Address) ? Company.Address : Address;
            foreach (var model in Subdivisions)
            {
                model.PrepareToSave();
            }

            Company.Subdivisions = Company.Id == default ?
                Subdivisions.Select(model => model.Subdivision).ToList() :
                Subdivisions.Where(model => model.IsChanged).Select(model => model.Subdivision).ToList();
        }

        public override void Remove()
        {
            Task.Run(async () => await Facade.CompanyWorker.RemoveEntity(new Company() {Id = Company.Id}));
        }

        public void AddSubdivision()
        {
            Subdivision subdivision = new()
            {
                CompanyId = Company.Id
            };

            var item = new SubdivisionViewModel(subdivision)
            {
                IsChanged = true
            };

            Subdivisions.Add(item);
        }

        private async Task Update()
        {
            var result = await Facade.CompanyWorker.UpdateAsync(Company);
            Sync(result);
        }

        private async Task Add()
        {
            var result = await StorageFacade.Instance.CompanyWorker.AddAsync(Company);
            Sync(result);
        }

        private void Sync(bool result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsChanged = !result;
                foreach (var changedModel in Subdivisions.Where(model => model.IsChanged))
                {
                    changedModel.IsChanged = !result;
                }
            });
        }

        private void InitSubscribes()
        {
            this.WhenAnyPropertyChanged(nameof(Name), nameof(CreationDate), nameof(Address))
                .WhereNotNull()
                .Subscribe(model => model.PrepareToSave());

            foreach (var subdivision in Subdivisions)
            {
                subdivision.WhenAnyValue(model => model.IsChanged)
                           .Skip(1)
                           .Subscribe(result =>
                           {
                               IsChanged = result;
                               if (result)
                               {
                                   PrepareToSave();
                               }
                           });
            }

            Subdivisions.ObserveCollectionChanges()
                        .Skip(1)
                        .Subscribe(_ =>
                        {
                            Company.Subdivisions = Company.Id == default ?
                                Subdivisions.Select(model => model.Subdivision).ToList() :
                                Subdivisions.Where(model => model.IsChanged)
                                            .Select(model => model.Subdivision)
                                            .ToList();

                            foreach (var model in Subdivisions.Where(model => model.IsChanged))
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