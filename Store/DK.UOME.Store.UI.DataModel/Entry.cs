using DK.Framework.Store.Attributes;
using DK.Framework.Store.Validation;
using System;
using System.Runtime.Serialization;

namespace DK.UOME.Store.UI.DataModel
{
    /// <summary>
    /// Basic entry state.
    /// </summary>
    [DataContract]
    public abstract class Entry : ValidatedType
    {
        const string ThingPropertyName = "Thing";
        const string OtherPartyPropertyName = "OtherParty";
        const string DueDatePropertyName = "DueDate";
        const string FormattedCreateDatePropertyName = "FormattedCreateDate";
        const string FormattedDueDatePropertyName = "FormattedDueDate";
        const string HasDueDatePropertyName = "HasDueDate";

        const string DateFormat = "{0}/{1}/{2}";
        const string MissingMessageFormat = "{0} is missing";
        const string DueDateErrorMessage = "Due date must be after today";
        const int DefaultDueDateAdvance = 1; // Number of days ahead for the default due date
        
        string _thing;
        string _note;
        DateTime _createDate;
        DateTime? _dueDate;
        string _otherParty;
        bool _hasDueDate;

        /// <summary>
        /// Gets or sets the identifier of this entry.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the thing associated with this entry.
        /// </summary>
        [DataMember]
        [TrackChange]
        public string Thing
        {
            get { return _thing; }
            set { SetProperty(ref _thing, value); }
        }

        /// <summary>
        /// Gets or sets the note associated with this entry.
        /// </summary>
        [DataMember]
        [TrackChange]
        public string Note
        {
            get { return _note; }
            set { SetProperty(ref _note, value); }
        }

        /// <summary>
        /// Gets or sets the creation date of this entry.
        /// </summary>
        [DataMember]
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                SetProperty(ref _createDate, value);
                OnPropertyChanged(FormattedCreateDatePropertyName);
            }
        }

        /// <summary>
        /// Gets the create date formatted for display.
        /// </summary>
        public string FormattedCreateDate
        {
            get
            {
                return string.Format(DateFormat, CreateDate.Month, CreateDate.Day, CreateDate.Year);
            }
        }

        /// <summary>
        /// Gets or sets the date the entry is due.
        /// </summary>
        [DataMember]
        [TrackChange]
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                SetProperty(ref _dueDate, value);
                OnPropertyChanged(FormattedDueDatePropertyName);
                OnPropertyChanged(HasDueDatePropertyName);
            }
        }

        /// <summary>
        /// Gets the due date formatted for display.
        /// </summary>
        public string FormattedDueDate
        {
            get
            {
                if (DueDate.HasValue)
                {
                    return string.Format(DateFormat, DueDate.Value.Month, DueDate.Value.Day, DueDate.Value.Year);
                }
                
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the other party associated with this entry.
        /// </summary>
        [DataMember]
        [TrackChange]
        public string OtherParty
        {
            get { return _otherParty; }
            set { SetProperty(ref _otherParty, value); }
        }

        /// <summary>
        /// Gets the type of this entry.
        /// </summary>
        [DataMember]
        public abstract EntryType Type { get; }

        /// <summary>
        /// Gets the label for the thing associated with this entry.
        /// </summary>
        [DataMember]
        public abstract string ThingLabel { get; }

        /// <summary>
        /// Gets the label for the other party associated with this entry.
        [DataMember]
        public abstract string OtherPartyLabel { get; }

        public bool HasDueDate
        {
            get { return _dueDate.HasValue; }
            set
            {
                if (value)
                {
                    DueDate = DateTime.Today.AddDays(DefaultDueDateAdvance);
                }
                else
                {
                    DueDate = null;
                }

                SetProperty(ref _hasDueDate, value);
            }
        }

        /// <summary>
        /// Runs validation.
        /// </summary>
        public Entry()
            : base(new string[] { ThingPropertyName, OtherPartyPropertyName, DueDatePropertyName })
        {
            foreach (string validatedProperty in ValidatedProperties)
            {
                OnPropertyChanged(validatedProperty);
            }
        }

        /// <summary>
        /// Checks the validation of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>An error message if the property is invalid; Otherwise null.</returns>
        protected override string GetValidationError(string propertyName)
        {
            switch (propertyName)
            {
                case ThingPropertyName:
                    if (null == Thing)
                    {
                        return string.Format(MissingMessageFormat, ThingLabel);
                    }
                    
                    if (string.IsNullOrEmpty(Thing.Trim()))
                    {
                        return string.Format(MissingMessageFormat, ThingLabel);
                    }
                    break;

                case OtherPartyPropertyName:
                    if (null == OtherParty)
                    {
                        return string.Format(MissingMessageFormat, OtherPartyLabel);
                    }
                    
                    if (string.IsNullOrEmpty(OtherParty.Trim()))
                    {
                        return string.Format(MissingMessageFormat, OtherPartyLabel);
                    }
                    break;

                default:
                    return null;
            }
            
            return null;
        }
    }
}
