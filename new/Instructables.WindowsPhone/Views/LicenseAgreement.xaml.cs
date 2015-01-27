using Instructables.Common;
using Instructables.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LicenseAgreement : Instructables.Common.LayoutAwarePage
    {
        public LicenseAgreement()
        {
            this.InitializeComponent();
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            return;
            //e.Handled = true;
            //GoBack(this, new RoutedEventArgs());
        }

        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            LicenseAgreementViewModel vm = this.DataContext as LicenseAgreementViewModel;
            if(vm != null)
                await vm.LoadLicenseAgreementFile();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void OnDeclinePress(object sender, RoutedEventArgs e)
        {
            //GoBack(this, new RoutedEventArgs());
            Application.Current.Exit();
        }

        private async Task UpdateStatus()
        {
            SessionManager.NeverShowLicenseAgreement();
            await SessionManager.WriteSession();
        }

        private async void OnAgreePress(object sender, RoutedEventArgs e)
        {
            await UpdateStatus();
            this.Frame.Navigate(typeof(WalkThroughs));
        }
    }
}
