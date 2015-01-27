using Instructables.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Instructables.DataModel
{
    public class File : BindableBase
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool image { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string embedHtmlCode { get; set; }
        public string embedType { get; set; }
        public List<ImageNote> imageNotes { get; set; }
        public bool ifNotes
        {
            get
            {
                if (imageNotes == null || isGif == true) return false;
                return (imageNotes.Count!=0?true:false);
            }
        }
        private string _tinyUrl = String.Empty;
        public string tinyUrl 
        { 
            get
            {
                return _tinyUrl;
            }

            set
            {
                _tinyUrl = value;
                OnPropertyChanged();
            }
        }
        public string squareUrl { get; set; }
        public string square2Url { get; set; }
        public string square3Url { get; set; }
        public string _mediumUrl = String.Empty;
        public string mediumUrl
        {
            get
            {
                return _mediumUrl;
            }
            
            set
            {
                _mediumUrl = value;
                OnPropertyChanged();
            }
        }
        public string _largeUrl = String.Empty;
        public string largeUrl
        {
            get
            {
                return _largeUrl;
            }

            set
            {
                _largeUrl = value;
                OnPropertyChanged();
            }
        }

        public string downloadUrl { get; set; }

        private bool _isgif = false;
        public bool isGif
        {
            get 
            {
                var extName = System.IO.Path.GetExtension(_largeUrl);
                _isgif = ((extName == ".gif" || extName == ".GIF") ? true : false);
                return _isgif;
            }

        }

        public int fitHeight
        {
            get
            {
                float ratio = (float)height / (float)width;
                var screenWidth = Window.Current.Bounds.Width;
                return (int)(screenWidth * ratio);
            }
        }

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width+1;
            }
        }

        public int fitHeighttab
        {
            get
            {
                return (int)Window.Current.Bounds.Height;
            }
        }

        public int fitWidthtab
        {
            get
            {
                float ratio1 = (float)width / (float)height;
                var screenHeight = Window.Current.Bounds.Height;
                return (int)(screenHeight * ratio1);
            }
        }

        public int fitDelsize
        {
            get
            {
                return (int)(fitWidth / 12);
            }
        }

        public int fitSize
        {
            get
            {
                return (int)((fitWidth - 39) / 3);
            }
        }

        public string thumbUrl { get; set; }
        public string smallUrl { get; set; }
        public string rectangle1Url { get; set; }
    }

    public class ImageNote
    {
        public string id { get; set; }
        public string author { get; set; }
        public string text { get; set; }
        public double top { get; set; }
        public double left { get; set; }
        public double height { get; set; }
        public double width { get; set; }
    }
}
