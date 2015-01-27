using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Instructables.Common;
using System.ComponentModel;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.Utils;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Instructables.ViewModels
{
    public class ContestViewModel : BindableBase
    {
        public double ScreenHeight { get; set; }
        private bool _isLoading = false;
        private Contest _contest;
        //private string _ContestId = "";
        public string ContestId { get; set; }    

        public ContestViewModel()
        {
        }

        public async Task loadContest(string contestId, bool bForced = false)
        {
            if (Contest != null)
            {
                if (!bForced)
                {
                    if (Contest.id != contestId)
                    {
                        Contest = null;
                        ContestEntriesCollect = null;
                        await loadContestInteral(contestId);
                    }
                }
                else
                {
                    Contest = null;
                    ContestEntriesCollect = null;
                    IsLoading = true;
                    await loadContestInteral(contestId);
                }
                
            }
            else
            {
                await loadContestInteral(contestId);
            }
        }

        private async Task loadContestInteral(string contestId)
        { 
            VisualState = "Normal";
            ContestId = contestId;

            var dataService = InstructablesDataService.DataServiceSingleton;
            Contest = await dataService.GetContestInfo(contestId);
            if (Contest == null)
            {
                VisualState = "Offline";
            }

            // Update returned messages.
            _updateHtmlMessages();

            ScreenItems _counts = null;

            if (_viewState == "FullScreenPortrait")
            {
                if (ScreenHeight < 1400)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Portrait768"
                               select s).FirstOrDefault();
                }
                else if (ScreenHeight < 2000)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Portrait1080"
                               select s).FirstOrDefault();

                }
                else if (ScreenHeight >= 2000)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Portrait1440"
                               select s).FirstOrDefault();

                }
                else
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Portrait768"
                               select s).FirstOrDefault();
                }

            }
            else
            {
                if (_viewState != "Snapped")
                {
                    if (ScreenHeight <= 858)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "768"
                                   select s).FirstOrDefault();
                    }
                    else if (ScreenHeight > 858 && ScreenHeight < 1201)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "1080"
                                   select s).FirstOrDefault();
                    }
                    else if (ScreenHeight > 1201)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "1440"
                                   select s).FirstOrDefault();
                    }
                    else
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "768"
                                   select s).FirstOrDefault();
                    }
                }
                else
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Snapped"
                               select s).FirstOrDefault();
                }

            }


            if (_counts == null)
                _counts = new ScreenItems() { DualFeatureCount = 5 };

            CurrentScreenMetrics = _counts;

            //CurrentScreenMetrics = new ScreenItems() { DualFeatureCount = 9 };  // hard code

            //if (ContestEntries == null)
            //{
            //    ContestEntries = new ContestEntryCollection { contestId = contestId };
            //    ContestEntries.NetworkErrorEvent += NetworkError;
            //}
            //else
            //{
            //    if (ContestEntries.contestId != contestId)
            //        ContestEntries.Clear();
            //    ContestEntries.contestId = contestId;
            //}
            //await ContestEntries.LoadMoreItemsAsync(4);

            if (ContestEntriesCollect == null)
            {
                ContestEntriesCollect = new ObservableCollection<ContestEntry>();                
            }
            else
            {
                ContestEntriesCollect.Clear();
            }
            //_ContestId = contestId;

            //var dataService = InstructablesDataService.DataServiceSingleton;
            //_contest = await dataService.GetContestInfo(contestId);

            //// Update returned messages.
            //_updateHtmlMessages();
            ObservableCollection<ContestEntry> tempContestEntriesCollect = new ObservableCollection<ContestEntry>();
            var data = await dataService.GetContestEntries(contestId, 0, 12);
            if (data != null)
            {
                tempContestEntriesCollect = data.entries;
            }
            //tempContestEntriesCollect = (await dataService.GetContestEntries(contestId, 0, 12)).entries;
            //ContestEntriesCollect = (await dataService.GetContestEntries(contestId, 0, 12)).entries;
            int count = tempContestEntriesCollect.Count;
            if (count > CurrentScreenMetrics.DualFeatureCount)
            {
                count = CurrentScreenMetrics.DualFeatureCount;
            }
            for (int i = 0; i < count; ++i)
            {
                tempContestEntriesCollect[i].order = i;
                ContestEntriesCollect.Add(tempContestEntriesCollect[i]);
            }
            

            IsLoading = false;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("Contest");
                OnPropertyChanged("ContestEntries");
                OnPropertyChanged("PrizeVisible");
                OnPropertyChanged("ContestInfoVisible");
                OnPropertyChanged("ContestEntriesVisible");
                OnPropertyChanged("ContestEntriesCollect");                
            }
        }

        public Contest Contest
        {
            get { return _contest; }
            set
            {
                _contest = value;
            }
        }

        public ObservableCollection<ContestEntry> ContestEntriesCollect
        {
            get;
            set;
        }

        public ContestEntryCollection ContestEntries
        {
            get;
            set;
        }

        public Visibility PrizeVisible
        {
            get 
            {
                if (Contest != null && Contest.prizes.Count > 0)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility ContestInfoVisible
        {
            get
            {
                if (Contest != null)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility ContestEntriesVisible
        {
            get
            {
                if (ContestEntriesCollect != null && ContestEntriesCollect.Count > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }


        private void _updateHtmlMessages()
        {
            // Remove the unnecessary span.
            if (_contest != null)
            {
                foreach (var price in _contest.prizes)
                {
                    price.description = price.description.Replace("<br />", "");
                }
            }
        }

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }

        private string _viewState;

        internal void UpdateState(string visualState)
        {
            Debug.WriteLine(visualState);
            _viewState = visualState;
        }

        private void NetworkError(object sender, EventArgs e)
        {
            IsLoading = false;
            VisualState = "Offline";
        }

        public class ScreenItems
        {
            public string Name { get; set; }
            public int HeroItemCount { get; set; }
            public int EBookItemCount { get; set; }
            public int SingleFeatureCount { get; set; }
            public int DualFeatureCount { get; set; }
        }

        private List<ScreenItems> ScreenMetrics = new List<ScreenItems>()
            { 
                new ScreenItems(){Name = "Portrait768", HeroItemCount = 4, EBookItemCount = 8, DualFeatureCount = 9, SingleFeatureCount = 9},
                new ScreenItems(){Name = "Portrait1080", HeroItemCount = 7, EBookItemCount = 12, DualFeatureCount = 12, SingleFeatureCount = 9},
                new ScreenItems(){Name = "Portrait1440", HeroItemCount = 13, EBookItemCount = 16, DualFeatureCount = 12, SingleFeatureCount = 15},
                new ScreenItems(){Name = "768", HeroItemCount = 7, EBookItemCount = 6, DualFeatureCount = 9, SingleFeatureCount = 6},
                new ScreenItems(){Name = "1080", HeroItemCount = 9, EBookItemCount = 9, DualFeatureCount = 12, SingleFeatureCount = 9},
                new ScreenItems(){Name = "1440", HeroItemCount = 14, EBookItemCount = 12, DualFeatureCount = 12, SingleFeatureCount = 12},
                new ScreenItems(){Name = "Snapped", HeroItemCount = 6, EBookItemCount = 6, DualFeatureCount = 9, SingleFeatureCount = 6}
            };

        public static ScreenItems CurrentScreenMetrics = null;
    }
}
