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

namespace DK.UOME.Store.UI.WindowsPhone.Controls
{
    public sealed partial class EntryTypeControl : UserControl
    {
        public EntryTypeControl()
        {
            this.InitializeComponent();
        }

        public string EntryType
        {
            get { return (string)GetValue(EntryTypeProperty); }
            set { SetValue(EntryTypeProperty, value); }
        }
        public static DependencyProperty EntryTypeProperty = DependencyProperty.Register("EntryType", typeof(string), typeof(EntryTypeControl), new PropertyMetadata(new PropertyChangedCallback(OnEntryTypeChanged)));

        private static void OnEntryTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntryTypeControl control = d as EntryTypeControl;

            if (null != control)
            {
                VisualStateManager.GoToState(control, control.EntryType, true);
            }
        }
    }
}
