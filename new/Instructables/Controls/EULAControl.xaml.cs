using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Instructables.Common;
using Instructables.ViewModels;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Controls
{
    public sealed partial class EULAControl : UserControl
    {
        public EULAControl()
        {
            this.InitializeComponent();
            
        }

        private async void OnEULAAcceptButtonClicked(object sender, RoutedEventArgs e)
        {
            SessionManager.NeverShowLicenseAgreement();
            await SessionManager.WriteSession();
            this.Visibility = Visibility.Collapsed;
        }

        private void OnEULACancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }


    }
}
