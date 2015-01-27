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
using Windows.UI.Xaml.Controls;

namespace Instructables.ViewModels
{
    public class ContestEntriesViewModel : BindableBase
    {
        private bool _isLoading = false;
        private Contest _contest;

        public ContestEntriesViewModel()
        { }

        public ContestEntryCollection ContestEntries
        {
            get;
            set;
        }

        public Contest Contest
        {
            get { return _contest; }
            set
            {
                _contest = value;
            }
        }

        public Visibility ContestEntriesTitleVisible
        {
            get 
            {
                if (ContestEntries.Count > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public async Task loadContestEntries(Contest contest)
        {
            VisualState = "Normal";

            Contest = contest;

            if (ContestEntries == null)
            {
                ContestEntries = new ContestEntryCollection { contestId = contest.id };
                ContestEntries.NetworkErrorEvent += NetworkError;
                ContestEntries.StartIncrementalLoadEvent += StartIncrementalLoading;
                ContestEntries.StopIncrementalLoadEvent += StopIncrementalLoading;
            }

            ContestEntries.Clear();
            ContestEntries.contestId = contest.id;
            await ContestEntries.LoadMoreItemsAsync(20);

            IsLoading = false;
        }

        private void NetworkError(object sender, EventArgs e)
        {
            IsLoading = false;
            VisualState = "Offline";
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
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
                OnPropertyChanged("Contest");
                OnPropertyChanged("ContestEntries");
                OnPropertyChanged("ContestEntriesTitleVisible");
                OnPropertyChanged("IsLoading");
            }
        }

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }
    }
}
