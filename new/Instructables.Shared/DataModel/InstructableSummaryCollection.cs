using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataServices;
using Instructables.Utils;
using Instructables.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Instructables.Common;
using Windows.UI.Xaml.Controls;

namespace Instructables.DataModel
{
    public class InstructableSummaryCollection : ObservableCollection<InstructableSummary>, ISupportIncrementalLoading
    {
        public event EventHandler NetworkErrorEvent;
        public event EventHandler StartIncrementalLoadEvent;
        public event EventHandler StopIncrementalLoadEvent;

        private Category _category;
        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private SortOption _sort;
        public SortOption Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        private Channel _channel;
        public Channel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private InstructableType _type;
        public InstructableType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        //private ProgressBar _progressBar = null;
        //public ProgressBar ProgressBar
        //{
        //    get
        //    {
        //        return _progressBar;
        //    }
            
        //    set
        //    {
        //        _progressBar = value;
        //    }
        //}

        private bool _hasMoreItems = true;
        public bool HasMoreItems
        {
            get
            {
                /*if (DesignMode.IsInDesignMode())
                    return false;
                if (this.Count == 0)
                    _hasMoreItems = true;*/
                return _hasMoreItems;
            }
        }
        //private double ENABLE_TIME_GAP = 2000.0;
        //private DispatcherTimer DelayTimer = new DispatcherTimer();

        //public InstructableSummaryCollection()
        //{
        //    TimeSpan DelayTimeSpan = TimeSpan.FromMilliseconds(ENABLE_TIME_GAP);
        //    DelayTimer.Interval = DelayTimeSpan;
        //    DelayTimer.Tick += (sender, args) =>
        //    {
        //        DelayTimer.Stop();
        //        StopIncrementalLoadEvent(this, new EventArgs());
        //    };
        //}

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            const int MinCount = 20;
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
                            StartIncrementalLoadEvent(this,new EventArgs());
                            //DelayTimer.Start();
                        });
                     }
                     if (0 == this.Count && count < MinCount)
                     {
                         count = MinCount;
                     }
                     var instructableSummaries = await LoadMoreItemsAsync(this.Count, (int)count);
                     if (instructableSummaries == null)
                     {
                         _hasMoreItems = false;
                         if (NetworkErrorEvent != null)
                             await dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 () => NetworkErrorEvent(this, new EventArgs()));
                     }
                     else 
                     {
                         _hasMoreItems = true;
                     }
                     int totalLoaded = 0;
                     if (instructableSummaries != null)
                         totalLoaded = instructableSummaries.Count;
                     await dispatcher.RunAsync(
                          CoreDispatcherPriority.Normal,async
                         () =>
                         {
                             if (instructableSummaries != null)
                             {
                                 foreach (InstructableSummary summary in instructableSummaries)
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

                     //if (_progressBar != null)
                     //{
                     //    await dispatcher.RunAsync(
                     //    CoreDispatcherPriority.Normal, async
                     //   () => 
                     //   {
                     //       await Task.Delay(2000);
                     //       _progressBar.Visibility = Visibility.Collapsed;
                     //   });
                     //}
                     return new LoadMoreItemsResult() { Count = (uint)totalLoaded };
                 }).AsAsyncOperation<LoadMoreItemsResult>();
        }

        private async Task<ObservableCollection<InstructableSummary>> LoadMoreItemsAsync(int offset, int count)
        {
            var svc = InstructablesDataService.DataServiceSingleton;
            var category = Category != null ? Category.CategoryName : null;
            var channel = Channel != null ? Channel.title : null;
            var categorySummaries = await svc.GetSummaries(category, channel, Sort.Network, Type.Network, offset, count);
            if (categorySummaries != null && categorySummaries.items != null)
            {
                if (categorySummaries.items.Count < count)
                {
                    _hasMoreItems = false;
                }
                else
                {
                    _hasMoreItems = true;
                }
                    
                return categorySummaries.items;
            }
            return null;
        }
    }
}
