using _4People.Database.Models;
using _4People.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels.Main
{
    public abstract class BaseDbModelViewModel : ReactiveObject
    {
        protected StorageFacade Facade = StorageFacade.Instance;

        protected BaseDbModelViewModel() { }

        protected BaseDbModelViewModel(BaseDbModel model) => IsChanged = model.Id == default;

        [Reactive] public bool IsChanged { get; set; }

        public abstract void Save();
        public abstract void PrepareToSave();
        public abstract void Remove();
    }
}