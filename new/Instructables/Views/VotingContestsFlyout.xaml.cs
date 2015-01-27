using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.UI.Core;
using Instructables.DataServices;
using System.Diagnostics;
using Windows.UI.Popups;
//using Facebook.Client;
//using Facebook;
using Windows.Security.Authentication.Web;
using System.Threading.Tasks;
using Instructables.ViewModels;
using Instructables.DataModel;
using Instructables.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VotingContestsFlyout : SettingsFlyout
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
        private static LoginFlyout _loginFlyout = null;
        public VotingContestsFlyout()
        {
            this.InitializeComponent();
        }

        private int votedCount = 0;
        private string _fromCreating;

        protected async Task LoadState()
        {
                var dataService = InstructablesDataService.DataServiceSingleton;
                var vm = this.DataContext as InstructableDetailViewModel;
                if (vm.SelectedInstructable == null)
                    return;
                ContestsListView.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Visibility.Visible;
                if (_defferedVotingList.Count > 0)
                {
                    if (_defferedVotingList != null)
                    {
                        for (var i = 0; i < _defferedVotingList.Count; i++)
                        {
                            if (_defferedVotingList[i] != vm.SelectedInstructable.votableContests[i].votedFor)
                            {
                                await dataService.Vote(instructableId, vm.SelectedInstructable.votableContests[i].id, _defferedVotingList[i]);
                            }
                        }
                    }
                }
                if (vm != null)
                {
                    vm.LoadVotableContests();
                }
                instructableId = vm.SelectedInstructable.id;
                votingList = null;
                votingList = new List<bool>();
                votedCount = 0;
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
                LoadingPanel.Visibility = Visibility.Collapsed;
                _defferedVotingList.Clear();
                if (votedCount > 0)
                    Vote.IsEnabled = true;
                else
                    Vote.IsEnabled = false;
            }


        private async void AppBarButton_Vote_Click(object sender, RoutedEventArgs e)
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

                Hide();
                ShowLoginLayout();
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
                            if (result == null || result.isSucceeded != true)
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
                    GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "succeed");
                    Hide();
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_vote", "status", "error");
                }
            }
   
        }

        private void ShowLoginLayout()
        {
            if (_loginFlyout == null)
            {
                _loginFlyout = new LoginFlyout();
            }
            _loginFlyout.Show();
        }

        private void Voting_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.OriginalSource as CheckBox;
            if (cb.IsChecked == true)
            {
                if (votedCount >= MAX_SELECT_THRESHOLD)
                {
                    cb.IsChecked = !cb.IsChecked;
                    
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
                if (votedCount > 0)
                    Vote.IsEnabled = true;
                else
                    Vote.IsEnabled = false;
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
            //Voting_Error.Hide();
        }

        private async void OnVotingContestsLoaded(object sender, RoutedEventArgs e)
        {
            await LoadState();
        }

        private void OnVotingContestsUnloaded(object sender, RoutedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (vm.VotableContests != null)
                vm.VotableContests = null;
        }

    }
}
