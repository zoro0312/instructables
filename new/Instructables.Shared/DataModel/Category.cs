using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;

namespace Instructables.DataModel
{
    public class Category : BindableBase
    {
        private string _categoryName;
        private string _urlParam;

        public string CategoryName
        {
            get { return _categoryName; }
            set { _categoryName = value; }
        }

        public string UrlParama
        {
            get { return _urlParam; }
            set { _urlParam = value; }
        }
        
        public List<Channel> ChannelCollection
        {
            get; 
            set;
        }
    }

    public class CategoryOptions : List<Category>
    {
        public Category Find(string categoryName)
        {
            return this.Find(x => x.CategoryName == categoryName);
        }
    }
}
