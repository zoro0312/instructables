using Instructables.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Popups;
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
    public sealed partial class ResetPassword : Instructables.Common.LayoutAwarePage
    {
        
        public ResetPassword()
        {
            this.InitializeComponent();
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            if (_isProcessing == true)
                return;
            base.OnHardwareBackPressed(sender, e);
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _emailAddress = navigationParameter as string;
        }

        private string _emailAddress = null;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ExecuteResetPassword();
        }

        private bool _isProcessing = false;

        private async void ExecuteResetPassword()
        {
            var DataService = InstructablesDataService.DataServiceSingleton;
            if (_emailAddress == null)
                return;
            LoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            ResetPasswordButton.IsEnabled = false;
            _isProcessing = true;
            var result = await DataService.ResetPassword(_emailAddress, ResetCode.Text, NewPassword.Password);
            LoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            ResetPasswordButton.IsEnabled = true;
            _isProcessing = false;
            if (!result.isSucceeded)
            {
                /*reset_password_error.ShowAt(this);
                var error = result.error;
                if (error.validationErrors.resetCode != null)
                    resetpasswordEmailResult.Text = error.validationErrors.resetCode;
                if (error.validationErrors.password != null)
                    resetpasswordPasswordResult.Text = error.validationErrors.password;*/
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Error";
                dialog.Content = String.Empty;
                var error = result.error;
                if (error.validationErrors.resetCode != null)
                {
                    dialog.Content += error.validationErrors.resetCode + "\n";
                }

                if (error.validationErrors.password != null)
                {
                    dialog.Content += error.validationErrors.password + "\n";
                }

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
            }
            else
            {
                var resultDetail = result.message;
                if (resultDetail.screenName != null)
                {
                    var screenName = resultDetail.screenName;
                    //this.Frame.Navigate(typeof(LoginPage), screenName);
                    var res = await DataService.Login(screenName, NewPassword.Password);
                    if (res.isSucceeded)
                    {
                        GoHome(this, new RoutedEventArgs());
                        DataService.FireLoginSucceed();
                    }
                }

            }
        }

        protected override void GoHome(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                int backwardCount = 3;
                while (this.Frame.CanGoBack && backwardCount-- > 0)
                    this.Frame.GoBack();
            }
        }

        private void reset_error_ok_Click(object sender, RoutedEventArgs e)
        {
            reset_password_error.Hide();
        }

        private void ResetCode_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    NewPassword.Focus(FocusState.Keyboard);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void NewPassword_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    ResetPasswordButton.Focus(FocusState.Pointer);
                    ExecuteResetPassword();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void FlyoutClosed(object sender, object e)
        {
            NewPassword.Password = "";
        }
    }
}
