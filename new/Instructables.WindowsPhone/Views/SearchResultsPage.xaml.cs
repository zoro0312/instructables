using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Instructables.DataModel;
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
using Windows.System;
using Windows.Phone.UI.Input;
using Instructables.Common;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace Instructables.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : Instructables.Common.LayoutAwarePage
    {

        public SearchResultsPage()
        {
            this.InitializeComponent();
        }

        private static string _queryText = String.Empty;
        //private static Array<string> = new Array<string>(){"\""};
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
            if (_queryText == String.Empty)
                _queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            if (_queryText!=null && _queryText != String.Empty)
            {
                SearchKeyWord.Text = _queryText;
                await PerformSearch(SearchKeyWord.Text);
            }
            //await PerformSearch();
        }

        private string PreErrorHandel(string keyWord)
        {
            char[] errString = {'\"'};
            char[] newString = { '\'' };
            if (keyWord == new String(errString, 0, 1))
            {
                return new String(newString, 0, 1);
            }
            else
                return keyWord;
        }

        private async Task PerformSearch(string keyWord)
        {
            if (keyWord.Trim() == string.Empty)
            {
                search_error.ShowAt(this);
                return;
            }
            GoogleAnalyticsTracker.SendEvent("search", "keyword", keyWord);
            //var filterList = new List<Filter>();
            //filterList.Add(new Filter("All", 0, true));
            //ProgressRing.IsActive = true;
            keyWord = PreErrorHandel(keyWord);//Since searching '\"' will cause a run-time exception. So correct it to '\''
            loadingPanel.Visibility = Visibility.Visible;
            SearchKeyWord.Visibility = Visibility.Collapsed;
            resultsPanel.Visibility = Visibility.Collapsed;
            networkErrorBorder.Visibility = Visibility.Collapsed;
            SearchBottomBar.IsEnabled = false;
            InstructableSearchSummaryCollection collection = new InstructableSearchSummaryCollection();
            collection.StartIncrementalLoadEvent += StartIncrementalLoading;
            collection.StopIncrementalLoadEvent += StopIncrementalLoading;
            collection.NetworkErrorEvent += (sender, args) => 
            { 
                networkErrorBorder.Visibility = Visibility.Visible; 
            };
            _queryText = keyWord;
            collection.SearchTerm = Uri.EscapeUriString(keyWord);
            await collection.LoadMoreItemsAsync(4);
            this.DefaultViewModel["Results"] = collection;
            //ProgressRing.IsActive = false;
            loadingPanel.Visibility = Visibility.Collapsed;
            SearchKeyWord.Visibility = Visibility.Visible;
            resultsPanel.Visibility = Visibility.Visible;
            SearchBottomBar.IsEnabled = true;
            if(collection.Count <= 0)
            {
                noResultsTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                noResultsTextBlock.Visibility = Visibility.Collapsed;
            }
            //this.DefaultViewModel["QueryText"] = '\u201c' + keyWord + '\u201d';
            
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            loadingPanel.Visibility = Visibility.Visible;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            loadingPanel.Visibility = Visibility.Collapsed;
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

        private async void RetrySearch(object sender, RoutedEventArgs e)
        {
            try
            {
                //Dummy.Focus(FocusState.Pointer);
                await PerformSearch(SearchKeyWord.Text);
                //SearchKeyWord.Text = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return;
            }
        }

        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            _queryText = String.Empty;
            int lastFrame = this.Frame.BackStack.Count - 1;
            if (lastFrame >= 0 && this.Frame.BackStack[lastFrame].SourcePageType == typeof(WalkThroughs))
            {
                e.Handled = true;
                this.Frame.Navigate(typeof(LandingPage), "init");
            }
            else
            {
                base.OnHardwareBackPressed(sender, e);
            }
        }

        private async void Search_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                try
                {
                    //Dummy.Focus(FocusState.Pointer);
                    await PerformSearch(SearchKeyWord.Text);
                    //SearchKeyWord.Text = "";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
            }
        }

        private void AppBarButton_Home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LandingPage));
            _queryText = string.Empty;
        }

        private void search_error_ok_Click(object sender, RoutedEventArgs e)
        {
            search_error.Hide();
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        /*void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }*/

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        /*void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }*/

        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        /*private sealed class Filter : Instructables.Common.BindableBase
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
        }*/
    }
}
