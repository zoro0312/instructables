
using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//using Facebook;
//using Facebook.Client;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Instructables.Common.LayoutAwarePage
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            if (_isProcessing == true)
                return;
            LandingPage._backFromLogin = true;
            base.OnHardwareBackPressed(sender, e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var transferName = e.Parameter as string;
            if (transferName != null)
            {
                userName.Text = transferName;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void GoHome(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                int backwardCount = 1;
                while (this.Frame.CanGoBack && backwardCount-- > 0) 
                    this.Frame.GoBack();
            }
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private bool _isProcessing = false;
        private async void Login()
        {
            LandingPage._backFromLogin = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            LoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            logInButton.IsEnabled = false;
            forgetPassword.IsEnabled = false;
            facebook_login.IsEnabled = false;
            _isProcessing = true;
            var result = await DataService.Login(userName.Text, passWord.Password);
            LoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            logInButton.IsEnabled = true;
            forgetPassword.IsEnabled = true;
            facebook_login.IsEnabled = true;
            _isProcessing = false;
            if(result == null)
            {
                GoogleAnalyticsTracker.SendEvent("login", "normal", "error");
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Error";
                dialog.Content = "Login failed.";

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
            }
            else if (!result.isSucceeded)
            {
                GoogleAnalyticsTracker.SendEvent("login", "normal", "error");
                if (result.error == null)
                {
                    networkErrorBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    //login_error.ShowAt(this);
                    //loginResult.Text = result.error.toString();
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Error";
                    dialog.Content = result.error.toString();

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                    await dialog.ShowAsync();
                }
            }
            else
            {
                GoogleAnalyticsTracker.SendEvent("login", "normal", "succeed");
                GoHome(this, new RoutedEventArgs());
                DataService.FireLoginSucceed();
            }
        }

        private void signUp_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SignUpPage));
        }

        private void login_error_ok_Click(object sender, RoutedEventArgs e)
        {
            login_error.Hide();
        }

        private void forgetPassword_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("forget password", "forget password", "forget password");
            this.Frame.Navigate(typeof(ForgetPassword));
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
                    logInButton.Focus(FocusState.Pointer);
                    Login();
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
            //userName.Text = "";
            passWord.Password = "";
        }

        //private FacebookSession session;
        //string _facebookAppId = "840371729341091"; // You must set your own AppId here
        //string _permissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here
        //private FacebookClient _fb = new FacebookClient();

        private void FacebookLogin(object sender, RoutedEventArgs e)
        {
            //string message = String.Empty;
            //try
            //{
            //    session = await App.FacebookSessionClient.LoginAsync("http://www.instructables.com/");
            //    App.AccessToken = session.AccessToken;
            //    App.FacebookId = session.FacebookId;
            //}
            //catch (InvalidOperationException ex)
            //{
            //    message = "Login failed! Exception details: " + ex.Message;
            //    MessageDialog dialog = new MessageDialog(message);
            //    dialog.ShowAsync();
            //}
            //var redirectUrl = "http://www.instructables.com/";
            //try
            //{
            //    //fb.AppId = facebookAppId;
            //    var loginUrl = _fb.GetLoginUrl(new
            //    {
            //        client_id = _facebookAppId,
            //        redirect_uri = redirectUrl,
            //        scope = _permissions,
            //        display = "popup",
            //        response_type = "token"
            //    });

            //    var endUri = new Uri(redirectUrl);

            //    WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
            //                                            WebAuthenticationOptions.None,
            //                                            loginUrl,
            //                                            endUri);
            //    if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            //    {
            //        var callbackUri = new Uri(WebAuthenticationResult.ResponseData.ToString());
            //        var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(callbackUri);
            //        var accessToken = facebookOAuthResult.AccessToken;
            //        if (String.IsNullOrEmpty(accessToken))
            //        {
            //            MessageDialog confirmationDialog = new MessageDialog(
            //                     "Facebook Login Error",
            //                     "Login Error");

            //            await confirmationDialog.ShowAsync();
            //        }
            //        else
            //        {
            //            // User is logged in and token was returned
            //            var DataService = InstructablesDataService.DataServiceSingleton;
            //            var result = await DataService.FacebookLogin(accessToken);
            //            if (result.isSucceeded == false)
            //            {
            //                MessageDialog confirmationDialog = new MessageDialog(
            //                    result.error.toString(),
            //                    "Login Error");

            //                await confirmationDialog.ShowAsync();
            //            }


            //        }

            //    }
            //    else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            //    {
            //        throw new InvalidOperationException("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
            //    }
            //    else
            //    {
            //        // The user canceled the authentication
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //
            //    // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            //    //
            //    throw ex;
            //}
        }

    }
}
