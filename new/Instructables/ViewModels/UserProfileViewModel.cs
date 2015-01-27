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
using Instructables.Converters;

namespace Instructables.ViewModels
{
    public class UserProfileViewModel : BindableBase
    {

        public double FitHight
        {
            get {
                return ScreenHeight * 0.79;
            }
        }
        private string _screenName;
        public string userName
        {
            get { return this._screenName; }
            set
            {
                _screenName = value;
                OnPropertyChanged();
            }
        }

        public string titleUserName
        {
            get
            {
                if (_isLoginUser)
                    return "Profile"; //return "My Profile";
                else
                    return this._screenName;
            }
            set
            {
                _screenName = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditPage;
        public bool isEditPage
        {
            get { return this._isEditPage; }
            set
            {
                _isEditPage = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoginUser;
        public bool isLoginUser 
        {
            get { return this._isLoginUser; }
            set
            {
                _isLoginUser = value;
                OnPropertyChanged();
            }
        }

        private string _tinyUrl;
        public string userPicture
        {
            get { return this._tinyUrl; }
            set
            {
                _tinyUrl = value;
                OnPropertyChanged();
            }
        }

        private string _about;
        public string userAbout
        {
            get 
            {
                if (_about == null || _about == string.Empty)
                    return "[No about information.]";
                else
                    return this._about; 
            }
            set
            {
                _about = value;
                OnPropertyChanged();
            }
        }

        private int _instructablesCount;
        public int instructablesCount
        {
            get { return this._instructablesCount; }
            set
            {
                _instructablesCount = value;
                OnPropertyChanged();
            }
        }

        public bool noUserInstructables
        {
            get
            {
                if (!this._isLoginUser && this._instructablesCount == 0)
                    return true;
                return false;
            }
        }

        private int _views;
        public int viewCount
        {
            get { return this._views; }
            set
            {
                _views = value;
                OnPropertyChanged();
            }
        }

        private int _followersCount;
        public int followersCount
        {
            get { return this._followersCount; }
            set
            {
                _followersCount = value;
                OnPropertyChanged();
            }
        }

        private int _draftCount = 0;
        public int draftcount
        {
            get { return this._draftCount; }
            set
            {
                _draftCount = value;
                OnPropertyChanged();
            }
        }

        private int _publishCount = 0;
        public int publishCount
        {
            get { return this._publishCount; }
            set
            {
                _publishCount = value;
                OnPropertyChanged();
            }
        }

        private int _favoritesCount = 0;
        public int favoritesCount
        {
            get { return this._favoritesCount; }
            set
            {
                _favoritesCount = value;
                OnPropertyChanged();
            }
        }

        private int _subscriptionsCount = 0;
        public int subscriptionsCount
        {
            get { return this._subscriptionsCount; }
            set
            {
                _subscriptionsCount = value;
                OnPropertyChanged();
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

        private UserProfile _userProfile;
        public UserProfile userProfile
        {
            get { return this._userProfile; }
        }

        private InstructableCollection _draftInstructables;
        public InstructableCollection DraftInstructables
        {
            get
            {
                return this._draftInstructables;
            }
            set
            {
                this._draftInstructables = value;
                OnPropertyChanged();
            }
        }

        private InstructableCollection _publishedInstrutables;
        public InstructableCollection PublishedInstrutables
        {
            get
            {
                return this._publishedInstrutables;
            }
            set
            {
                this._publishedInstrutables = value;
                OnPropertyChanged();
            }
        }

        private InstructableCollection _favoriteInstructables;
        public InstructableCollection FavoriteInstructables
        {
                    get
            {
                return this._favoriteInstructables;
            }
            set
            {
                this._favoriteInstructables = value;
                OnPropertyChanged();
            }
        }

        private AuthorCollection _followers;
        public AuthorCollection Followers
        {
            get
            {
                return this._followers;
            }
            set
            {
                this._followers = value;
                OnPropertyChanged();
            }
        }

        private AuthorCollection _followings;
        public AuthorCollection Followings
        {
            get
            {
                return this._followings;
            }
            set
            {
                this._followings = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DataGroup> _draftGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> DraftGroups
        {
            get { return this._draftGroups; }
        }

        private ObservableCollection<DataGroup> _publishedGroups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> PublishedGroups
        {
            get { return this._publishedGroups; }
        }

        public string FollowText
        {
            get
            {
                ScreenNameIsFavoritedConverter converter = new ScreenNameIsFavoritedConverter();
                bool isFollowing = (bool)converter.Convert(_screenName, typeof(bool), null, string.Empty);
                if (isFollowing)
                    return "Following";
                return "Follow";
            }
        }

        public bool IsFollowAuthor
        {
            get 
            {
                var profile = ViewModelLocator.Instance.LandingVM.userProfile;

                if (profile != null)
                {
                    ObservableCollection<Author> subscriptions = profile.subscriptions;
                    if (subscriptions != null)
                    {
                        foreach (var author in subscriptions)
                        {
                            if (author.screenName == _screenName)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool isLogin
        {
            get 
            {
                return InstructablesDataService.DataServiceSingleton.isLogin();
            }
        }

        public double ScreenHeight { get; set; }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = true;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = false;
        }

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }

        public async Task Initialize(string screenName)
        {
            IsLoading = true;
            if (screenName != _screenName)
            {
                ResetUserProfile();
            }
            var dataService = InstructablesDataService.DataServiceSingleton;
            var loginUserProfile = ViewModelLocator.Instance.LandingVM.userProfile;
            //var createVM = ViewModelLocator.Instance.CreateVM;

            VisualState = "Normal";
            _userProfile = await dataService.GetUserProfile(screenName);
            if (_userProfile == null)
                VisualState = "Offline";

            if (loginUserProfile != null && loginUserProfile.screenName == screenName)
            {
                _userProfile = loginUserProfile;
                isLoginUser = true;

                DraftInstructables = new InstructableCollection
                {
                    loadMoreItemsAsync = async delegate(int offset, int count)
                    {
                        InstructableList instructableList = await dataService.GetDrafts(screenName, offset, count);
                        if (instructableList != null && instructableList.instructables != null)
                        {
                            //_draftCount = instructableList.fullListSize;
                            return instructableList.instructables;
                        }
                        return null;
                    }
                };
                DraftInstructables.Clear();
                await DraftInstructables.LoadMoreItemsAsync(8);

                dataService.LogoutSucceed -= HandleLogoutSucceed;
                dataService.LogoutSucceed += HandleLogoutSucceed;

            } else {
                isLoginUser = false;
            }
            
            PublishedInstrutables = new InstructableCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    InstructableList instructableList = await dataService.GetInstructablesByAuthor(screenName, offset, count);
                    if (instructableList != null && instructableList.instructables != null)
                    {
                        //_publishCount = instructableList.fullListSize;
                        return instructableList.instructables;
                    }
                    return null;
                }
            };

            PublishedInstrutables.Clear();
            await PublishedInstrutables.LoadMoreItemsAsync(15);

            FavoriteInstructables = new InstructableCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    InstructableFavoriteList instructableList = await dataService.GetFavorites(screenName, offset, count);
                    if (instructableList != null && instructableList.favorites != null)
                    {
                        return instructableList.favorites;
                    }
                    return null;
                }
            };

            FavoriteInstructables.Clear();
            await FavoriteInstructables.LoadMoreItemsAsync(15);

            Followers = new AuthorCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    FollowersList authorList = await dataService.GetFollowers(screenName, offset, count);
                    if (authorList != null && authorList.followers != null)
                    {
                        return authorList.followers;
                    }
                    return null;
                }
            };
            Followers.Clear();
            Followers.StartIncrementalLoadEvent += StartIncrementalLoading;
            Followers.StopIncrementalLoadEvent += StopIncrementalLoading;
            await Followers.LoadMoreItemsAsync(8);

            Followings = new AuthorCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    SubscriptionsList authorList = await dataService.GetFollowings(screenName, offset, count);
                    if (authorList != null && authorList.subscriptions != null)
                    {
                        return authorList.subscriptions;
                    }
                    return null;
                }
            };
            Followings.Clear();
            Followers.StartIncrementalLoadEvent += StartIncrementalLoading;
            Followers.StopIncrementalLoadEvent += StopIncrementalLoading;
            await Followings.LoadMoreItemsAsync(8);

            RefreshCollections();

            IsLoading = false;
        }

        private async void HandleLogoutSucceed(object sender, EventArgs e)
        {
            await Task.Delay(1000);
            ExecuteRefreshCommand();
        }

        private void RefreshCollections()
        {
            if (_userProfile != null)
            {
                userName = _userProfile.screenName;
                titleUserName = _userProfile.screenName;
                userPicture = _userProfile.square3Url;
                instructablesCount = _userProfile.instructablesCount;
                viewCount = _userProfile.views;
                followersCount = _userProfile.followersCount;
                userAbout = _userProfile.about;
                draftcount = _userProfile.draftsCount;
                publishCount = _userProfile.instructablesCount;
                favoritesCount = _userProfile.favoritesCount;
                subscriptionsCount = _userProfile.subscriptionsCount;

                ScreenItems _counts = null;

                if (_viewState == "FullScreenPortrait")
                {
                    if (ScreenHeight < 1400)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "Portrait768"
                                   select s).FirstOrDefault();
                    }
                    else if (ScreenHeight < 2000)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "Portrait1080"
                                   select s).FirstOrDefault();

                    }
                    else if (ScreenHeight >= 2000)
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "Portrait1440"
                                   select s).FirstOrDefault();

                    }
                    else
                    {
                        _counts = (from s in ScreenMetrics
                                   where s.Name == "Portrait768"
                                   select s).FirstOrDefault();
                    }

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
                    _counts = new ScreenItems() { DualFeatureCount = 5};

