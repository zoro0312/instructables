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

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace Instructables.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class ContestPage : LayoutAwarePage
    {
        private static OfficialRulesFlyout _officialRulesFlyout = null;
        private static LoginFlyout _loginFlyout = null;
        
        public ContestPage()
        {
            this.InitializeComponent();
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

        protected override string DetermineVisualState(Windows.UI.ViewManagement.ApplicationViewState viewState)
        {
            var vm = this.DataContext as ContestViewModel;
            if (vm != null)
            {
                vm.ScreenHeight = Window.Current.Bounds.Height;
                vm.UpdateState(viewState.ToString());
            }
            return base.DetermineVisualState(viewState);
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


            string contestId = e.Parameter as string;
            var vm = DataContext as ContestViewModel;
            if (vm != null)
            {
                //vm.Contest = null;
                await vm.loadContest(contestId);               
            }
            //if (LandingPageViewModel.CurrentScreenMetrics.Name == "1080")
            //    contestsPanel.Margin = new Thickness(0,0,0,75);
            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void Retry(object sender, RoutedEventArgs e)
        {

        }

        private void Rules_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;

            if (_officialRulesFlyout == null)
                _officialRulesFlyout = new OfficialRulesFlyout();
            var vm = this.DataContext as ContestViewModel;
            if (vm.Contest != null)
            {
                string html = vm.Contest.rules;
                _officialRulesFlyout.Load(html.Replace("<a href", "<a target=blank href"));
                _officialRulesFlyout.Show();
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private async void EntryItemClicked(object sender, ItemClickEventArgs e)
        {
            ContestEntry entry = e.ClickedItem as ContestEntry;

            if (entry != null)
            {
                if (entry.order == ContestPageContestEntryTemplateSelector.MoreIndex)
                {
                    var vm = this.DataContext as ContestViewModel;
                    this.Frame.Navigate(typeof(Views.ContestEntriesPage), vm.Contest);                    
                }
                else
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

        private void LowerAppBar_OnClosed(object sender, object e)
        {

        }

        private async void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;

            var vm = DataContext as ContestViewModel;
            if (vm != null)
            {
                await vm.loadContest(vm.ContestId, true);
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
        }


    }
}
