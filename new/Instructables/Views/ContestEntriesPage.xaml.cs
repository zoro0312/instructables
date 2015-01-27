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
using Instructables.DataServices;
using Instructables.Selectors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContestEntriesPage : LayoutAwarePage
    {
        private static LoginFlyout _loginFlyout = null;

        public ContestEntriesPage()
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;
            base.OnNavigatedTo(e);

            Contest contest = e.Parameter as Contest;
            var vm = DataContext as ContestEntriesViewModel;
            if (vm != null && contest != null)
            {
                if (vm.Contest == null || vm.Contest.id != contest.id || vm.VisualState == "Offline")
                {
                    ScrollToBeginning();
                    await vm.loadContestEntries(contest);
                }
            }
            LoadingPanel.Visibility = Visibility.Collapsed;
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
            GoHome(sender, e);
        }

        private void TopContests_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ContestsPage));
        }

        private void TopMyInstructables_Click(object sender, RoutedEventArgs e)
        {
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
            var vm = ViewModelLocator.Instance.LandingVM;
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }

        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContestEntry entry = e.ClickedItem as ContestEntry;

            if (entry != null)
            {
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", entry.title);
                var dataService = InstructablesDataService.DataServiceSingleton;
                Instructable instructable = await dataService.GetInstructable(entry.id);
                if (instructable != null)
                {
                    if (instructable.type == "guide" || instructable.type == "guide&ebookFlag=true")
                        this.Frame.Navigate(typeof(GuideDetail), instructable.id);
                    else if (instructable.type == "video")
                        this.Frame.Navigate(typeof(VideoDetail), instructable.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), instructable.id);
                }
                else
                {                
                    this.Frame.Navigate(typeof(InstructableDetail), entry.id);
                }
            }
        }

        private void onItemGridViewLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void LowerAppBar_OnClosed(object sender, object e)
        {

        }

        private async void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;

            var vm = DataContext as ContestEntriesViewModel;
            if (vm != null)
            {
                ScrollToBeginning();
                await vm.loadContestEntries(vm.Contest);              
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        internal void ScrollToBeginning()
        {
            if (ItemGridView.Items.Count > 0)
            {
                var beginningItem = ItemGridView.Items[0];
                if (beginningItem != null)
                {
                    this.ItemGridView.ScrollIntoView(beginningItem);
                } 
            }            
        }
    }
}
