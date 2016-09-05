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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CYaPass
{
    public sealed partial class SiteKeyDialog : ContentDialog
    {
        public String siteKey { get; set; }
        public SiteKeyDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_OkButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (SiteKeyTextBox.Text == String.Empty)
            {
                args.Cancel = true;
            }
            siteKey = SiteKeyTextBox.Text;

        }

        private void ContentDialog_CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
