using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.ViewModels;
// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Instructables.Views
{
    public enum SettingOptions
    {
        option = 0,
        about = 1,
        feedback = 2,
        logout = 3
    }
    public sealed partial class SettingFlyout : SettingsFlyout
    {
       // public static SettingOptions _settingOption = SettingOptions.option;

        public SettingFlyout()
        {
            this.InitializeComponent();
            AboutViewModel vm = new AboutViewModel();
            vm.LoadAboutTextFile();
            this.AboutText.DataContext = vm;
            
        }

        private static bool _isLogout = false;
        public static bool IsLogout
        {
            get
            {
                return _isLogout;
            }
        
        }

        public void GotoSetting(SettingOptions op)
        {
            _isLogout = false;
            switch (op)
            {
                case SettingOptions.option:
                    OptionPane.Visibility = Visibility.Visible;
                    AboutPane.Visibility = Visibility.Collapsed;
                    LogoutPane.Visibility = Visibility.Collapsed;
                    gaSwitch.IsOn = SessionManager.IsGAEnable();
                    Title = "Options";
                    BackClick += OnBackClick;
                    break;
                case SettingOptions.about:
                    OptionPane.Visibility = Visibility.Collapsed;
                    AboutPane.Visibility = Visibility.Visible;
                    LogoutPane.Visibility = Visibility.Collapsed;
                    Title = "About";
                    break;
                case SettingOptions.feedback:
                    OptionPane.Visibility = Visibility.Collapsed;
                    AboutPane.Visibility = Visibility.Collapsed;
                    LogoutPane.Visibility = Visibility.Collapsed;
                    Title = "Feed back";
                    break;
                case SettingOptions.logout:
                    OptionPane.Visibility = Visibility.Collapsed;
                    AboutPane.Visibility = Visibility.Collapsed;
                    LogoutPane.Visibility = Visibility.Visible;
                    Title = "Log out";
                    break;
                default: break;
            }
        }

        private async void OnBackClick(object sender, BackClickEventArgs e)
        {
            BackClick -= OnBackClick;
            await ExecuteSettings();
        }
     
        private async Task ExecuteSettings()
        {
           
            SessionManager.SetGAEnable(gaSwitch.IsOn);
            await SessionManager.WriteSession();
        }

        private async void onLogoutExecute(object sender, RoutedEventArgs e)
        {
            var DataService = InstructablesDataService.DataServiceSingleton;
            GoogleAnalyticsTracker.SendEvent("logout", "logout", "logout");
            await DataService.Logout();
            _isLogout = true;
            Hide();
        }

        private void onLogoutCancel(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