                CurrentScreenMetrics = _counts;

                DraftGroups.Clear();
                var draftGroup = new DataGroup()
                {
                    GroupOrdinal = 0,
                    GroupName = "Draft",
                    Layout = DataGroup.LayoutType.DualFeature
                };

                MoveListItems(DraftInstructables, draftGroup, _counts.DualFeatureCount);                
                DraftGroups.Add(draftGroup);

                PublishedGroups.Clear();
                var publishedGroup = new DataGroup()
                {
                    GroupOrdinal = 0,
                    GroupName = "Published",
                    Layout = DataGroup.LayoutType.DualFeature
                };

                MoveListItems(PublishedInstrutables, publishedGroup, _counts.DualFeatureCount);
                PublishedGroups.Add(publishedGroup);

            }
        }

        private void MoveListItems(InstructableCollection sourceList, DataGroup targetGroup, int count)
        {
            try
            {
                if (sourceList.Count < count)
                    count = sourceList.Count;
                for (int i = 0; i < count; i++)
                {
                    sourceList[i].GroupOrdinal = i;
                    sourceList[i].Group = targetGroup;
                    targetGroup.GroupDetailItems.Add(sourceList[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }

        }

        private void ResetUserProfile()
        {
            userName = string.Empty;
            titleUserName = string.Empty;
            userPicture = string.Empty;
            userAbout = string.Empty;
            instructablesCount =0;
            viewCount = 0;
            followersCount = 0;
            draftcount = 0;
            publishCount = 0;
            favoritesCount = 0;
            subscriptionsCount = 0;

            if(PublishedInstrutables != null)
                PublishedInstrutables = null;

            IsLoading = true;
            isLoginUser = false;
        }

        private string _viewState;

        internal void UpdateState(string visualState)
        {
            Debug.WriteLine(visualState);
            _viewState = visualState;
        }

        public class ScreenItems
        {
            public string Name { get; set; }
            public int DualFeatureCount { get; set; }            
        }

        private List<ScreenItems> ScreenMetrics = new List<ScreenItems>()
            { 
                new ScreenItems(){Name = "Portrait768", DualFeatureCount = 12},
                new ScreenItems(){Name = "Portrait1080", DualFeatureCount = 11},
                new ScreenItems(){Name = "Portrait1440", DualFeatureCount = 14},
                new ScreenItems(){Name = "768", DualFeatureCount = 9},
                new ScreenItems(){Name = "1080", DualFeatureCount = 12},
                new ScreenItems(){Name = "1440", DualFeatureCount = 15},
                new ScreenItems(){Name = "Snapped", DualFeatureCount = 12}
            };

        public static ScreenItems CurrentScreenMetrics = null;


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

        private async void ExecuteRefreshCommand()
        {
            IsLoading = true;
            PublishedInstrutables.Clear();
            PublishedGroups.Clear();
            FavoriteInstructables.Clear();
            Followers.Clear();
            Followings.Clear();
            await Initialize(_screenName);
        }

    }
}
