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
    public class InstructableList 
    {
        private ObservableCollection<Instructable> _instructables;

        public ObservableCollection<Instructable> instructables
        {
            get
            {
                if (_instructables == null)
                    _instructables = new ObservableCollection<Instructable>();
                return _instructables;
            } 
            set
            {
                if (_instructables != value)
                {
                    _instructables = value;
                }
            }
        }

        public int fullListSize { get; set; }
        public int fullListOffset { get; set; }
    }

    public class InstructableFavoriteList
    {
        private ObservableCollection<Instructable> _favorites;

        public ObservableCollection<Instructable> favorites
        {
            get
            {
                if (_favorites == null)
                    _favorites = new ObservableCollection<Instructable>();
                return _favorites;
            }
            set
            {
                if (_favorites != value)
                {
                    _favorites = value;
                }
            }
        }

        public int fullListSize { get; set; }
        public int fullListOffset { get; set; }
    }

    public class InstructableCollection : ObservableCollection<Instructable>, ISupportIncrementalLoading
    {
        public event EventHandler NetworkErrorEvent;
        public event EventHandler LoadMoreEvent;
        public event EventHandler StartIncrementalLoadEvent;
        public event EventHandler StopIncrementalLoadEvent;

        public delegate Task<ObservableCollection<Instructable>> LoadMoreItems(int offset, int count);

        public LoadMoreItems loadMoreItemsAsync;

        private bool _hasMoreItems = true;
        public bool HasMoreItems
        {
            get
            {
                //if (this.Count == 0)
                //    _hasMoreItems = false;
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
                     if (StartIncrementalLoadEvent != null && _hasMoreItems != false)
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
                     var instructableSummaries = await loadMoreItemsAsync(this.Count, (int)count);
                     if (instructableSummaries == null)
                     {
                         _hasMoreItems = false;
                         if (NetworkErrorEvent != null)
                             await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => NetworkErrorEvent(this, new EventArgs()));
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
                     else if (instructableSummaries.Count < count)
                     {
                         _hasMoreItems = false;
                     }
                     int totalLoaded = 0;
                     if (instructableSummaries != null)
                         totalLoaded = instructableSummaries.Count;
                     await dispatcher.RunAsync(
                          CoreDispatcherPriority.Normal, async
                         () =>
                         {
                             if (instructableSummaries != null)
                             {
                                 foreach (Instructable summary in instructableSummaries)
                                 {
                                     // check to make sure an item isn't already in the collection, if it is then skip adding it
                                     var x = (from y in this
                                              where y.id == summary.id
                                              select y).FirstOrDefault();
                                     if (x == null)
                                         this.Add(summary);
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

                     if(LoadMoreEvent != null)
                         await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => LoadMoreEvent(this, new EventArgs()));
                     return new LoadMoreItemsResult() { Count = (uint)totalLoaded };
                 }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
