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
    public sealed partial class ForgetPassword : Instructables.Common.LayoutAwarePage
    {

        public ForgetPassword()
        {
            this.InitializeComponent();
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            if (_isProcessing == true)
                return;
            base.OnHardwareBackPressed(sender, e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }


        private void EmailPassword_Click(object sender, RoutedEventArgs e)
        {
            SendPassword();
        }

        private bool _isProcessing = false;

        private async void SendPassword()
        {
            var DataService = InstructablesDataService.DataServiceSingleton;
            LoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            _isProcessing = true;
            EmailPassword.IsEnabled = false;
            var result = await DataService.ForgetPassword(emailAddress.Text);
            LoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            EmailPassword.IsEnabled = true;
            _isProcessing = false;
            if (!result.isSucceeded)
            {
                /*forget_password_error.ShowAt(this);
                var error = result.error;
                if (error.validationErrors.email != null)
                    forgetpasswordResult.Text = error.validationErrors.email;*/
                var error = result.error;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Error";
                dialog.Content = error.validationErrors.email;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
            }
            else
            {
                this.Frame.Navigate(typeof(ResetPassword), emailAddress.Text);
            }
        }

        private void forget_error_ok_Click(object sender, RoutedEventArgs e)
        {
            forget_password_error.Hide();
        }

        private void Email_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    EmailPassword.Focus(FocusState.Pointer);
                    SendPassword();
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
            emailAddress.Text = "";
        }
    }
}
