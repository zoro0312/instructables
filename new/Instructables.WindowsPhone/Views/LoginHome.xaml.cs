

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Instructables.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginHome : LayoutAwarePage
    {

        private Color loginEdgeOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color loginInsideOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color signupEdgeOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color signupInsideOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color loginTextOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color signupTextOriginal = Color.FromArgb(0, 0, 0, 0);
        private Color loginEdgeNew = Color.FromArgb(0xFF, 0, 0, 0);
        private Color loginInsideNew = Color.FromArgb(0xFF, 0, 0, 0);
        private Color signupEdgeNew = Color.FromArgb(0xFF, 0, 0, 0);
        private Color signupInsideNew = Color.FromArgb(0xFF, 0, 0, 0);
        private Color loginTextNew = Color.FromArgb(0xFF, 0, 0, 0);
        private Color signupTextNew = Color.FromArgb(0xFF, 0, 0, 0);

        public LoginHome()
        {
            this.InitializeComponent();

            SolidColorBrush brush = (SolidColorBrush)login_icon_edge.Fill;
            loginEdgeOriginal = brush.Color;
            brush = (SolidColorBrush)login_icon_inside.Fill;
            loginInsideOriginal = brush.Color;
            brush = (SolidColorBrush)signup_icon_edge.Fill;
            signupEdgeOriginal = brush.Color;
            brush = (SolidColorBrush)signup_icon_inside.Fill;
            signupInsideOriginal = brush.Color;

            brush = (SolidColorBrush)Login_Text.Foreground;
            loginTextOriginal = brush.Color;

            brush = (SolidColorBrush)Signup_Text.Foreground;
            signupTextOriginal = brush.Color;
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void signup_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SignUpPage));
        }

        private void login_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = loginEdgeNew;
            login_icon_edge.Fill = newBrush;
            newBrush.Color = loginInsideNew;
            login_icon_inside.Fill = newBrush;
            newBrush.Color = loginTextNew;
            Login_Text.Foreground = newBrush;
        }

        private void login_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush oldBrush = new SolidColorBrush();
            oldBrush.Color = loginEdgeOriginal;
            login_icon_edge.Fill = oldBrush;
            oldBrush.Color = loginInsideOriginal;
            login_icon_inside.Fill = oldBrush;
            SolidColorBrush oldBrush2 = new SolidColorBrush();
            oldBrush2.Color = loginTextOriginal;
            Login_Text.Foreground = oldBrush2;
        }

        private void signup_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = signupEdgeNew;
            signup_icon_edge.Fill = newBrush;
            newBrush.Color = signupEdgeNew;
            signup_icon_inside.Fill = newBrush;
            newBrush.Color = signupTextNew;
            Signup_Text.Foreground = newBrush;
        }

        private void signup_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush oldBrush = new SolidColorBrush();
            oldBrush.Color = signupEdgeOriginal;
            signup_icon_edge.Fill = oldBrush;
            oldBrush.Color = signupInsideOriginal;
            signup_icon_inside.Fill = oldBrush;
            SolidColorBrush oldBrush2 = new SolidColorBrush();
            oldBrush2.Color = signupTextOriginal;
            Signup_Text.Foreground = oldBrush2;
        }
    }
}
