using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Phone.UI.Input;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.DataServices;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views 
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfilePage : Instructables.Common.LayoutAwarePage
    {
        private string currentUserName;

        private static Random backgroundSelector = new Random();
        private Uri[] backgroundSources = new Uri[]{
            new Uri("ms-appx:///Assets/ProfileBackground1.jpg"),
            new Uri("ms-appx:///Assets/ProfileBackground2.jpg"),
            new Uri("ms-appx:///Assets/ProfileBackground3.jpg")
        };

        public UserProfilePage()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.SizeChanged += (sender, args) =>
            {
                userPictBackground.Rect = new Rect(0, 0, this.ActualWidth, 240);
                userPictCircle.Center = new Point(this.ActualWidth / 2.0, 110);
                userPictCircle.RadiusX = userPictCircle.RadiusY = this.ActualWidth / 6.0;

                userDefaultPictTrans.ScaleX = userDefaultPictTrans.ScaleY = this.ActualWidth / 3.0 / 64.0;
                userDefaultPictTrans.TranslateX = this.ActualWidth / 2.0 - this.ActualWidth / 6.0;
                userDefaultPictTrans.TranslateY = 110 - this.ActualWidth / 6.0;

                userPictBackroundSource.ImageSource = new BitmapImage() { UriSource = backgroundSources[backgroundSelector.Next(backgroundSources.Count())] };
            };
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
                var vm = DataContext as UserProfileViewModel;
                if (vm != null)
                    vm.ClearData();
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            int? selectedIndex = MainPivot.SelectedIndex;
            pageState.Add("CurrentPivotItem", selectedIndex);
        }

        private int _currentIndex = -1;

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if(pageState != null)
            {
                var currentPivotItem = pageState["CurrentPivotItem"] as int?;
                
                if (currentPivotItem != null)
                {
                    int currentIndex = (int)currentPivotItem;
                    _currentIndex = currentIndex;
                    //MainPivot.SelectedIndex = currentIndex;
                }
                    
            }
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadData();
        }

        public async Task Refresh()
        {
            var vm = DataContext as UserProfileViewModel;
            vm.ClearData();
            await LoadData();
        }

        private async Task LoadData()
        {
            var vm = DataContext as UserProfileViewModel;
            LoadingPanel.Visibility = Visibility.Visible;
            BottomBar.Visibility = Visibility.Collapsed;
            string menuName = "";
            currentUserName = vm.InitData[vm.CurrentInitData].screenName;
            if (vm.InitData[vm.CurrentInitData].menuName != String.Empty)
                menuName = vm.InitData[vm.CurrentInitData].menuName;


            if (vm != null)
            {
                await vm.Initialize(currentUserName);
            }

            if (vm.isLoginUser == false)
            {
                if (vm.isUserFollowed == true)
                {
                    LargeFollowButton.Visibility = Visibility.Collapsed;
                    LargeUnfollowButton.Visibility = Visibility.Visible;
                }
                else
                {
                    LargeFollowButton.Visibility = Visibility.Visible;
                    LargeUnfollowButton.Visibility = Visibility.Collapsed;
                }
            }

            if (menuName == "MyInstruactables")
                MainPivot.SelectedIndex = 1;
            else if (menuName == "Favorites")
                MainPivot.SelectedIndex = 2;
            else if (_currentIndex >= 0 && _currentIndex < MainPivot.Items.Count)
            {
                MainPivot.SelectedIndex = _currentIndex;
            }
            LoadingPanel.Visibility = Visibility.Collapsed;
            BottomBar.Visibility = Visibility.Visible;
            vm.IsLoading = false;
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditProfilePage));
        }

        private void Home_Clicked(object sender, RoutedEventArgs e)
        {
            GoHome(this, e);
        }

        protected override void GoHome(object sender, RoutedEventArgs e)
        {
            // Use the navigation frame to return to the topmost page which should be landing page.
            this.Frame.Navigate(typeof(LandingPage));
        }

        private void Camera_Clicked(object sender, RoutedEventArgs e)
        {
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;

            if (svc.isLogin())
            {
                if (!SessionManager.ShowedCreateTutorial())
                    this.Frame.Navigate(typeof(CreateTutorial));
                else
                    this.Frame.Navigate(typeof(CreateInstructable));
            }
            else
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private void Find_Clicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SearchResultsPage));
        }

        private void myInstructable_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Grid;
            if (item != null)
            {
                switch(item.Name)
                {
                    case "Drafts":
                        this.Frame.Navigate(typeof(DraftsListPage));
                        break;
                    case "Published":
                        this.Frame.Navigate(typeof(PublishedListPage));
                        break;
                    case "Help":
                        this.Frame.Navigate(typeof(CreateTutorial), "showing");
                        break;
                }
            }
        }

        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            string instructableId = button.Content as string;

            if (instructableId != null)
            {
                var svc = InstructablesDataService.DataServiceSingleton;
                if ((bool)button.IsChecked)
                {
                    await svc.ToggleFavorite(instructableId);
                }
                else
                {
                    //unfavorite
                    await svc.ToggleFavorite(instructableId);
                }
            }
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;

            var vm = DataContext as UserProfileViewModel;
            if (vm != null)
            {
                if (pivot != null && pivot.SelectedIndex == 0 && vm.isLoginUser)
                {
                    vm.isEditPage = true;
                }
                else
                {
                    vm.isEditPage = false;
                }
            }
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock textblock = GetSiblingTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;
            var vm = DataContext as UserProfileViewModel;
            var svc = InstructablesDataService.DataServiceSingleton;
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                if (vm.isUserFollowed == true)
                {
                    LargeUnfollowButton.IsEnabled = false;
                    var result = await svc.UnfollowAuthor(authorName);
                    LargeUnfollowButton.IsEnabled = true;
                    if (result.isSucceeded == true)
                    {
                        vm.isUserFollowed = false;
                        LargeFollowButton.Visibility = Visibility.Visible;
                        LargeUnfollowButton.Visibility = Visibility.Collapsed;
                        vm.followersCount -= 1;
                    }
                }
                else
                {
                    LargeFollowButton.IsEnabled = false;
                    var result = await svc.FollowAuthor(authorName);
                    LargeFollowButton.IsEnabled = true;
                    if (result.isSucceeded == true)
                    {
                        vm.isUserFollowed = true;
                        LargeFollowButton.Visibility = Visibility.Collapsed;
                        LargeUnfollowButton.Visibility = Visibility.Visible;
                        vm.followersCount += 1;
                    }
                }
            }
            
            //if ((bool)button.IsChecked)
            //{
            //    await svc.FollowAuthor(authorName);
            //}
            //else
            //{
            //    await svc.UnfollowAuthor(authorName);
            //}
        }

        private DependencyObject GetSiblingTextBlock(DependencyObject obj)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                if ((current.GetType()).Equals(typeof(TextBlock)))
                {
                    return current;
                }
            }
            return null;
        }

        private void ImageBrush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var obj = sender;
        }

        private void ImageBrush_ImageOpened(object sender, RoutedEventArgs e)
        {
            var obj = sender;
        }

        private void following_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Author;
            var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
            //List<string> param = new List<string>();
            //param.Add(item.screenName);
            var param = new UserProfileViewModel.ProfileInitData();
            param.screenName = item.screenName;
            userProfileVM.InitData.Add(param);
            userProfileVM.CurrentInitData += 1;
            this.Frame.Navigate(typeof(UserProfilePage));
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
            if (userProfileVM != null)
            {
                var currentItem = userProfileVM.InitData[userProfileVM.CurrentInitData];
                if (currentItem != null)
                {
                    userProfileVM.InitData.Remove(currentItem);
                    userProfileVM.CurrentInitData -= 1;
                    GoBack(this, new RoutedEventArgs());
                }
            }
        }

        private void favorite_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Instructable;
            if (item != null)
            {
                if (item.type != null)
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", item.title);
                    if (item.type.ToLower() == "guide" || item.type == "guide&ebookFlag=true")
                        this.Frame.Navigate(typeof(GuideDetail), item.id);
                    else if (item.type.ToLower() == "video")
                        this.Frame.Navigate(typeof(VideoDetail), item.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), item.id);
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", item.title);
                    this.Frame.Navigate(typeof(InstructableDetail), item.id);
                }
                    
            }
        }

        private void userInstructable_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Instructable;
            if (item != null)
            {
                if (item.type != null)
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", item.title);
                    if (item.type.ToLower() == "guide" || item.type == "guide&ebookFlag=true")
                        this.Frame.Navigate(typeof(GuideDetail), item.id);
                    else if (item.type.ToLower() == "video")
                        this.Frame.Navigate(typeof(VideoDetail), item.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), item.id);
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", item.title);
                    this.Frame.Navigate(typeof(InstructableDetail), item.id);
                }
            }
        }

        private async void FollowingButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock textblock = GetSiblingTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;
            var vm = DataContext as UserProfileViewModel;
            Author followingAuth = null;
            if (vm != null)
            {
                foreach (var author in vm.Followings)
                {
                    if (author.screenName == authorName)
                    {
                        followingAuth = author;
                    }
                }
            }
            //followingAuth.isNotFollowing = false;
            var svc = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await svc.EnsureLogin();
            if (ifLogin != true)
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                var result = await svc.FollowAuthor(authorName);
                if (result.isSucceeded)
                {
                    followingAuth.isFollowed = true;
                    followingAuth.isNotFollowing = false;
                }
            }
        }

        private async void UnFollowingButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock textblock = GetSiblingTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;
            var vm = DataContext as UserProfileViewModel;
            Author followingAuth = null;
            if (vm != null)
            {
                foreach (var author in vm.Followings)
                {
                    if (author.screenName == authorName)
                    {
                        followingAuth = author;
                    }
                }
            }
            //followingAuth.isNotFollowing = true;
            var svc = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await svc.EnsureLogin();
            if (ifLogin != true)
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                var result = await svc.UnfollowAuthor(authorName);
                if (result.isSucceeded)
                {
                    followingAuth.isFollowed = false;
                    followingAuth.isNotFollowing = true;
                }
            }
            
        }

        private async void FollowerButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock textblock = GetSiblingTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;
            var vm = DataContext as UserProfileViewModel;
            Author followingAuth = null;
            if (vm != null)
            {
                foreach (var author in vm.Followers)
                {
                    if (author.screenName == authorName)
                    {
                        followingAuth = author;
                    }
                }
            }
            //followingAuth.isNotFollowing = false;
            var svc = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await svc.EnsureLogin();
            if (ifLogin != true)
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                var result = await svc.FollowAuthor(authorName);
                if (result.isSucceeded)
                {
                    followingAuth.isFollowed = true;
                    followingAuth.isNotFollowing = false;
                }
            }
        }

        private async void UnFollowerButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock textblock = GetSiblingTextBlock(button) as TextBlock;
            string authorName = textblock != null ? textblock.Text : currentUserName;
            var vm = DataContext as UserProfileViewModel;
            Author followingAuth = null;
            if (vm != null)
            {
                foreach (var author in vm.Followers)
                {
                    if (author.screenName == authorName)
                    {
                        followingAuth = author;
                    }
                }
            }
            //followingAuth.isNotFollowing = false;
            var svc = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await svc.EnsureLogin();
            if (ifLogin != true)
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                var result = await svc.UnfollowAuthor(authorName);
                if (result.isSucceeded)
                {
                    followingAuth.isFollowed = false;
                    followingAuth.isNotFollowing = true;
                }
            }
        }

    }
}
