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

    public class ContestEntry : BindableBase
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
        public string smallUrl { get; set; }
        public string addedDate { get; set; }
        public bool finalist { get; set; }

        public CoveredImage coverImage { get; set; }
        public int order { get; set; }
    }

    public class ContestEntries : BindableBase
    {
        public ObservableCollection<ContestEntry> entries { get; set; }
    }

    public class Price : BindableBase
    {
        public string level { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string mediumUrl { get; set; }
        public string square2Url { get; set; }
        public string square3Url { get; set; }
        public int numWinners { get; set; }
    }

    public class Contest : BindableBase
    {
        public string id { get; set; }
        public string title { get; set; }
        public string urlString { get; set; }
        public string thumbUrl { get; set; }
        public string locale { get; set; }
        public bool hasAppleProducts { get; set; }
        public string state { get; set; }
        public int weight { get; set; }
        public string startDate { get; set; }
        public string deadline { get; set; }
        public string closesDate
        {
            get
            {
                DateTime date = DateTime.Parse(deadline);
                return "Closes " + date.ToString("MMM d, yyyy");
            }

        }

        public int numEntries { get; set; }
        public string squareUrl { get; set; }
        public string bannerUrl { get; set; }
        public string headerUrl { get; set; }
        public string judgingOpen { get; set; }
        public string judgingClosed { get; set; }
        public string body { get; set; }
        public string rules { get; set; }
        public ObservableCollection<Price> prizes { get; set; }
        public DataGroup Group { get; set; }
        public int GroupOrdinal { get; set; }
        public int GroupIndex { get; set; }
    }

    public class ContestEntryCollection : ObservableCollection<ContestEntry>, ISupportIncrementalLoading
    {
        public event EventHandler NetworkErrorEvent;
        public event EventHandler StartIncrementalLoadEvent;
        public event EventHandler StopIncrementalLoadEvent;
        public event EventHandler LoadMoreEvent;
        public string contestId; 

        private bool _hasMoreItems = true;
        public bool HasMoreItems
        {
            get
            {
                //if (this.Count == 0)
                //    _hasMoreItems = true;
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
                     var contestEntries = await LoadMoreItemsAsync(this.Count, (int)count);
                     if (contestEntries == null)
                     {
                         _hasMoreItems = false;
                         if (NetworkErrorEvent != null)
                             await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => NetworkErrorEvent(this, new EventArgs()));
                     }
                     int totalLoaded = 0;
                     if (contestEntries != null)
                         totalLoaded = contestEntries.Count;
                     await dispatcher.RunAsync(
                          CoreDispatcherPriority.Normal, async
                         () =>
                         {
                             if (contestEntries != null)
                             {
                                 foreach (ContestEntry entry in contestEntries)
                                 {
                                     // check to make sure an item isn't already in the collection, if it is then skip adding it
                                     var x = (from y in this
                                              where y.id == entry.id
                                              select y).FirstOrDefault();
                                     if (x == null)
                                         this.Add(entry);
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
                     if (LoadMoreEvent != null)
                         await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => LoadMoreEvent(this, new EventArgs()));
                     return new LoadMoreItemsResult() { Count = (uint)totalLoaded };
                 }).AsAsyncOperation<LoadMoreItemsResult>();
        }

        private async Task<ObservableCollection<ContestEntry>> LoadMoreItemsAsync(int offset, int count)
        {
            var svc = InstructablesDataService.DataServiceSingleton;
            var contestEntries = await svc.GetContestEntries(contestId, offset, count);
            if (contestEntries != null && contestEntries.entries != null)
            {
                if (contestEntries.entries.Count < count)
                    _hasMoreItems = false;
                else
                    _hasMoreItems = true;
                return contestEntries.entries;
            }
            return null;
        }
    }
}
