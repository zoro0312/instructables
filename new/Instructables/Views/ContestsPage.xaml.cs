using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Instructables.DataModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Instructables.Common;
using Instructables.ViewModels;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace Instructables.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class ContestsPage : LayoutAwarePage
    {
        private static LoginFlyout _loginFlyout = null;

        public ContestsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        async protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            LoadingPanel.Visibility = Visibility.Visible;
            var vm = DataContext as ContestsListViewModel;
            if (vm != null)
                await vm.LoadContestList(true);

            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var vm = DataContext as ContestsListViewModel;
            if (vm != null)
            {
                LoadingPanel.Visibility = Visibility.Visible;
                await vm.LoadContestList();
                LoadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void Retry(object sender, RoutedEventArgs e)
        {

        }

        private void ContestsItemClicked(object sender, ItemClickEventArgs e)
        {
            Contest contest = e.ClickedItem as Contest;
            if(contest != null)
            {
                this.Frame.Navigate(typeof(ContestPage), contest.id);
            }
        }

        private void ShowLoginLayout(object sender, RoutedEventArgs e)
        {
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
                _loginFlyout.Unloaded += async (sender2, args) =>
                {
                    var vm = ViewModelLocator.Instance.LandingVM;
                    if (vm != null)
                        await vm.Initialize();
                };
            }
            _loginFlyout.Show();

        }

        private void TopHome_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            GoHome(sender, e);
        }

        private void TopContests_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            this.Frame.Navigate(typeof(ContestsPage));
        }

        private void TopMyInstructables_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            var vm = ViewModelLocator.Instance.LandingVM;
            if (vm.IsLogin)
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                if (userProfileVM != null)
                    userProfileVM.Initialize(vm.userProfile.screenName);
                this.Frame.Navigate(typeof(ExplorePage), "Published");
            }
            else
                ShowLoginLayout(this, null);
        }

        private void TopProfile_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            var vm = ViewModelLocator.Instance.LandingVM;
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }

        private async void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;

            var vm = DataContext as ContestsListViewModel;
            if (vm != null)
            {
                await vm.LoadContestList(true);
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void LowerAppBar_OnClosed(object sender, object e)
        {

        }
    }
}
