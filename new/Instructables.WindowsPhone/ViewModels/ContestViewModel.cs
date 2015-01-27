using System;
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

        private bool _isLoading = false;
        private Contest _contest;

        public void ClearData()
        {
            _contest = null;
            ContestEntries.Clear();
            ContestEntries = null;
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private int _entriesCount = 1;
        public int EntriesCount
        {
            get 
            { 
                return this._entriesCount; 
            }

            private set
            {
                this._entriesCount = value;
                OnPropertyChanged();
            }
        }

        public ContestViewModel()
        {
        }

        public async Task loadContest(string contestId)
        {
            VisualState = "Normal";
            IsLoading = true;
            if (ContestEntries == null)
            {
                ContestEntries = new ContestEntryCollection { contestId = contestId };
                ContestEntries.StartIncrementalLoadEvent += StartIncrementalLoading;
                ContestEntries.StopIncrementalLoadEvent += StopIncrementalLoading;
                ContestEntries.NetworkErrorEvent += NetworkError;
                ContestEntries.LoadMoreEvent += ContestLoadMoreHandler;
            }
            else
            {
                ContestEntries.contestId = contestId;
            }
            await ContestEntries.LoadMoreItemsAsync(4);

            var dataService = InstructablesDataService.DataServiceSingleton;
            Contest = await dataService.GetContestInfo(contestId);

            // Update returned messages.
            _updateHtmlMessages();

            IsLoading = false;
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            if(VisualState != "Offline")
                IsLoading = true;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = false;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged("Contest");
                OnPropertyChanged("ContestEntries");
            }
        }

        public Contest Contest
        {
            get 
            { 
                return _contest; 
            }

            set
            {
                _contest = value;
                OnPropertyChanged();
            }
        }

        public ContestEntryCollection ContestEntries
        {
            get;
            set;
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

        private void NetworkError(object sender, EventArgs e)
        {
            IsLoading = false;
            VisualState = "Offline";
        }

        private void ContestLoadMoreHandler(object sender, EventArgs e)
        {
            ContestEntryCollection entries = sender as ContestEntryCollection;
            EntriesCount = entries.Count;
        }
    }
}
