using System;
using System.Collections.Generic;
using System.Diagnostics;
using Callisto.Controls;
using Instructables.Controls;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.Selectors;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Popups;
using Instructables.DataServices;
using Instructables.Common;
using Windows.UI.Xaml.Shapes;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace Instructables.Views
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class UserProfilePage : Instructables.Common.LayoutAwarePage
    {
        private string currentUserName;
        private static SearchFlyout _searchFlyout = null;
        private static LoginFlyout _loginFlyout = null;

        public UserProfilePage()
        {
            this.InitializeComponent();
            //CategoryButton.ItemsSource
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
            CornerButton.Visibility = Visibility.Collapsed;
            base.LoadState(navigationParameter, pageState);

            CornerButton.Visibility = Visibility.Visible;
        }

        protected override string DetermineVisualState(Windows.UI.ViewManagement.ApplicationViewState viewState)
        {
            var vm = this.DataContext as UserProfileViewModel;
            if (vm != null)
            {
                vm.ScreenHeight = Window.Current.Bounds.Height;
                vm.UpdateState(viewState.ToString());
            }
            return base.DetermineVisualState(viewState);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ProfileHub.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            base.OnNavigatedTo(e);

            currentUserName = e.Parameter as string;

            var vm = DataContext as UserProfileViewModel;

                if (vm != null)
                    await vm.Initialize(currentUserName);



            vm.IsLoading = false;

            if (ViewModelLocator.Instance.LandingVM.IsFavorites == true)
            {
                ProfileHub.DefaultSectionIndex = 2;
                ViewModelLocator.Instance.LandingVM.IsFavorites = false;
            }
            else
                ProfileHub.DefaultSectionIndex = 0;
            ProfileHub.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
        }




        private void LowerAppBar_OnClosed(object sender, object e)
        {
 
        }

 

        private void TopNavButtonClicked(object sender, RoutedEventArgs e)
        {
            if (UpperAppBar != null && UpperAppBar.IsOpen)
                UpperAppBar.IsOpen = false;
            if (LowerAppBar != null && LowerAppBar.IsOpen)
                LowerAppBar.IsOpen = false;
        }

        private void SortData_OnTapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void SelectTypes_OnTapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void GridViewItemClicked(object sender, ItemClickEventArgs e)
        {
            Instructable summary = e.ClickedItem as Instructable;

            if (summary != null)
            {
                if (summary.GroupOrdinal == UserProfileTemplateSelector.MoreIndex)
                {
                    this.Frame.Navigate(typeof(ExplorePage), summary.Group.GroupName);
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", summary.title);
                    if (summary.type == "guide" || summary.type == "guide&ebookFlag=true")
                        this.Frame.Navigate(typeof(GuideDetail), summary.id);
                    else if (summary.type == "video")
                        this.Frame.Navigate(typeof(VideoDetail), summary.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), summary.id);
                    //this.Frame.Navigate(typeof(VideoDetail), "E0OU1AXHSKV1GRL");
                }
            }
        }

        private void Header_Click(object sender, RoutedEventArgs e)
        {

        }

        private void favorite_ItemClick(object sender, ItemClickEventArgs e)
        {
            var instructable = e.ClickedItem as Instructable;
            if(instructable != null)
            {
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", instructable.title);
                if (instructable.type == "guide" || instructable.type == "guide&ebookFlag=true")
                    this.Frame.Navigate(typeof(GuideDetail), instructable.id);
                else if (instructable.type == "video")
                    this.Frame.Navigate(typeof(VideoDetail), instructable.id);
                else
                    this.Frame.Navigate(typeof(InstructableDetail), instructable.id);
            }
        }

        private void following_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Author;
            this.Frame.Navigate(typeof(UserProfilePage), item.screenName);
        }

        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            string instructableId = button.Content as string;

            if (instructableId != null)
            {
                var svc = InstructablesDataService.DataServiceSingleton;
                await svc.ToggleFavorite(instructableId);
            }
        }

        private DependencyObject GetFollowerTextBlock(DependencyObject obj)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            Grid grid = VisualTreeHelper.GetParent(parent) as Grid;
            if (grid != null)
                return grid.FindName("followerTextBlock") as TextBlock;
            return null;
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            TextBlock textblock = GetFollowerTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;

            var svc = InstructablesDataService.DataServiceSingleton;
            if (svc.isLogin())
            {
                if ((bool)button.IsChecked)
                {
                    await svc.FollowAuthor(authorName);
                }
                else
                {
                    await svc.UnfollowAuthor(authorName);
                }
                var landingVM = ViewModelLocator.Instance.LandingVM;
                await landingVM.UpdateUserProfile();
            }
            else
            {
                if ((bool)button.IsChecked)
                    button.IsChecked = false;
                else
                    button.IsChecked = true;
                ShowLoginLayout(this, null);
            }
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            if (_searchFlyout == null)
            {
                _searchFlyout = new SearchFlyout();
            }
            _searchFlyout.clearInput();
            _searchFlyout.searchHandle = new SearchFlyout.SearchHandle(_searchFlyout.handleSearch);
            _searchFlyout.searchHandle += this.HandleSearch;
            _searchFlyout.Show();
        }

        private async void HandleSearch(String queryText)
        {
            _searchFlyout.searchHandle -= this.HandleSearch;
            if (queryText.Trim() == string.Empty)
            {
                MessageDialog confirmationDialog = new MessageDialog(
                    "We can't read your mind!",
                     "Empty Search");

                await confirmationDialog.ShowAsync();
            }
            else
            {
                this.Frame.Navigate(typeof(SearchResultsPage), queryText);
            }
        }

        private Color followColor = Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00);
        private Color unfollowColor = Color.FromArgb(0xFF, 0x69, 0x69, 0x69);

        private async void Follow_OnTapped(object sender, TappedRoutedEventArgs e)
        {            
            var dataService = InstructablesDataService.DataServiceSingleton;
            if(dataService.isLogin())
            {
                var vm = this.DataContext as UserProfileViewModel;

                string userName = vm.userName;
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    TextBlock textblock = grid.FindName("followTextBlock") as TextBlock;
                    Path followIcon = grid.FindName("FollowIcon") as Path;
                    SolidColorBrush solidBrush = new SolidColorBrush();
                    
                    if (textblock.Text == "Follow")
                    {
                        var result = await dataService.FollowAuthor(userName);
                        if (result.isSucceeded)
                        {
                            textblock.Text = "Following";
                            if (followIcon != null)
                            {
                                solidBrush.Color = unfollowColor;
                                followIcon.Fill = solidBrush;
                            }
                            
                            var landingVM = ViewModelLocator.Instance.LandingVM;
                            await landingVM.UpdateUserProfile();
                        }
                    }
                    else if (textblock.Text == "Following")
                    {
                        var result = await dataService.UnfollowAuthor(userName);
                        if (result.isSucceeded)
                        {
                            textblock.Text = "Follow";
                            if (followIcon != null)
                            {
                                solidBrush.Color = followColor;
                                followIcon.Fill = solidBrush;
                            }
                            var landingVM = ViewModelLocator.Instance.LandingVM;
                            await landingVM.UpdateUserProfile();
                        }
                    }
                }                
            }
            else
                ShowLoginLayout(this, null);
            
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

        private async void OnRefreshData_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ProfileHub.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            LowerAppBar.IsOpen = false;
            UpperAppBar.IsOpen = false;

            var vm = DataContext as UserProfileViewModel;
            if (vm != null)
                await vm.Initialize(currentUserName);

            vm.IsLoading = false;

            ProfileHub.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
        }


    }
}
