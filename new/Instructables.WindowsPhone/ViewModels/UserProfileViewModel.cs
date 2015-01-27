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
    public class UserProfileViewModel : BindableBase
    {
        public class ProfileInitData
        {
            public string screenName = String.Empty;
            public string menuName = String.Empty;
        };

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
            
        }

        private int _currentInitData = -1;

        public int CurrentInitData
        {
            get
            {
                return _currentInitData;
            }

            set
            {
                _currentInitData = value;
                OnPropertyChanged();
            }
        }

        private List<ProfileInitData> _initData;
        public List<ProfileInitData> InitData
        {
            get
            {
                if (_initData == null)
                    _initData = new List<ProfileInitData>();
                return _initData;
            }
            set
            {
                _initData = value;
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
                    return "Me";
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

        private bool _isUserFollowed = false;
        public bool isUserFollowed
        {
            get { return this._isUserFollowed; }
            set
            {
                _isUserFollowed = value;
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
            get 
            { 
                return this._instructablesCount; 
            }

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
            get 
            { 
                return this._views; 
            }

            set
            {
                _views = value;
                OnPropertyChanged();
            }
        }

        private int _followersCount;
        public int followersCount
        {
            get 
            { 
                return this._followersCount; 
            }

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

        private string _visualState = "Normal";
        public string VisualState
        {
            get { return _visualState; }
            set { this.SetProperty(ref this._visualState, value); }
        }

        public void ClearData()
        {
            if (Followings != null)
            {
                Followings.Clear();
                Followings = null;
            }

            if (Followers != null)
            {
                Followers.Clear();
                Followers = null;
            }

            if (FavoriteInstructables != null)
            {
                FavoriteInstructables.Clear();
                FavoriteInstructables = null;
            }

            if (PublishedInstrutables != null)
            {
                PublishedInstrutables.Clear();
                PublishedInstrutables = null;
            }

            if (DraftInstructables != null)
            {
                DraftInstructables.Clear();
                DraftInstructables = null;
            }
        }

        public async Task Initialize(string screenName)
        {
            IsLoading = true;
            isUserFollowed = false;
            if (screenName != _screenName)
            {
                ResetUserProfile();
            }
            await ViewModelLocator.Instance.LandingVM.UpdateUserProfile();
            var dataService = InstructablesDataService.DataServiceSingleton;
            var loginUserProfile = ViewModelLocator.Instance.LandingVM.userProfile;
            var createVM = ViewModelLocator.Instance.CreateVM;
            
            VisualState = "Normal";
            bool allSubscriptions = false;
            //if (loginUserProfile != null && loginUserProfile.screenName == screenName)
            //    allSubscriptions = true;
            _userProfile = await dataService.GetUserProfile(screenName, allSubscriptions);
            if (_userProfile == null)
                VisualState = "Offline";

            if (loginUserProfile != null && loginUserProfile.subscriptionsForLoginUser != null && _userProfile != null)
            {
                foreach (var followedAuthor in loginUserProfile.subscriptionsForLoginUser)
                {
                    if (_userProfile.id == followedAuthor.id)
                    {
                        isUserFollowed = true;
                    }
                }
            }

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

            } else {                
                isLoginUser = false;
            }

            instructablesCount = _userProfile.instructablesCount;

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
            await PublishedInstrutables.LoadMoreItemsAsync(8);

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
            await FavoriteInstructables.LoadMoreItemsAsync(8);

            Followings = new AuthorCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    SubscriptionsList authorList = await dataService.GetFollowings(screenName, offset, count);
                    if (loginUserProfile != null && authorList != null && loginUserProfile.subscriptionsForLoginUser != null && authorList.subscriptions != null)
                    {
                        foreach (var author in authorList.subscriptions)
                        {
                            foreach (var followedAuthor in loginUserProfile.subscriptionsForLoginUser)
                            {
                                if (author.id == followedAuthor.id)
                                {
                                    author.isFollowed = true;
                                }
                            }
                        }
                    }
                    

                    if (authorList != null && authorList.subscriptions != null)
                    {
                        return authorList.subscriptions;
                    }
                    return null;
                }
            };
            Followings.Clear();
            Followings.StartIncrementalLoadEvent += StartIncrementalLoading;
            Followings.StopIncrementalLoadEvent += StopIncrementalLoading;
            await Followings.LoadMoreItemsAsync(8);

            Followers = new AuthorCollection
            {
                loadMoreItemsAsync = async delegate(int offset, int count)
                {
                    FollowersList authorList = await dataService.GetFollowers(screenName, offset, count);
                    var profile = ViewModelLocator.Instance.LandingVM.userProfile;

                    if (profile != null)
                    {
                        foreach (var author in authorList.followers)
                        {
                            ObservableCollection<Author> subscriptions = profile.subscriptions;
                            if (subscriptions != null && loginUserProfile != null && loginUserProfile.subscriptionsForLoginUser != null)
                            {
                                foreach (var followedAuthor in loginUserProfile.subscriptionsForLoginUser)
                                {
                                    if (author.id == followedAuthor.id)
                                        author.isFollowed = true;
                                }
                            }
                        }
                    }

                    if (authorList != null && authorList.followers != null)
                    {
                        return authorList.followers;
                    }
                    return null;
                    
                }
            };
            Followers.StartIncrementalLoadEvent += StartIncrementalLoading;
            Followers.StopIncrementalLoadEvent += StopIncrementalLoading;
            Followers.Clear();
            await Followers.LoadMoreItemsAsync(8);

            RefreshCollections();

            IsLoading = false;
        }

        private void StartIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = true;
        }

        private void StopIncrementalLoading(object sender, EventArgs e)
        {
            IsLoading = false;
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

            IsLoading = true;
            isLoginUser = false;
        }
    }
}
