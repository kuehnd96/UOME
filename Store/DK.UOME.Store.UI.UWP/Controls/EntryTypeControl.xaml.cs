using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DK.UOME.Store.UI.UWP.Controls
{
    public sealed partial class EntryTypeControl : UserControl
    {
        public EntryTypeControl()
        {
            this.InitializeComponent();
        }

        public EntryType EntryState
        {
            get { return (EntryType)GetValue(EntryStateProperty); }
            set { SetValue(EntryStateProperty, value); }
        }
        public static DependencyProperty EntryStateProperty = DependencyProperty.Register("EntryState", 
            typeof(EntryType), 
            typeof(EntryTypeControl), 
            new PropertyMetadata(new PropertyChangedCallback(OnEntryStateChanged)));

        private static void OnEntryStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntryTypeControl control = d as EntryTypeControl;

            if (null != control)
            {
                EntryType newType = (EntryType)e.NewValue;

                VisualStateManager.GoToState(control, newType.ToString(), true);
            }
        }
    }
}
