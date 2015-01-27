using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using System.Collections.ObjectModel;

namespace Instructables.DataModel
{
    public class VotableContest : BindableBase
    {
        public string id { get; set; }
        private string _title;
        public string title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _instructableTitle;
        public string instructableTitle
        {
            get 
            {
                return _instructableTitle;
            }

            set 
            {
                _instructableTitle = value;
                OnPropertyChanged();
            }
        }
        public string urlString { get; set; }
        private string _thumbUrl;
        public string thumbUrl 
        {
            get
            {
                return _thumbUrl;
            }

            set
            {
                _thumbUrl = value;
                OnPropertyChanged();
            }
        
        }
        public string locale { get; set; }
        private bool _votedFor;
        public bool votedFor
        {
            get
            {
                return _votedFor;
            }
            set
            {
                _votedFor = value;
                OnPropertyChanged();
            }
        }
    }

    public class VotableContestGroup : BindableBase
    {
        private ObservableCollection<VotableContest> _votableContestsGroup = new ObservableCollection<VotableContest>();
        public ObservableCollection<VotableContest> VotableContestsGroup
        {
            get { return this._votableContestsGroup; }
            set
            {
                _votableContestsGroup = value;
                OnPropertyChanged();
            }
        }

        //private string _stepTitle = "";
        //public string ContestName { get; set; }
        /*private string _contestTitle;
        public string ContestTitle
        {
            get 
            {
                return _contestTitle;
            }
            set 
            {
                _contestTitle = value;
                OnPropertyChanged();
            }
        }*/
    }
}
