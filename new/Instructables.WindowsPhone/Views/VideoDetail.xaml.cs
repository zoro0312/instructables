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
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Navigation;
using Instructables.DataServices;
using System.Net;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Instructables.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class VideoDetail : Instructables.Common.LayoutAwarePage
    {
        private const string INSTRUCTABLE_BASE_URL = "http://www.instructables.com/id/";
        public static InstructableAfterLogin afterLogin = InstructableAfterLogin.None;
        public VideoDetail()
        {
            this.InitializeComponent();
            DataTransferManager dtManager = DataTransferManager.GetForCurrentView();
            dtManager.DataRequested += dtManager_DataRequested;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
            e.Request.Data.SetWebLink(new Uri(INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString));
            //e.Request.Data.SetUri(new Uri(INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString));
            //e.Request.Data.SetHtmlFormat("");
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
            vm.DetailAppBar = DetailAppBar;
            GuideViewPortraitView.Visibility = Visibility.Collapsed;
            BottomAppBar.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            var dataService = InstructablesDataService.DataServiceSingleton;

            if (!String.IsNullOrEmpty(_requestedInstructable) && vm != null)
                await vm.LoadInstructable(_requestedInstructable);
            if (vm != null)
            {
                vm.IsFollowing = vm.SelectedInstructable.following;
                vm.IsFavorite = vm.SelectedInstructable.favorite;

                if (SessionManager.IsLoginSuccess() == true && afterLogin != InstructableAfterLogin.None)
                {
                    switch (afterLogin)
                    {
                        case InstructableAfterLogin.Follow:
                            if (vm.IsFollowing == false)
                            {
                                GoogleAnalyticsTracker.SendEvent("Ible_follow", "author name", vm.SelectedInstructable.author.screenName);
                                string authorName = vm.SelectedInstructable.author.screenName;
                                var result = await dataService.FollowAuthor(authorName);
                                if (result.isSucceeded)
                                {
                                    vm.IsFollowing = true;
                                    vm.SelectedInstructable.following = true;
                                }
                            }
                            break;
                        case InstructableAfterLogin.Like:
                            if (vm.IsFavorite == false)
                            {
                                GoogleAnalyticsTracker.SendEvent("Ible_favorite", "id", vm.SelectedInstructable.title);
                                string instructbleId = vm.SelectedInstructable.id;
                                var result = await dataService.ToggleFavorite(instructbleId);
                                if (result.isSucceeded)
                                {
                                    vm.IsFavorite = true;
                                    vm.SelectedInstructable.favorite = true;
                                }
                            }
                            break;
                        case InstructableAfterLogin.Flag_Inapropraite:
                            {
                                var result = await dataService.Flag(vm.SelectedInstructable.id, "inappropriate");
                                if (result.isSucceeded)
                                {
                                    flagResultText = "Success to flag the instructable as Inapropraite.";
                                }
                                else
                                {
                                    flagResultText = "Fail to flag the instructable";
                                }
                            }

                            break;
                        case InstructableAfterLogin.Flag_Incomplete:
                            {
                                var result = await dataService.Flag(vm.SelectedInstructable.id, "incomplete");
                                if (result.isSucceeded)
                                {
                                    flagResultText = "Success to flag the instructable as incomplete.";
                                }
                                else
                                {
                                    flagResultText = "Fail to flag the instructable";
                                }
                            }

                            break;
                        case InstructableAfterLogin.Flag_Spam:
                            {
                                var result = await dataService.Flag(vm.SelectedInstructable.id, "spam");
                                if (result.isSucceeded)
                                {
                                    flagResultText = "Success to flag the instructable as spam.";
                                }
                                else
                                {
                                    flagResultText = "Fail to flag the instructable";
                                }

                            }

                            break;
                        case InstructableAfterLogin.Flag_Wrong_Category:
                            {
                                var result = await dataService.Flag(vm.SelectedInstructable.id, "wrong-category");
                                if (result.isSucceeded)
                                {
                                    flagResultText = "Success to flag the instructable as wrong category.";
                                }
                                else
                                {
                                    flagResultText = "Fail to flag the instructable";
                                }
                            }

                            break;
                    }
                    //LoginInfo.loginSuccess = false;
                }

                if (vm.SelectedInstructable.favorite == true)
                {
                    Like.Label = "unfavorite";
                    Like.Icon = new SymbolIcon(Symbol.Dislike);
                }
                else
                {
                    Like.Label = "favorite";
                    Like.Icon = new SymbolIcon(Symbol.Like);
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
            }



            //this.DataContext = vm;
            if (vm != null && vm.SelectedInstructable != null)
            {
                try
                {
                    //SizeHeaderText(vm);
                    ProcessText();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Exception in video web view: {0}", ex.Message));
                }
            }
            GuideViewPortraitView.Visibility = Visibility.Visible;
            BottomAppBar.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
            if (afterLogin == InstructableAfterLogin.Flag_Inapropraite || afterLogin == InstructableAfterLogin.Flag_Incomplete || afterLogin == InstructableAfterLogin.Flag_Spam || afterLogin == InstructableAfterLogin.Flag_Wrong_Category)
            {
                if (SessionManager.IsLoginSuccess() == true)
                {
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Success";
                    dialog.Content = flagResultText;

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));
                }
            }
            afterLogin = InstructableAfterLogin.None;
        }

        private string _requestedInstructable;

        //private string _currentViewState;
        //Ethan temp
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

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (vm != null)
            {
                if (vm.BackFromFullScreenPicture != true && vm.ShowVideoViewer != true)
                {
                    InstructableDetailViewModel.CurrentInstructbaleDV = null;
                    GuideViewPortraitView.Visibility = Visibility.Collapsed;
                    GoBack(this, new RoutedEventArgs());
                }
                else
                {
                    //StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    // Hide the status bar
                    //statusBar.ShowAsync();
                    vm.BackFromFullScreenPicture = false;
                    vm.ShowVideoViewer = false;
                    BottomAppBar.Visibility = Visibility.Visible;
                }
            }
        }

        private void ProcessText()
        {
            var vm = this.DataContext as InstructableDetailViewModel;
            ContentTextBlockPortrait.Blocks.Clear();
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
                //for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                //{
                //    Block b = newRichText.Blocks[i];
                //    newRichText.Blocks.RemoveAt(i);
                //    ContentTextBlockPortrait.Blocks.Insert(0, b);
                //}
                //LandScapeRTC.ResetLayout();

                //newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                // Move the blocks in the new RichTextBlock to the target RichTextBlock
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    ContentTextBlockPortrait.Blocks.Insert(0, b);
                }

                //newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                // Move the blocks in the new RichTextBlock to the target RichTextBlock
                //for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                //{
                //    Block b = newRichText.Blocks[i];
                //    newRichText.Blocks.RemoveAt(i);
                //    ContentTextBlockSnapped.Blocks.Insert(0, b);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Exception formatting HTML content: {0}",ex.Message));  
                throw;
            }
        }

        /*private void SizeHeaderText(InstructableDetailViewModel vm)
        {
            pageTitle.MaxWidth = Window.Current.Bounds.Width / 2;
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
            Debug.WriteLine(String.Format("FontSize : {0}", fontSize));
        }*/

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //base.SaveState(pageState);
            //InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            //vm.ClearData();
            //webView.NavigateToString("about:blank");
            //webViewPortrait.NavigateToString("about:blank");
            //webViewSnapped.NavigateToString("about:blank");
        }

        private async void UriTapped(object sender, TappedRoutedEventArgs e)
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
                await Launcher.LaunchUriAsync(new Uri(uriString));
        }

        private async void RetryLoadInstructable(object sender, RoutedEventArgs e)
        {
            await LoadInstructable();
        }

        private void WebViewSizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private async void AppBarButton_Like_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Like;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GoogleAnalyticsTracker.SendEvent("Ible_favorite", "id", vm.SelectedInstructable.title);
                string instructbleId = vm.SelectedInstructable.id;
                Like.IsEnabled = false;

                var result = await dataService.ToggleFavorite(instructbleId);
                Like.IsEnabled = true;
                if (result == null)
                    return;

                if (result.isSucceeded)
                {
                    vm.SelectedInstructable.favorite = !vm.SelectedInstructable.favorite;
                    vm.IsFavorite = vm.SelectedInstructable.favorite;
                    //Like.IsChecked = true;
                }
                else
                {
                    //Like.IsChecked = vm.IsFavorite;
                }

                if (vm.SelectedInstructable.favorite == true)
                {
                    Like.Label = "unfavorite";
                    Like.Icon = new SymbolIcon(Symbol.Dislike);
                }
                else
                {
                    Like.Label = "favorite";
                    Like.Icon = new SymbolIcon(Symbol.Like);
                    //Like.Icon = null;
                    //Like.Icon = Favorite;
                }
            }



        }

        private async void FollowAuthor(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Follow;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                string authorName = vm.SelectedInstructable.author.screenName;
                GoogleAnalyticsTracker.SendEvent("Ible_follow", "author name", authorName);
                vm.FollowEnable = false;
                var result = await dataService.FollowAuthor(authorName);
                vm.FollowEnable = true;
                if (result == null)
                    return;

                if (result.isSucceeded)
                {
                    vm.SelectedInstructable.following = true;
                    vm.IsFollowing = vm.SelectedInstructable.following;
                }
            }

        }

        private async void UnfollowAuthor(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Follow;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                string authorName = vm.SelectedInstructable.author.screenName;
                GoogleAnalyticsTracker.SendEvent("Ible_unfollow", "author name", authorName);
                vm.FollowEnable = false;
                var result = await dataService.UnfollowAuthor(authorName);
                vm.FollowEnable = true;
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    vm.SelectedInstructable.following = false;
                    vm.IsFollowing = vm.SelectedInstructable.following;
                }
            }

        }

        private void AppBarButton_Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private void AppBarButton_Comment_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            var vm = this.DataContext as InstructableDetailViewModel;
            this.Frame.Navigate(typeof(CommentsPage), vm.SelectedInstructable.urlString);
            //this.Frame.Navigate(typeof(CommentsPage), "Wooden-Bulls-Head-Remix");
        }

        private void AppBarButton_Voting_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "start");
            this.Frame.Navigate(typeof(VotingContests));
        }

        private void AppBarButton_Flag_Click(object sender, RoutedEventArgs e)
        {
            BottomAppBar.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private String flagResultText = String.Empty;

        private async void AppBarButton_Inapropraite_Click(object sender, RoutedEventArgs e)
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                afterLogin = InstructableAfterLogin.Flag_Inapropraite;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GuideViewPortraitView.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "inappropriate");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Inappropriate.";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                }
                Recover_App_Bar();
                GuideViewPortraitView.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Success";
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
                afterLogin = InstructableAfterLogin.Flag_Spam;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GuideViewPortraitView.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "spam");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Spam.";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                }
                Recover_App_Bar();
                GuideViewPortraitView.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Success";
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
                afterLogin = InstructableAfterLogin.Flag_Incomplete;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GuideViewPortraitView.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "incomplete");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Incomplete.";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                }
                Recover_App_Bar();
                GuideViewPortraitView.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Success";
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
                afterLogin = InstructableAfterLogin.Flag_Wrong_Category;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                GuideViewPortraitView.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                var result = await dataService.Flag(vm.SelectedInstructable.id, "wrong-category");
                if (result == null)
                    return;
                if (result.isSucceeded)
                {
                    flagResultText = "Success to flag the instructable as Wrong Category.";
                }
                else
                {
                    flagResultText = "Fail to flag the instructable";
                }
                Recover_App_Bar();
                GuideViewPortraitView.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Success";
                dialog.Content = flagResultText;

                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();
                //Flag_Result.ShowAt(this);
            }
        }


        private void Recover_App_Bar()
        {
            if (BottomAppBar.Visibility == Visibility.Collapsed)
                BottomAppBar.Visibility = Visibility.Visible;
        }

        private void AppBarButton_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Recover_App_Bar();
        }


        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
            //List<string> param = new List<string>();
            //param.Add(vm.SelectedInstructable.author.screenName);
            //userProfileVM.InitData = param;
            var param = new UserProfileViewModel.ProfileInitData();
            param.screenName = vm.SelectedInstructable.author.screenName;
            userProfileVM.InitData.Add(param);
            userProfileVM.CurrentInitData += 1;
            this.Frame.Navigate(typeof(UserProfilePage));
        }

        private void Flag_Flyout_Closing(object sender, object e)
        {
            Recover_App_Bar();
        }

        private void flag_result_ok_Click(object sender, RoutedEventArgs e)
        {
            Flag_Result.Hide();
        }
    }
}
