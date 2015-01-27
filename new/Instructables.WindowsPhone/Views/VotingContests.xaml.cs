using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VotingContests : Instructables.Common.LayoutAwarePage
    {
        private string instructableId = "";
        private List<bool> votingList = null;
        public static List<bool> _defferedVotingList = new List<bool>();
        private const int MAX_SELECT_THRESHOLD = 3;
        private const string VOTING_ERROR = "Your voting is failed due to a network error.";
        private const string VOTING_ERROR_TITLE = "Voting Error";
        private const string CHECKING_ERROR = "You can on only enter up to 3 contests.";
        private const string CHECKING_ERROR_TITLE = "Too many entries";
        private List<string> enterContestList = null;
        public VotingContests()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                if (vm.VotableContests != null)
                    vm.VotableContests = null;
            }
        }


        private int votedCount = 0;
        private string _fromCreating;
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
            _fromCreating = navigationParameter as string;

            if (_fromCreating != "creating")  //Vote contests from selected instructable.
            {
                var dataService = InstructablesDataService.DataServiceSingleton;
                var vm = this.DataContext as InstructableDetailViewModel;
                ContestsListView.Visibility = Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;                
                if (vm != null)
                {
                    vm.LoadVotableContests();
                }
                instructableId = vm.SelectedInstructable.id;
                votingList = null;
                votingList = new List<bool>();
                foreach (var vote in vm.SelectedInstructable.votableContests)
                {
                    if (vote.votedFor == true)
                    {
                        votedCount++;
                    }
                    bool isVote = vote.votedFor;
                    votingList.Add(isVote);
                }
                ContestsListView.Visibility = Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Collapsed;
                
                Vote.Label = "vote";
            }
            else //Enter in contests from creating
            {
                var _dataContext = ViewModelLocator.Instance.LandingVM;
                ContestsListView.ItemsSource = _dataContext.Contests;

                enterContestList = null;
                enterContestList = new List<string>();
                LoadingPanel.Visibility = Visibility.Collapsed;
                Vote.Label = "done";
            }
        }

        private async void HandleLoginSucceed(object sender, EventArgs e)
        {
            var vm = this.DataContext as InstructableDetailViewModel;
            var dataService = InstructablesDataService.DataServiceSingleton;
            dataService.LoginSucceed -= HandleLoginSucceed;

            bool postResult = true;
            Vote.IsEnabled = false;
            if (_defferedVotingList.Count > 0)
            {
                if (_defferedVotingList != null)
                {
                    for (var i = 0; i < _defferedVotingList.Count; i++)
                    {
                        if (_defferedVotingList[i] != vm.SelectedInstructable.votableContests[i].votedFor)
                        {
                            var result = await dataService.Vote(instructableId, vm.SelectedInstructable.votableContests[i].id, _defferedVotingList[i]);
                            if (result.isSucceeded != true)
                            {
                                postResult = false;
                            }
                            else
                            {
                                vm.SelectedInstructable.votableContests[i].votedFor = _defferedVotingList[i];
                            }
                        }
                    }
                }
            }
            _defferedVotingList.Clear();
            Vote.IsEnabled = true;
            GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "succeed");
            if (postResult == true)
            {
                GoBack(this, new RoutedEventArgs());
            }
            else
            {
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = VOTING_ERROR_TITLE;
                dialog.Content = VOTING_ERROR;
                GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "error");
                dialog.Commands.Add(new UICommand("OK", (command) =>
                {

                }));
                await dialog.ShowAsync();

            }
        }

        private async void AppBarButton_Vote_Click(object sender, RoutedEventArgs e)
        {
            if (_fromCreating != "creating")  //Vote contests from selected instructable.
            {
                var vm = this.DataContext as InstructableDetailViewModel;
                var dataService = InstructablesDataService.DataServiceSingleton;
                bool ifLogin = await dataService.EnsureLogin();
                if (ifLogin != true)
                {
                    foreach (var vote in votingList)
                    {
                        var newVote = vote;
                        _defferedVotingList.Add(newVote);
                    }

                    dataService.LoginSucceed += HandleLoginSucceed;
                    this.Frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    bool postResult = true;
                    Vote.IsEnabled = false;
                    if (votingList != null)
                    {
                        for (var i = 0; i < votingList.Count; i++)
                        {
                            if (votingList[i] != vm.SelectedInstructable.votableContests[i].votedFor)
                            {
                                var result = await dataService.Vote(instructableId, vm.SelectedInstructable.votableContests[i].id, votingList[i]);
                                if (result.isSucceeded != true)
                                {
                                    postResult = false;
                                }
                                else
                                {
                                    vm.SelectedInstructable.votableContests[i].votedFor = votingList[i];
                                }
                            }
                        }
                    }
                    Vote.IsEnabled = true;
                    if (postResult == true)
                    {
                        GoBack(this, e);
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("", "");
                        dialog.Title = VOTING_ERROR_TITLE;
                        dialog.Content = VOTING_ERROR;

                        dialog.Commands.Add(new UICommand("OK", (command) =>
                        {

                        }));
                        await dialog.ShowAsync();
                    }
                }
            }
            else  //Enter in contests from creating
            {
                var vm = ViewModelLocator.Instance.CreateVM;
                vm.Instructable.contestID = enterContestList;
                GoBack(this, e);
            }
        }

        private async void Voting_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.OriginalSource as CheckBox;
            if (cb.IsChecked == true)
            {
                if (votedCount >= MAX_SELECT_THRESHOLD)
                {
                    cb.IsChecked = false;
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = CHECKING_ERROR_TITLE;
                    dialog.Content = CHECKING_ERROR;

                    dialog.Commands.Add(new UICommand("OK", (command) =>
                    {

                    }));

                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    votedCount++;
                }

            }
            else
            {
                votedCount--;
            }

            var item = cb as DependencyObject;
            ContentPresenter itemTemplate = null;
            do
            {
                var item2 = VisualTreeHelper.GetParent(item);
                item = item2;
                itemTemplate = item as ContentPresenter;
                if (itemTemplate != null)
                {
                    break;
                }
            } while (item != null);

            if (_fromCreating != "creating")  //Vote contests from selected instructable.
            {
                if (itemTemplate != null)
                {
                    var votableContest = (VotableContest)itemTemplate.Content;
                    var vm = this.DataContext as InstructableDetailViewModel;
                    for (var i = 0; i < vm.SelectedInstructable.votableContests.Count; i++)
                    {
                        if (votableContest.id == vm.SelectedInstructable.votableContests[i].id)
                        {
                            votingList[i] = (bool)cb.IsChecked;
                            break;
                        }
                    }
                }
            }
            else  //Enter in contests from creating
            {
                if (itemTemplate != null)
                {
                    var contest = (Contest)itemTemplate.Content;
                    if (cb.IsChecked == true)
                        enterContestList.Add(contest.id);
                    else
                        enterContestList.Remove(contest.id);
                }

            }            
        }

        private void voting_error_ok_Click(object sender, RoutedEventArgs e)
        {
            Voting_Error.Hide();
        }
    }
}
