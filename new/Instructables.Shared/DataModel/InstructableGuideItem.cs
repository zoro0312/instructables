using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Instructables.DataModel
{
    public class InstructableGuideItem
    {
        public string id { get; set; }
        public string title { get; set; }
        public string urlString { get; set; }
        public Author author { get; set; }
        public string ByAuthor
        {
            get
            {
                return author.byScreenName;
            }
        }
        public string square2Url { get; set; }
        public string rectangle1Url { get; set; }
        public string mediumUrl { get; set; }
        public string type { get; set; }
        public string publishDate { get; set; }
        public int editVersion { get; set; }
        public bool sponsoredFlag { get; set; }
        public bool isCollection
        {
            get
            {
                return type.StartsWith("guide");
            }
        }

        public int fitHeight
        {
            get
            {
                float ratio = (float)3.0f / (float)4.0f;
                var screenWidth = Window.Current.Bounds.Width;
                return (int)(screenWidth * ratio);
            }
        }

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }
    }
}
