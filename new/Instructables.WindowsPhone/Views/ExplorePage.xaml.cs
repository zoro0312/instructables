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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExplorePage : LayoutAwarePage
    {
        private bool _isSelectingCategory = false;
        private bool _isSelectingChannel = false;
        private string _groupName;

        public ExplorePage()
        {
            this.InitializeComponent();
        }

        private void ExploreItemsLoad(object sender, RoutedEventArgs e)
        {
            var pivot = (Pivot)sender;
            pivot.ItemsSource = groupedItemsViewSource.View.CollectionGroups;
        }

        // Invoked when a instructable itme is clicked.
        private void InstructableItemClicked(object sender, ItemClickEventArgs e)
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

        async protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _groupName = navigationParameter as string;
            var vm = DataContext as ExploreViewModel;
            pivot.Visibility = Visibility.Collapsed;
            pivotHead.Visibility = Visibility.Collapsed;
            BottomAppBar.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            vm.IsRefreshing = true;
            if (vm != null)
            {
                //vm.DismissPopup += HandleDismissPopup;
                //vm.DismissCollectionPopup += HandleDismissCollectionPopup;

                vm.VisualState = "Normal";
                if (vm.IsValidGroup(_groupName))
                {
                    //if (vm.IsEmptyGroup(groupName))
                    await vm.InitializeGroup(_groupName);

                    vm.SetCurrent(_groupName);
                }
                else
                    vm.VisualState = "Offline";
            }

            if (_groupName == "All")
            {
                ChannelButton.Visibility = Visibility.Collapsed;
            }
            vm.IsRefreshing = false;
            pivot.Visibility = Visibility.Visible;
            pivotHead.Visibility = Visibility.Visible;
            BottomAppBar.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var vm = this.DataContext as ExploreViewModel;

            /*if (vm != null)
            {
                vm.DismissPopup -= HandleDismissPopup;
                vm.DismissCollectionPopup -= HandleDismissCollectionPopup;
            }*/

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (vm != null)
                {
                    //if(vm.CurrentGroup.GroupName == "Collections")
                    vm.SetCurrent("All");
                    vm.SelectedChannelIndex = 0;
                    //vm = null;
                }
            }
        }

        private void _showChannelMenu()
        {
            // Hide app buttons.
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            // Show channel list.
            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;

            _isSelectingChannel = true;
        }

        private void _hideChannelMenu()
        {
            // Show app buttons.
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            // Hide channel list.
            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            _isSelectingChannel = false;
        }

        private void _showCollectionCategoryMenu()
        {
            // Clear the selection
            CollectionCategoryMenuList.SelectedIndex = -1;
            // Hide app buttons.
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            // Show category menu.
            CollectionCategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;

            _isSelectingCategory = true;
        }

        private void _hideCollectionCategoryMenu()
        {
            // Hide category menu.
            CollectionCategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            _isSelectingCategory = false;
        }

        private void HandleDismissPopup(object sender, EventArgs e)
        {
            _hideChannelMenu();
        }

        private void HandleDismissCollectionPopup(object sender, EventArgs e)
        {
            _hideCollectionCategoryMenu();
            _showChannelMenu();
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        { 
            var vm = DataContext as ExploreViewModel;
            if (CollectionCategoryMenu.Visibility == Windows.UI.Xaml.Visibility.Visible ||
                ChannelMenu.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                e.Handled = true;
                _hideCollectionCategoryMenu();
                _hideChannelMenu();
            }
            else 
            {
                base.OnHardwareBackPressed(sender, e);
                pivot.Visibility = Visibility.Collapsed;
                pivotHead.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// This is the click handler for the 'Channel' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Channel_Clicked(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ExploreViewModel;
            if (vm.CurrentGroup.GroupName == "Collections"
                && !_isSelectingCategory)
            {
                 // Show collection category menu.
                _showCollectionCategoryMenu();
            } else {
                // Show channel menu.
                _showChannelMenu();
            }
        }

        /// <summary>
        /// This is the click handler for the 'Find' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Find_Clicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SearchResultsPage));
        }

        /// <summary>
        /// This is the click handler for the 'Find' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Refresh_Clicked(object sender, RoutedEventArgs e)
        {
            var landingVM = ViewModelLocator.Instance.LandingVM;
            //pivot.Visibility = Visibility.Collapsed;
            var vm = this.DataContext as ExploreViewModel;
            vm.IsRefreshing = true;
            landingVM.ExecuteRefreshCommand();
            
            if (vm != null)
            {
                vm.RefreshGroups();
                vm.VisualState = "Normal";
                if (vm.IsValidGroup(_groupName))
                {
                    //if (vm.IsEmptyGroup(groupName))
                    await vm.InitializeGroup(_groupName);

                    vm.SetCurrent(_groupName);
                    await vm.RefreshCurrentCroup();
                }
                else
                    vm.VisualState = "Offline";                
            }
            //pivot.Visibility = Visibility.Visible;
            vm.IsRefreshing = false;
        }

        private async void onChannelChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Selection Changed");
            if (_isSelectingChannel == false)
                return;
            _hideChannelMenu();
            var vm = this.DataContext as ExploreViewModel;
            Debug.WriteLine(String.Format("Select channel: {0}", vm.SelectedChannelIndex));
            if (vm != null)
            {
                pivot.Visibility = Visibility.Collapsed;
                pivotHead.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                await vm.GetSelectedChannelData(vm.SelectedChannelIndex);
                pivot.Visibility = Visibility.Visible;
                pivotHead.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void onCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelectingCategory == false)
                return;
            _hideCollectionCategoryMenu();
            _showChannelMenu();
        }
    }
}
