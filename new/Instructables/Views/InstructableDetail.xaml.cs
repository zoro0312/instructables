using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Instructables.Common;
using Instructables.Controls;
using Instructables.DataModel;
using Instructables.Utils;
using Instructables.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Popups;

using Instructables.DataServices;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Instructables.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public enum InstructableAfterLogin
    {
        None = 0,
        Follow = 1,
        Like = 2,
        Flag_Inapropraite = 3,
        Flag_Spam = 4,
        Flag_Incomplete = 5,
        Flag_Wrong_Category = 6
    }

    public sealed partial class InstructableDetail : LayoutAwarePage
    {
        private const string INSTRUCTABLE_BASE_URL = "http://www.instructables.com/id/";

        public static InstructableAfterLogin afterLogin = InstructableAfterLogin.None; 
        private static CommentsFlyout _commentsFlyout = null;
        private static VotingContestsFlyout _votingContestsFlyout = null;
        private static SearchFlyout _searchFlyout = null;
        private static LoginFlyout _loginFlyout = null;

        private String flagResultText = String.Empty;
        private String flagResultTitle = String.Empty;

        public InstructableDetail()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (vm != null)
                InstructableDetailViewModel.CurrentInstructbaleDV = vm;
            DataTransferManager dtManager = DataTransferManager.GetForCurrentView();
            dtManager.DataRequested += dtManager_DataRequested;


            vm.ShowPhotoViewer = false;
            vm.ShowVideoViewer = false;
            vm.ShowBottomBar = true;
        }

        private void dtManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            e.Request.Data.Properties.Title = "Checkout this awesome Instructable";
            /*e.Request.Data.Properties.Description = "Check out this awesome Instructable \"" + vm.SelectedInstructable.title + "\"\n"
                                                    + "You can view this Instructable here: \"" + INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString + "\n\n"
                                                    + "Instructables is a place that lets you explore, document, and share your creations.\n \n"
                                                    + "Download Instructables for your Windows Phone device from Windows App Store now.";*/
            //e.Request.Data.Properties.
            /*string textBody = "Check out this awesome Instructable \"" + vm.SelectedInstructable.title + "\"\n"
                                                    + "You can view this Instructable here: \"" + INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString + "\n\n"
                                                    + "Instructables is a place that lets you explore, document, and share your creations.\n \n"
                                                    + "Download Instructables for your Windows Phone device from Windows App Store now.";*/
            //e.Request.Data.SetText(textBody);
            if(vm.SelectedInstructable != null)
            e.Request.Data.SetWebLink(new Uri(INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString));
            //e.Request.Data.SetUri(new Uri(INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString));
            //e.Request.Data.SetHtmlFormat("");
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
            _requestedInstructable = navigationParameter as string;
            await LoadInstructable();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                if (vm != null) 
                    vm.ClearData();
            }
        }

        private bool IfVoted(Instructable vms)
        {
            if (vms.votableContests == null)
            {
                return false;
            }

            foreach (var vote in vms.votableContests)
            {
                if (vote.votedFor == true)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task LoadInstructable()
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;

            MainView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            vm.ClearData();
            if (!String.IsNullOrEmpty(_requestedInstructable))
                await vm.LoadInstructable(_requestedInstructable);
            if (vm.SelectedInstructable == null)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                return;
            }
            var dataService = InstructablesDataService.DataServiceSingleton;
            if (SessionManager.IsLoginSuccess() == true && afterLogin != InstructableAfterLogin.None)
            {
                switch (afterLogin)
                {
                    case InstructableAfterLogin.Follow:
                        if (vm.SelectedInstructable.following == false)
                        {
                            GoogleAnalyticsTracker.SendEvent("Ible_follow", "author name", vm.SelectedInstructable.author.screenName);
                            string authorName = vm.SelectedInstructable.author.screenName;
                            bool _follow = !vm.SelectedInstructable.following;
                            var result = await dataService.FollowAuthor(authorName, _follow);
                            if (result.isSucceeded)
                            {
                                vm.SelectedInstructable.following = !vm.SelectedInstructable.following;
                            }
                        }
                        break;
                    case InstructableAfterLogin.Like:
                        if (vm.SelectedInstructable.favorite == false)
                        {
                            GoogleAnalyticsTracker.SendEvent("Ible_favorite", "id", vm.SelectedInstructable.title);
                            string instructbleId = vm.SelectedInstructable.id;
                            var result = await dataService.ToggleFavorite(instructbleId);
                            if (result.isSucceeded)
                            {
                                vm.SelectedInstructable.favorite = !vm.SelectedInstructable.favorite;
                            }
                        }
                        break;
                    case InstructableAfterLogin.Flag_Inapropraite:
                        {
                            var result = await dataService.Flag(vm.SelectedInstructable.id, "inappropriate");
                            if (result.isSucceeded)
                            {
                                flagResultText = "Success to flag the instructable as Inappropriate.";
                                flagResultTitle = "Success";
                            }
                            else
                            {
                                flagResultText = "Fail to flag the instructable";
                                flagResultTitle = "Error";
                            }
                        }

                        break;
                    case InstructableAfterLogin.Flag_Incomplete:
                        {
                            var result = await dataService.Flag(vm.SelectedInstructable.id, "incomplete");
                            if (result.isSucceeded)
                            {
                                flagResultText = "Success to flag the instructable as Incomplete.";
                                flagResultTitle = "Success";
                            }
                            else
                            {
                                flagResultText = "Fail to flag the instructable";
                                flagResultTitle = "Error";
                            }
                        }

                        break;
                    case InstructableAfterLogin.Flag_Spam:
                        {
                            var result = await dataService.Flag(vm.SelectedInstructable.id, "spam");
                            if (result.isSucceeded)
                            {
                                flagResultText = "Success to flag the instructable as Spam.";
                                flagResultTitle = "Success";
                            }
                            else
                            {
                                flagResultText = "Fail to flag the instructable";
                                flagResultTitle = "Error";
                            }

                        }

                        break;
                    case InstructableAfterLogin.Flag_Wrong_Category:
                        {
                            var result = await dataService.Flag(vm.SelectedInstructable.id, "wrong-category");
                            if (result.isSucceeded)
                            {
                                flagResultText = "Success to flag the instructable as Wrong Category.";
                                flagResultTitle = "Success";
                            }
                            else
                            {
                                flagResultText = "Fail to flag the instructable";
                                flagResultTitle = "Error";
                            }
                        }

                        break;
                }
                //LoginInfo.loginSuccess = false;
            }

           
            if (vm.SelectedInstructable.votableContests != null)
            {
                if (IfVoted(vm.SelectedInstructable) == true)
                {
                    Voting.Visibility = Visibility.Collapsed;
                    Voted.Visibility = Visibility.Visible;
                }
                else
                {
                    Voting.Visibility = Visibility.Visible;
                    Voted.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                Voting.Visibility = Visibility.Collapsed;
                Voted.Visibility = Visibility.Collapsed;
            }

            if (vm.SelectedInstructable.following == true)
            {
                FollowText.Text = "Unfollow";
                //SolidColorBrush foreBrush = new SolidColorBrush();
                //Color foreColor = Color.FromArgb(0xFF, 0xE9, 0x07, 0x8B);
                //foreBrush.Color = foreColor;
                //Follow.Foreground = foreBrush;
                //FollowText.Foreground = foreBrush;
            }
            else
            {
                FollowText.Text = "Follow";
                //SolidColorBrush foreBrush = new SolidColorBrush();
                //Color foreColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
                //foreBrush.Color = foreColor;
                //Follow.Foreground = foreBrush;
                //FollowText.Foreground = foreBrush;
            }

            if (vm.SelectedInstructable.favorite == true)
            {
                FavoriteText.Text = "Unfavorite";
                SolidColorBrush foreBrush = new SolidColorBrush();
                Color foreColor = Color.FromArgb(0xFF, 0xE9, 0x07, 0x8B);
                foreBrush.Color = foreColor;
                Favorite.Foreground = foreBrush;
                FavoriteText.Foreground = foreBrush;

            }
            else
            {
                FavoriteText.Text = "Favorite";
                SolidColorBrush iconBrush = new SolidColorBrush();
                SolidColorBrush foreBrush = new SolidColorBrush();
                Color iconColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                Color foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                iconBrush.Color = iconColor;
                Favorite.Foreground = iconBrush;
                foreBrush.Color = foreColor;
                FavoriteText.Foreground = foreBrush;
            }

            vm.ABSSteps.Insert(0,vm.SelectedInstructable);
            MainView.ItemsSource = vm.ABSSteps;
            MainView.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
            SizeHeaderText(vm);
            if (afterLogin == InstructableAfterLogin.Flag_Inapropraite || afterLogin == InstructableAfterLogin.Flag_Incomplete || afterLogin == InstructableAfterLogin.Flag_Spam || afterLogin == InstructableAfterLogin.Flag_Wrong_Category)
            {
                if (SessionManager.IsLoginSuccess() == true)
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = flagResultTitle;
                    dialog.Content = flagResultText;

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                    await dialog.ShowAsync();
                }
            }
            afterLogin = InstructableAfterLogin.None;
        }

        private string _requestedInstructable;

        private void SizeHeaderText(InstructableDetailViewModel vm)
        {
            
            
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            //vm.ClearData();
        }

        private void NavigateToStep()
        {
            if (ViewModelLocator.Instance.DetailVM.SelectedStep != null)
            {
                
            }
        }


        private Popup _popup;
        
        private void OpenNavPanel(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine(sender.ToString());
            var sp = sender as StackPanel;
            if (sp != null)
            {
                var offset = sp.TransformToVisual(this);
                var childToParent = offset.TransformPoint((new Point(0, 0)));
                InstructableNavigationDropDown dropDown = new InstructableNavigationDropDown();
                dropDown.Height = Window.Current.Bounds.Height;
                dropDown.SetTopOffset(childToParent.Y);
                dropDown.DataContext = this.DataContext;
                dropDown.Arrange(new Rect(0, 0, dropDown.MaxWidth, Window.Current.Bounds.Height));
                dropDown.SetSelectedIndex(ViewModelLocator.Instance.DetailVM.SelectedStep.stepIndex);
                dropDown.StepSelected += (o, args) =>
                    {
                        if (_popup != null)
                            _popup.IsOpen = false;
                        _popup = null;
                        NavigateToStep();
                    };
                _popup = new Popup();
                _popup.Height = Window.Current.Bounds.Height;
                _popup.Width = dropDown.ActualWidth;
                _popup.Child = dropDown;
                _popup.HorizontalOffset = childToParent.X - 5.0;
                _popup.IsLightDismissEnabled = true;
                _popup.IsOpen = true;
            }
        }

        private async void RetryLoadInstructable(object sender, RoutedEventArgs e)
        {
            await LoadInstructable();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MainView.SelectedIndex == 0)
            {
                MainTitle.Visibility = Visibility.Collapsed;
                CornerButton.Visibility = Visibility.Collapsed;
                SolidColorBrush foreBrush = new SolidColorBrush();
                Color foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                foreBrush.Color = foreColor;
                backButton.Content = foreBrush;
                
            }
            else
            {
                MainTitle.Visibility = Visibility.Visible;
                CornerButton.Visibility = Visibility.Visible;
                SolidColorBrush foreBrush = new SolidColorBrush();
                Color foreColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
                foreBrush.Color = foreColor;
                backButton.Content = foreBrush;
            }
        }

        private void OnTopStepSelecterClick(object sender, RoutedEventArgs e)
        {
            TopStepFlyout.ShowAt(this);
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

        private void FlyoutClosed(object sender, object e)
        {

        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var stepGroup = e.ClickedItem as StepGroup;
            var step = stepGroup.Steps[0];
            var stepIndex = step.stepIndex;
            MainView.SelectedIndex = stepIndex + 1;
            //TopStepFlyout.Hide();
        }

        private void OnCommentClick(object sender, RoutedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (vm.SelectedInstructable == null)
                return;
            if (_commentsFlyout == null)
            {
                _commentsFlyout = new CommentsFlyout(vm.SelectedInstructable.urlString);
                /*_commentsFlyout.Unloaded += async (sender2, args) =>
                {
                    await Refresh();
                };*/
            }
            else
            {
                _commentsFlyout.UrlString = vm.SelectedInstructable.urlString;
            }
            _commentsFlyout.Show();
        }

        private void OnVoteClick(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "start");

            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (_votingContestsFlyout == null)
            {
                _votingContestsFlyout = new VotingContestsFlyout();
                /*_commentsFlyout.Unloaded += async (sender2, args) =>
                {
                    await Refresh();
                };*/
            }

            _votingContestsFlyout.Show();
        }

        private void OnShareClick(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private async void AppBarButton_Inapropraite_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Flag_Inapropraite;
                ShowLoginLayout();
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;

                LoadingPanelMiddle.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "inappropriate");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Inappropriate.";
                    flagResultTitle = "Success";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                    flagResultTitle = "Error";
                }

                LoadingPanelMiddle.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = flagResultTitle;
                dialog.Content = flagResultText;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
                //Flag_Result.ShowAt(this);
            }
        }

        private async void AppBarButton_Spam_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Flag_Inapropraite;
                ShowLoginLayout();
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;

                LoadingPanelMiddle.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "spam");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Spam.";
                    flagResultTitle = "Success";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                    flagResultTitle = "Error";
                }
                LoadingPanelMiddle.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = flagResultTitle;
                dialog.Content = flagResultText;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
                //Flag_Result.ShowAt(this);
            }
        }

        private async void AppBarButton_Incomplete_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Flag_Inapropraite;
                ShowLoginLayout();
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;

                LoadingPanelMiddle.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "incomplete");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Incomplete.";
                    flagResultTitle = "Success";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                    flagResultTitle = "Error";
                }
                LoadingPanelMiddle.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = flagResultTitle;
                dialog.Content = flagResultText;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
                //Flag_Result.ShowAt(this);
            }
        }

        private async void AppBarButton_Wrong_Category_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Flag_Inapropraite;
                ShowLoginLayout();
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;

                LoadingPanelMiddle.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "wrong-category");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Wrong Category.";
                    flagResultTitle = "Success";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                    flagResultTitle = "Error";
                }
                LoadingPanelMiddle.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = flagResultTitle;
                dialog.Content = flagResultText;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
                //Flag_Result.ShowAt(this);
            }
        }

        private void AppBarButton_Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnFlagFlyoutOpened(object sender, object e)
        {
            SolidColorBrush foreBrush = new SolidColorBrush();
            Color foreColor = Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D);
            foreBrush.Color = foreColor;
            SolidColorBrush backBrush = new SolidColorBrush();
            Color backColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            backBrush.Color = backColor;
            Flag.Foreground = foreBrush;
            Flag.Background = backBrush;
        }

        private void OnFlagFlyoutClosed(object sender, object e)
        {
            SolidColorBrush foreBrush = new SolidColorBrush();
            Color foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF); 
            foreBrush.Color = foreColor;
            SolidColorBrush backBrush = new SolidColorBrush();
            Color backColor = Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D);
            backBrush.Color = backColor;
            Flag.Foreground = foreBrush;
            Flag.Background = backBrush;
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            if (_overviewBottomClick == false)
                return;
            SolidColorBrush foreBrush = new SolidColorBrush();
            Color foreColor = Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D);
            foreBrush.Color = foreColor;
            SolidColorBrush backBrush = new SolidColorBrush();
            Color backColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            backBrush.Color = backColor;
            Overview.Foreground = foreBrush;
            Overview.Background = backBrush;
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            if (_overviewBottomClick == false)
                return;
            SolidColorBrush foreBrush = new SolidColorBrush();
            Color foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            foreBrush.Color = foreColor;
            SolidColorBrush backBrush = new SolidColorBrush();
            Color backColor = Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D);
            backBrush.Color = backColor;
            Overview.Foreground = foreBrush;
            Overview.Background = backBrush;
            _overviewBottomClick = false;
        }

        private bool _overviewBottomClick = false;
        private void OnOverViewClicked(object sender, RoutedEventArgs e)
        {
            _overviewBottomClick = true;
        }

        //private bool _instructableFollowed = false;
        private async void OnFollowClick(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Follow;
                ShowLoginLayout();
                //this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                if (vm.SelectedInstructable == null)
                    return;
                string authorName = vm.SelectedInstructable.author.screenName;
                //LoadingPanelMiddle.Visibility = Visibility.Visible;
                //Follow.IsEnabled = false;
                if (vm.SelectedInstructable.following == false)
                {
                    FollowText.Text = "Unfollow";
                    //SolidColorBrush foreBrush = new SolidColorBrush();
                    //Color foreColor = Color.FromArgb(0xFF, 0xE9, 0x07, 0x8B);
                    //foreBrush.Color = foreColor;
                    //Follow.Foreground = foreBrush;
                    //FollowText.Foreground = foreBrush;

                }
                else
                {
                    FollowText.Text = "Follow";
                    //SolidColorBrush foreBrush = new SolidColorBrush();
                    //Color foreColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
                    //foreBrush.Color = foreColor;
                    //Follow.Foreground = foreBrush;
                    //FollowText.Foreground = foreBrush;
                }
                bool _follow = !vm.SelectedInstructable.following;
                var result = await dataService.FollowAuthor(authorName, _follow);
                if(_follow == true)
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_follow", "author name", authorName);
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_unfollow", "author name", authorName);
                }
                
                //Follow.IsEnabled = true;
                if (result == null)
                    return;

                if (result.isSucceeded)
                {
                    vm.SelectedInstructable.following = !vm.SelectedInstructable.following;
                    var landingVM = ViewModelLocator.Instance.LandingVM;
                    await landingVM.UpdateUserProfile();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Error";
                    dialog.Content = "Fail to follow.";

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                    await dialog.ShowAsync();
                }

                //LoadingPanelMiddle.Visibility = Visibility.Collapsed;
            }
        }

        //private bool _instructableFavorite = false;
        private async void OnFavoriteClick(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Like;
                ShowLoginLayout();
                //this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GoogleAnalyticsTracker.SendEvent("Ible_favorite", "id", vm.SelectedInstructable.title);
                if (vm.SelectedInstructable.favorite == false)
                {
                    FavoriteText.Text = "Unfavorite";
                    SolidColorBrush foreBrush = new SolidColorBrush();
                    Color foreColor = Color.FromArgb(0xFF, 0xE9, 0x07, 0x8B);
                    foreBrush.Color = foreColor;
                    Favorite.Foreground = foreBrush;
                    FavoriteText.Foreground = foreBrush;
                }
                else
                {
                    FavoriteText.Text = "Favorite";
                    SolidColorBrush iconBrush = new SolidColorBrush();
                    SolidColorBrush foreBrush = new SolidColorBrush();
                    Color iconColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                    Color foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                    iconBrush.Color = iconColor;
                    Favorite.Foreground = iconBrush;
                    foreBrush.Color = foreColor;
                    FavoriteText.Foreground = foreBrush;
                }
                string instructbleId = vm.SelectedInstructable.id;
                //Favorite.IsEnabled = false;
                //LoadingPanelMiddle.Visibility = Visibility.Visible;
                var result = await dataService.ToggleFavorite(instructbleId);
                //Favorite.IsEnabled = true;
                if (result == null)
                    return;

                if (result.isSucceeded)
                {
                    vm.SelectedInstructable.favorite = !vm.SelectedInstructable.favorite;
                    var landingVM = ViewModelLocator.Instance.LandingVM;
                    await landingVM.UpdateUserProfile();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Error";
                    dialog.Content = "Fail to favorite.";

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                    await dialog.ShowAsync();
                }

                
                //LoadingPanelMiddle.Visibility = Visibility.Collapsed;
            }
        }

        private void OnAuthorClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as InstructableDetailViewModel;
            if (vm.SelectedInstructable != null)
            {
                var screenName = vm.SelectedInstructable.author.screenName;
                this.Frame.Navigate(typeof(UserProfilePage), screenName);
            }
        }

        private void ShowLoginLayout()
        {
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
                _loginFlyout.Unloaded += async (sender2, args) =>
                {
                    if (SessionManager.IsLoginSuccess() == true)
                    {
                        await LoadInstructable();
                        var vm = ViewModelLocator.Instance.LandingVM;
                        await vm.UpdateUserProfile();
                    }
                };
            }
            _loginFlyout.Show();
        }

        private void ShowLoginLayout(object sender, RoutedEventArgs e)
        {
            ShowLoginLayout();
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
            if (SessionManager.IsLoginSuccess())
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }

    }
}
