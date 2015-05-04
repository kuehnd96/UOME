using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using System;
using System.ComponentModel;

namespace DK.UOME.Store.Windows.Tests.Fakes
{
    /// <summary>
    /// Fake entry screen for testing purposes.
    /// </summary>
    [Screen(typeof(IScreen<EntryViewModel>))]
    public sealed class FakeEntryScreen : IScreen<EntryViewModel>
    {
        public EntryViewModel ViewModel { get; set; }

        public FakeEntryScreen()
        {
            ViewModel = Initializer.GetSingleExport<EntryViewModel>();
        }

        public void End(Action completed)
        {
            completed();
        }

        public string Location
        {
            get { return "FakeEntryScreen.cs"; }
        }

        public Type ScreenType
        {
            get { return typeof(FakeEntryScreen); }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
