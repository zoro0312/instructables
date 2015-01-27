using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using System.Collections.ObjectModel;

namespace Instructables.DataModel
{
    public class CoveredImage : BindableBase
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool image { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string downloadUrl { get; set; }
        public string tinyUrl { get; set; }
        public string squareUrl { get; set; }
        public string square2Url { get; set; }
        public string square3Url { get; set; }
        public string thumbUrl { get; set; }
        public string smallUrl { get; set; }
        public string mediumUrl { get; set; }
        public string rectangle1Url { get; set; }
        public string urlString { get; set; }
        public string largeUrl { get; set; }
        public string rect2100Url { get; set; }
        public string rect1350Url { get; set; }
        public string rect840Url { get; set; }
        public string rect600Url { get; set; }
        public string rect390Url { get; set; }
        public string rect270Url { get; set; }
    }
}
