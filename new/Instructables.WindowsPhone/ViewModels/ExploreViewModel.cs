using System.Collections.ObjectModel;
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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Instructables.ViewModels
{
    public class ExploreDataGroup : BindableBase
    {
        public ExploreDataGroup(string name, Category category)
        {
            _groupName = name;
            _category = category; 
        }

        private ObservableCollection<DataGroup> _groupItems = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> GroupItems
        {
            get { return this._groupItems; }
        }

        private string _groupName = "";
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                OnPropertyChanged("GroupName");
                OnPropertyChanged("GroupIcon");
            }
        }

        // Channel display name that binding with explore page.
        public string ChannelDisplayName
        {
            get
            {
                string display = GroupName;
                if (Channel != null && Channel.title != null)
                {
                    display = Channel.display;
                }
                return display;
            }
        }

        private Channel _channel = new Channel() { CategoryName = "All", title = null, display = "All" };
        public Channel Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                OnPropertyChanged("Channel");
                OnPropertyChanged("ChannelDisplayName");
            }
        }

        private Category _category = null;
        public Category Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged("Category");
                OnPropertyChanged("CurrentGroupChannels");
            }
        }

        private List<Channel> _currentGroupChannels = new List<Channel>();
        public List<Channel> CurrentGroupChannels
        {
            get {
                if (Category == null)
                    return null;
                return Category.ChannelCollection; }
        }

        private SolidColorBrush _backgroundColor = (SolidColorBrush)Application.Current.Resources["AllBrush"];
        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public DataTemplate GroupIcon
        {
            get
            {
                return (DataTemplate)Application.Current.Resources[GroupName + "HeaderIcon"];
            }
        }
    }

    public class ExploreViewModel : BindableBase
    {
        Dictionary<string, ExploreDataGroup> _allGroups = new Dictionary<string, ExploreDataGroup>();
        ExploreDataGroup _currentGroup = new ExploreDataGroup("All", ViewModelLocator.Instance.LandingVM.Categories == null ? null : ViewModelLocator.Instance.LandingVM.Categories.Find("All"));

        private int _selectedChannelIndex = 0;
        private int _selectedCollectionCategoryIndex = 0;
        private bool _isLoading = false;
        //private bool _dataLoaded = false;

        public ExploreViewModel()
        {
            _initializeGroup();
        }

        private void _initializeGroup()
        {
            if (ViewModelLocator.Instance.LandingVM.Categories != null)
            {
                _allGroups["All"] = new ExploreDataGroup("All", ViewModelLocator.Instance.LandingVM.Categories.Find("All"));
                _allGroups["Tech"] = new ExploreDataGroup("Tech", ViewModelLocator.Instance.LandingVM.Categories.Find("Technology"));
                _allGroups["Workshop"] = new ExploreDataGroup("Workshop", ViewModelLocator.Instance.LandingVM.Categories.Find("Workshop"));
                _allGroups["Living"] = new ExploreDataGroup("Living", ViewModelLocator.Instance.LandingVM.Categories.Find("Living"));
                _allGroups["Food"] = new ExploreDataGroup("Food", ViewModelLocator.Instance.LandingVM.Categories.Find("Food"));
                _allGroups["Play"] = new ExploreDataGroup("Play", ViewModelLocator.Instance.LandingVM.Categories.Find("Play"));
                _allGroups["Outside"] = new ExploreDataGroup("Outside", ViewModelLocator.Instance.LandingVM.Categories.Find("Outside"));
                _allGroups["Collections"] = new ExploreDataGroup("Collections", new Category() { CategoryName = "Collections", UrlParama = null, ChannelCollection = new List<Channel>() });

                _allGroups["All"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["AllBrush"];
                _allGroups["Tech"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["TechnologyBrush"];
                _allGroups["Workshop"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["WorkshopBrush"];
                _allGroups["Living"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["LivingBrush"];
                _allGroups["Food"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["FoodBrush"];
                _allGroups["Play"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["PlayBrush"];
                _allGroups["Outside"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["OutsideBrush"];
                _allGroups["Collections"].BackgroundColor = (SolidColorBrush)Application.Current.Resources["CollectionsBrush"];
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public bool IsValidGroup(string groupName)
        {
            return _allGroups.ContainsKey(groupName);
        }

        public bool IsEmptyGroup(string groupName)
        {
            return _allGroups[groupName].GroupItems.Count == 0;
        }

        public async Task InitializeGroup(string groupName)
        {
            IsLoading = true;
            //_dataLoaded = false;            
            await FetchData(groupName);
            //_dataLoaded = true;
            IsLoading = false;
        }

        // Used to display a category menu for collection group.
        public List<Category> CollectionCategoryList
        {
            get { return ViewModelLocator.Instance.LandingVM.Categories; }
        }

        public Category SelectedCollectionCategory
        {
            get { return ViewModelLocator.Instance.LandingVM.Categories[SelectedCollectionCategoryIndex]; }
        }

        public int SelectedCollectionCategoryIndex
        {
            get { return _selectedCollectionCategoryIndex; }
            set
            {
                if (value < 0 )
                    value = 0;

                _selectedCollectionCategoryIndex = value;

                CurrentGroup.Category = SelectedCollectionCategory;

                OnPropertyChanged();
            }
        }

        public Channel SelectedChannel
        {
            get { return CurrentGroup.CurrentGroupChannels[SelectedChannelIndex]; }
        }

        public void SetCurrent(string groupName)
        {
            if (_currentGroup.GroupItems.Count > 0 &&
                groupName == _currentGroup.GroupName)
                return;

            _currentGroup.GroupItems.Clear();
            var group = _allGroups[groupName];
            for (int i = 0; i < group.GroupItems.Count; i++)
            {
                _currentGroup.GroupItems.Add(group.GroupItems[i]);
            }

            // Update binding properties.
            _currentGroup.BackgroundColor = group.BackgroundColor;
            _currentGroup.GroupName = group.GroupName;
            _currentGroup.Category = group.Category;
            _currentGroup.Channel = group.Channel;

            // Clear current selected.
            SelectedChannelIndex = 0;

            // Notify UI to update.
            OnPropertyChanged();
        }

        public void RefreshGroups()
        {
            _initializeGroup();
        }

        async public Task RefreshCurrentCroup()
        {
            if(CurrentGroup.Category != null)
                await FetchData(CurrentGroup.GroupName, CurrentGroup.Category.UrlParama, CurrentGroup.Channel.title);
        }

        public ExploreDataGroup CurrentGroup
        {
            get { return _currentGroup; }
        }

        public int SelectedChannelIndex
        {
            get { return _selectedChannelIndex; }
            set
            {
                if (value < 0) return;

                if (_selectedChannelIndex != value)
                {
                    _selectedChannelIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task GetSelectedChannelData(int selectedData)
        {
            if (CurrentGroup.CurrentGroupChannels.Count > 0)
            {
                CurrentGroup.Channel = CurrentGroup.CurrentGroupChannels[selectedData];
                await FetchData(CurrentGroup.GroupName, CurrentGroup.Category.UrlParama, CurrentGroup.Channel.title);
            }
        }

        private async Task FetchData(string groupName, string category=null, string channel=null)
        {
            if (category != null) {
                category = category.ToLower();
                if (category == "all") 
                    category = null;
            }

            if (channel != null)
            {
                channel = channel.ToLower();
                if (channel == "all")
                    channel = null;
            }

            var dataService = InstructablesDataService.DataServiceSingleton;
            switch (groupName)
            {
                case "Tech":
                    await RefreshDataGroup(groupName, "Technology", channel);
                    break;
                case "Workshop":
                    await RefreshDataGroup(groupName, "Workshop", channel);
                    break;
                case "Living":
                    await RefreshDataGroup(groupName, "Living", channel);
                    break;
                case "Food":
                    await RefreshDataGroup(groupName, "Food", channel);
                    break;
                case "Play":
                    await RefreshDataGroup(groupName, "Play", channel);
                    break;
                case "Outside":
                    await RefreshDataGroup(groupName, "Outside", channel);
                    break;
                case "All":
                    await RefreshDataGroup(groupName, "All", channel);
                    break;
                case "Collections":
                    await RefreshDataGroup(groupName, category, channel, "Guides");
                    break;
                default:
                    break;
            }
        }

        private async Task RefreshDataGroup(string groupName, string category, string channel = null, string type = "All Types")
        {
            VisualState = "Normal";
            DataGroup featuredGroup, popularGroup, recentGroup;
            // Add sort groups if explore data group is empty.
            if (_allGroups[groupName].GroupItems.Count == 0)
            {
                featuredGroup = new DataGroup()
                {
                    GroupOrdinal = 1,
                    GroupName = "featured",
                    Layout = DataGroup.LayoutType.SingleFeature
                };

                popularGroup = new DataGroup()
                {
                    GroupOrdinal = 2,
                    GroupName = "popular",
                    Layout = DataGroup.LayoutType.SingleFeature
                };

                recentGroup = new DataGroup()
                {
                    GroupOrdinal = 3,
                    GroupName = "recent",
                    Layout = DataGroup.LayoutType.SingleFeature
                };

                _allGroups[groupName].GroupItems.Add(recentGroup);
                _allGroups[groupName].GroupItems.Add(featuredGroup);
                _allGroups[groupName].GroupItems.Add(popularGroup);
            }

            recentGroup = _allGroups[groupName].GroupItems[0];
            featuredGroup = _allGroups[groupName].GroupItems[1];
            popularGroup = _allGroups[groupName].GroupItems[2];

            var cateOptions = ViewModelLocator.Instance.LandingVM.Categories;
            var sortOptions = new InstructableSortOptions();
            var typeOptions = new InstructableTypeOptions();

            recentGroup.GroupSummaries.Category = category == "All" ? null : cateOptions.Find(category);
            recentGroup.GroupSummaries.Channel = new Channel { title = channel };
            recentGroup.GroupSummaries.Type = typeOptions.Find(type);
            recentGroup.GroupSummaries.Sort = sortOptions.Find("Recent");
            recentGroup.GroupSummaries.NetworkErrorEvent += NetworkError;
            recentGroup.GroupSummaries.Clear();
            recentGroup.GroupSummaries.StartIncrementalLoadEvent += StartIncrementalLoading;
            recentGroup.GroupSummaries.StopIncrementalLoadEvent += StopIncrementalLoading;
            await recentGroup.GroupSummaries.LoadMoreItemsAsync(4);

            featuredGroup.GroupSummaries.Category = category == "All" ? null : cateOptions.Find(category);
            featuredGroup.GroupSummaries.Channel = new Channel { title = channel };
            featuredGroup.GroupSummaries.Type = typeOptions.Find(type);
            featuredGroup.GroupSummaries.Sort = sortOptions.Find("Featured");
            featuredGroup.GroupSummaries.NetworkErrorEvent += NetworkError;
            featuredGroup.GroupSummaries.Clear();
            featuredGroup.GroupSummaries.StartIncrementalLoadEvent += StartIncrementalLoading;
            featuredGroup.GroupSummaries.StopIncrementalLoadEvent += StopIncrementalLoading;
            await featuredGroup.GroupSummaries.LoadMoreItemsAsync(4);

            popularGroup.GroupSummaries.Category = category == "All" ? null : cateOptions.Find(category);
            popularGroup.GroupSummaries.Channel = new Channel { title = channel };
            popularGroup.GroupSummaries.Type = typeOptions.Find(type);
            popularGroup.GroupSummaries.Sort = sortOptions.Find("Popular");
            popularGroup.GroupSummaries.NetworkErrorEvent += NetworkError;
            popularGroup.GroupSummaries.Clear();
            popularGroup.GroupSummaries.StartIncrementalLoadEvent += StartIncrementalLoading;
            popularGroup.GroupSummaries.StopIncrementalLoadEvent += StopIncrementalLoading;
            await popularGroup.GroupSummaries.LoadMoreItemsAsync(4);
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

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }
    }
}
