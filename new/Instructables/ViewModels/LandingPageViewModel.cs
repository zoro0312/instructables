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
    public class LandingPageViewModel : BindableBase
    {

        /*private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }*/

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
                return (int)(fitWidth / 4);
            }
        }
        private static bool _IsFavorites;
        public bool IsFavorites { get; set; }

        private ObservableCollection<DataGroup> _featuredGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> FeaturedGroups
        {
            get { return this._featuredGroups; }
        }

        private ObservableCollection<DataGroup> _recentGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> RecentGroups
        {
            get { return this._recentGroups; }
        }

        private ObservableCollection<DataGroup> _popularGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> PopularGroups
        {
            get { return this._popularGroups; }
        }

        private ObservableCollection<DataGroup> _followedGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> FollowedGroups
        {
            get { return this._followedGroups; }
        }

        private ObservableCollection<DataGroup> _contestGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> ContestGroups
        {
            get { return this._contestGroups; }
        }

        private InstructableCollection _followings;
        public InstructableCollection Followings
        {
            get { return this._followings; }
            private set { this.SetProperty(ref this._followings, value); }
        }

        private UserProfile _userProfile = new UserProfile();
        public UserProfile userProfile
        {
            get { return this._userProfile; }
            private set { this.SetProperty(ref this._userProfile, value); }
        }

        private bool _isLogin = false;
        public bool IsLogin
        {
            get { return this._isLogin; }
            private set { this.SetProperty(ref this._isLogin, value); }
        }

        private Visibility _noFollowingVisible;
        public Visibility NoFollowingVisible
        {
            get
            {
                if (VisualState == "Offline" || _followedList == null || _followedList.fullListSize > 0)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            set
            {
                _noFollowingVisible = value;
                OnPropertyChanged();
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


        public LandingPageViewModel()
        {
            /*if (DesignMode.IsInDesignMode())
            {
                InitializeDesignData();
            }*/
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
                OnPropertyChanged();
            }
        }

        private CategoryOptions _categoryOptions;

        public CategoryOptions Channels
        {
            get { return _categoryOptions; }
            set { _categoryOptions = value; }
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
                    _refreshCommand = new RelayCommand(ExecuteRefreshCommand);
                    _refreshCommand.IsEnabled = true;
                }
                return _refreshCommand;
            }
        }

        private RelayCommand _retryCommand;

        public RelayCommand RetryDataCommand
        {
            get
            {
                if (_retryCommand == null)
                {
                    _retryCommand = new RelayCommand(ExecuteRefreshCommand);
                    _retryCommand.IsEnabled = true;
                }
                return _retryCommand;
            }
        }


        private async void ExecuteRefreshCommand()
        {
            VisualState = "Normal";
            IsLoading = true;
            FeaturedGroups.Clear();
            RecentGroups.Clear();
            PopularGroups.Clear();
            //await Task.Run(new Action(Initialize));
            await Initialize();
        }

        private InstructableCategoryList _featuredList = null;
        private InstructableCategoryList _recentList = null;
        private InstructableCategoryList _popularList = null;
        private InstructableList _followedList = null;
        private InstructableContestList _contestList = null;
        //private InstructableCategoryList _eBooksList;
        //private InstructableCategoryList _technologyList;
        //private InstructableCategoryList _workshopList;
        //private InstructableCategoryList _livingList;
        //private InstructableCategoryList _foodList;
        //private InstructableCategoryList _playList;
        //private InstructableCategoryList _outsideList;

        private async Task FetchData()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(UpdateUserProfile());
            await Task.WhenAll(tasks);
            var dataService = InstructablesDataService.DataServiceSingleton;
            if (Channels == null)
            {
                Channels = await PrepareChannels();
            }
            // Only reason for Channels to be null is that we can't connect to the service, so let's check why and set our state to "Offline"
            VisualState = Channels == null ? "Offline" : "Normal";

            if (Channels != null)
            {
                _featuredList = await dataService.GetSummaries(null, null, "featured");
                if (_featuredList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _recentList = await dataService.GetSummaries(null, null, "recent");
                if (_recentList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _popularList = await dataService.GetSummaries(null, null, "popular");
                if (_popularList == null)
                {
                    VisualState = "Offline";
                    return;
                }

                /*var result = await dataService.Login("utest_888","abcd1234");
                if (result.isSucceeded)
                {*/
                    if (SessionManager.GetLoginUserName() != string.Empty)
                    {
                        _followedList = await dataService.GetFollowedInstructable();
                        if (_followedList == null)
                        {
                            VisualState = "Offline";
                            return;
                        }
                    }
                /*}*/
                /*else
                {
                    var Text = result.error.toString();
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", Text));
                }*/

              _contestList = await dataService.GetContests();
              if (_contestList == null)
              {
                  VisualState = "Offline";
                  return;
              }
                
                /*_eBooksList = await dataService.GeteBooks();
                if (_eBooksList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _technologyList = await dataService.GetSummaries("technology");
                if (_technologyList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _workshopList = await dataService.GetSummaries("workshop");
                if (_workshopList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _livingList = await dataService.GetSummaries("living");
                if (_livingList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _foodList = await dataService.GetSummaries("food");
                if (_foodList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _playList = await dataService.GetSummaries("play");
                if (_playList == null)
                {
                    VisualState = "Offline";
                    return;
                }
                _outsideList = await dataService.GetSummaries("outside");
                if (_outsideList == null)
                {
                    VisualState = "Offline";
                    return;
                }// if any of these are null we are likely offline*/
                VisualState = "Normal";

            }
            else
            {
                VisualState = "Offline";
            }
        }

        private async Task<CategoryOptions> PrepareChannels()
        {
            CategoryOptions categoryOptions = null;
            var dataService = InstructablesDataService.DataServiceSingleton;
            var rawChannels = await dataService.GetChannels();
            if (rawChannels != null)
            {
                categoryOptions = new CategoryOptions();
                var allChannels = CombinedChannels(rawChannels);
                allChannels.Insert(0, new Channel() { CategoryName = "All Channels", title = "All Channels", display = "All Channels" });

                categoryOptions.Add(new Category() { CategoryName = "All", ChannelCollection = allChannels });

                foreach (var channelEntry in rawChannels.food)
                    channelEntry.CategoryName = "Food";
                rawChannels.food.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Food", ChannelCollection = rawChannels.food });

                foreach (var channelEntry in rawChannels.living)
                    channelEntry.CategoryName = "Living";
                rawChannels.living.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Living", ChannelCollection = rawChannels.living });

                foreach (var channelEntry in rawChannels.outside)
                    channelEntry.CategoryName = "Outside";
                rawChannels.outside.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Outside", ChannelCollection = rawChannels.outside });

                foreach (var channelEntry in rawChannels.play)
                    channelEntry.CategoryName = "Play";
                rawChannels.play.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Play", ChannelCollection = rawChannels.play });

                foreach (var channelEntry in rawChannels.technology)
                    channelEntry.CategoryName = "Technology";
                rawChannels.technology.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Technology", ChannelCollection = rawChannels.technology });

                foreach (var channelEntry in rawChannels.workshop)
                    channelEntry.CategoryName = "Workshop";
                rawChannels.workshop.Insert(0, new Channel() { CategoryName = "All Channels", display = "All Channels" });
                categoryOptions.Add(new Category() { CategoryName = "Workshop", ChannelCollection = rawChannels.workshop });
            }
            return categoryOptions;
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

        public async Task Initialize()
        {
            _dataLoaded = false;
            VisualState = "Normal";
            Followings = new InstructableCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    InstructableList instructableList = await InstructablesDataService.DataServiceSingleton.GetFollowedInstructable(offset, count);
                    if (instructableList != null && instructableList.instructables != null)
                    {
                        return instructableList.instructables;
                    }
                    return null;
                },
            };

            await FetchData();
            NoFollowingVisible = Visibility.Collapsed;
            _dataLoaded = true;
            RefreshCollections();
            IsLoading = false;
        }

        private bool _dataLoaded = false;

        /*private async void InitializeDesignData()
        {
            IsLoading = false;
            CurrentScreenMetrics = (from s in ScreenMetrics
                                    where s.Name == "768"
                                    select s).FirstOrDefault();

            var dataService = InstructablesDataService.DataServiceSingleton;
            int ColumnHeight = 3;
            var recentGroup = new DataGroup()
            {
                GroupOrdinal = 0,
                GroupName = "Recent",
                Layout = DataGroup.LayoutType.MainFeature
            };
            var recent = await dataService.GetSummaries(null, null, "recent", count: ColumnHeight + 1);
            int count = 0;
            foreach (var item in recent.items)
            {
                item.GroupOrdinal = count++;
                item.Group = recentGroup;
                recentGroup.GroupItems.Add(item);
                if (count > ColumnHeight)
                    break;
            }
            AllGroups.Add(recentGroup);

            var featuredGroup = new DataGroup()
            {
                GroupOrdinal = 1,
                GroupName = "Featured",
                Layout = DataGroup.LayoutType.DualFeature
            };
            var featured = await dataService.GetSummaries(null, null, "featured", count: ColumnHeight + 2);
            count = 0;
            foreach (var item in featured.items)
            {
                item.GroupOrdinal = count++;
                item.Group = featuredGroup;
                featuredGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            AllGroups.Add(featuredGroup);

            var popularGroup = new DataGroup()
            {
                GroupOrdinal = 2,
                GroupName = "Popular",
                Layout = DataGroup.LayoutType.DualFeature
            };
            var popular = await dataService.GetSummaries(null, null, "popular", count: ColumnHeight + 2);
            count = 0;
            foreach (var item in popular.items)
            {
                item.GroupOrdinal = count++;
                item.Group = popularGroup;
                popularGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            AllGroups.Add(popularGroup);

            var eBooksGroup = new DataGroup()
            {
                GroupOrdinal = 3,
                GroupName = "eBooks",
                Layout = DataGroup.LayoutType.EBook
            };
            var ebooks = await dataService.GeteBooks(limit: ColumnHeight + 3);
            count = 0;
            foreach (var item in ebooks.items)
            {
                item.GroupOrdinal = count++;
                item.Group = eBooksGroup;
                eBooksGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            AllGroups.Add(eBooksGroup);

            var technologyGroup = new DataGroup()
            {
                GroupOrdinal = 4,
                GroupName = "Technology",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            var technology = await dataService.GetSummaries("technology", count: ColumnHeight + 3);
            count = 0;
            foreach (var item in technology.items)
            {
                item.GroupOrdinal = count++;
                item.Group = technologyGroup;
                technologyGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            AllGroups.Add(technologyGroup);

            var workshopGroup = new DataGroup()
            {
                GroupOrdinal = 5,
                GroupName = "Workshop",
                Layout = DataGroup.LayoutType.DualFeature
            };
            var workshop = await dataService.GetSummaries("workshop", count: ColumnHeight + 2);
            count = 0;
            foreach (var item in workshop.items)
            {
                item.GroupOrdinal = count++;
                item.Group = workshopGroup;
                workshopGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            AllGroups.Add(workshopGroup);

            var livingGroup = new DataGroup()
            {
                GroupOrdinal = 6,
                GroupName = "Living",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            var living = await dataService.GetSummaries("living", count: ColumnHeight + 3);
            count = 0;
            foreach (var item in living.items)
            {
                item.GroupOrdinal = count++;
                item.Group = livingGroup;
                livingGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            AllGroups.Add(livingGroup);

            var foodGroup = new DataGroup()
            {
                GroupOrdinal = 7,
                GroupName = "Food",
                Layout = DataGroup.LayoutType.DualFeature
            };
            var food = await dataService.GetSummaries("food", count: ColumnHeight + 2);
            count = 0;
            foreach (var item in food.items)
            {
                item.GroupOrdinal = count++;
                item.Group = foodGroup;
                foodGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            AllGroups.Add(foodGroup);

            var playGroup = new DataGroup()
            {
                GroupOrdinal = 8,
                GroupName = "Play",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            var play = await dataService.GetSummaries("play", count: ColumnHeight + 3);
            count = 0;
            foreach (var item in play.items)
            {
                item.GroupOrdinal = count++;
                item.Group = playGroup;
                playGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            AllGroups.Add(playGroup);

            var outsideGroup = new DataGroup()
            {
                GroupOrdinal = 9,
                GroupName = "Outside",
                Layout = DataGroup.LayoutType.DualFeature
            };
            var outside = await dataService.GetSummaries("outside", count: ColumnHeight + 2);
            count = 0;
            foreach (var item in outside.items)
            {
                item.GroupOrdinal = count++;
                item.Group = outsideGroup;
                outsideGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            AllGroups.Add(outsideGroup);
            Channels = await PrepareChannels();
        }*/

        private List<ScreenItems> ScreenMetrics = new List<ScreenItems>()
            { 
                new ScreenItems(){Name = "Portrait768", HeroItemCount = 4, EBookItemCount = 8, DualFeatureCount = 8, SingleFeatureCount = 9},
                new ScreenItems(){Name = "Portrait1080", HeroItemCount = 7, EBookItemCount = 12, DualFeatureCount = 11, SingleFeatureCount = 9},
                new ScreenItems(){Name = "Portrait1440", HeroItemCount = 13, EBookItemCount = 16, DualFeatureCount = 14, SingleFeatureCount = 15},
                new ScreenItems(){Name = "768", HeroItemCount = 7, EBookItemCount = 6, DualFeatureCount = 8, SingleFeatureCount = 6},
                new ScreenItems(){Name = "1080", HeroItemCount = 9, EBookItemCount = 9, DualFeatureCount = 12, SingleFeatureCount = 9},
                new ScreenItems(){Name = "1440", HeroItemCount = 14, EBookItemCount = 12, DualFeatureCount = 16, SingleFeatureCount = 12},
                new ScreenItems(){Name = "Snapped", HeroItemCount = 6, EBookItemCount = 6, DualFeatureCount = 8, SingleFeatureCount = 6}
            };

        public static ScreenItems CurrentScreenMetrics = null;

        public void RefreshCollections()
        {
            ScreenItems _counts = null;

            if (_viewState == "FullScreenPortrait")
            {
                if (ScreenHeight <= 858)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "768"
                               select s).FirstOrDefault();
                }
                else if (ScreenHeight > 858 && ScreenHeight < 1201)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "1080"
                               select s).FirstOrDefault();
                }
                else if (ScreenHeight > 1201)
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "1440"
                               select s).FirstOrDefault();
                }
                else
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "768"
                               select s).FirstOrDefault();
                }
                //if (ScreenHeight < 1400)
                //{
                //    _counts = (from s in ScreenMetrics
                //               where s.Name == "Portrait768"
                //               select s).FirstOrDefault();
                //}
                //else if (ScreenHeight < 2000)
                //{
                //    _counts = (from s in ScreenMetrics
                //               where s.Name == "Portrait1080"
                //               select s).FirstOrDefault();

                //}
                //else if (ScreenHeight >= 2000)
                //{
                //    _counts = (from s in ScreenMetrics
                //               where s.Name == "Portrait1440"
                //               select s).FirstOrDefault();

                //}
                //else
                //{
                //    _counts = (from s in ScreenMetrics
                //               where s.Name == "Portrait768"
                //               select s).FirstOrDefault();
                //}

            }
            else
            {
                if (_viewState != "Snapped")
                {
                    if (ScreenHeight <= 858)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "768"
                                   select s).FirstOrDefault();
                    }
                    else if (ScreenHeight > 858 && ScreenHeight < 1201)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "1080"
                                   select s).FirstOrDefault();
                    }
                    else if (ScreenHeight > 1201)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "1440"
                                   select s).FirstOrDefault();
                    }
                    else
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "768"
                                   select s).FirstOrDefault();
                    }
                }
                else
                {
                    _counts = (from s in ScreenMetrics
                               where s.Name == "Snapped"
                               select s).FirstOrDefault();
                }

            }


            if (_counts == null)
                _counts = new ScreenItems() { DualFeatureCount = 5, SingleFeatureCount = 6, EBookItemCount = 6, HeroItemCount = 4 };

            CurrentScreenMetrics = _counts;

            FeaturedGroups.Clear();
            RecentGroups.Clear();
            PopularGroups.Clear();
            FollowedGroups.Clear();
            ContestGroups.Clear();

            if (_recentList == null || _recentList.items.Count == 0)
                return;

            var featuredGroup = new DataGroup()
            {
                GroupOrdinal = 1,
                GroupName = "Featured",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_featuredList, featuredGroup, _counts.DualFeatureCount);
            //            MoveListItems(_featuredList, featuredGroup);
            featuredGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["FeaturedBrush"];
            FeaturedGroups.Add(featuredGroup);

            var recentGroup = new DataGroup()
            {
                GroupOrdinal = 0,
                GroupName = "Recent",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_recentList, recentGroup, _counts.DualFeatureCount);
            //            MoveListItems(_recentList, recentGroup);
            recentGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["RecentBrush"];
            RecentGroups.Add(recentGroup);

            var popularGroup = new DataGroup()
            {
                GroupOrdinal = 2,
                GroupName = "Popular",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_popularList, popularGroup, _counts.DualFeatureCount);
            //            MoveListItems(_popularList, popularGroup);
            popularGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["PopularBrush"];
            PopularGroups.Add(popularGroup);
            
            if(_followedList != null)
            {
                var followedGroup = new DataGroup()
                {
                    GroupOrdinal = 3,
                    GroupName = "Followed",
                    Layout = DataGroup.LayoutType.DualFeature
                };
                MoveListItems(_followedList, followedGroup, _counts.DualFeatureCount);
                //            MoveListItems(_popularList, popularGroup);
                followedGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["FollowedBrush"];
                FollowedGroups.Add(followedGroup);
            }

            if (_contestList != null)
            {
                var contestGroup = new DataGroup()
                {
                    GroupOrdinal = 4,
                    GroupName = "Contest",
                    Layout = DataGroup.LayoutType.DualFeature
                };
                MoveListItems(_contestList, contestGroup, _counts.DualFeatureCount);
                //            MoveListItems(_popularList, popularGroup);
                contestGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["FollowedBrush"];
                ContestGroups.Add(contestGroup);
            }
            
            /*var eBooksGroup = new DataGroup()
            {
                GroupOrdinal = 3,
                GroupName = "eBooks",
                Layout = DataGroup.LayoutType.EBook
            };
            MoveListItems(_eBooksList, eBooksGroup, _counts.EBookItemCount);
            //            MoveListItems(_eBooksList, eBooksGroup);
            eBooksGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["CollectionsBrush"];
            AllGroups.Add(eBooksGroup);

            var technologyGroup = new DataGroup()
            {
                GroupOrdinal = 4,
                GroupName = "Technology",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            MoveListItems(_technologyList, technologyGroup, _counts.SingleFeatureCount);
            //            MoveListItems(_technologyList, technologyGroup);
            technologyGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["TechnologyBrush"];
            AllGroups.Add(technologyGroup);

            var workshopGroup = new DataGroup()
            {
                GroupOrdinal = 5,
                GroupName = "Workshop",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_workshopList, workshopGroup, _counts.DualFeatureCount);
            //            MoveListItems(_workshopList, workshopGroup);
            workshopGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["WorkshopBrush"];
            AllGroups.Add(workshopGroup);

            var livingGroup = new DataGroup()
            {
                GroupOrdinal = 6,
                GroupName = "Living",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            MoveListItems(_livingList, livingGroup, _counts.SingleFeatureCount);
            //            MoveListItems(_livingList, livingGroup);
            livingGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["LivingBrush"];
            AllGroups.Add(livingGroup);

            var foodGroup = new DataGroup()
            {
                GroupOrdinal = 7,
                GroupName = "Food",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_foodList, foodGroup, _counts.DualFeatureCount);
            //            MoveListItems(_foodList, foodGroup);
            foodGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["FoodBrush"];
            AllGroups.Add(foodGroup);

            var playGroup = new DataGroup()
            {
                GroupOrdinal = 8,
                GroupName = "Play",
                Layout = DataGroup.LayoutType.SingleFeature
            };
            MoveListItems(_playList, playGroup, _counts.SingleFeatureCount);
            //            MoveListItems(_playList, playGroup);
            playGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["PlayBrush"];
            AllGroups.Add(playGroup);

            var outsideGroup = new DataGroup()
            {
                GroupOrdinal = 9,
                GroupName = "Outside",
                Layout = DataGroup.LayoutType.DualFeature
            };
            MoveListItems(_outsideList, outsideGroup, _counts.DualFeatureCount);
            //            MoveListItems(_outsideList, outsideGroup);
            outsideGroup.GroupBrush = (SolidColorBrush)Application.Current.Resources["OutsideBrush"];
            AllGroups.Add(outsideGroup);*/
        }

        //private void MoveListItems(InstructableCategoryList sourceList, DataGroup targetGroup)
        //{
        //    int i = 0;
        //    foreach (var item in sourceList.items)
        //    {
        //        item.GroupOrdinal = i++;
        //        item.Group = targetGroup;
        //        targetGroup.GroupItems.Add(item);
        //    }
        //}


        private void MoveListItems(InstructableCategoryList sourceList, DataGroup targetGroup, int count)
        {
            try 
            { 
                if (sourceList.items.Count < count)
                count = sourceList.items.Count;
                for (int i = 0; i < count; i++)
                {
                    sourceList.items[i].GroupOrdinal = i;
                    sourceList.items[i].Group = targetGroup;
                    targetGroup.GroupItems.Add(sourceList.items[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }
            
        }

        private void MoveListItems(InstructableList sourceList, DataGroup targetGroup, int count)
        {
            try
            {
                if (sourceList.instructables.Count < count)
                    count = sourceList.instructables.Count;
                for (int i = 0; i < count; i++)
                {
                    sourceList.instructables[i].GroupOrdinal = i;
                    sourceList.instructables[i].Group = targetGroup;
                    targetGroup.GroupDetailItems.Add(sourceList.instructables[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }

        }


        private void MoveListItems(InstructableContestList sourceList, DataGroup targetGroup, int count)
        {
            try 
            {
                ObservableCollection<Contest> OpenContests = new ObservableCollection<Contest>();
                foreach (var contest in sourceList.contests)
                {
                    if (contest.state == "open")
                    {
                        OpenContests.Add(contest);
                    } 
                }

                if (OpenContests.Count < count)
                {
                    count = OpenContests.Count;
                }

                for (int i = 0; i < count; ++i)
                {
                    OpenContests[i].GroupOrdinal = i;
                    OpenContests[i].Group = targetGroup;
                    targetGroup.GroupContestItems.Add(OpenContests[i]);
                }


                //if (sourceList.contests.Count < count)
                //    count = sourceList.contests.Count;
                //for (int i = 0; i < count; i++)
                //{
                //    sourceList.contests[i].GroupOrdinal = i;
                //    sourceList.contests[i].Group = targetGroup;
                //    targetGroup.GroupContestItems.Add(sourceList.contests[i]);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }
        }

        private double _screenHeight;

        public double ScreenHeight
        {
            get { return _screenHeight; }
            set
            {
                if (_screenHeight != value)
                {
                    _screenHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _viewState;
        private string _previousState;


        internal void UpdateState(string visualState)
        {
            Debug.WriteLine(visualState);
            _viewState = visualState;
            if (_dataLoaded && _previousState != _viewState)
            {
                _previousState = _viewState;
                RefreshCollections();
            }
        }

        internal void CheckAppBarState()
        {
            // Fix because we can't actually bind to IsOpen on the AppBar and get notified if it is closing. So if the AppBar closes, we will deselect the item in the GridView
            SelectedInstructable = null;
        }

        public async Task UpdateUserProfile()
        {
            var loginUserName = SessionManager.GetLoginUserName();
            IsLogin = loginUserName != null && loginUserName != string.Empty;

            userProfile = new UserProfile();
            if(Followings != null)
                Followings.Clear();

            if (IsLogin)
            {
                IsLoading = true;
                var dataService = InstructablesDataService.DataServiceSingleton;
                var newUp = await dataService.GetUserProfile(loginUserName);
                if (newUp != null)
                {
                    userProfile = newUp;
                    await Followings.LoadMoreItemsAsync(4);
                }
                IsLoading = false;
            }
        }
    }

    public class ScreenItems
    {
        public string Name { get; set; }
        public int HeroItemCount { get; set; }
        public int EBookItemCount { get; set; }
        public int SingleFeatureCount { get; set; }
        public int DualFeatureCount { get; set; }
    }

}
