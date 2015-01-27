using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Instructables.DataServices;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Instructables.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class VideoDetail : Instructables.Common.LayoutAwarePage
    {
        public static InstructableAfterLogin afterLogin = InstructableAfterLogin.None;
        private static CommentsFlyout _commentsFlyout = null;
        private static VotingContestsFlyout _votingContestsFlyout = null;
        private static LoginFlyout _loginFlyout = null;
        private String flagResultText = String.Empty;
        private String flagResultTitle = String.Empty;
        private static SearchFlyout _searchFlyout = null;


        public VideoDetail()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
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
            if (!String.IsNullOrEmpty(_requestedInstructable) && vm != null)
                await vm.LoadInstructable(_requestedInstructable);
            if (vm.SelectedInstructable == null)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                return;
            }
            //this.DataContext = vm;
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
            if (vm != null && vm.SelectedInstructable != null)
            {
                //SizeHeaderText(vm);
                //ProcessText();
                //webView.NavigateToString(vm.SelectedInstructable.video);
                //webViewPortrait.NavigateToString(vm.SelectedInstructable.video);
                //webView.Navigate(vm.VideoUri);
                //webViewPortrait.Navigate(vm.VideoUri);
                //webViewSnapped.Navigate(vm.VideoUri);
                ObservableCollection<object> _guideDataSource = new ObservableCollection<object>();
                _guideDataSource.Add(vm.SelectedInstructable);
                _guideDataSource.Add(vm);
                //_guideDataSource.Add(vm.SelectedInstructable.instructables);
                MainView.ItemsSource = _guideDataSource;
            }
            MainView.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
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

        private string _currentViewState;

        /*protected override string DetermineVisualState(Windows.UI.ViewManagement.ApplicationViewState viewState)
        {
            _currentViewState = viewState.ToString();
            if (_currentViewState == "Snapped")
            {
                webViewSnapped.Width = Window.Current.Bounds.Width - 40;
                var vm = this.DataContext as InstructableDetailViewModel;
                if (vm != null)
                {
                    var ratio = (float) vm.VideoHeight/(float) vm.VideoWidth;
                    webViewSnapped.Height = vm.VideoHeight*ratio;
                }
            }
            return base.DetermineVisualState(viewState);
        }*/

        private void ProcessText()
        {
            /*var vm = this.DataContext as InstructableDetailViewModel;
            ContentTextBlock.Blocks.Clear();
            // Wrap the value of the Html property in a div and convert it to a new RichTextBlock
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:utils=\"using:Instructables.Utils\" >");
            sb.AppendLine(vm.SelectedInstructable.steps[0].XamlBody);
            sb.AppendLine("</RichTextBlock>");

            try
            {
                RichTextBlock newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                // Move the blocks in the new RichTextBlock to the target RichTextBlock
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    ContentTextBlock.Blocks.Insert(0, b);
                }
                LandScapeRTC.ResetLayout();

                newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                // Move the blocks in the new RichTextBlock to the target RichTextBlock
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    ContentTextBlockPortrait.Blocks.Insert(0, b);
                }

                newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                // Move the blocks in the new RichTextBlock to the target RichTextBlock
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    ContentTextBlockSnapped.Blocks.Insert(0, b);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Exception formatting HTML content: {0}",ex.Message));  
                throw;
            }*/
        }

        private void SizeHeaderText(InstructableDetailViewModel vm)
        {
            /*pageTitle.MaxWidth = Window.Current.Bounds.Width / 2;
            // This code could be put in a custom control at some point, it just adjusts the font size of the title until it fits
            // within the maximum size allowed for the title.  It will shrink the font from 48 to 24 points and select the largest
            // font that will work. If its still too long at 24 point for one line, it will set it to 24 and the TextBox will wrap to
            // two lines automatically, and if still too long will use word ellipses to cut off the second line.
            int fontSize = 48;
            if (vm.SelectedInstructable != null)
            {
                TextBlock tb = new TextBlock();
                tb.Style = Resources["TitleStyle"] as Style;
                tb.Text = vm.SelectedInstructable.title;
                double requiredSize;
                tb.Measure(new Size(double.MaxValue, double.MaxValue));
                requiredSize = tb.DesiredSize.Width;
                while (requiredSize > pageTitle.MaxWidth && fontSize > 24)
                {
                    fontSize--;
                    tb.FontSize = fontSize;
                    tb.Measure(new Size(double.MaxValue, double.MaxValue));
                    requiredSize = tb.DesiredSize.Width;
                }
                pageTitle.FontSize = fontSize;
            }
            Debug.WriteLine(String.Format("FontSize : {0}", fontSize));*/
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            vm.ClearData();
            /*webView.NavigateToString("about:blank");
            webViewPortrait.NavigateToString("about:blank");
            webViewSnapped.NavigateToString("about:blank");*/
        }

        /*private void UriTapped(object sender, TappedRoutedEventArgs e)
        {
            var richTB = sender as RichTextBlock;
            TextPointer tp = null;
            if (richTB != null)
            {
                tp = richTB.GetPositionFromPoint(e.GetPosition(richTB));
            }
            else
            {
                var richTBO = sender as RichTextBlockOverflow;
                if (richTBO == null)
                    return;
                tp = richTBO.GetPositionFromPoint(e.GetPosition(richTBO));
            }
            if (tp == null)
                return;

            var element = tp.Parent as TextElement;
            if (element == null)
                return;
            var uriString = Utils.AttachedUri.GetUriSource(element);
            if (uriString != String.Empty)
                Launcher.LaunchUriAsync(new Uri(uriString));
        }*/

        private async void RetryLoadInstructable(object sender, RoutedEventArgs e)
        {
            await LoadInstructable();
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            InstructableGuideItem summary = e.ClickedItem as InstructableGuideItem;
            if (summary != null)
            {
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", summary.title);
                this.Frame.Navigate(typeof(InstructableDetail), summary.id);
            }

        }

        private void UriTapped(object sender, TappedRoutedEventArgs e)
        {
            var richTB = sender as RichTextBlock;
            TextPointer tp = null;
            if (richTB != null)
            {
                tp = richTB.GetPositionFromPoint(e.GetPosition(richTB));
            }
            else
            {
                var richTBO = sender as RichTextBlockOverflow;
                if (richTBO == null)
                    return;
                tp = richTBO.GetPositionFromPoint(e.GetPosition(richTBO));
            }
            if (tp == null)
                return;

            var element = tp.Parent as TextElement;
            if (element == null)
                return;
            var uriString = Utils.AttachedUri.GetUriSource(element);
            if (uriString != String.Empty)
                Launcher.LaunchUriAsync(new Uri(uriString));

        }

        /*private void OnTopStepSelecterClick(object sender, RoutedEventArgs e)
        {
            TopStepFlyout.ShowAt(this);
        }*/

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

        /*private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var stepGroup = e.ClickedItem as StepGroup;
            var step = stepGroup.Steps[0];
            var stepIndex = step.stepIndex;
            MainView.SelectedIndex = stepIndex + 1;
            TopStepFlyout.Hide();
        }*/

        private void OnCommentClick(object sender, RoutedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
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
                if (_follow == true)
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

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainView.SelectedIndex == 0)
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

        private void OnAuthorClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as InstructableDetailViewModel;
            var screenName = vm.SelectedInstructable.author.screenName;
            this.Frame.Navigate(typeof(UserProfilePage), screenName);
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
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }
    }
}
