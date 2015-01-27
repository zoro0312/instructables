using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Instructables.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.Utils;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Instructables.ViewModels
{
    public class LandingViewModel : BindableBase
    {
        private InstructableSummaryCollection _recents;

        public InstructableSummaryCollection HomeList
        {
            get { return this._recents; }
            private set { this.SetProperty(ref this._recents, value);}
        }



        private InstructableCollection _followings;
        public InstructableCollection Followings
        {
            get { return this._followings; }
            private set { this.SetProperty(ref this._followings, value);}
        }

        private int _followingCount = 1;
        public int FollowingCount
        {
            get { return this._followingCount; }
            private set 
            {
                this._followingCount = value;
                OnPropertyChanged();
            }
        }

        private InstructableContestCollection _contests;
        public InstructableContestCollection Contests
        {
            get { return this._contests; }
            private set { this.SetProperty(ref this._contests, value); }
        }

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }

        }

        public int usernameWidth
        {
            get
            {
                return (int)(fitWidth - 180);
            }

        }


        private ObservableCollection<DataGroup> _exploreGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> ExploreGroups
        {
            get { return this._exploreGroups; }
        }

        private UserProfile _userProfile = new UserProfile();
        public UserProfile userProfile
        {
            get { return this._userProfile; }
            private set { this.SetProperty(ref this._userProfile, value); }
        }

        private Author _loginAuthor = new Author();
        public Author LoginAuthor
        {
            get
            {
                if (_userProfile != null)
                {
                    _loginAuthor.id = _userProfile.id;
                    _loginAuthor.screenName = _userProfile.screenName;
                    _loginAuthor.tinyUrl = _userProfile.tinyUrl;
                }
                return _loginAuthor;
            }
        }
        private bool _isMainMenuEnable = true;
        public bool IsMainMenuEnable
        {
            get 
            {
                return _isMainMenuEnable;
            }

            set 
            {
                _isMainMenuEnable = value;
                OnPropertyChanged();
            }
        }

        private bool _isLogin = false;
        public bool IsLogin
        {
            get { return this._isLogin; }
            private set { this.SetProperty(ref this._isLogin, value); }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get 
            { 
                return _isLoading; 
            }
            private set { this.SetProperty(ref this._isLoading, value); }
        }

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }

        public LandingViewModel()
        {
            if (DesignMode.IsInDesignMode())
            {
                InitializeDesignData();
            }
        }

        public async Task Recover()
        {
            IsMainMenuEnable = false;
            IsLoading = true;
            await UpdateUserProfile();
            IsLoading = false;
            IsMainMenuEnable = true;
        }

        public async Task Initialize()
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            IsMainMenuEnable = false;
            if (ExploreGroups.Count == 0)
            {
                var everythingGroup = new DataGroup()
                {
                    GroupOrdinal = 0,
                    GroupName = "All",
                    Layout = DataGroup.LayoutType.MainFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["AllBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupAll.jpg"
                };
                ExploreGroups.Add(everythingGroup);

                var technologyGroup = new DataGroup()
                {
                    GroupOrdinal = 4,
                    GroupName = "Tech",
                    Layout = DataGroup.LayoutType.SingleFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["TechnologyBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupTech.jpg"
                };
                ExploreGroups.Add(technologyGroup);

                var workshopGroup = new DataGroup()
                {
                    GroupOrdinal = 5,
                    GroupName = "Workshop",
                    Layout = DataGroup.LayoutType.DualFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["WorkshopBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupWorkshop.jpg"
                };
                ExploreGroups.Add(workshopGroup);

                var livingGroup = new DataGroup()
                {
                    GroupOrdinal = 6,
                    GroupName = "Living",
                    Layout = DataGroup.LayoutType.SingleFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["LivingBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupLiving.jpg"
                };
                ExploreGroups.Add(livingGroup);

                var foodGroup = new DataGroup()
                {
                    GroupOrdinal = 7,
                    GroupName = "Food",
                    Layout = DataGroup.LayoutType.DualFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["FoodBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupFood.jpg"
                };
                ExploreGroups.Add(foodGroup);

                var outsideGroup = new DataGroup()
                {
                    GroupOrdinal = 9,
                    GroupName = "Outside",
                    Layout = DataGroup.LayoutType.DualFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["OutsideBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupOutside.jpg"
                };
                ExploreGroups.Add(outsideGroup);

                var playGroup = new DataGroup()
                {
                    GroupOrdinal = 8,
                    GroupName = "Play",
                    Layout = DataGroup.LayoutType.SingleFeature,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["PlayBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupPlay.jpg"
                };
                ExploreGroups.Add(playGroup);

                var collectionsGroup = new DataGroup()
                {
                    GroupOrdinal = 3,
                    GroupName = "Collections",
                    Layout = DataGroup.LayoutType.EBook,
                    GroupBrush = (SolidColorBrush)Application.Current.Resources["CollectionsBrush"],
                    GroupBackground = "ms-appx:///Assets/LiveTile/GroupCollection.jpg"
                };
                ExploreGroups.Add(collectionsGroup);
            }

            var type = TypeOptions.Find("All Types");
            var sort = SortOptions.Find("Recent");
            HomeList = new InstructableSummaryCollection {Sort = sort, Type = type };
            
            Contests = new InstructableContestCollection { };

            Followings = new InstructableCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    InstructableList instructableList = await dataService.GetFollowedInstructable(offset, count);
                    if (instructableList != null && instructableList.instructables != null && instructableList.instructables.Count!=0)
                    {                        
                        return instructableList.instructables;
                    }
                    return null;
                },
            };
            Followings.LoadMoreEvent += FollowingLoadMoreHandler;

            HomeList.NetworkErrorEvent += NetworkError;
            HomeList.StartIncrementalLoadEvent += StartIncrementalLoading;
            Followings.StartIncrementalLoadEvent += StartIncrementalLoading;
            Contests.NetworkErrorEvent += NetworkError;
            Contests.StartIncrementalLoadEvent += StartIncrementalLoading;
            HomeList.StopIncrementalLoadEvent += StopIncrementalLoading;
            Followings.StopIncrementalLoadEvent += StopIncrementalLoading;
            Contests.StopIncrementalLoadEvent += StopIncrementalLoading;
            //Followings.NetworkErrorEvent += NetworkError;

            await FetchData();
            IsLoading = false;
            IsMainMenuEnable = true;
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = true;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = false;
        }

        private void NetworkError(object sender, EventArgs e)
        {
            IsLoading = false;
            VisualState = "Offline";
        }

        private void FollowingLoadMoreHandler(object sender, EventArgs e)
        {
            InstructableCollection following = sender as InstructableCollection;
            FollowingCount = following.Count;
        }

        private CategoryOptions _categories;
        public CategoryOptions Categories
        {
            get { return _categories; }
            private set { _categories = value; }
        }

        private readonly InstructableSortOptions _sortOptions = new InstructableSortOptions();
        public InstructableSortOptions SortOptions
        {
            get { return _sortOptions; }
        }

        private InstructableTypeOptions _typeOptions = new InstructableTypeOptions();
        public InstructableTypeOptions TypeOptions
        {
            get { return _typeOptions; }
        }

        private List<Channel> CombinedChannels(Channels rawChannels)
        {
            List<Channel> allChannels = new List<Channel>();
            allChannels.AddRange(rawChannels.food);
            allChannels.AddRange(rawChannels.living);
            allChannels.AddRange(rawChannels.outside);
            allChannels.AddRange(rawChannels.play);
            allChannels.AddRange(rawChannels.technology);
            allChannels.AddRange(rawChannels.workshop);
            var sorted = from a in allChannels
                         orderby a.display
                         select a;
            return sorted.ToList();
        }

        private async Task PrepareChannels()
        {
            if (Categories != null)
                return;

            var dataService = InstructablesDataService.DataServiceSingleton;
            var rawChannels = await dataService.GetChannels();
            if (rawChannels != null)
            {
                Categories = new CategoryOptions();
                var allChannels = CombinedChannels(rawChannels);
                allChannels.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "All", UrlParama = null, ChannelCollection = allChannels });

                foreach (var channelEntry in rawChannels.food)
                    channelEntry.CategoryName = "Food";
                rawChannels.food.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Food", UrlParama = "food", ChannelCollection = rawChannels.food });

                foreach (var channelEntry in rawChannels.living)
                    channelEntry.CategoryName = "Living";
                rawChannels.living.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Living", UrlParama = "living", ChannelCollection = rawChannels.living });

                foreach (var channelEntry in rawChannels.outside)
                    channelEntry.CategoryName = "Outside";
                rawChannels.outside.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Outside", UrlParama = "outside", ChannelCollection = rawChannels.outside });

                foreach (var channelEntry in rawChannels.play)
                    channelEntry.CategoryName = "Play";
                rawChannels.play.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Play", UrlParama = "play", ChannelCollection = rawChannels.play });

                foreach (var channelEntry in rawChannels.technology)
                    channelEntry.CategoryName = "Technology";
                rawChannels.technology.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Technology", UrlParama = "technology", ChannelCollection = rawChannels.technology });

                foreach (var channelEntry in rawChannels.workshop)
                    channelEntry.CategoryName = "Workshop";
                rawChannels.workshop.Insert(0, new Channel() { CategoryName = "All", title = null, display = "All" });
                Categories.Add(new Category() { CategoryName = "Workshop", UrlParama = "workshop", ChannelCollection = rawChannels.workshop });
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(ExecuteRefreshCommand);
                    _refreshCommand.IsEnabled = true;
                }
                return _refreshCommand;
            }
        }

        public async void ExecuteRefreshCommand()
        {
            if (HomeList != null)
                HomeList.Clear();
            if (Contests != null)
                Contests.Clear();
            if (Followings != null)
                Followings.Clear();
            VisualState = "Normal";
            await Initialize();
        }

        private async Task FetchData()
        {
            IsLoading = true;
            List<Task> tasks = new List<Task>();
            tasks.Add(PrepareChannels());
            tasks.Add(UpdateUserProfile());
            tasks.Add(HomeList.LoadMoreItemsAsync(4).AsTask());
            tasks.Add(Contests.LoadMoreItemsAsync(4).AsTask());
            await Task.WhenAll(tasks);
            IsLoading = false;
        }

        public async Task UpdateUserProfile()
        {
            var loginUserName = SessionManager.GetLoginUserName();
            IsLogin = loginUserName != null && loginUserName != string.Empty;
            //if (userProfile.screenName != loginUserName)
            //{
            //    userProfile = new UserProfile();
            //    Followings.Clear();

            //    if (IsLogin)
            //    {
            //        IsLoading = true;
            //        var dataService = InstructablesDataService.DataServiceSingleton;
            //        var newUp = await dataService.GetUserProfile(loginUserName);
            //        if (newUp != null)
            //        {
            //            userProfile = newUp;
            //            await Followings.LoadMoreItemsAsync(4);
            //        }
            //        IsLoading = false;
            //    }
            //}
            if (userProfile.subscriptions != null)
            {
                userProfile.subscriptions.Clear();
                userProfile.subscriptions = null;
                userProfile.subscriptionsCount = 0;
            }
            
            userProfile = null;
            userProfile = new UserProfile();
            if(Followings != null)
                Followings.Clear();

            if (IsLogin)
            {
                IsLoading = true;
                var dataService = InstructablesDataService.DataServiceSingleton;
                var newUp = await dataService.GetUserProfile(loginUserName,true);
                if (newUp != null)
                {
                    userProfile = newUp;
                    if(Followings != null)
                        await Followings.LoadMoreItemsAsync(4);
                }
                IsLoading = false;
            }
        }

        private void InitializeDesignData()
        {
        }
    }
}
