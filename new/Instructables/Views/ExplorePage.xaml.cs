using System;
using System.Collections.Generic;
using System.Diagnostics;
using Callisto.Controls;
using Instructables.Controls;
using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Instructables.Common;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace Instructables.Views
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class ExplorePage : Instructables.Common.LayoutAwarePage
    {
        private static LoginFlyout _loginFlyout = null;

        public ExplorePage()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
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
            var vm = this.DataContext as ExploreViewModel;
            var groupName = navigationParameter as string;
            vm.GroupName = groupName;
            var landingVM = ViewModelLocator.Instance.LandingVM;
            var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
            if (groupName == "Draft")
                ProfileItemGridView.ItemsSource = userProfileVM.DraftInstructables;
            else if (groupName == "Published")
            {
                vm.GroupName = "Instructables";
                ProfileItemGridView.ItemsSource = userProfileVM.PublishedInstrutables;
            }
            else if (groupName == "Followed")
            {
                vm.GroupName = "Following";
                ProfileItemGridView.ItemsSource = landingVM.Followings;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = this.DataContext as ExploreViewModel;
            if (vm != null)
            {
                vm.IsLoading = true;
                var groupName = (string)e.Parameter;
                if (groupName == "Draft" || groupName == "Published" || groupName == "Followed")
                {
                    vm.FromUserProfile = true;
                    var landingVM = ViewModelLocator.Instance.LandingVM;
                    var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                    if (groupName == "Draft")
                        ProfileItemGridView.ItemsSource = userProfileVM.DraftInstructables;
                    else if (groupName == "Published")
                    {
                        vm.GroupName = "Instructables";

                        if (userProfileVM.instructablesCount == 0 || userProfileVM.userName != landingVM.userProfile.screenName)
                        {
                            await userProfileVM.Initialize(userProfileVM.userName);
                            if (userProfileVM.instructablesCount == 0)
                                NoPubishedView.Visibility = Visibility.Visible;
                            else
                                NoPubishedView.Visibility = Visibility.Collapsed;
                        }
                        ProfileItemGridView.ItemsSource = userProfileVM.PublishedInstrutables;
                    }
                    else if (groupName == "Followed")                    
                        ProfileItemGridView.ItemsSource = landingVM.Followings;                    
                }
                else
                {
                    vm.FromUserProfile = false;
                    vm.VisualState = "Normal";
                    vm.DismissPopup += HandleDismissPopup;
                    if (e.NavigationMode != NavigationMode.Back)
                        vm.SetCategory((string)e.Parameter);
                    if ((string)e.Parameter == "Featured")
                    {
                        this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xff, 0xae, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Colors.White));//(Color.FromArgb(0x66, 0xff, 0xae, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedDisabledBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xff, 0xae, 0x00)));
                        this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xff, 0xae, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xff, 0xae, 0x00)));
                    }
                    else if ((string)e.Parameter == "Recent")
                    {
                        this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xfc, 0x65, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Colors.White)); //(Color.FromArgb(0x66, 0xfc, 0x65, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedDisabledBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xfc, 0x65, 0x00)));
                        this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xfc, 0x65, 0x00)));
                        this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x66, 0xfc, 0x65, 0x00)));
                    }
                    else
                    {
                        this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x33, 0xc2, 0x00, 0x89)));
                        this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Colors.White));  //(Color.FromArgb(0x33, 0xc2, 0x00, 0x89)));
                        this.Resources.Add("ListBoxItemSelectedDisabledBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x33, 0xc2, 0x00, 0x89)));
                        this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x33, 0xc2, 0x00, 0x89)));
                        this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x33, 0xc2, 0x00, 0x89)));
                    }
                }

                vm.IsLoading = false;
            }

        }

        async void Refresh()
        {
            var vm = this.DataContext as ExploreViewModel;
            vm.Refresh();

            if(vm.GroupName == "Instructables")
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                userProfileVM.PublishedInstrutables.Clear();
                await userProfileVM.PublishedInstrutables.LoadMoreItemsAsync(15);
            }
            else if(vm.GroupName == "Following")
            {
                var landingVM = ViewModelLocator.Instance.LandingVM;
                landingVM.Followings.Clear();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (channelDropDown != null)
                channelDropDown.IsOpen = false;
            base.OnNavigatedFrom(e);
            var vm = this.DataContext as ExploreViewModel;
            if (vm != null)
            {
                vm.DismissPopup -= HandleDismissPopup;
            }
        }

        private void HandleDismissPopup(object sender, EventArgs e)
        {
            CategoryList.IsEnabled = true;
            /*if (categoryDropDown != null && categoryDropDown.IsOpen)
                categoryDropDown.IsOpen = false;*/
            /*if (CategoryList.Visibility != Visibility.Collapsed)
                CategoryList.Visibility = Visibility.Collapsed;*/
            if (channelDropDown != null && channelDropDown.IsOpen)
                channelDropDown.IsOpen = false;
            /*if (sortFlyout != null && sortFlyout.IsOpen)
                sortFlyout.IsOpen = false;
            if (typeFlyout != null && typeFlyout.IsOpen)
                typeFlyout.IsOpen = false;*/
            if(ItemGridView.Items.Count > 0)
                ItemGridView.ScrollIntoView(ItemGridView.Items[0]);
        }


        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
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

        private Callisto.Controls.Flyout categoryDropDown;

        private void CategoryDropDownTapped(object sender, TappedRoutedEventArgs e)
        {
            if (categoryDropDown == null)
            {
                // create the Flyout
                categoryDropDown = new Callisto.Controls.Flyout();
                var selector = new CategorySelector();
                selector.DataContext = this.DataContext;
                categoryDropDown.Content = selector;
                // set the placement
                categoryDropDown.Placement = PlacementMode.Bottom;
                categoryDropDown.BorderThickness = new Thickness(0);
                categoryDropDown.VerticalOffset = -20.0;
                categoryDropDown.HorizontalOffset = 50.0;
                categoryDropDown.PlacementTarget = sender as UIElement;
            }
            // open the flyout
            categoryDropDown.IsOpen = true;
        }

        private Callisto.Controls.Flyout channelDropDown;

        private void ChannelDropDownTapped(object sender, TappedRoutedEventArgs e)
        {
            if (channelDropDown == null)
            {
                // create the Flyout
                channelDropDown = new Callisto.Controls.Flyout();
                var selector = new ChannelSelector();
                selector.DataContext = this.DataContext;
                channelDropDown.Content = selector;
                // set the placement
                channelDropDown.Placement = PlacementMode.Bottom;
                channelDropDown.BorderThickness = new Thickness(0);
                channelDropDown.VerticalOffset = -50.0;
                channelDropDown.HorizontalOffset = 30.0;
                channelDropDown.PlacementTarget = sender as UIElement;
            }
            // open the flyout
            channelDropDown.IsOpen = true;
        }


        private void LowerAppBar_OnClosed(object sender, object e)
        {
            /*Debug.WriteLine("Lower App Bar Closed");
            var vm = this.DataContext as MainViewModel;
            if (vm != null)
            {
                vm.CheckAppBarState();
            }*/
        }

        private Callisto.Controls.Flyout typeFlyout;

        private void SelectTypes_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (typeFlyout == null)
            {
                // create the Flyout
                typeFlyout = new Callisto.Controls.Flyout();
                var selector = new TypeSelector();
                selector.DataContext = this.DataContext;
                typeFlyout.Content = selector;
                // set the placement
                typeFlyout.Placement = PlacementMode.Top;
                typeFlyout.BorderThickness = new Thickness(0);
                //typeFlyout.VerticalOffset = -50.0;
                //typeFlyout.HorizontalOffset = 30.0;
                typeFlyout.PlacementTarget = sender as UIElement;
            }
            // open the flyout
            typeFlyout.IsOpen = true;
        }

        private Callisto.Controls.Flyout sortFlyout;

        private void SortData_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sortFlyout == null)
            {
                // create the Flyout
                sortFlyout = new Callisto.Controls.Flyout();
                var selector = new SortSelector();
                sortFlyout.DataContext = this.DataContext;
                sortFlyout.Content = selector;
                // set the placement
                sortFlyout.Placement = PlacementMode.Top;
                sortFlyout.BorderThickness = new Thickness(0);
                //typeFlyout.VerticalOffset = -50.0;
                //typeFlyout.HorizontalOffset = 30.0;
                sortFlyout.PlacementTarget = sender as UIElement;
            }
            // open the flyout
            sortFlyout.IsOpen = true;
        }

        private void TopNavButtonClicked(object sender, RoutedEventArgs e)
        {
            if (UpperAppBar != null && UpperAppBar.IsOpen)
                UpperAppBar.IsOpen = false;
            if (LowerAppBar != null && LowerAppBar.IsOpen)
                LowerAppBar.IsOpen = false;
        }

        private void OnCategoryListShow(object sender, RoutedEventArgs e)
        {
            /*(if (categoryDropDown == null)
            {
                // create the Flyout
                categoryDropDown = new Callisto.Controls.Flyout();
                var selector = new CategorySelector();
                selector.DataContext = this.DataContext;
                categoryDropDown.Content = selector;
                // set the placement
                categoryDropDown.Placement = PlacementMode.Bottom;
                categoryDropDown.BorderThickness = new Thickness(0);
                categoryDropDown.VerticalOffset = -25.0;
                categoryDropDown.HorizontalOffset = 0.0;
                categoryDropDown.PlacementTarget = sender as UIElement;
            }
            // open the flyout
            categoryDropDown.IsOpen = true;*/
            if(CategoryList.Visibility == Visibility.Collapsed)
            {
                SetCategoryListOn(true);
                var vm = this.DataContext as ExploreViewModel;
                if(vm.SelectedCategory != null && vm.SelectedCategory.CategoryName!="All" )
                {
                    if (channelDropDown!=null)
                    {
                        channelDropDown.IsOpen = true;
                    }
                }
            }
            else
            {
                SetCategoryListOn(false);
            }
        }

        private void SetCategoryListOn(bool visibility)
        {
            if(visibility == true)
            {
                CategoryList.Visibility = Visibility.Visible;
                DropUpGlyph.Visibility = Visibility.Visible;
                DropDownGlyph.Visibility = Visibility.Collapsed;
            }
            else
            {
                CategoryList.Visibility = Visibility.Collapsed;
                DropUpGlyph.Visibility = Visibility.Collapsed;
                DropDownGlyph.Visibility = Visibility.Visible;
            }
        }

        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        Category BackupCategory = null;

        private void OnCatagoryTapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedItem = CategoryList.SelectedItem as Category;
            var vm = this.DataContext as ExploreViewModel;
            //if (selectedItem.CategoryName != "All")
            {
                BackupCategory = vm.SelectedCategory;
                vm.SelectedCategory = selectedItem;
            }
            //else
            //{
            //    vm.SelectedCategory = null;
            //    return;
            //}

            if (channelDropDown == null)
            {
                // create the Flyout
                channelDropDown = new Callisto.Controls.Flyout();
                var selector = new ChannelSelector();
                selector.DataContext = this.DataContext;
                channelDropDown.Content = selector;
                channelDropDown.Closed += ChannelFlyoutClosing;
                // set the placement
                channelDropDown.Placement = PlacementMode.Right;
                channelDropDown.BorderThickness = new Thickness(0);
                SolidColorBrush foreBrush = new SolidColorBrush();
                Color foreColor = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF);
                foreBrush.Color = foreColor;
                channelDropDown.BorderBrush = foreBrush;
                channelDropDown.VerticalOffset = 210.0;
                channelDropDown.HorizontalOffset = -10.0;
                channelDropDown.PlacementTarget = sender as UIElement;
                //channelDropDown.Height = 800;
            }
            // open the flyout
            channelDropDown.IsOpen = true;
            CategoryList.IsEnabled = false;
        }

        private void RestoreBackupChannel()
        {
            var vm = this.DataContext as ExploreViewModel;
            if(BackupCategory!=null)
            {
                vm.SelectedCategory = BackupCategory;
                BackupCategory = null;
            }
        }

        private ScrollViewer _ItemGridViewScrollViewer = null; 
        private const float LIST_DISABLE_OFFSET = 1.05f;
        private void ChannelFlyoutClosing(object sender, object e)
        {
            CategoryList.IsEnabled = true;
            var vm = this.DataContext as ExploreViewModel;
            CategoryList.SelectedItem = vm.CurrentCategory;
            var selectedItem = CategoryList.SelectedItem as Category;            
            if (selectedItem.CategoryName != "All")
            {
                BackupCategory = vm.SelectedCategory;
                vm.SelectedCategory = selectedItem;
            }
            else
            {
                vm.SelectedCategory = null;
                return;
            }
        }

        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                child = v as T;

                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        private void onItemGridViewLoaded(object sender, RoutedEventArgs e)
        {
            _ItemGridViewScrollViewer = GetVisualChild<ScrollViewer>(ItemGridView);

            if (_ItemGridViewScrollViewer != null)
            {
                _ItemGridViewScrollViewer.ViewChanged += ItemGridViewScrollViewer_ViewChanged;
            }
            
        }

        private void ItemGridViewScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Close filters when the list of recipes is over the filters.
            if (_ItemGridViewScrollViewer.HorizontalOffset > /*(ItemGridView.Padding.Left - (CategoryList.Margin.Left + CategoryList.ActualWidth))*/LIST_DISABLE_OFFSET)
            {
                SetCategoryListOn(false);
            }
            else
            {
                SetCategoryListOn(true);
            }

        }

        private void ProfileItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Instructable instructable = e.ClickedItem as Instructable;
            if (instructable != null)
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
        /*private void onCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = CategoryButton.SelectedItem as Category;
            var vm = this.DataContext as ExploreViewModel;
            vm.SelectedCategory = selectedItem;
            if (channelDropDown == null)
            {
                // create the Flyout
                channelDropDown = new Callisto.Controls.Flyout();
                var selector = new ChannelSelector();
                selector.DataContext = this.DataContext;
                channelDropDown.Content = selector;
                // set the placement
                channelDropDown.Placement = PlacementMode.Right;
                channelDropDown.BorderThickness = new Thickness(0);
                channelDropDown.VerticalOffset = 200.0;
                channelDropDown.HorizontalOffset = 0.0;
                channelDropDown.PlacementTarget = sender as UIElement;
                //channelDropDown.Height = 800;
            }
            // open the flyout
            channelDropDown.IsOpen = true;
        }*/

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
            BottomAppBar.IsOpen = false;
            GoHome(sender, e);
        }

        private void TopContests_Click(object sender, RoutedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
            this.Frame.Navigate(typeof(ContestsPage));
        }

        private void TopMyInstructables_Click(object sender, RoutedEventArgs e)
        {
            this.TopAppBar.IsOpen = false;
            this.BottomAppBar.IsOpen = false;
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
            BottomAppBar.IsOpen = false;
            var vm = ViewModelLocator.Instance.LandingVM;
            if (vm.IsLogin)
                this.Frame.Navigate(typeof(UserProfilePage), vm.userProfile.screenName);
            else
                ShowLoginLayout(this, null);
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
