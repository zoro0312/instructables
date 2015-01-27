using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using Windows.UI.Xaml;

namespace Instructables.DataModel
{
    public class Video : BindableBase
    {
        public string UriString { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public string VideoUri
        {
            get
            {
                if (UriString.Contains("http:"))
                    return UriString;
                else
                    return "http:" + UriString;
            }
        }

        public int VideoWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width - 20;
            }
        }

        public int VideoHeight
        {
            get
            {
                float ratio = (float)Height / (float)Width;
                return (int)(VideoWidth * ratio);
            }
        }

        private string _thumbnailURI;
        public string ThumbnailURI
        {
            get { return _thumbnailURI; }
            set
            {
                if (_thumbnailURI != value)
                {
                    _thumbnailURI = value;
                    OnPropertyChanged();
                }
            }

        }
        public string Source { get; set; }
    }
}
