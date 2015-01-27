using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Instructables.DataModel
{
    public class DataGroup
    {
        public enum LayoutType
        {
            MainFeature,
            SingleFeature,
            DualFeature,
            EBook
        };

        public int GroupOrdinal { get; set; }
        public string GroupName { get; set; }
        public LayoutType Layout { get; set; }

        private ObservableCollection<InstructableSummary> _groupItems = new ObservableCollection<InstructableSummary>();
        public ObservableCollection<InstructableSummary> GroupItems
        {
            get { return this._groupItems; }
        }

        private ObservableCollection<Instructable> _groupDetailItems = new ObservableCollection<Instructable>();
        public ObservableCollection<Instructable> GroupDetailItems
        {
            get { return this._groupDetailItems; }
        }

        private InstructableSummaryCollection _groupSummaries = new InstructableSummaryCollection();
        public InstructableSummaryCollection GroupSummaries
        {
            get { return this._groupSummaries; }
        }

        private ObservableCollection<Contest> _groupContestItems = new ObservableCollection<Contest>();
        public ObservableCollection<Contest> GroupContestItems
        {
            get { return this._groupContestItems; }
        }

        public string GroupImage 
        { 
            get
            {
                if (GroupItems.Count > 0)
                    return GroupItems[0].imageUrl;
                else
                    return null;
            }
        }
        public Brush GroupBrush { get; set; }
        public string GroupBackground { get; set; }

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }

        public int fitSize
        {
            get
            {
                return (int)(fitWidth / 3);
            }
        }


    }
}
