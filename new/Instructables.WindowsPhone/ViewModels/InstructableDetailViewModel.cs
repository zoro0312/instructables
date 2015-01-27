using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Instructables.ViewModels
{
    public class InstructableDetailViewModel : BindableBase
    {
        private ObservableCollection<StepGroup> _steps = new ObservableCollection<StepGroup>();

        public int fitwidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }

        public int fitheight
        {
            get
            {
                float ratio = (float)VideoHeight / (float)VideoWidth;
                var screenWidth = Window.Current.Bounds.Width;
                return (int)(screenWidth * ratio);
            }
        }

        public ObservableCollection<StepGroup> Steps
        {
            get { return this._steps; }
        }

        private ObservableCollection<VotableContestGroup> _votableContests = new ObservableCollection<VotableContestGroup>();

        public ObservableCollection<VotableContestGroup> VotableContests
        {
            get { return this._votableContests; }
            set 
            {
                _votableContests = value;
                OnPropertyChanged();
            }
        }

        private CommentCollection _commentsList = new CommentCollection();
        public CommentCollection CommentsList
        {
            get { return this._commentsList; }
            set
            {
                _commentsList = value;
                OnPropertyChanged();
            }
        }

        private CommandBar _detailAppBar = null;
        public CommandBar DetailAppBar
        {
            get
            {
                return _detailAppBar;
            }
            set
            {
                _detailAppBar = value;
            }
        }

        public bool IfNoComment
        {
            get 
            {
                return (CommentsList.Count <= 0 ? true : false);
            }
        }

        private bool _followEnable = true;
        public bool FollowEnable
        {
            get { return _followEnable; }
            set
            {
                _followEnable = value;
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

        private ObservableCollection<MediaItem> _allFiles = new ObservableCollection<MediaItem>();
        public ObservableCollection<MediaItem> AllFiles
        {
            get { return _allFiles; }
        }

        private ObservableCollection<MediaItem> _currentStepFiles = new ObservableCollection<MediaItem>();
        public ObservableCollection<MediaItem> CurrentStepFiles
        {
            get { return _currentStepFiles; }
        }

        int _currentFileIndex = 0;
        public int CurrentFileIndex
        {
            get { return _currentFileIndex; }
            set 
            { 
                _currentFileIndex = value;
                OnPropertyChanged();
            }
        }

        Step _currentStep = null;
        public Step CurrentStep
        {
            get { return _currentStep; }
            set
            {
                _currentStep = value;
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

        private bool _isFollowing = false;
        public bool IsFollowing
        {
            get { return _isFollowing; }
            set
            {
                _isFollowing = value;
                OnPropertyChanged();
            }
        }

        private bool _isFavorite = false;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                OnPropertyChanged();
            }
        }

        private void UpdatePictureIndex(File fileParam)
        {
            if(CurrentStep!=null)
            {
                int index = 0;
                foreach (var f in CurrentStep.files)
                {
                    index++;
                    if (f == fileParam)
                    {
                        CurrentFileIndex = index;
                        return;
                    }
                }

                index = 0;
                foreach(var f in CurrentStep.ImageNames)
                {
                    index++;
                    if (f == fileParam)
                    {
                        CurrentFileIndex = index;
                        return;
                    }
                }
            }
        }

        private MediaItem _selectedMedia;
        public MediaItem SelectedMedia
        {
            get { return _selectedMedia; }
            set
            {
                if (value!=null &&_selectedMedia != value)
                {
                    _selectedMedia = value;
                    UpdatePictureIndex(_selectedMedia.FileMedia);
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedVideo;
        public string SelectedVideo
        {
            get
            {
                return _selectedVideo;
            }

            set
            {
                _selectedVideo = value;
                OnPropertyChanged();
            }
        }

        private static InstructableDetailViewModel _currentInstructableDV = null;
        public static InstructableDetailViewModel CurrentInstructbaleDV
        {
            get { return _currentInstructableDV; }
            set { _currentInstructableDV = value; }
        }

        private string _noteText = "";
        public  string NoteText
        {
            get { return _noteText; }
            set 
            {
                _noteText = value;
                OnPropertyChanged();
                HasNote = (_noteText.Count() > 0 ? true : false);
            }
        }

        private bool _hasNote = false;
        public bool HasNote
        {
            get { return _hasNote; }
            set
            {
                _hasNote = value;
                OnPropertyChanged();
            }
        }

        private bool _showPhotoViewer;

        public bool ShowPhotoViewer
        {
            get { return _showPhotoViewer; }
            set
            {
                _showPhotoViewer = value;
                OnPropertyChanged();
            }
        }

        private bool _showVideoViewer;

        public bool ShowVideoViewer
        {
            get { return _showVideoViewer; }
            set
            {
                _showVideoViewer = value;
                OnPropertyChanged();
            }
        }

        private bool _showBottomBar = true;

        public bool ShowBottomBar
        {
            get { return _showBottomBar; }
            set
            {
                _showBottomBar = value;
                OnPropertyChanged();
            }
        }

        private int _videoHeight;

        public int VideoHeight
        {
            get { return _videoHeight; }
            set
            {
                if (_videoHeight != value)
                {
                    _videoHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public Uri _videoUri;
        public Uri VideoUri
        {
            get { return _videoUri; }
            set
            {
                if (_videoUri != value)
                {
                    _videoUri = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _videoWidth;

        public int VideoWidth
        {
            get { return _videoWidth; }
            set
            {
                if (_videoWidth != value)
                {
                    _videoWidth = value;
                    OnPropertyChanged();
                }
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


        public InstructableDetailViewModel()
        {
            if (DesignMode.IsInDesignMode())
            {
                InitializeDesignData();
            }
        }

        public void ClearData()
        {
            SelectedInstructable = null;
            Steps.Clear();
        }

        private string _lastID;

        public void LoadVotableContests()
        {
            IsLoading = true;
            VotableContests = null;
            VotableContests = new ObservableCollection<VotableContestGroup>();
            if(SelectedInstructable.votableContests != null)
            {
                foreach(var votableContest in SelectedInstructable.votableContests)
                {
                    VotableContestGroup vcg = new VotableContestGroup();
                    votableContest.instructableTitle = SelectedInstructable.title;
                    vcg.VotableContestsGroup.Add(votableContest);
                    VotableContests.Add(vcg);
                }
            }
            IsLoading = false;
        }

        public async Task LoadComments(string urlString)
        {
            IsLoading = true;
            var dataService = InstructablesDataService.DataServiceSingleton;
            CommentsList = new CommentCollection();
            CommentsList.commentlist = await dataService.GetComments(urlString);
            await CommentsList.LoadMoreItemsAsync(20);
            IsLoading = false;
        }

        public async void LoadInstructableFromCreating()
        {
            var vm = ViewModelLocator.Instance.CreateVM;
            VisualState = "Normal";
            IsLoading = true;

            var result = vm.Instructable;

            if (result != null)
            {
                SelectedInstructable = result;
                _steps.Clear();
                _allFiles.Clear();

                if (SelectedInstructable.steps != null)
                {
                    foreach (var step in SelectedInstructable.steps)
                    {
                        StepGroup sg = new StepGroup();
                        sg.StepName = step.StepName;
                        sg.StepTitle = step.title;
                        sg.Steps.Add(step);
                        Steps.Add(sg);

                        try
                        {
                            step.XamlBody = Html2XamlConverter.Convert2Xaml(step.body);
                            step.VideoList = VideoParser.ParseVideos(step.body);
                            if (step.VideoList != null && step.VideoList.Count > 0)
                            {
                                SelectedInstructable.HasVideos = true;
                                foreach (var video in step.VideoList)
                                {
                                    await GetVideoThumbnail(video);
                                    AllFiles.Add(new MediaItem() { VideoMedia = video, MediaType = MediaTypeOption.Video });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(String.Format("Exception in HTML Conversion: {0}", ex.Message));
                        }
                     }
                }
                else
                {
                    if (SelectedInstructable.introduction != null && SelectedInstructable.introduction.body != null)
                    {
                        SelectedInstructable.introduction.XamlBody = Html2XamlConverter.Convert2Xaml(SelectedInstructable.introduction.body);
                    }
                }
                if (SelectedInstructable.type == "Video")
                {
                    var vResult = VideoParser.ParseVideos(SelectedInstructable.video);
                    if (vResult != null && vResult.Count == 1)
                    {
                        var videoResult = vResult[0];
                        float ratio = (float)videoResult.Height / (float)videoResult.Width;
                        VideoWidth = (int)Window.Current.Bounds.Width;
                        VideoHeight = (int)(VideoWidth * ratio);
                        VideoUri = new Uri("http:" + videoResult.UriString);
                    }
                }
                NotifyPropertyChange("ZoomWrapWidth");
                IsLoading = false;
            }
            else
            {
                VisualState = "Offline";
                IsLoading = false;
            }

        }

        public async Task LoadInstructable(string instructableID)
        {
            _lastID = instructableID;
            try
            {
                VisualState = "Normal";
                IsLoading = true;
                if (String.IsNullOrEmpty(instructableID) && !DesignMode.IsInDesignMode())
                    return;
                var dataService = InstructablesDataService.DataServiceSingleton;
                var result = await dataService.GetInstructable(instructableID);
                if (result != null)
                {
                    SelectedInstructable = result;

                    _steps.Clear();
                    _allFiles.Clear();

                    if (SelectedInstructable.steps != null)
                    {
                        foreach (var step in SelectedInstructable.steps)
                        {
                            StepGroup sg = new StepGroup();
                            sg.StepName = step.StepName;
                            sg.StepTitle = step.title;
                            sg.Steps.Add(step);
                            Steps.Add(sg);
                            step.totalStepNum = SelectedInstructable.steps.Count - 1;
                            try
                            {
                                step.XamlBody = Html2XamlConverter.Convert2Xaml(step.body);
                                step.VideoList = VideoParser.ParseVideos(step.body);
                                if (step.VideoList == null 
                                    && step.files[0].embedType != null
                                    && step.files[0].embedType == "VIDEO")
                                {
                                    step.VideoList = VideoParser.ParseVideos(step.files[0].embedHtmlCode);
                                }

                                if (step.VideoList != null && step.VideoList.Count > 0)
                                {
                                    SelectedInstructable.HasVideos = true;
                                    foreach (var video in step.VideoList)
                                    {
                                        await GetVideoThumbnail(video);
                                        AllFiles.Add(new MediaItem() { VideoMedia = video, MediaType = MediaTypeOption.Video });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(String.Format("Exception in HTML Conversion: {0}", ex.Message));
                            }

                            List<File> removeList = new List<File>();
                            if (step.files != null && step.files.Count > 0)
                            {
                                foreach (var file in step.files)
                                {
                                    Uri fileURI = new Uri(file.downloadUrl);
                                    var extension = System.IO.Path.GetExtension(fileURI.LocalPath);
                                    if (".png|.jpg|.gif".IndexOf(extension) < 0)
                                    {
                                        Debug.WriteLine("Removing file with extension: {0}", extension);
                                        removeList.Add(file);
                                    }
                                    else
                                    {
                                        //AllFiles.Add(new MediaItem() { FileMedia = file, MediaType = MediaTypeOption.Photo });
                                    }
                                }
                                if (removeList.Count > 0)
                                    step.DownloadFiles = new List<File>();
                                foreach (var f in removeList)
                                {
                                    step.DownloadFiles.Add(f);
                                    step.files.Remove(f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SelectedInstructable.introduction != null && SelectedInstructable.introduction.body != null)
                        {
                            SelectedInstructable.introduction.XamlBody = Html2XamlConverter.Convert2Xaml(SelectedInstructable.introduction.body);
                        }
                    }
                    if (SelectedInstructable.type == "Video")
                    {
                        var vResult = VideoParser.ParseVideos(SelectedInstructable.video);
                        if (vResult != null && vResult.Count == 1)
                        {
                            var videoResult = vResult[0];
                            float ratio = (float)videoResult.Height/(float)videoResult.Width;
                            VideoWidth = (int)Window.Current.Bounds.Width;
                            VideoHeight = (int)(VideoWidth * ratio);
                            
                            //VideoUri = new Uri("http:"+videoResult.UriString);
                            String uriString = videoResult.UriString;
                            if(uriString.StartsWith("http") == true)
                            {
                                VideoUri = new Uri(videoResult.UriString);
                            }
                            else 
                            {
                                VideoUri = new Uri("http:" + videoResult.UriString);
                            }

                            Video = videoResult;
                            await GetVideoThumbnail(Video);
                        }
                    }
                    NotifyPropertyChange("ZoomWrapWidth");
                    IsLoading = false;
                }
                else
                {
                    VisualState = "Offline";
                    IsLoading = false;
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("Exception in retriving Instructble");
            }
        }

        private Video _video;
        public Video Video
        {
            get
            {
                return _video;
            }
            set
            {
                _video = value;
            }
        }

        async private Task GetVideoThumbnail(Video video)
        {
            var fName = System.IO.Path.GetFileName(video.UriString);
            var fUri = new Uri(video.UriString);
            var source = fUri.Host;
            if (source.ToLower().IndexOf("vimeo") > 0)
            {
                video.Source = "vimeo";
                video.ThumbnailURI = await GetVimeoThumbnail(video.UriString);
            }
            if (source.ToLower().IndexOf("youtube") > 0)
            {
                video.Source = "youtube";
                video.ThumbnailURI = GetYouTubeThumbnail(video.UriString);
            }
        }

        public static readonly Regex VimeoVideoRegex = new Regex(@"vimeo\.com/(?:.*#|.*/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public static readonly Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);

        async private Task<string> GetVimeoThumbnail(string uriString)
        {
            Match vimeoMatch = VimeoVideoRegex.Match(uriString);

            string id = string.Empty;

            if (vimeoMatch.Success)
                id = vimeoMatch.Groups[1].Value;

            if (!String.IsNullOrEmpty(id))
            {
                var requestUri = String.Format(@"http://vimeo.com/api/v2/video/{0}.xml", id);
                var client = new HttpClient();
                XmlDocument doc = await XmlDocument.LoadFromUriAsync(new Uri(requestUri));
                XmlNodeList result = doc.GetElementsByTagName("thumbnail_large");
                if (result != null)
                {
                    return result[0].InnerText;
                }
            }
            return "";
        }

        private string GetYouTubeThumbnail(string uriString)
        {
            Match youtubeMatch = YoutubeVideoRegex.Match(uriString);

            string id = string.Empty;

            if (youtubeMatch.Success)
                id = youtubeMatch.Groups[4].Value;
            if (!String.IsNullOrEmpty(id))
            {
                return String.Format(@"http://img.youtube.com/vi/{0}/0.jpg", id);
            }
            return "";
        }


        public int ZoomWrapWidth
        {
            get
            {
                if (SelectedInstructable != null && SelectedInstructable.steps != null)
                {
                    int half = (int)SelectedInstructable.steps.Count / 2;
                    if (half % 2 == 1)
                        return half + 1;
                    else
                        return half;
                }
                else
                {
                    return 0;
                }
            }
        }


        private Instructable _selectedInstructable;

        public Instructable SelectedInstructable
        {
            get { return _selectedInstructable; }
            set
            {
                _selectedInstructable = value;
                //if ( _selectedInstructable != null && _selectedInstructable.steps != null && _selectedInstructable.steps.Count > 14)
                //    ZoomWrapWidth = _selectedInstructable.steps.Count / 2;
                //else
                //{
                //    ZoomWrapWidth = 7;
                //}
                if (_selectedInstructable != null && _selectedInstructable.steps != null && _selectedInstructable.steps.Count > 0)
                    SelectedStep = _selectedInstructable.steps[0];
                PinSelectedCommand.IsEnabled = _selectedInstructable != null;
                IsLowerAppBarSticky = _selectedInstructable != null;
                LowerAppBarIsOpen = _selectedInstructable != null;
                OnPropertyChanged();
            }
        }

        private Step _selectedStep;
        public Step SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                if (_selectedStep != value)
                {
                    _selectedStep = value;
                    OnPropertyChanged();
                }
            }
        }


        public void NotifyPropertyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        private CategoryOptions _categoryOptions;

        public CategoryOptions Channels
        {
            get { return _categoryOptions; }
            set { _categoryOptions = value; }
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
            IsLoading = true;
            VisualState = "Normal";
            await LoadInstructable(_lastID);
        }


        private RelayCommand<object> _showPhotoViewerCommand;

        public RelayCommand<object> ShowPhotoViewerCommand
        {
            get
            {
                if (_showPhotoViewerCommand == null)
                {
                    _showPhotoViewerCommand = new RelayCommand<object>(ExecuteShowPhotoViewer);
                    _showPhotoViewerCommand.IsEnabled = true;
                }
                return _showPhotoViewerCommand;
            }
        }

        private void ExecuteShowPhotoViewer(object param)
        {
            if (param is File)
            {
                SelectedMedia = null;
                CurrentStepFiles.Clear();
                File fileParam = param as File;
                foreach (var step in SelectedInstructable.steps)
                {
                    bool isLocalFile = false;
                    
                    int index = 0;
                    File sf = null;
                    foreach(var f in step.files)
                    {
                        index++;
                        if (f == fileParam)
                        {
                            sf = f;
                            CurrentStep = step;
                            CurrentFileIndex = index;
                            break;
                        }
                    }

                    foreach (var f in step.ImageNames)
                    {
                        index++;
                        if (f == fileParam)
                        {
                            sf = f;
                            CurrentStep = step;
                            CurrentFileIndex = index;
                            isLocalFile = true;
                            break;
                        }
                    }

                    /*var sf = (from f in step.files
                              where f == fileParam
                              select f).FirstOrDefault();*/

                    if (sf != null)
                    {
                        if (!isLocalFile)
                        {
                            foreach (var file in step.files)
                            {
                                MediaItem mediaItem = new MediaItem() { FileMedia = file, MediaType = MediaTypeOption.Photo };
                                CurrentStepFiles.Add(mediaItem);
                                if (file == sf)
                                {
                                    SelectedMedia = mediaItem;
                                }
                            }
                        }
                        else
                        {
                            foreach (var file in step.ImageNames)
                            {
                                MediaItem mediaItem = new MediaItem() { FileMedia = file, MediaType = MediaTypeOption.Photo };
                                CurrentStepFiles.Add(mediaItem);
                                if (file == sf)
                                {
                                    SelectedMedia = mediaItem;
                                }
                            }
                        }
                    }
                }
            }
            if (param is Video)
            {
                Video videoParam = param as Video;
                var s = (from f in AllFiles
                         where f.VideoMedia == videoParam
                         select f).FirstOrDefault();
                if (s != null)
                    SelectedMedia = s;
            }
            ShowPhotoViewer = true;
            if(DetailAppBar!=null)
            {
                DetailAppBar.Visibility = Visibility.Collapsed;
            }
        }

        private RelayCommand _dismissPhotoViewerCommand;

        public RelayCommand DismissPhotoViewerCommand
        {
            get
            {
                if (_dismissPhotoViewerCommand == null)
                {
                    _dismissPhotoViewerCommand = new RelayCommand(ExecuteDismissPhotoViewer);
                    _dismissPhotoViewerCommand.IsEnabled = true;
                }
                return _dismissPhotoViewerCommand;
            }
        }

        private void ExecuteDismissPhotoViewer()
        {
            ShowPhotoViewer = false;
            IsLoading = false;
        }


        private RelayCommand<object> _showVideoViewerCommand;

        public RelayCommand<object> ShowVideoViewerCommand
        {
            get
            {
                if (_showVideoViewerCommand == null)
                {
                    _showVideoViewerCommand = new RelayCommand<object>(ExecuteShowVideoViewer);
                    _showVideoViewerCommand.IsEnabled = true;
                }
                return _showVideoViewerCommand;
            }
        }


        private void ExecuteShowVideoViewer(object param)
        {
            var video = param as Video;
            var videoUrl = video.VideoUri;
            if (videoUrl != null)
            {
                SelectedVideo = videoUrl.ToString();
            }
            else if (param is string)
            {
                SelectedVideo = param as string;
            }

            VideoHeight = video.VideoHeight;
            VideoWidth = video.VideoWidth;

            ShowVideoViewer = true;
            if (DetailAppBar != null)
            {
                DetailAppBar.Visibility = Visibility.Collapsed;
            }
        }

        private RelayCommand _dismissVideoViewerCommand;

        public RelayCommand DismissVideoViewerCommand
        {
            get
            {
                if (_dismissVideoViewerCommand == null)
                {
                    _dismissVideoViewerCommand = new RelayCommand(ExecuteDismissVideoViewer);
                    _dismissVideoViewerCommand.IsEnabled = true;
                }
                return _dismissVideoViewerCommand;
            }
        }

        private void ExecuteDismissVideoViewer()
        {
            ShowVideoViewer = false;
            if (DetailAppBar != null)
            {
                SelectedVideo = String.Empty;
                DetailAppBar.Visibility = Visibility.Visible;
            }
        }

        public void BackButtonCallExecuteDismissVideoViewer()
        {
            ExecuteDismissVideoViewer();
        }

        private bool _backFromFullScreenPicture;
        public bool BackFromFullScreenPicture
        {
            get
            {
                return _backFromFullScreenPicture;
            }

            set 
            {
                _backFromFullScreenPicture = value;
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
                var response = await httpClient.GetAsync(SelectedInstructable.rectangle1Url);
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
                    imageNode[0].Attributes[1].InnerText = SelectedInstructable.square2Url;

                    var narrowTemplate =
                        TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquarePeekImageAndText04);
                    var narrowTextNode = narrowTemplate.GetElementsByTagName("text");
                    narrowTextNode[0].AppendChild(narrowTemplate.CreateTextNode(SelectedInstructable.title));
                    var narrowImageNode = narrowTemplate.GetElementsByTagName("image");
                    narrowImageNode[0].Attributes[1].InnerText = SelectedInstructable.square2Url;

                    var narrowNode = wideTemplate.ImportNode(narrowTemplate.GetElementsByTagName("binding").Item(0), true);
                    var xmlNode = wideTemplate.GetElementsByTagName("visual").Item(0);
                    if (xmlNode != null)
                        xmlNode.AppendChild(narrowNode);

                    var tileNotification = new TileNotification(wideTemplate);
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(SelectedInstructable.id).Update(tileNotification);
                }
            }
        }

        private async void InitializeDesignData()
        {
            IsLoading = false;
            var dataService = InstructablesDataService.DataServiceSingleton;
            var result = await dataService.GetInstructable("test");
            if (result != null)
                SelectedInstructable = result;
            Steps.Clear();
            if (SelectedInstructable.steps != null)
            {
                foreach (var step in SelectedInstructable.steps)
                {
                    StepGroup sg = new StepGroup();
                    sg.StepName = step.StepName;
                    sg.StepTitle = step.title;
                    sg.Steps.Add(step);
                    Steps.Add(sg);
                    if (step.files != null && step.files.Count > 0)
                    {
                        foreach (var f in step.files)
                            _allFiles.Add(new MediaItem() { FileMedia = f, MediaType = MediaTypeOption.Photo });
                    }
                }
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
    }
}
