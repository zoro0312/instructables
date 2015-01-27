using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using Windows.UI.Xaml;

namespace Instructables.DataModel
{
    public class InstructableSummary : BindableBase
    {
        private const int SQUARE_MARGIN = 10;
        private const float SQUARE_RATIO = 1.0f;
        public DataGroup Group { get; set; }

        public string id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string channel { get; set; }
        private string _author = String.Empty;

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }

        public int fitHeight
        {
            get
            {
                return (int)(fitWidth * 1.5);
            }
        }


        public string author 
        { 
            get
            {
                return _author;
            }

            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }
        public string ByAuthor
        {
            get
            {
                String byAuthor = "by " + _author;
                return byAuthor;
            }
        }
        public string publishDate { get; set; }
        public string imageUrl { get; set; }
        public string square3Url { get; set; }
        public string rectangle1Url { get; set; }
        public string tinyUrl { get; set; }
        public string smallUrl { get; set; }
        public string squareUrl { get; set; }
        public string mediumUrl { get; set; }
        public string thumbUrl { get; set; }
        public string rect2100Url { get; set; }
        public string rect1350Url { get; set; }
        public string rect840Url { get; set; }
        public string rect600Url { get; set; }
        public string rect390Url { get; set; }
        public string rect270Url { get; set; }
        public bool featured { get; set; }
        public int favorites { get; set; }
        public string instructableType { get; set; }

        
        public bool isCollection 
        {
            get 
            {
                return instructableType == "G" || instructableType == "E";
            }
        }
        public int views { get; set; }
        public int squareWidth
        {
            get 
            {
                return (int)Window.Current.Bounds.Width - SQUARE_MARGIN;            
            }
        }

        public int squareHeight
        {
            get
            {
                var width = Window.Current.Bounds.Width - SQUARE_MARGIN;
                return (int)(width * SQUARE_RATIO);
            }
        }

        public string guideImageUrl
        {
            get
            {
                if (imageUrl.ToLower().Contains("default"))
                    return rectangle1Url;
                else
                    return imageUrl.Replace("SQUARE2", "SMALL"); 
            }
        }
        public string TypeGraphic 
        {
            get
            {
                switch (instructableType)
                {
                    case "I":
                        return "/assets/MenuImages/stepbystep.png";
                    case "S":
                        return "/assets/MenuImages/photos.png";
                    case "V":
                        return "/assets/MenuImages/video.png";
                    case "G":
                        return "/assets/MenuImages/guide.png";
                    case "E":
                        return "/assets/MenuImages/ebook.png";
                    default:
                        return null;
                }
            }
        }
        public int GroupOrdinal { get; set; }
        public int GroupIndex { get; set; }
        public bool sponsoredFlag { get; set; }

        //public int tabletHeight
        //{
        //    get
        //    {
        //        return (int)Window.Current.Bounds.Height;
        //    }
        //}

        //public int tempHeight
        //{
        //    get
        //    {
        //        return (int)((tabletHeight - 250) / 3);
        //    }
        //}

        //public int tempWidth
        //{
        //    get
        //    {
        //        return (int)(tempHeight * 1.5);
        //    }
        //}

    }
}
