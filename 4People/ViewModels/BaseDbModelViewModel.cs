using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using _4People.Database.Models;
using _4People.Services;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace _4People.ViewModels
{
    public abstract class BaseDbModelViewModel : ReactiveObject
    {
        protected StorageFacade Facade = StorageFacade.Instance;

        protected BaseDbModelViewModel() { }

        protected BaseDbModelViewModel(BaseDbModel model)
        {
            IsChanged = model.Id == default;
        }

        [Reactive] public bool IsChanged { get; set; }

        public abstract Unit Save();
        public abstract void PrepareToSave();
        public abstract void Remove();
    }
}
