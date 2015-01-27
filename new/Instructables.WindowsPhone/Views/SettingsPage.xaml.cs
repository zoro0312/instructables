using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : LayoutAwarePage
    {
        public SettingsPage()
        {
            this.InitializeComponent();

        }

        protected override async void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            if (_isProcessing == true)
                return;

            ExecuteSettings();
            base.OnHardwareBackPressed(sender, e);

        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            gaSwitch.IsOn = SessionManager.IsGAEnable();
            if (!InstructablesDataService.DataServiceSingleton.isLogin())
                logoutButton.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "about_us");
            await ExecuteSettings();
            this.Frame.Navigate(typeof(AboutPage));
        }

        private bool _isProcessing = false;

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("", "");
            dialog.Title = "Log out";
            dialog.Content = "Are you sure you want to log out from instructables?";
            dialog.Commands.Add(new UICommand("ok", (command) =>
            {
                GoogleAnalyticsTracker.SendEvent("logout", "logout", "logout");
                Logout();
            }));
            dialog.Commands.Add(new UICommand("cancel", (command) =>
            {
            }));

            await dialog.ShowAsync();
        }

        private async void Logout()
        {
            LandingPage._backFromLogout = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            LoadingPanel.Visibility = Visibility.Visible;
            ContentRoot.Visibility = Visibility.Collapsed;
            _isProcessing = true;
            await DataService.Logout();
            LoadingPanel.Visibility = Visibility.Collapsed;
            ContentRoot.Visibility = Visibility.Visible;
            _isProcessing = false;
            await ExecuteSettings();
            GoBack(this, new RoutedEventArgs());
        }

        /*private void GA_Switch(object sender, RoutedEventArgs e)
        {
            if (gaSwitch == null)
                return;

            if(gaSwitch.IsOn)
            {
                GoogleAnalyticsTracker._GAGlobalSwitch = true;
            }
            else 
            {
                GoogleAnalyticsTracker._GAGlobalSwitch = false;
            }
        }*/

        private async Task ExecuteSettings() 
        {
            SessionManager.SetGAEnable(gaSwitch.IsOn);
            await SessionManager.WriteSession();
        }

        private void SwitchThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

        }

        private void gaSwitch_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
