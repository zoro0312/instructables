using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataModel
{
    public class SortOption
    {
        public string Display { get; set; }

        public string Network { get; set; }
    }

    public class InstructableSortOptions : List<SortOption>
    {
        public InstructableSortOptions()
        {
            this.Add(new SortOption() { Display = "Featured", Network = "featured" });
            this.Add(new SortOption() { Display = "Popular", Network = "popular" });
            this.Add(new SortOption() { Display = "Recent", Network = "recent" });
        }

        public SortOption Find(string sortOption)
        {
            return this.Find(x => x.Display == sortOption);
        }
    }
}
