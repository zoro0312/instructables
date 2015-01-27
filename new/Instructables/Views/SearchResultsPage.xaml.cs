using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.ViewModels;
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
using Windows.UI.Popups;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace Instructables.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : LayoutAwarePage
    {
        private static SearchFlyout _searchFlyout = null;
        private static LoginFlyout _loginFlyout = null;

        public SearchResultsPage()
        {
            this.InitializeComponent();

        }

        private string _queryText = String.Empty;

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
            if(_queryText == String.Empty)
                _queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.
            if (_queryText != null && _queryText != String.Empty)
            {
            //    string keyWord = _queryText.PadLeft(_queryText.Length + 1, '"');
                
              //  keyWord = keyWord.PadRight(keyWord.Length + 1, '"');
                GoogleAnalyticsTracker.SendEvent("search", "keyword", _queryText);
                queryText.Text = '"' + _queryText + '"';
                await PerformSearch();
            }
           // await PerformSearch();
        }

        private async Task PerformSearch()
        {

            var filterList = new List<Filter>();
            filterList.Add(new Filter("All", 0, true));

            ProgressRing.IsActive = true;
            loadingPanel.Visibility = Visibility.Visible;
            resultsPanel.Visibility = Visibility.Collapsed;
            networkErrorBorder.Visibility = Visibility.Collapsed;
            searchKeyWord.Visibility = Visibility.Collapsed;

            InstructableSearchSummaryCollection collection = new InstructableSearchSummaryCollection();
            collection.NetworkErrorEvent += (sender, args) => 
            { 
                networkErrorBorder.Visibility = Visibility.Visible; 
            };
            //collection.CollectionChanged += (sender, args) =>
            //    {
            //        if (collection.Count > 0)
            //        {
            //            ProgressRing.IsActive = false;
            //            loadingPanel.Visibility = Visibility.Collapsed;
            //        }
            //    };
            collection.SearchTerm = Uri.EscapeUriString(_queryText);
            await collection.LoadMoreItemsAsync(20);
            this.DefaultViewModel["Results"] = collection;
            ProgressRing.IsActive = false;
            loadingPanel.Visibility = Visibility.Collapsed;
            resultsPanel.Visibility = Visibility.Visible;
            searchKeyWord.Visibility = Visibility.Visible;

            if (collection.Count <= 0)
            {
                noResultsTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                noResultsTextBlock.Visibility = Visibility.Collapsed;
            }

            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + _queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        private sealed class Filter : Instructables.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }

        private void SearchItemClicked(object sender, ItemClickEventArgs e)
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

        async private void RetrySearch(object sender, RoutedEventArgs e)
        {
            networkErrorBorder.Visibility = Visibility.Collapsed;
            loadingPanel.Visibility = Visibility.Visible;
            await PerformSearch();
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

        private async void HandleSearch(String queryKeyWord)
        {
            _searchFlyout.searchHandle -= this.HandleSearch;
            if (queryKeyWord.Trim() == string.Empty)
            {
                MessageDialog confirmationDialog = new MessageDialog(
                    "We can't read your mind!",
                     "Empty Search");

                await confirmationDialog.ShowAsync();
            }
            else
            {
                _queryText = queryKeyWord;
                queryText.Text = '"' + _queryText + '"';
                await PerformSearch();
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

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            RetrySearch(this, null);
        }



    }
}
