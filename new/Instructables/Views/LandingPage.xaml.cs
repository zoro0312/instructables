using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Instructables.Common;
using Instructables.Selectors;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;
using Windows.UI.ApplicationSettings;
using Windows.ApplicationModel.DataTransfer;
using Instructables.DataServices;
using Windows.Devices.Enumeration;

// The Grouped items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace Instructables.Views
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class LandingPage : LayoutAwarePage
    {
        private static int _defaultSectionIndex = 0;
        private static LoginFlyout _loginFlyout = null;
        private static SearchFlyout _searchFlyout = null;
        private static SettingsPane settingPane = null;
        private static SettingFlyout _settingFlyout = null;

        private DispatcherTimer AnimationTimer = new DispatcherTimer();
        private double ANIMATION_TIME_GAP = 4000.0;

        private DispatcherTimer AnimationDelayTimer = new DispatcherTimer();
        private double ANIMATION_DELAY_GAP = 5000.0;
        private int animationDirection = 1;

        private String _currentSectionName = String.Empty;
        public LandingPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            if (settingPane == null)
            {
                settingPane = SettingsPane.GetForCurrentView();
                settingPane.CommandsRequested += settingPane_OnCommandRequest;
            }

            TimeSpan animationTimeGap = TimeSpan.FromMilliseconds(ANIMATION_DELAY_GAP);
            AnimationDelayTimer.Interval = animationTimeGap;
            AnimationDelayTimer.Tick += (sender1, args) =>
            {
                AnimationDelayTimer.Stop();
                AnimationTimer.Start();
            };
           
        }

        private void settingPane_OnCommandRequest(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "settings");
            IList<SettingsCommand> conmmand = e.Request.ApplicationCommands;

            conmmand.Add(new SettingsCommand("options", "Options", new UICommandInvokedHandler(this.settingOptions)));
            conmmand.Add(new SettingsCommand("about", "About", new UICommandInvokedHandler(this.settingOptions)));
            conmmand.Add(new SettingsCommand("feedback", "Send feedback", new UICommandInvokedHandler(this.settingOptions)));
            var vm = DataContext as LandingPageViewModel;
            if (vm.IsLogin)
                conmmand.Add(new SettingsCommand("logout", "Log out", new UICommandInvokedHandler(this.settingOptions)));
        }

        private async void settingOptions(IUICommand e)
        {
            string id = e.Id as string;
            if (_settingFlyout == null)
                _settingFlyout = new SettingFlyout();
            if (id == "options")
            {
                _settingFlyout.GotoSetting(SettingOptions.option);
                _settingFlyout.Show();
            }
            else if (id == "about")
            {
                GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "about_us");
                _settingFlyout.GotoSetting(SettingOptions.about);
                _settingFlyout.Show();
            }
            else if (id == "feedback")
            {
                GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "feedback");
                Feedback_Tapped();
            }
            else if (id == "logout")
            {
                //MessageDialog dialog = new MessageDialog("", "");
                //dialog.Title = "Log out";
                //dialog.Content = "Are you sure you want to log out from Instructables?";
                //dialog.Commands.Add(new UICommand("ok", (command) =>
                //{
                //    Logout();
                //}));
                //dialog.Commands.Add(new UICommand("cancel", (command) =>
                //{
                //}));
                //await dialog.ShowAsync();
                _settingFlyout.Unloaded += async (sender2, args) =>
                {
                    if (SettingFlyout.IsLogout == true)
                        await Refresh();
                };
                _settingFlyout.GotoSetting(SettingOptions.logout);
                _settingFlyout.Show();
            }
            
        }

        private async void Feedback_Tapped()
        {

            string address = "service@instructables.com";
            string subject = "Instructables App Feedback";

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("\n\n\n\nIf this is a bug report, please be as descriptive as possible. Let us know exactly what steps you took to product the bug. Thank you!%0D%0A");
            builder.Append("=============================%0D%0A");
            
            var vm = DataContext as LandingPageViewModel;
            string usrLoginStatus = string.Empty;
            if (vm.IsLogin)
                usrLoginStatus = vm.userProfile.screenName;
            else
                usrLoginStatus = "Logged out user";
            builder.Append("Username: ");
            builder.Append(usrLoginStatus);
            builder.Append("%0D%0A");

            builder.Append("Device: ");
            builder.Append(Windows.Networking.Proximity.PeerFinder.DisplayName);
            builder.Append("%0D%0A");

            builder.Append("Instructables version: ");
            builder.Append(GetApplicationVersion());
            builder.Append("%0D%0A");

            builder.Append("Windows store app version: Windows 8.1");
            builder.Append("%0D%0A");

            string body = builder.ToString();
                
            var mailto = "mailto:?to=" + address + "&subject=" + subject + "&body=" + body;
            var options = await Windows.System.Launcher.LaunchUriAsync(new Uri(mailto));
            if (options)
            {

            }
            else
            {
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Can't Share";
                dialog.Content = "There are no apps to share with.";
                
                await dialog.ShowAsync();
            }

        }

        private string GetApplicationVersion()
        {
            var ver = Windows.ApplicationModel.Package.Current.Id.Version;
            return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
        }
        private async void Logout()
        {
            var DataService = InstructablesDataService.DataServiceSingleton;
            await DataService.Logout();
            await Refresh();
        }

        protected override string DetermineVisualState(Windows.UI.ViewManagement.ApplicationViewState viewState)
        {
            var vm = this.DataContext as LandingPageViewModel;
            if (vm != null)
            {
                vm.ScreenHeight = Window.Current.Bounds.Height;
                vm.UpdateState(viewState.ToString());
            }
            return base.DetermineVisualState(viewState);
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (SessionManager.IsShowLicenseAgreement())
            {
                var licenseVM = ViewModelLocator.Instance.LicenseVM;
                await licenseVM.LoadLicenseAgreementFile();
                EULAControl.Visibility = Visibility.Visible;
            }

            this.BottomAppBar.IsOpen = false;
            this.TopAppBar.IsOpen = false;

            base.OnNavigatedTo(e);

            var vm = DataContext as LandingPageViewModel;
            if (e.NavigationMode == NavigationMode.New)
            {
                //await vm.Initialize();  //Initialized in LoadState()
            }
            else
            {
                await vm.UpdateUserProfile();
            }
            vm.IsFavorites = false;
            /*var HeroPageData = new HeroPageDataSource();
            HeroPages.ItemsSource = HeroPageData.Items;
            ContextControl.ItemsSource = HeroPageData.Items;*/
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            //this.DefaultViewModel["Groups"] = sampleDataGroups;

            if (pageState == null)
            {
                MainGrid.Background = new SolidColorBrush(Colors.White);
                LoadingPanel.Visibility = Visibility.Visible;
                SplashImage.Visibility = Visibility.Visible;
                MainHubView.Visibility = Visibility.Collapsed;
                CornerButton.Visibility = Visibility.Collapsed;
                //CornerButton.Visibility = Visibility.Collapsed;

                var vm = DataContext as LandingPageViewModel;
                if (vm != null)
                    await vm.Initialize();

                SplashImage.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Collapsed;
                MainHubView.Visibility = Visibility.Visible;
                //CornerButton.Visibility = Visibility.Visible;
                MainGrid.Background = new SolidColorBrush(Color.FromArgb(0xff,0xf6,0xf6,0xf6));
            }
        }

        protected async override void SaveState(Dictionary<String, Object> pageState)
        {

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
        protected async Task Refresh()
        {
            // // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            //this.DefaultViewModel["Groups"] = sampleDataGroups;
            LoadingPanel.Visibility = Visibility.Visible;
            MainHubView.Visibility = Visibility.Collapsed;
            bool cornerButtonVisible = false;
            if (CornerButton.Visibility == Visibility.Visible)
            {
                CornerButton.Visibility = Visibility.Collapsed;
                cornerButtonVisible = true;
            }
            

            var vm = DataContext as LandingPageViewModel;
            if (vm != null)
                await vm.Initialize();

            LoadingPanel.Visibility = Visibility.Collapsed;
            MainHubView.Visibility = Visibility.Visible;
            if (cornerButtonVisible == true)
                CornerButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;
            var groupname = ((DataGroup)group).GroupName;
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            if (groupname == "Contest")
                this.Frame.Navigate(typeof(ContestsPage));
            else
                this.Frame.Navigate(typeof(ExplorePage), ((DataGroup)group).GroupName);
        }

        private void LowerAppBar_OnClosed(object sender, object e)
        {
            Debug.WriteLine("Lower App Bar Closed");
            var vm = this.DataContext as LandingPageViewModel;
            if (vm != null)
            {
                vm.CheckAppBarState();
            }
        }

        private void GridViewItemClicked(object sender, ItemClickEventArgs e)
        {
            InstructableSummary summary = e.ClickedItem as InstructableSummary;

            if (summary != null)
            {
                _currentSectionName = summary.Group.GroupName;

                if (summary.GroupOrdinal == LandingPageTemplateSelector.MoreIndex)
                {
                    this.Frame.Navigate(typeof(ExplorePage), summary.Group.GroupName);
                }
                else 
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", summary.title);
                    if (summary.instructableType == "G" || summary.instructableType == "E")
                        this.Frame.Navigate(typeof(GuideDetail), summary.id);
                    else if (summary.instructableType == "V")
                        this.Frame.Navigate(typeof(VideoDetail), summary.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), summary.id);
                    //this.Frame.Navigate(typeof(VideoDetail), "E0OU1AXHSKV1GRL");
                }
                
            }
        }

        private Color TitleOriginalColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        private Color TitleNewColor = Color.FromArgb(0xFF, 0, 0, 0);

        private void OnSectionChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            SolidColorBrush newBrush = new SolidColorBrush();
            var sectionsList = MainHubView.SectionsInView;
            bool ifHeroPage = false;
            foreach (var section in sectionsList)
            {
                if(section.Name=="HeroPage")
                {
                    ifHeroPage = true;
                    break;
                }
            }

            bool ifFeaturedpage = false;
            foreach (var section in sectionsList)
            {
                if ( section.Name == "Featured" || section.Name == "Recent" || section.Name == "Popular" || section.Name == "Followed" || section.Name == "Contest")
                {
                    ifFeaturedpage = true;
                    break;
                }
            }

            if (ifHeroPage == true)
            {
                newBrush.Color = TitleOriginalColor;
            }
            else
            {
                newBrush.Color = TitleNewColor;
                
            }

            if (ifFeaturedpage == true)
            {
                CornerButton.Visibility = Visibility.Visible;
                //SearchButton.Visibility = Visibility.Visible;
            }
            else
            {
                CornerButton.Visibility = Visibility.Collapsed;
                //SearchButton.Visibility = Visibility.Collapsed;
            }
            pageTitle.Foreground = newBrush;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginFlyout._pageState = LoginPageEnum.Login;
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
                _loginFlyout.Unloaded += async (sender2, args) =>
                {
                    if(SessionManager.IsLoginSuccess() == true)
                    await Refresh();
                };
            }
            _loginFlyout.SwitchPage();
            _loginFlyout.Show();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            LoginFlyout._pageState = LoginPageEnum.Signup;
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
                _loginFlyout.Unloaded += async (sender2, args) =>
                {
                    if (SessionManager.IsLoginSuccess() == true)
                    await Refresh();
                };
            }
            _loginFlyout.SwitchPage();
            _loginFlyout.Show();
        }

        private void ContestItemClicked(object sender, ItemClickEventArgs e)
        {
            Contest contest = e.ClickedItem as Contest;

            if (contest != null)
            {
                if (contest.GroupOrdinal == LandingPageTemplateSelector.MoreIndex)
                {
                    this.Frame.Navigate(typeof(ContestsPage));
                }
                else
                {
                    this.Frame.Navigate(typeof(ContestPage), contest.id);
                }
            }
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var heroFilpView = sender as FlipView;
            int currentIndex = heroFilpView.SelectedIndex;
            if (currentIndex < 0 || currentIndex > heroFilpView.Items.Count - 1)
                return;

            var vm = HeroPage.DataContext as HeroPageDataSource;
            if(vm != null)
            {
                var currentItem = vm.Items[currentIndex] as HeroPageDataItem;
                if(currentItem != null)
                {
                    vm.CurrentItemTitle = currentItem.Title;
                }
            }
        }

        private void ContextControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void SearchKeyword(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            HeroPageDataSource dataSource = button.DataContext as HeroPageDataSource;

            this.Frame.Navigate(typeof(SearchResultsPage), dataSource.CurrentItemTitle);
        }

               


        private void onContextControlLoaded(object sender, RoutedEventArgs e)
        {
            var contentControl = sender as ListBox;
            if (contentControl != null)
            {
                contentControl.SelectedIndex = 0;
            }
        }

        private void ShowLoginLayout(object sender, RoutedEventArgs e)
        {
            LoginFlyout._pageState = LoginPageEnum.Login;
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
                _loginFlyout.Unloaded += async (sender2, args) =>
                {
                    if (SessionManager.IsLoginSuccess() == true)
                    await Refresh();
                };
            }
            _loginFlyout.SwitchPage();
            _loginFlyout.Show();
            
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

        private void OnProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LandingPageViewModel;
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
            {
                LoginFlyout loginFlyout = new LoginFlyout();
                loginFlyout.Unloaded += async (sender2, args) =>
                {
                    if (SessionManager.IsLoginSuccess() == true)
                    {
                        await Refresh();
                        this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
                    }
                };
                
                loginFlyout.Show();
            }
        }

        private void OnSetting_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }

        private void FollowingGridViewItemClicked(object sender, ItemClickEventArgs e)
        {
            Instructable instructable = e.ClickedItem as Instructable;

            if (instructable != null)
            {
                _currentSectionName = instructable.Group.GroupName;
                
                if (instructable.GroupOrdinal == LandingPageTemplateSelector.MoreIndex)
                {
                    this.Frame.Navigate(typeof(ExplorePage), instructable.Group.GroupName);
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_View", "id", instructable.title);
                    if (instructable.type == "guide" || instructable.type == "guide&ebookFlag=true")
                        this.Frame.Navigate(typeof(GuideDetail), instructable.id);
                    else if (instructable.type == "video")
                        this.Frame.Navigate(typeof(VideoDetail), instructable.id);
                    else
                        this.Frame.Navigate(typeof(InstructableDetail), instructable.id);
                    //this.Frame.Navigate(typeof(VideoDetail), "E0OU1AXHSKV1GRL");
                }
            }
        }

        private void TopHome_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            this.BottomAppBar.IsOpen = false;
        }

        private void TopContests_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ContestsPage));
        }

        private async void TopMyInstructables_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LandingPageViewModel;
            if (vm.IsLogin)
            {
                LoadingPanel.Visibility = Visibility.Visible;

                this.Frame.Navigate(typeof(ExplorePage), "Published");
                LoadingPanel.Visibility = Visibility.Collapsed;
            }
            else
                ShowLoginLayout(this, null);
        }

        private void TopProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LandingPageViewModel;
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }

        private void OnMyInstructables_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "my instructables");
            var vm = DataContext as LandingPageViewModel;
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

        private void OnFeedback_Clicked(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "feedback");
            Feedback_Tapped();
        }

        private void OnFavorites_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "favorite");
            var vm = DataContext as LandingPageViewModel;
            if (vm.IsLogin)
            {
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
                vm.IsFavorites = true;
            }
            else
                ShowLoginLayout(this, null);
        }

        private void OnHero_Loaded(object sender, RoutedEventArgs e)
        {
            if (AnimationTimer.IsEnabled)
                return;
            TimeSpan animationTimeGap = TimeSpan.FromMilliseconds(ANIMATION_TIME_GAP);
            AnimationTimer.Interval = animationTimeGap;
            AnimationTimer.Tick += (sender1, args) =>
            {
                
                FlipView heroPages = sender as FlipView;
                if (heroPages.SelectedIndex == 0)
                {
                    animationDirection = 1;
                    heroPages.SelectedIndex += animationDirection;
                }
                else if (heroPages.SelectedIndex < heroPages.Items.Count - 2)
                {
                    heroPages.SelectedIndex += animationDirection;
                }
                else if(heroPages.SelectedIndex == heroPages.Items.Count - 2)
                {
                    heroPages.SelectedIndex += animationDirection;
                    animationDirection = -1;
                }
                else
                {
                    animationDirection = -1;
                    heroPages.SelectedIndex += animationDirection;
                }

            };
            AnimationTimer.Start();
        }

        private void OnFollowingHeaderClick(object sender, RoutedEventArgs e)
        {
            ShowLoginLayout(this, null);
        }

        private async void OnRefresh(object sender, RoutedEventArgs e)
        {
            this.BottomAppBar.IsOpen = false;
            this.TopAppBar.IsOpen = false;
            await Refresh();
        }

        private void OnHero_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimationTimer.Stop();
            AnimationDelayTimer.Start();
        }


    }
}
