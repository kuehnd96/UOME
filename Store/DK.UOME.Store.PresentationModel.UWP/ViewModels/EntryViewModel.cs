using AutoMapper;
using DK.Framework.UWP.Commands;
using DK.Framework.UWP.ViewModels;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.Profiles;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Store.UI.DataModel.UWP;

namespace DK.UOME.Store.PresentationModel.UWP.ViewModels
{
    /// <summary>
    /// View logic for an entry editing screen.
    /// </summary>
    [Export(typeof(EntryViewModel))]
    [Shared]
    public class EntryViewModel : ScreenViewModel
    {
        const string ShareNewEntryMessage = "Save this entry and try again.";
        const string LoanedPhrase = "Loaned to";
        const string BorrowedPhrase = "Borrowed from";
        
        UIModel.Entry _entry;
        string _label;
        bool _isEntryPinned;
        DelegateCommand _saveCommand;
        DelegateCommand _deleteCommand;
        DelegateCommand _cancelCommand;

        [Import]
        public IEntryRepository EntryRepository { get; set; }

        /// <summary>
        /// Gets or sets the entry to edit.
        /// </summary>
        public UIModel.Entry Entry
        {
            get { return _entry; }
            set
            {
                if (_entry != null)
                {
                    _entry.PropertyChanged -= OnEntryPropertyChanged;
                }

                SetProperty(ref _entry, value);
                OnPropertyChanged("IsNewEntry");
                OnPropertyChanged("IsEntryPinned");
            }
        }

        /// <summary>
        /// Gets or sets the label for the entry being edited.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                SetProperty(ref _label, value);
            }
        }

        /// <summary>
        /// Gets a command that saves the entry being edited.
        /// </summary>
        public DelegateCommand SaveCommand
        {
            get
            {
                if (null == _saveCommand)
                {
                    _saveCommand = new DelegateCommand(parameter => Save());
                }

                return _saveCommand;
            }
        }

        /// <summary>
        /// Gets a command that deletes the entry being edited.
        /// </summary>
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (null == _deleteCommand)
                {
                    _deleteCommand = new DelegateCommand(parameter => Delete(), parameter => CanDelete());
                }

                return _deleteCommand;
            }
        }

        /// <summary>
        /// Gets a command that discards any changes made.
        /// </summary>
        public DelegateCommand CancelCommand
        {
            get
            {
                if (null == _cancelCommand)
                {
                    _cancelCommand = new DelegateCommand(Cancel);
                }

                return _cancelCommand;
            }
        }

        /// <summary>
        /// Gets whether the entry is a brand new (unsaved) entry.
        /// </summary>
        public bool IsNewEntry
        {
            get 
            { 
                if(null == Entry)
                {
                    return true;
                }
                
                return Entry.Id < 1;
            }
        }

        public bool IsEntryPinned
        {
            get { return _isEntryPinned; }
            set
            {
                if (_isEntryPinned != value)
                {
                    _isEntryPinned = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the identifier of the entry to display. Set this to have the view model load the entry.
        /// </summary>
        public int? EntryId { get; set; }

        public async Task Start()
        {
            if (EntryId.HasValue && (Entry == null))
            {
                await LoadEntry();
            }

            //await GetPinnedStatus();
        }

        async Task GetPinnedStatus()
        {
            var pinnedEntryIds = await EntryRepository.GetAllPinnedEntrtIds();

            IsEntryPinned = pinnedEntryIds.Any(pinnedEntryId => pinnedEntryId == Entry.Id);
        }

        async Task Save()
        {
            if (Entry.IsValid)
            {
                if (Entry.HasChanged)
                {
                    var entry = Mapper.Map<StorageModel.Entry>(Entry);
                    await EntryRepository.SaveEntry(entry); 
                }

                Navigation.GoBack();
            }
        }

        async Task Delete()
        {
            await EntryRepository.DeleteEntry(Entry.Id);

            Navigation.GoBack();
        }

        bool CanDelete()
        {
            return !IsNewEntry;
        }

        void Cancel(object parameter)
        {
            Navigation.GoBack();
        }

        public override bool GetShareContent(DataRequest request)
        {
            bool success = false;

            if (IsNewEntry)
            {
                // Can't share what isn't saved yet
                request.FailWithDisplayText(ShareNewEntryMessage);
            }
            else
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = string.Format("{0}: {1}", Entry.ThingLabel, Entry.Thing);
                requestData.SetText(GetEntryText());
                success = true;
            }

            return success;
        }

        string GetEntryText()
        {
            StringBuilder entryTextBuilder = new StringBuilder();

            switch (Entry.Type)
            {
                case UIModel.EntryType.Loaned:
                    entryTextBuilder.AppendFormat("{0} {1}", LoanedPhrase, Entry.OtherParty);
                    break;

                case UIModel.EntryType.Borrowed:
                    entryTextBuilder.AppendFormat("{0} {1}", BorrowedPhrase, Entry.OtherParty);
                    break;
                
                default:
                    Debug.Assert(false, string.Format("Entry type '{0}' not supported", Entry.Type));
                    break;
            }

            if (Entry.DueDate.HasValue)
            {
                entryTextBuilder.AppendFormat(" due {0}", Entry.FormattedDueDate);
            }

            return entryTextBuilder.ToString();
        }

        /// <summary>
        /// Updates the state of this view model for a newly-pinned entry.
        /// </summary>
        /// <returns>State machine task.</returns>
        public async Task PinEntry()
        {   
            await EntryRepository.PinEntry(Entry.Id);

            IsEntryPinned = true;
        }

        /// <summary>
        /// Updates the statue of this view model for a newly-unpinned entry.
        /// </summary>
        /// <returns></returns>
        public async Task UnpinEntry()
        {
            await EntryRepository.UnpinEntry(Entry.Id);

            IsEntryPinned = false;
        }

        async Task LoadEntry()
        {
            if (EntryId.HasValue)
            {
                var rawEntry = await EntryRepository.GetEntryById(EntryId.Value);

                if (rawEntry != null)
                {
                    var entry = EntryModelProfile.MapEntry(rawEntry);

                    Entry = entry;
                    Label = ShowEntryViewModel.GetEntryLabel(entry.Type);
                }
            }

            Entry.IsTrackingChanges = true;
            IsTrackingChanges = true;

            Entry.PropertyChanged += OnEntryPropertyChanged;
        }

        private void OnEntryPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanged")
            {
                HasChanged = Entry.HasChanged;
            }
        }
    }
}
