using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using Instructables.DataServices;
using Instructables.Common;

namespace Instructables.DataModel
{
    public class Comment : BindableBase
    {
        private bool _IMadeIt = false;
        public bool IMadeIt
        {
            get
            {
                return _IMadeIt;
            }
            set
            {
                _IMadeIt = value;
                OnPropertyChanged();
            }
        }

        public int fitwidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }

        private string _status = "";
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged();
            }

        }

        private bool _deleted = false;
        public bool deleted
        {
            get
            {
                return _deleted;
            }

            set
            {
                _deleted = value;
                OnPropertyChanged();
            }
        }

        private string _title = "";
        public string title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _body;
        public string body
        {
            get
            {
                return _body;
            }

            set
            {
                String stringValue = (string)value;
                _body = stringValue;
                OnPropertyChanged();
            }
        }

        private bool _sticky = false;
        public bool sticky
        {
            get
            {
                return _sticky;
            }
            set
            {
                _sticky = value;
                OnPropertyChanged();
            }
        }

        private bool _approved = false;
        public bool approved
        {
            get
            {
                return _approved;
            }

            set
            {
                _approved = value;
                OnPropertyChanged();
            }
        }

        private string _publishDate = "";
        public string publishDate
        {
            get
            {
                return _publishDate;
            }

            set
            {
                _publishDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime PublishDate
        {
            get { return DateTime.Parse(publishDate); }
        }

        public string _authorId = "";
        public string authorId
        {
            get
            {
                return _authorId;
            }

            set
            {
                _authorId = value;
                OnPropertyChanged();
            }
        }

        private string _author = "";
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

        private string _avatar = "ms-appx:///Assets/HeaderLogos/icon_profile.png";
        public string avatar
        {
            get
            {
                return _avatar;
            }

            set
            {
                if ((string)value == "/static/defaultIMG/user.TINY.gif")
                {
                    _avatar = "ms-appx:///Assets/HeaderLogos/icon_profile.png";
                }
                else
                {
                    _avatar = value;
                }
                OnPropertyChanged();
            }
        }
        private bool _quarantine = false;
        public bool quarantine
        {
            get
            {
                return _quarantine;
            }

            set
            {
                _quarantine = value;
                OnPropertyChanged();
            }

        }

        private bool _limbo = false;
        public bool limbo
        {
            get
            {
                return _limbo;
            }

            set
            {
                _limbo = value;
                OnPropertyChanged();
            }
        }

        private List<File> _files = new List<File>();
        public List<File> files
        {
            get
            {
                return _files;
            }

            set
            {
                _files = value;
                OnPropertyChanged();
            }
        }

        public bool ifFiles
        {
            get
            {
                return (files.Count > 0 ? true : false);
            }
        }

        private string _id = "";
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _instructableId = "";
        public string instructableId
        {
            get
            {
                return _instructableId;
            }

            set
            {
                _instructableId = value;
                OnPropertyChanged();
            }

        }
    }

    public class CommentsList : BindableBase
    {
        private ObservableCollection<Comment> _comments;

        public ObservableCollection<Comment> comments
        {
            get
            {
                if (_comments == null)
                    _comments = new ObservableCollection<Comment>();
                return _comments;
            }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class CommentCollection : ObservableCollection<Comment>, ISupportIncrementalLoading
    {
        public CommentsList commentlist;

        private bool _hasMoreItems = true;
        public bool HasMoreItems
        {
            get
            {
                if (this.Count == 0)
                    _hasMoreItems = true;
                return _hasMoreItems;
            }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return loadMoreItems(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> loadMoreItems(uint count)
        {
            var actualCount = count;
            if (commentlist != null)
            {
                int offset = this.Count + (int)count;
                int totalLoaded = commentlist.comments.Count;
                _hasMoreItems = offset < totalLoaded;
                for (int index = this.Count; index < offset && index < totalLoaded; index++)
                {
                    this.Add(commentlist.comments[index]);
                }
            }
            else
            {
                _hasMoreItems = true;
            }

            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }
    }
}
