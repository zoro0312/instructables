using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.ApplicationModel.Email;
using Windows.Phone.UI.Input;
using Windows.Storage;
using System.Diagnostics;
using Instructables.DataServices;

using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : LayoutAwarePage
    {
        private const string INSTRUCTABLE_BASE_URL = "http://www.instructables.com";        
        private string instructableUrl;

        private enum LoginContinueAction
        {
            MyInstructables,
            Favorites,
            Create
        };
        private LoginContinueAction _continueAction;


        public LandingPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            DataTransferManager dtManager = DataTransferManager.GetForCurrentView();
            dtManager.DataRequested += dtManager_DataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public static bool _backFromLogin = false;
        public static bool _backFromLogout = false;
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        /// 
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Clear the back stack so that the landing page will always be the bottom home page in the stack.
            this.Frame.BackStack.Clear();

            base.OnNavigatedTo(e);

            var vm = DataContext as LandingViewModel;
            if (vm != null)
            {
                vm.VisualState = "Normal";
                if (e.NavigationMode == NavigationMode.New)
                {
                    await vm.Initialize();
                } else
                {
                    if ((_backFromLogin == true && SessionManager.IsLoginSuccess() == true) || (_backFromLogout == true && SessionManager.IsLoginSuccess() == false))
                        await vm.Recover();
                }

            }

            if ((_backFromLogin == true && SessionManager.IsLoginSuccess() != true))
            {
                var svc = DataServices.InstructablesDataService.DataServiceSingleton;
                svc.LoginSucceed -= HandleLoginSucceed;
            }

            _backFromLogin = false;
            _backFromLogout = false;
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            return;
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Details about the click event.</param>
        private void HomeSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            InstructableSummary summary = e.ClickedItem as InstructableSummary;

            if (summary != null)
            {
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", summary.title);
                if (summary.instructableType == "G" || summary.instructableType == "E")
                    this.Frame.Navigate(typeof(GuideDetail), summary.id);
                else if (summary.instructableType == "V")
                    this.Frame.Navigate(typeof(VideoDetail), summary.id);
                else
                    this.Frame.Navigate(typeof(InstructableDetail), summary.id);
            }
        }

        private void ExploreSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataGroup group = e.ClickedItem as DataGroup;

            if (group != null)
            {
                if (!this.Frame.Navigate(typeof(ExplorePage), group.GroupName))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        private void ContestsSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            Contest contest = e.ClickedItem as Contest;

            if (contest != null)
            {
                if (!this.Frame.Navigate(typeof(ContestPage), contest.id))
                {
                    throw new Exception("Failed to create contest page");
                }
            }
        }
        
        private void FollowingSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            Instructable instructable = e.ClickedItem as Instructable;

            if (instructable != null)
            {
                GoogleAnalyticsTracker.SendEvent("Ible_View", "id", instructable.title);
                if (instructable.type == "Guide" || instructable.type == "guide&ebookFlag=true")
                    this.Frame.Navigate(typeof(GuideDetail), instructable.id);
                else if (instructable.type == "Video")
                    this.Frame.Navigate(typeof(VideoDetail), instructable.id);
                else
                    this.Frame.Navigate(typeof(InstructableDetail), instructable.id);
            }
        }

        private void Login_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;
            var vm = DataContext as LandingViewModel;
            
            if (svc.isLogin())
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(vm.userProfile.screenName);
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = vm.userProfile.screenName;
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
            else
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private void MyInstructables_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "my instructables");
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;
            var vm = DataContext as LandingViewModel;

            if (svc.isLogin())
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(vm.userProfile.screenName);
                //param.Add("MyInstruactables");
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = vm.userProfile.screenName;
                param.menuName = "MyInstruactables";
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
            else
            {
                svc.LoginSucceed += HandleLoginSucceed;
                _continueAction = LoginContinueAction.MyInstructables;
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private void Favorites_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "favorite");
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;
            var vm = DataContext as LandingViewModel;

            if (svc.isLogin())
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(vm.userProfile.screenName);
                //param.Add("Favorites");
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = vm.userProfile.screenName;
                param.menuName = "Favorites";
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
            else
            {
                svc.LoginSucceed += HandleLoginSucceed;
                _continueAction = LoginContinueAction.Favorites;
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private void Create_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "Ible_Create");
            GoogleAnalyticsTracker.SendEvent("Ible_Create", "status", "start");
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
                svc.LoginSucceed += HandleLoginSucceed;
                _continueAction = LoginContinueAction.Create;
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private async void Feedback_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "feedback");
            EmailRecipient sendTo = new EmailRecipient()
            {
                Address = "service@instructables.com"
            };

            //generate mail object
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Instructables App Feedback";
            mail.Body = "\n\n\n\nIf this is a bug report, please be as descriptive as possible. Let us know exactly what steps you took to product the bug. Thank you!\n";

            //add recipients to the mail object
            mail.To.Add(sendTo);
            //mail.Bcc.Add(sendTo);
            //mail.CC.Add(sendTo);

            //open the share contract with Mail only:
            await EmailManager.ShowComposeNewEmailAsync(mail); 
        }

        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "settings");
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SignUpPage));
        }

        private void AppBarButton_Search_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SearchResultsPage));
        }

        private void AppBarButton_Create_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("menu", "left_menu", "Ible_Create");
            GoogleAnalyticsTracker.SendEvent("Ible_Create", "status", "start");
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
                svc.LoginSucceed += HandleLoginSucceed;
                _continueAction = LoginContinueAction.Create;
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private DependencyObject GetListViewItem(DependencyObject obj)
        {
            DependencyObject rtnObj = null;
            while (obj != null)
            {
                if (obj is Grid)
                {
                    rtnObj = VisualTreeHelper.GetParent(obj);
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return rtnObj;
        }

        private void HomeSection_RightClick(object sender, RightTappedRoutedEventArgs e)
        {
            return;//Disable the long press function.
            //var obj = e.OriginalSource as DependencyObject;
            //Grid item = GetListViewItem(obj) as Grid;
            //if (item != null)
            //{
            //    try 
            //    { 
            //        Flyout.ShowAttachedFlyout(item);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(String.Format("{0}", ex.Message));
            //    }
            //}
        }

        private async void MenuFlyoutAddToFavoritesItem_Click(object sender, RoutedEventArgs e)
        {
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;

            if (svc.isLogin())
            {
                InstructableSummary instructable = (sender as MenuFlyoutItem).CommandParameter as InstructableSummary;
                await svc.ToggleFavorite(instructable.id);
            }
            else
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private void dtManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            
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
            e.Request.Data.SetWebLink(new Uri(INSTRUCTABLE_BASE_URL + instructableUrl));
            //e.Request.Data.SetUri(new Uri(INSTRUCTABLE_BASE_URL + vm.SelectedInstructable.urlString));
            //e.Request.Data.SetHtmlFormat("");
        }

        private void MenuFlyoutShareItem_Click(object sender, RoutedEventArgs e)
        {
            InstructableSummary instructable = (sender as MenuFlyoutItem).CommandParameter as InstructableSummary;
            instructableUrl = instructable.url;
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private async void HandleLoginSucceed(object sender, EventArgs e)
        {
            var vm = DataContext as LandingViewModel;
            await vm.UpdateUserProfile();
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;
            svc.LoginSucceed -= HandleLoginSucceed;

            if(_continueAction == LoginContinueAction.MyInstructables)
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(vm.userProfile.screenName);
                //param.Add("MyInstruactables");
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = vm.userProfile.screenName;
                param.menuName = "MyInstruactables";
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
            else if(_continueAction == LoginContinueAction.Favorites)
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(vm.userProfile.screenName);
                //param.Add("Favorites");
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = vm.userProfile.screenName;
                param.menuName = "Favorites";
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
            else if(_continueAction == LoginContinueAction.Create)
            {
                if (!SessionManager.ShowedCreateTutorial())
                    this.Frame.Navigate(typeof(CreateTutorial));
                else
                    this.Frame.Navigate(typeof(CreateInstructable));
            }
            else
            {

            }
        }
    }
}
