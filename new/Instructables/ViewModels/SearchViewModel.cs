using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using Instructables.DataModel;

namespace Instructables.ViewModels
{
    class SearchViewModel : BindableBase
    {
        private InstructableSummaryCollection _searchResults;

        public InstructableSummaryCollection SearchResults
        {
            get { return _searchResults; }
            set
            {
                if (_searchResults != value)
                {
                    _searchResults = value;
                    OnPropertyChanged();
                }
            }
        }

        public async void PerformSearch(string searchString)
        {
            
        }

    }
}
