using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using Instructables.Common;
using Instructables.DataServices;

namespace Instructables.DataModel
{
    public class Author : BindableBase
    {
        public string id { get; set; }
        public string screenName { get; set; }
        public string tinyUrl { get; set; }
        public bool _isFollowed = false;
        public bool isFollowed
        {
            get
            {
                return _isFollowed;
            }

            set
            {
                _isFollowed = value;
                OnPropertyChanged();
            }
        }
        private bool _isNotFollowing = true;
        public bool isNotFollowing
        {
            get
            {
                return _isNotFollowing;
            }
            set
            {
                _isNotFollowing = value;
                OnPropertyChanged();
            }
        }
        public string byScreenName
        {
            get 
            { 
                return ("By " + screenName);
            }
        }
    }

    public class FollowersList
    {
        private ObservableCollection<Author> _followers;

        public ObservableCollection<Author> followers
        {
            get
            {
                if (_followers == null)
                    _followers = new ObservableCollection<Author>();
                return _followers;
            }
            set
            {
                if (_followers != value)
                {
                    _followers = value;
                }
            }
        }

        public int fullListSize { get; set; }
        public int fullListOffset { get; set; }
    }

    public class SubscriptionsList
    {
        private ObservableCollection<Author> _subscriptions;

        public ObservableCollection<Author> subscriptions
        {
            get
            {
                if (_subscriptions == null)
                    _subscriptions = new ObservableCollection<Author>();
                return _subscriptions;
            }
            set
            {
                if (_subscriptions != value)
                {
                    _subscriptions = value;
                }
            }
        }

        public int fullListSize { get; set; }
        public int fullListOffset { get; set; }
    }

    public class AuthorCollection : ObservableCollection<Author>, ISupportIncrementalLoading
    {
        public event EventHandler NetworkErrorEvent;
        public event EventHandler StartIncrementalLoadEvent;
        public event EventHandler StopIncrementalLoadEvent;

        public delegate Task<ObservableCollection<Author>> LoadMoreItems(int offset, int count);

        public LoadMoreItems loadMoreItemsAsync;

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
            const uint MinCount = 20;
            // Variables
            CoreDispatcher dispatcher = Window.Current.Dispatcher;

            // Exec
            return Task.Run<LoadMoreItemsResult>(
                 async () =>
                 {
                     if (StartIncrementalLoadEvent != null)
                     {
                         await dispatcher.RunAsync(
                         CoreDispatcherPriority.Normal,
                        () =>
                        {
                            StartIncrementalLoadEvent(this, new EventArgs());
                            //DelayTimer.Start();
                        });
                     }
                     if (0 == this.Count && count < MinCount)
                     {
                         count = MinCount;
                     }
                     var authors = await loadMoreItemsAsync(this.Count, (int)count);
                     if (authors == null)
                     {
                         _hasMoreItems = false;
                         if (NetworkErrorEvent != null)
                             await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => NetworkErrorEvent(this, new EventArgs()));
                     }
                     else if (authors.Count < count)
                     {
                         _hasMoreItems = false;
                     }
                     int totalLoaded = 0;
                     if (authors != null)
                         totalLoaded = authors.Count;
                     await dispatcher.RunAsync(
                          CoreDispatcherPriority.Normal, async
                         () =>
                         {
                             if (authors != null)
                             {
                                 foreach (Author author in authors)
                                 {
                                     // check to make sure an item isn't already in the collection, if it is then skip adding it
                                     var x = (from y in this
                                              where y.id == author.id
                                              select y).FirstOrDefault();
                                     if (x == null)
                                         this.Add(author);
                                     else
                                     {
                                         Debug.WriteLine("Skipped adding item, was already in the collection");
                                     }
                                 }
                                 if (StopIncrementalLoadEvent != null)
                                 {
                                     await dispatcher.RunAsync(
                                     CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        StopIncrementalLoadEvent(this, new EventArgs());
                                        //DelayTimer.Start();
                                    });
                                 }
                             }
                         });

                     return new LoadMoreItemsResult() { Count = (uint)totalLoaded };
                 }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
