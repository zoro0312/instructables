using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataModel
{
    public class InstructableCategoryList 
    {
        private ObservableCollection<InstructableSummary> _items;

        public ObservableCollection<InstructableSummary> items
        {
            get
            {
                if (_items == null)
                    _items = new ObservableCollection<InstructableSummary>();
                return _items;
            } 
            set
            {
                if (_items != value)
                {
                    _items = value;
                }
            }
        }
    }
}
