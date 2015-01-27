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
    public class InstructableContestList 
    {
        private ObservableCollection<Contest> _contests;

        public ObservableCollection<Contest> contests
        {
            get
            {
                if (_contests == null)
                    _contests = new ObservableCollection<Contest>();
                return _contests;
            } 
            set
            {
                if (_contests != value)
                {
                    _contests = value;
                }
            }
        }
    }

    public class InstructableContestCollection : ObservableCollection<Contest>, ISupportIncrementalLoading
    {
        public event EventHandler NetworkErrorEvent;
        public event EventHandler StartIncrementalLoadEvent;
        public event EventHandler StopIncrementalLoadEvent;

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
                     int number = (int)count > 4 ? 4 : (int)count;
                     var contests = await LoadMoreItemsAsync(this.Count, number);
                     if (contests == null)
                     {
                         _hasMoreItems = false;
                         if (NetworkErrorEvent != null)
                             await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => NetworkErrorEvent(this, new EventArgs()));
                     }
                     int totalLoaded = 0;
                     if (contests != null)
                         totalLoaded = contests.Count;
                     await dispatcher.RunAsync(
                          CoreDispatcherPriority.Normal, async
                         () =>
                         {
                             if (contests != null)
                             {
                                 foreach (Contest contest in contests)
                                 {
                                     if (contest.state != "open")
                                     {
                                         _hasMoreItems = false;
                                         continue;
                                     }

                                     // check to make sure an item isn't already in the collection, if it is then skip adding it
                                     var x = (from y in this
                                              where y.id == contest.id
                                              select y).FirstOrDefault();
                                     if (x == null)
                                         this.Add(contest);
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

        private async Task<ObservableCollection<Contest>> LoadMoreItemsAsync(int offset, int count)
        {
            var svc = InstructablesDataService.DataServiceSingleton;
            var recentContests = await svc.GetContests(offset, count);
            if (recentContests != null && recentContests.contests != null)
            {
                if (recentContests.contests.Count < count)
                    _hasMoreItems = false;
                return recentContests.contests;
            }
            return null;
        }
    }
}
