using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Instructables.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.Utils;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Instructables.DataModel;

namespace Instructables.ViewModels
{
    public class ContestsListViewModel : BindableBase
    {
        private InstructableContestList _allContests;
        public InstructableContestList AllContests
        {
            get
            {
                if (_allContests == null)
                    _allContests = new InstructableContestList();
                return _allContests;
            }
            set 
            {
                _allContests = value;
                OnPropertyChanged();
            }
        }

        private InstructableContestList _openContests;
        public InstructableContestList OpenContests
        {
            get
            {
                if (_openContests == null)
                    _openContests = new InstructableContestList();
                return _openContests;
            }
            set
            {
                _openContests = value;
                OnPropertyChanged();
            }
        }

        private InstructableContestList _closedContests;
        public InstructableContestList ClosedContests
        {
            get
            {
                if (_closedContests == null)
                    _closedContests = new InstructableContestList();
                return _closedContests;
            }
            set
            {
                _closedContests = value;
                OnPropertyChanged();
            }
        }

        public ContestsListViewModel()
        {

        }

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }

        public async Task LoadContestList(bool bForced = false)
        {
            if (bForced)
            {
                await LoadContestListInternal();
            }
            else
            {
                if (VisualState == "Offline")
                {
                    await LoadContestListInternal();
                }
            }           
        }

        private async Task LoadContestListInternal()
        {
            VisualState = "Normal";
            OpenContests.contests.Clear();
            ClosedContests.contests.Clear();
            var dataService = InstructablesDataService.DataServiceSingleton;
            AllContests = await dataService.GetContests(0, -1);

            if (AllContests.contests.Count == 0)
                VisualState = "Offline";

            foreach (var contest in AllContests.contests)
            {
                if (contest.state == "open")
                    OpenContests.contests.Add(contest);
                else
                    ClosedContests.contests.Add(contest);
            }
        }
    }
}
