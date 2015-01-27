using Instructables.DataModel;
using Instructables.DataServices;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.System;
using Windows.Phone.UI.Input;
using Instructables.Common;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUpPage : Instructables.Common.LayoutAwarePage
    {
        public SignUpPage()
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


        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp();
        }

        private bool _isProcessing = false;

        private async void SignUp()
        {
            var DataService = InstructablesDataService.DataServiceSingleton;
            //if (newsLetter.IsOn != null)
            bool ifNewsletter = ((bool)newsLetter.IsOn ? false : true);
            screennameError.Text = "";
            emailError.Text = "";
            passwordError.Text = "";
            LoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            signUpButton.IsEnabled = false;
            facebook_signup.IsEnabled = false;
            _isProcessing = true;
            var result = await DataService.SignUp(userName.Text, passWord.Password, emailAddress.Text, ifNewsletter);
            LoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            LandingPage._backFromLogin = true;
            if (!result.isSucceeded)
            {
                /*signup_error.ShowAt(this);
                var error = result.error;
                if (error != null)
                {
                    if (error.validationErrors.screenName != null)
                        screennameError.Text = error.validationErrors.screenName;
                    if (error.validationErrors.email != null)
                        emailError.Text = error.validationErrors.email;
                    if (error.validationErrors.password != null)
                        passwordError.Text = error.validationErrors.password;
                }
                else
                {
                    screennameError.Text = "Sorry, there're something wrong in the backend service. Please try it later.";
                }*/
                GoogleAnalyticsTracker.SendEvent("signup", "normal", "error");
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Error";
                dialog.Content = String.Empty;
                var error = result.error;
                if (error.validationErrors.screenName != null)
                {
                    dialog.Content += error.validationErrors.screenName + "\n";
                }

                if (error.validationErrors.email != null)
                {
                    dialog.Content += error.validationErrors.email + "\n";
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
                GoogleAnalyticsTracker.SendEvent("signup", "normal", "succeed");
                var result2 = await DataService.Login(userName.Text, passWord.Password);
                if (!result2.isSucceeded)
                {
                    GoogleAnalyticsTracker.SendEvent("login", "normal", "error");
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Error";
                    dialog.Content = result.error.toString();

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                    await dialog.ShowAsync();
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("login", "normal", "succeed");
                    GoHome(this, new RoutedEventArgs());
                    DataService.FireLoginSucceed();
                }
            }
            signUpButton.IsEnabled = true;
            facebook_signup.IsEnabled = true;
            _isProcessing = false;
        }

        protected override void GoHome(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                int backwardCount = 2;
                while (this.Frame.CanGoBack && backwardCount-- > 0)
                    this.Frame.GoBack();
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void signup_error_ok_Click(object sender, RoutedEventArgs e)
        {
            signup_error.Hide();
        }

        private void Email_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    userName.Focus(FocusState.Keyboard);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void Username_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    passWord.Focus(FocusState.Keyboard);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void Password_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    signUpButton.Focus(FocusState.Pointer);
                    SignUp();
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
            passWord.Password = "";
        }



    }
}
