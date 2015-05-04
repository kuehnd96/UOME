using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using System;

namespace DK.UOME.Store.Windows.Tests.Fakes
{
    /// <summary>
    /// Fake entry group screen for testing purposes.
    /// </summary>
    [Screen(typeof(IScreen<EntryGroupViewModel>))]
    public sealed class FakeEntryGroupScreen : IScreen<EntryGroupViewModel>
    {
        public EntryGroupViewModel ViewModel { get; set; }

        public FakeEntryGroupScreen()
        {
            ViewModel = Initializer.GetSingleExport<EntryGroupViewModel>();
        }

        public void End(Action completed)
        {
            completed();
        }

        public string Location
        {
            get { return "FakeEntryGroupPage.cs"; }
        }

        public Type ScreenType
        {
            get { return typeof(FakeEntryGroupScreen); }
        }

        public void Start(Action completed)
        {
            completed();
        }

        public string Title
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
