using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

using Instructables.Common;
using Windows.UI.Xaml;

namespace Instructables.DataModel
{
    public class Step : BindableBase
    {
        public string id { get; set; }
        public string url { get; set; }
        public string _title = String.Empty;
        public string title 
        { 
            get
            {
#if WINDOWS_PHONE_APP
                return _title;
#else
                if(stepIndex == 0)
                {
                    return "Overview";
                }
                else
                {
                    return _title;
                }
#endif
            }
            
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _body = String.Empty;
        public string body 
        {
            get
            {
                return _body;
            }

            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }

        public int stepIndex { get; set; }
        public List<File> files { get; set; }
        public List<File> DownloadFiles { get; set; }
        public List<Video> VideoList { get; set; }
        public string XamlBody { get; set; }
        public string StepImageUrl
        {
            get
            {
                if (files != null && files.Count!=0)
                {
                    return files[0].largeUrl;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public bool IsStepImage
        {
            get
            {
                if (files != null && files.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string StepTinyImageUrl
        {
            get
            {
                if (files != null && files.Count != 0)
                {
                    return files[0].square3Url;
                }
                else
                {
                    return String.Format("ms-appx:///Assets/StepDummyTiny.png");
                }
            }
        }
        // ---------------------
        private int _totalStepNum = 0;
        public int totalStepNum
        {
            get
            {
                return _totalStepNum;
            }

            set
            {
                _totalStepNum = value;
                OnPropertyChanged();
            }
        }

        public bool IsLastStep
        {
            get
            {
                return (stepIndex == _totalStepNum ? true : false);
            }
        }

        public bool IfMoreImages
        {
            get
            {
                var result = files.Count > 1 ? true : false;
                return result;
            }
        }

        public string MoreImages
        {
            get
            {
                String returnString = String.Empty;
                if (files.Count - 1 > 1)
                    returnString = String.Format("{0} More Images", files.Count - 1);
                else
                    returnString = String.Format("{0} More Image", files.Count - 1);
                return returnString;
            }
        }

        public bool HasDownloads
        {
            get
            {
                if (DownloadFiles == null)
                    return false;
                return this.DownloadFiles.Count > 0;
            }
        }

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
                return (int)((fitWidth - 47) / 3);
            }
        }

        public string StepName
        {
            get
            {
                if (stepIndex == 0)
                {
                    return "Intro";
                }
                else
                {

#if WINDOWS_PHONE_APP
                     return String.Format("STEP {0}", stepIndex);
#else
                    return String.Format("STEP {0} of {1}", stepIndex, totalStepNum);
#endif
                }
            }
        }

        public string StepNameLowCase
        {
            get
            {
                if (stepIndex == 0)
                {
                    return "Introduction";
                }
                else
                {
                    return String.Format("Step {0}", stepIndex);
                }
            }
        }

        public string TinyStepName
        {
            get
            {
                if (stepIndex == 0)
                    return "INTRO";
                else
                {
                    return String.Format("{0}", stepIndex);
                }
            }
        }

        [DataMember(IsRequired = false)]
        public string StepThumbnail
        {
            get
            {
                if (ImageNames != null && ImageNames.Count != 0)
                    return ImageNames[0].name;
                if (files != null && files.Count != 0)
                    return files[0].tinyUrl;
                return String.Format("ms-appx:///Assets/Photos.png");
            }
        }

        [DataMember(IsRequired = false)]
        public ObservableCollection<File> ImageNames { get; set; }

        public Step()
        {
            id = string.Empty;
            files = new List<File>();
            ImageNames = new ObservableCollection<File>();

            ImageNames.CollectionChanged += new NotifyCollectionChangedEventHandler(ImageNames_CollectionChanged);
        }

        // Used to update the step thumbnail
        private void ImageNames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("StepThumbnail");
        }
    }
}
