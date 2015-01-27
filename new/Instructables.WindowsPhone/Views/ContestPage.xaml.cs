using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;
using Instructables.Utils;
using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContestPage : Instructables.Common.LayoutAwarePage
    {
        private static int lastPivotIndex = 0;
        private string _html;
        private string contestId;
        private List<string> officalRulesNavigationList = new List<string>();

        public ContestPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Override Pivot header forground color.
            //_pivotHeaderForegroundUnselectedBrush = (SolidColorBrush)Application.Current.Resources["PivotHeaderForegroundUnselectedBrush"];
            //_pivotHeaderForegroundSelectedBrush = (SolidColorBrush)Application.Current.Resources["PivotHeaderForegroundSelectedBrush"];
            //Application.Current.Resources["PivotHeaderForegroundUnselectedBrush"] = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            //Application.Current.Resources["PivotHeaderForegroundSelectedBrush"] = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            if (e.NavigationMode == NavigationMode.Back)
            {
                ContestPivot.SelectedIndex = lastPivotIndex;
            }

            contestId = e.Parameter as string;
            var vm = DataContext as ContestViewModel;
            if (vm != null)
            {
                if (ContestPivot.SelectedIndex == 0)
                    refreshButton.Visibility = Visibility.Collapsed;
                await vm.loadContest(contestId);

                if (vm.Contest != null)
                {
                    _html = "<html><head><style type=\"text/css\">";
                    _html += "li{font-size:46px;} li>p{font-size:46px;} ol>p{font-size:46px;} li>p>*{font-size:46px;}";
                    _html += "ol>li>ol>li>ol>li>*{font-size: 18px;} ol>li>ol>li>ol>li>p>*{font-size: 18px} ul>li{font-size: 18px;}";
                    _html += "</style></head><body>";
                    _html += vm.Contest.rules;
                    _html += "</body></html>";
                    ContestOfficalRules.NavigateToString(_html);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            lastPivotIndex = ContestPivot.SelectedIndex;
            if (e.NavigationMode == NavigationMode.Back)
            {
                var vm = DataContext as ContestViewModel;
                if (vm != null)
                    vm.ClearData();
            }
            // Set Pivot header forground color back to default.
            //Application.Current.Resources["PivotHeaderForegroundUnselectedBrush"] = _pivotHeaderForegroundUnselectedBrush;
            //Application.Current.Resources["PivotHeaderForegroundSelectedBrush"] = _pivotHeaderForegroundSelectedBrush;
        }

        private void ContestEntryClicked(object sender, ItemClickEventArgs e)
        {
            ContestEntry entry = e.ClickedItem as ContestEntry;
            if (entry != null)
            {
                // To do get summery from entry.
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", entry.title);
                this.Frame.Navigate(typeof(InstructableDetail), entry.id);
            }
        }

        private void _showOfficalRulePage()
        {
            // Hide app buttons.
            AppButtons.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            ContestOfficalRules.NavigateToString(_html);
            // Show offical rules.
            ContestOfficalRulePage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void _hideOfficalRulePage()
        {
            // Show app buttons.
            AppButtons.Visibility = Windows.UI.Xaml.Visibility.Visible;
            // Hide offical rules.
            ContestOfficalRulePage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (ContestOfficalRulePage.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                if (officalRulesNavigationList.Count > 0)
                {
                    officalRulesNavigationList.RemoveAt(officalRulesNavigationList.Count-1);
                    if (officalRulesNavigationList.Count > 0)
                        ContestOfficalRules.Navigate(new Uri(officalRulesNavigationList.Last()));
                    else
                        ContestOfficalRules.NavigateToString(_html);
                }
                else
                    _hideOfficalRulePage();            
            }
            else
            {
                this.GoBack(this, new RoutedEventArgs());
            }
        }

        private void Rules_Button_Click(object sender, RoutedEventArgs e)
        {
            _showOfficalRulePage();
        }

        private void Find_Button_Clicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SearchResultsPage));
        }


        private void OnNavigated(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (sender.Source != null)
            {
                string source = sender.Source.AbsoluteUri;
                if (!officalRulesNavigationList.Contains(source))
                    officalRulesNavigationList.Add(source);
            }
        }

        private async void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ContestViewModel;
            if (vm != null)
            {
                vm.ClearData();
                //ContestPivot.Visibility = Visibility.Collapsed;
                vm.IsRefreshing = true;
                await vm.loadContest(contestId);
                vm.IsRefreshing = false;
                //ContestPivot.Visibility = Visibility.Visible;
            }
        }

        private void onPivotChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ContestPivot.SelectedIndex)
            {
                case 0://Info
                    refreshButton.Visibility = Visibility.Collapsed;
                    break;
                case 1://Entry
                    refreshButton.Visibility = Visibility.Visible;
                    break;
                case 2://Prices
                    refreshButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    refreshButton.Visibility = Visibility.Collapsed;
                    break;
            }
            
        }
    }
}
