using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.UI.Core;
using Instructables.DataServices;
using System.Diagnostics;
using Windows.UI.Popups;
//using Facebook.Client;
//using Facebook;
using Windows.Security.Authentication.Web;
using Instructables.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    public enum LoginPageEnum
    {
        Login = 0,
        Signup = 1,
        Forget = 2,
        Reset = 3
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginFlyout : SettingsFlyout
    {
        public static LoginPageEnum _pageState = LoginPageEnum.Login;

        public LoginFlyout()
        {
            this.InitializeComponent();
            //var baseUri = loginButton.BaseUri;
            //Debug.WriteLine("{0}",baseUri.ToString());
            logInButton.Focus(FocusState.Pointer);
            GotoPage(_pageState);
        }

        public void SwitchPage()
        {
            GotoPage(_pageState);
        }

        private bool _handling = false;

        private void GotoPage(LoginPageEnum state)
        {
            _pageState = state;
            switch(state)
            {
                case LoginPageEnum.Login:
                    LoginPage.Visibility = Visibility.Visible;
                    SignupPage.Visibility = Visibility.Collapsed;
                    ForgetPasswordPage.Visibility = Visibility.Collapsed;
                    ResetPasswordPage.Visibility = Visibility.Collapsed;
                    userName.Text = String.Empty;
                    passWord.Password = String.Empty;
                    break;
                case LoginPageEnum.Signup:
                    LoginPage.Visibility = Visibility.Collapsed;
                    SignupPage.Visibility = Visibility.Visible;
                    ForgetPasswordPage.Visibility = Visibility.Collapsed;
                    ResetPasswordPage.Visibility = Visibility.Collapsed;
                    userName2.Text = String.Empty;
                    passWord2.Password = String.Empty;
                    emailAddress.Text = String.Empty;
                    break;
                case LoginPageEnum.Forget:
                    LoginPage.Visibility = Visibility.Collapsed;
                    SignupPage.Visibility = Visibility.Collapsed;
                    ForgetPasswordPage.Visibility = Visibility.Visible;
                    ResetPasswordPage.Visibility = Visibility.Collapsed;
                    emailAddress2.Text = String.Empty;
                    break;
                case LoginPageEnum.Reset:
                    LoginPage.Visibility = Visibility.Collapsed;
                    SignupPage.Visibility = Visibility.Collapsed;
                    ForgetPasswordPage.Visibility = Visibility.Collapsed;
                    ResetPasswordPage.Visibility = Visibility.Visible;
                    ResetCode.Text = String.Empty;
                    NewPassword.Password = String.Empty;
                    break;
                default:
                    break;
            }
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            Login();
            signUp.Focus(FocusState.Programmatic);
        }

        private async void Login()
        {
            if (_handling == true)
                return;
            else
                _handling = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            //LoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            LoginLoadingPanel.Visibility = Visibility.Visible;
            logInButton.IsEnabled = false;
            forgetPassword.IsEnabled = false;
            facebook_login.IsEnabled = false;

            var result = await DataService.Login(userName.Text, passWord.Password);
            //LoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            LoginLoadingPanel.Visibility = Visibility.Collapsed;
            logInButton.IsEnabled = true;
            forgetPassword.IsEnabled = true;
            facebook_login.IsEnabled = true;

            if (result!=null && !result.isSucceeded)
            {
                //login_error.ShowAt(this);
                //loginResult.Text = result.error.toString();
                GoogleAnalyticsTracker.SendEvent("login", "normal", "error");
                MessageDialog confirmationDialog = new MessageDialog(
                result.error.toString(),
                "Login Error");
                await confirmationDialog.ShowAsync();
            }
            else
            {
                GoogleAnalyticsTracker.SendEvent("login", "normal", "succeed");
                Hide();
                //GoHome(this, new RoutedEventArgs());
            }
            _handling = false;
        }

        //private FacebookSession session;
        //string _facebookAppId = "840371729341091"; // You must set your own AppId here
        //string _permissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here
        //private FacebookClient _fb = new FacebookClient();
        private async void FacebookLogin(object sender, RoutedEventArgs e)
        {
            /*string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("http://www.instructables.com/");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;
            }
            catch (InvalidOperationException ex)
            {
                message = "Login failed! Exception details: " + ex.Message;
                MessageDialog dialog = new MessageDialog(message);
                dialog.ShowAsync();
            }*/
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
            //            if(result.isSucceeded == false)
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

        private void forgetPassword_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("forget password", "forget password", "forget password");
            GotoPage(LoginPageEnum.Forget);
        }

        private void Username_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
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
            if (e.Key == VirtualKey.Tab)
            {
                try
                {
                    logInButton.Focus(FocusState.Pointer);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
            else if (e.Key == VirtualKey.Enter)
            {
                Login();
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp();
        }


        private async void SignUp()
        {
            if (_handling == true)
                return;
            else
                _handling = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            //if (newsLetter.IsOn != null)
            bool ifNewsletter = ((bool)newsLetter.IsOn ? false : true);
            LoginLoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            signUpButton.IsEnabled = false;
            facebook_signup.IsEnabled = false;

            var result = await DataService.SignUp(userName2.Text, passWord2.Password, emailAddress.Text, ifNewsletter);
            LoginLoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;

            if (!result.isSucceeded)
            {
                GoogleAnalyticsTracker.SendEvent("signup", "normal", "error");
                var error = result.error;
                var errorMessage = String.Format("{0}\n{1}\n{2}", error.validationErrors.screenName, error.validationErrors.email, error.validationErrors.password);
                MessageDialog confirmationDialog = new MessageDialog(
                    errorMessage,
                    "Signup Error");

                await confirmationDialog.ShowAsync();
            }
            else
            {
                GoogleAnalyticsTracker.SendEvent("signup", "normal", "succeed");
                var result2 = await DataService.Login(userName2.Text, passWord2.Password);
                if (!result2.isSucceeded)
                {
                    var error = result2.error;
                    MessageDialog confirmationDialog = new MessageDialog(
                    result2.error.toString(),
                    "Login Error");

                    await confirmationDialog.ShowAsync();
                }
                else
                {
                    Hide();
                }
            }
            signUpButton.IsEnabled = true;
            facebook_signup.IsEnabled = true;
            _handling = false;
        }

        private void Email_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                try
                {
                    //userName2.Focus(FocusState.Keyboard);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void Username_Keydown2(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                try
                {
                    //passWord2.Focus(FocusState.Keyboard);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void Password_Keydown2(object sender, KeyRoutedEventArgs e)
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

        private void OnSwitchLogin(object sender, RoutedEventArgs e)
        {
            GotoPage(LoginPageEnum.Login);
        }

        private void OnSwitchSignUp(object sender, RoutedEventArgs e)
        {
            GotoPage(LoginPageEnum.Signup);
        }


        private void EmailPassword_Click(object sender, RoutedEventArgs e)
        {
            SendPassword();
        }

        private async void SendPassword()
        {
            if (_handling == true)
                return;
            else
                _handling = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            _emailAddress = null;
            LoginLoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            BackButton.IsEnabled = false;
            EmailPassword.IsEnabled = false;
            var result = await DataService.ForgetPassword(emailAddress2.Text);
            LoginLoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            EmailPassword.IsEnabled = true;
            BackButton.IsEnabled = true;
            if(result == null)
            {
                MessageDialog confirmationDialog = new MessageDialog(
                    emailAddress2.Text,
                    "Send Password Error");

                await confirmationDialog.ShowAsync();
            }
            else if (!result.isSucceeded)
            {
                var error = result.error;
                MessageDialog confirmationDialog = new MessageDialog(
                    error == null ? emailAddress2.Text : error.validationErrors.email,
                    "Send Password Error");

                await confirmationDialog.ShowAsync();
            }
            else
            {
                _emailAddress = emailAddress2.Text;
                GotoPage(LoginPageEnum.Reset);
            }
            _handling = false;
        }

        private void Email_Keydown2(object sender, KeyRoutedEventArgs e)
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

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ExecuteResetPassword();
        }

        private string _emailAddress = null;

        private async void ExecuteResetPassword()
        {
            if (_handling == true)
                return;
            else
                _handling = true;
            var DataService = InstructablesDataService.DataServiceSingleton;
            if (_emailAddress == null)
                return;
            LoginLoadingPanel.Visibility = Visibility.Visible;
            //ContentRoot.Visibility = Visibility.Collapsed;
            ResetPasswordButton.IsEnabled = false;
            BackButton2.IsEnabled = false;
            var result = await DataService.ResetPassword(_emailAddress, ResetCode.Text, NewPassword.Password);
            LoginLoadingPanel.Visibility = Visibility.Collapsed;
            //ContentRoot.Visibility = Visibility.Visible;
            ResetPasswordButton.IsEnabled = true;
            BackButton2.IsEnabled = true;
            if (!result.isSucceeded)
            {
                var error = result.error;
                var errorMessage = String.Format("{0}\n{1}", error.validationErrors.resetCode, error.validationErrors.password);
                MessageDialog confirmationDialog = new MessageDialog(
                    errorMessage,
                    "Signup Error");

                await confirmationDialog.ShowAsync();
            }
            else
            {
                Hide();
                GotoPage(LoginPageEnum.Login);
            }
            _handling = false;
        }

        private void ResetCode_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
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

        private void OnForgetBack(object sender, RoutedEventArgs e)
        {
            GotoPage(LoginPageEnum.Login);
        }

        private void OnResetBack(object sender, RoutedEventArgs e)
        {
            GotoPage(LoginPageEnum.Forget);
        }

        //private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        //{

        //}

    }
}
