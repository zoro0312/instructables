using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Instructables.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.Utils;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Instructables.ViewModels
{
    public class ExploreViewModel : BindableBase
    {
        public delegate void DismissPopupHandler(object sender, EventArgs e);

        public event DismissPopupHandler DismissPopup;

        private string _groupName = String.Empty;
        public string GroupName
        {
            get 
            {
                return _groupName;
            }

            set
            {
                _groupName = value;
                OnPropertyChanged();
            }
        }

        private bool _fromUserProfile = false;
        public bool FromUserProfile
        {
            get
            {
                return _fromUserProfile;
            }

            set
            {
                _fromUserProfile = value;
                OnPropertyChanged();
            }
        }

        private InstructableSummaryCollection _summaries;

        public InstructableSummaryCollection Summaries
        {
            get { return _summaries; }
            set
            {
                if (_summaries != value)
                {
                    if (_summaries != null)
                    {
                        _summaries.CollectionChanged -= Summaries_CollectionChanged;
                        _summaries.NetworkErrorEvent -= Summaries_NetworkError;
                    }
                    _summaries = value;
                    OnPropertyChanged();
                    if (_summaries != null)
                    {
                        _summaries.CollectionChanged += Summaries_CollectionChanged;
                        _summaries.NetworkErrorEvent += Summaries_NetworkError;
                    }
                }
            }
        }

        void Summaries_NetworkError(object sender, EventArgs e)
        {
            IsLoading = false;
            VisualState = "Offline";
        }

        void Summaries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_summaries != null && _summaries.Count > 0)
            {
                IsLoading = false;
                VisualState = "Normal";
            }
        }

        private void FireDismissPopup()
        {
            if (DismissPopup != null)
                DismissPopup(this, null);
        }

        private string _visualState = "Normal";

        public string VisualState
        {
            get { return _visualState; }
            set
            {
                if (_visualState != value)
                {
                    _visualState = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private Category _selectedCategory;

        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (value != null && _selectedCategory != value)
                {
                    _selectedCategory = value;
                    _suppressUpdates.Push(true);
                    //SelectedChannel = _selectedCategory.ChannelCollection[0];
                    _suppressUpdates.Pop();
                    //FireDismissPopup();
                    //UpdateData();
                    OnPropertyChanged();
                }
                else if(value == null)
                {
                    _selectedCategory = null;
                    SelectedChannel = null;
                }
            }
        }

        private Category _currentCategory;
        public Category CurrentCategory
        {
            get 
            {
                return _currentCategory; 
            }
            set
            {
                _currentCategory = value;
            }

        }

        private Channel _selectedChannel;

        public Channel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                if (value != null && _selectedChannel != value)
                {
                    _selectedChannel = value;
                    FireDismissPopup();
                    UpdateData();
                    OnPropertyChanged();
                    CurrentCategory = SelectedCategory;
                }
                else if (value == null)
                {
                    _selectedChannel = null;
                    FireDismissPopup();
                    //UpdateData();
                }
            }
        }

        private SortOption _selectedSort;

        public SortOption SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    //FireDismissPopup();
                    UpdateData();
                    OnPropertyChanged();
                }
            }
        }

        private InstructableType _selectedType;
        public InstructableType SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    //FireDismissPopup();
                    UpdateData();
                    OnPropertyChanged();
                }
            }
        }

        private bool _lowerAppBarOpen;

        public bool LowerAppBarIsOpen
        {
            get { return _lowerAppBarOpen; }
            set
            {
                _lowerAppBarOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isLowerAppBarSticky;

        public bool IsLowerAppBarSticky
        {
            get { return _isLowerAppBarSticky; }
            set
            {
                _isLowerAppBarSticky = value;
                OnPropertyChanged();
            }
        }

        private CategoryOptions _categories;
        public CategoryOptions Categories
        {
            get { return _categories; }
        }

        private InstructableTypeOptions _typeOptions = new InstructableTypeOptions();
        public InstructableTypeOptions TypeOptions
        {
            get { return _typeOptions; }
        }

        private readonly InstructableSortOptions _sortOptions = new InstructableSortOptions();

        public InstructableSortOptions SortOptions
        {
            get { return _sortOptions; }
        }


        public void Refresh()
        {
            if (Summaries != null)
            {
                Summaries.Clear();
                Summaries.LoadMoreItemsAsync(20);
            }
        }

        private void UpdateData()
        {
            if (_suppressUpdates.Count > 0)
                return;
            IsLoading = true;
            var collection = new InstructableSummaryCollection { Category = SelectedCategory.CategoryName=="All" ? null : SelectedCategory, Channel = SelectedChannel, Sort = SelectedSort, Type = SelectedType };
            Summaries = collection;
            collection.LoadMoreItemsAsync(20);
        }

        public ExploreViewModel()
        {
            if (DesignMode.IsInDesignMode())
            {
                var svc = InstructablesDataService.DataServiceSingleton;
                _categories = ViewModelLocator.Instance.LandingVM.Channels;
                _suppressUpdates.Push(true);
                SelectedCategory = Categories.Find("Technology");
                SelectedChannel = SelectedCategory.ChannelCollection[0];
                SelectedSort = SortOptions.Find("Recent");
                SelectedType = TypeOptions.Find("All Types");
                _suppressUpdates.Pop();
                var collection = new InstructableSummaryCollection { Category = SelectedCategory };
                var designTimeSummaries = svc.GetSummaries(SelectedCategory.CategoryName, SelectedChannel.title, SelectedSort.Network, SelectedType.Network, 0, 24);
                foreach (var summary in designTimeSummaries.Result.items)
                    collection.Add(summary);
                Summaries = collection;
                IsLoading = false;
            }
            _categories = ViewModelLocator.Instance.LandingVM.Channels;

            //UpdateData();
        }

        private InstructableSummary _selectedInstructable;

        public InstructableSummary SelectedInstructable
        {
            get { return _selectedInstructable; }
            set
            {
                _selectedInstructable = value;
                PinSelectedCommand.IsEnabled = _selectedInstructable != null;
                IsLowerAppBarSticky = _selectedInstructable != null;
                LowerAppBarIsOpen = _selectedInstructable != null;
                RefreshDataCommand.IsEnabled = _selectedInstructable == null;
                DummyCommand.IsEnabled = _selectedInstructable == null;
                OnPropertyChanged();
            }
        }

        private RelayCommand _pinSelectedCommand;

        public RelayCommand PinSelectedCommand
        {
            get
            {
                if (_pinSelectedCommand == null)
                {
                    _pinSelectedCommand = new RelayCommand(ExecutePinCommand);
                }
                return _pinSelectedCommand;
            }
        }

        private async void ExecutePinCommand()
        {
            if (SelectedInstructable != null)
            {
                string filename = string.Format("{0}.png", SelectedInstructable.id);
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync(SelectedInstructable.imageUrl);
                var imageFile =
                    await
                    ApplicationData.Current.LocalFolder.CreateFileAsync(filename,
                                                                        CreationCollisionOption.ReplaceExisting);
                using (var fs = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var outStream = fs.GetOutputStreamAt(0))
                    {
                        DataWriter writer = new DataWriter(outStream);
                        writer.WriteBytes(await response.Content.ReadAsByteArrayAsync());
                        await writer.StoreAsync();
                        writer.DetachStream();
                        await outStream.FlushAsync();
                    }
                }

                // Pin the instructable to the start screen now
                Uri image = new Uri(string.Format("ms-appdata:///local/{0}", filename));

                var tile = new SecondaryTile(
                    SelectedInstructable.id, // Tile ID
                    SelectedInstructable.title, // Tile short name
                    SelectedInstructable.title, // Tile display name
                    SelectedInstructable.id, // Activation argument
                    TileOptions.ShowNameOnWideLogo | TileOptions.CopyOnDeployment, // Tile options
                    image // Tile logo URI
                    );
                tile.BackgroundColor = Color.FromArgb(255, 254, 193, 57);
                tile.ForegroundText = ForegroundText.Light;
                tile.WideLogo = image;
                bool isPinned = await tile.RequestCreateAsync();
                // now create a tile update to make it a live tile
                if (isPinned)
                {
                    var wideTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideSmallImageAndText03);
                    var textNode = wideTemplate.GetElementsByTagName("text");
                    textNode[0].AppendChild(wideTemplate.CreateTextNode(SelectedInstructable.title));
                    var imageNode = wideTemplate.GetElementsByTagName("image");
                    imageNode[0].Attributes[1].InnerText = SelectedInstructable.square3Url;

                    var narrowTemplate =
                        TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquarePeekImageAndText04);
                    var narrowTextNode = narrowTemplate.GetElementsByTagName("text");
                    narrowTextNode[0].AppendChild(narrowTemplate.CreateTextNode(SelectedInstructable.title));
                    var narrowImageNode = narrowTemplate.GetElementsByTagName("image");
                    narrowImageNode[0].Attributes[1].InnerText = SelectedInstructable.square3Url;

                    var narrowNode = wideTemplate.ImportNode(narrowTemplate.GetElementsByTagName("binding").Item(0), true);
                    var xmlNode = wideTemplate.GetElementsByTagName("visual").Item(0);
                    if (xmlNode != null)
                        xmlNode.AppendChild(narrowNode);

                    var tileNotification = new TileNotification(wideTemplate);
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(SelectedInstructable.id).Update(tileNotification);
                }
                SelectedInstructable = null;
            }
        }

        private RelayCommand _refreshCommand;

        public RelayCommand RefreshDataCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(ExecuteRefreshCommand) {IsEnabled = true};
                }
                return _refreshCommand;
            }
        }

        private async void ExecuteRefreshCommand()
        {
            VisualState = "Normal";
            Summaries = null;
            IsLoading = true;
            UpdateData();
        }

        private RelayCommand _dummyCommand;

        public RelayCommand DummyCommand
        {
            get
            {
                if (_dummyCommand == null)
                {
                   _dummyCommand = new RelayCommand(ExecuteDummyCommand) {IsEnabled = true};
                }
                return _dummyCommand;
            }
        }

        private void ExecuteDummyCommand()
        {
            
        }

        private RelayCommand<string> _topNavCommand;

        public RelayCommand<string> TopNavCommand
        {
            get
            {
                if (_topNavCommand == null)
                {
                    _topNavCommand = new RelayCommand<string>(ExecuteTopNavCommand) { IsEnabled = true };
                }
                return _topNavCommand;
            }
        }

        private void ExecuteTopNavCommand(object param)
        {
            string commandParam = param as string;
            if (commandParam != null)
            {
                SetCategory(commandParam);
            }
        }


        private string _viewState;

        internal void UpdateState(string visualState)
        {
            Debug.WriteLine(visualState);
            _viewState = visualState;
        }

        internal void CheckAppBarState()
        {
            // Fix because we can't actually bind to IsOpen on the AppBar and get notified if it is closing. So if the AppBar closes, we will deselect the item in the GridView
            SelectedInstructable = null;
        }

        private Stack<bool> _suppressUpdates = new Stack<bool>();

        internal void SetCategory(string category)
        {
            _suppressUpdates.Push(true);

            // set SelectedType first so we can overwrite it in the case of e-books
            SelectedType = TypeOptions.Find("All Types");
            switch (category)
            {
                case "Recent":
                    SelectedCategory = Categories.Find("All");
                    CurrentCategory = Categories.Find("All");
                    SelectedSort = SortOptions.Find("Recent");
                    break;
                case "Featured":
                    SelectedCategory = Categories.Find("All");
                    CurrentCategory = Categories.Find("All");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Popular":
                    SelectedCategory = Categories.Find("All");
                    CurrentCategory = Categories.Find("All");
                    SelectedSort = SortOptions.Find("Popular");
                    break;
                case "eBooks":
                    SelectedCategory = Categories.Find("All");
                    CurrentCategory = Categories.Find("All");
                    SelectedSort = SortOptions.Find("Featured");
                    SelectedType = TypeOptions.Find("e-Book");
                    break;
                case "Technology":
                    SelectedCategory = Categories.Find("Technology");
                    CurrentCategory = Categories.Find("Technology");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Workshop":
                    SelectedCategory = Categories.Find("Workshop");
                    CurrentCategory = Categories.Find("Workshop");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Living":
                    SelectedCategory = Categories.Find("Living");
                    CurrentCategory = Categories.Find("Living");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Food":
                    SelectedCategory = Categories.Find("Food");
                    CurrentCategory = Categories.Find("Food");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Play":
                    SelectedCategory = Categories.Find("Play");
                    CurrentCategory = Categories.Find("Play");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                case "Outside":
                    SelectedCategory = Categories.Find("Outside");
                    CurrentCategory = Categories.Find("Outside");
                    SelectedSort = SortOptions.Find("Featured");
                    break;
                default:
                    throw new Exception("Unknown Category in SetCategory");
            }
            //SelectedChannel = SelectedCategory.ChannelCollection[0];
            var collection = new InstructableSummaryCollection { Category = SelectedCategory.CategoryName != "All"?SelectedCategory:null, Channel = null, Sort = SelectedSort, Type = SelectedType };
            Summaries = collection;
            Summaries.StartIncrementalLoadEvent += StartIncrementalLoading;
            Summaries.StopIncrementalLoadEvent += StopIncrementalLoading;
            Summaries.LoadMoreItemsAsync(20);
            var landingVM = ViewModelLocator.Instance.LandingVM;
            if (landingVM.Followings != null)
            {
                landingVM.Followings.StartIncrementalLoadEvent += StartIncrementalLoading;
                landingVM.Followings.StopIncrementalLoadEvent += StopIncrementalLoading;
            }
            var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
            if (userProfileVM.DraftInstructables != null)
            {
                userProfileVM.DraftInstructables.StartIncrementalLoadEvent += StartIncrementalLoading;
                userProfileVM.DraftInstructables.StopIncrementalLoadEvent += StopIncrementalLoading;
            }

            if (userProfileVM.PublishedInstrutables != null)
            {
                userProfileVM.PublishedInstrutables.StartIncrementalLoadEvent += StartIncrementalLoading;
                userProfileVM.PublishedInstrutables.StartIncrementalLoadEvent += StartIncrementalLoading; 
            }

            if (userProfileVM.FavoriteInstructables != null)
            {
                userProfileVM.FavoriteInstructables.StartIncrementalLoadEvent += StartIncrementalLoading;
                userProfileVM.FavoriteInstructables.StartIncrementalLoadEvent += StartIncrementalLoading; 
            }
            _suppressUpdates.Pop();
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = true;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = false;
        }
    }
}
