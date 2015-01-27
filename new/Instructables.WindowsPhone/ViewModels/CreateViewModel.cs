using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using Windows.Storage.FileProperties;

namespace Instructables.ViewModels
{
    public class CreateViewModel : BindableBase
    {
        public void Initialize(Instructable instructable)
        {
            // initialize the instructable
            if (instructable == null)
                return;

            _instructable = instructable;
            InitVisualSteps();
            InitStepImages();

            // clean up the temp data
            //StepImages.Clear();
            ThumbImages.Clear();
        }

        private Instructable _instructable;
        public Instructable Instructable
        {
            private set { this._instructable = value; }
            get { return this._instructable; }
        }

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
                return (int)((fitWidth - 47) / 3);
            }
        }

        private const string COVER_EMPTY_IMAGE = "ms-appx:///Assets/empty_cover.png";
        private string _coverImage = COVER_EMPTY_IMAGE;
        public string CoverImage
        {
            get
            {
                //if (Instructable != null && Instructable.coverImage != null)
                //    _coverImage = Instructable.coverImage.rectangle1Url;
                //if (Instructable != null && Instructable.steps.Count != 0 && Instructable.steps[0].ImageNames.Count != 0)
                //    _coverImage = Instructable.steps[0].ImageNames[0].name;
                //if (Instructable != null && Instructable.steps.Count != 0 && Instructable.steps[0].files.Count != 0)
                //    _coverImage = Instructable.steps[0].files[0].rectangle1Url;
                return _coverImage;
            }

            set
            {
                _coverImage = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get
            {
                if (Instructable.channel == null || Instructable.channel == "null")
                {
                    return "Tap to select";
                }
                else if (Instructable.channel.ToLower() == "all")
                {
                    return Instructable.category;
                }
                else
                {
                    return Instructable.channel;
                }
            }
        }

        public string Keywords
        {
            get
            {
                if (Instructable.keywords.Count != 0)
                {
                    return string.Join(",", Instructable.keywords);
                }
                return string.Empty;
            }
            set
            {
                Instructable.keywords.Clear();
                if (value != null && value != string.Empty)
                {
                    char[] seperator = new char[] { ' ', ';', ',' };
                    string[] keys = value.Split(seperator);

                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i] != string.Empty)
                            Instructable.keywords.Add(keys[i]);
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool _isSaving = false;
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                _isSaving = value;
                OnPropertyChanged();
            }
        }

        public Step CreateStep()
        {
            var step = new Step();
            step.stepIndex = Instructable.steps.Count;
            Instructable.steps.Add(step);

            UpdataVisualSteps();
            return step;
        }

        public bool DeleteStep(Step step)
        {
            if (step == null)
                return false;

            int stepIndex = step.stepIndex;

            for (int index = stepIndex + 1; index < Instructable.steps.Count; index++)
            {
                Instructable.steps[index].stepIndex--;
            }
            Instructable.steps.Remove(step);

            UpdataVisualSteps();

            return true;
        }

        private ObservableCollection<Step> _visualSteps = new ObservableCollection<Step>();
        public ObservableCollection<Step> VisualSteps
        {
            get { return this._visualSteps; }
        }

        private void InitVisualSteps()
        {
            if (Instructable.steps.Count == 0)
            {
                Step IntroStep = new Step();
                IntroStep.stepIndex = 0;
                IntroStep.title = Instructable.title;
                Instructable.steps.Add(IntroStep);
            }

            UpdataVisualSteps();
        }

        private void UpdataVisualSteps()
        {
            VisualSteps.Clear();

            foreach (var step in Instructable.steps)
            {
                VisualSteps.Add(step);
            }

            // Appending the special steps
            Step overviewStep = new Step();
            overviewStep.stepIndex = -1;
            VisualSteps.Add(overviewStep);

            Step publishStep = new Step();
            publishStep.stepIndex = -2;
            VisualSteps.Add(publishStep);
        }

        public int VisualStepIndex(Step step)
        {
            if (step.stepIndex < 0)
                return Instructable.steps.Count - step.stepIndex;
            else
                return step.stepIndex;
        }

        //private ObservableCollection<IStorageFile> _stepImages = new ObservableCollection<IStorageFile>();
        //public ObservableCollection<IStorageFile> StepImages
        //{
        //    get { return this._stepImages; }
        //}

        private Dictionary<string, BitmapImage> _thumbImages = new Dictionary<string, BitmapImage>();
        public Dictionary<string, BitmapImage> ThumbImages
        {
            get { return this._thumbImages; }
        }

        public BitmapImage GetBitmapImage(StorageFile file)
        {
            try
            {
                var thumbImage = new BitmapImage();

                Windows.Storage.StorageFolder tempFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
                var copy = file.CopyAsync(tempFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask();
                copy.Wait();

                var get = tempFolder.GetFileAsync(file.Name).AsTask();
                get.Wait();
                StorageFile newFile = get.Result;

                //var stream = newFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 240, ThumbnailOptions.UseCurrentScale).AsTask();
                var stream = newFile.GetScaledImageAsThumbnailAsync(ThumbnailMode.SingleItem).AsTask();
                stream.Wait();
                thumbImage.SetSource(stream.Result);

                var del = newFile.DeleteAsync().AsTask();
                del.Wait();

                return thumbImage;
            } catch (Exception)
            {
                return null;
            }
        }

        public List<StorageFile> AddStepImages(Step step, IReadOnlyList<StorageFile> images)
        {
            if (step == null)
                return null;

            List<StorageFile> newImages = new List<StorageFile>();
            foreach (var image in images)
            {
                BitmapImage bitmap = GetBitmapImage(image);
                if (bitmap == null)
                    continue;

                if (!ThumbImages.ContainsKey(image.Name))
                {
                    ThumbImages.Add(image.Name, bitmap);
                }

                if (step.stepIndex==0 && CoverImage == COVER_EMPTY_IMAGE && ThumbImages.Count>0)
                {
                    CoverImage = image.Name;
                }

                bool isInImageNames = false;
                foreach (var file in step.ImageNames)
                {
                    if (file.name == image.Name)
                    {
                        isInImageNames = true;
                        break;
                    }
                }
                if (!isInImageNames)
                {
                    File file = new File();
                    file.name = image.Name;
                    file.width = bitmap.PixelWidth;
                    file.height = bitmap.PixelHeight;
                    step.ImageNames.Add(file);
                    newImages.Add(image);
                    //StepImages.Add(image);
                }
            }
            return newImages;
        }

        public bool DeleteStepImage(Step step, string imageName)
        {
            foreach (var file in step.files)
            {
                if (imageName == file.name)
                {
                    step.files.Remove(file);
                    break;
                }
            }
            foreach (var file in step.ImageNames)
            {
                if(imageName == file.name)
                {
                    step.ImageNames.Remove(file);
                    break;
                }
            }

            ThumbImages.Remove(imageName);
            if (step.stepIndex == 0)
            {
                if (step.ImageNames.Count == 0)
                {
                    CoverImage = COVER_EMPTY_IMAGE;
                }
                else
                {
                    CoverImage = step.ImageNames[0].name;
                }
            }
            
            return true;
        }

        private void InitStepImages()
        {
            if (Instructable == null || Instructable.steps == null)
                return;

            foreach (Step step in Instructable.steps)
            {
                if (step.files != null)
                {
                    step.ImageNames.Clear();
                    foreach (File file in step.files)
                    {
                        File tempFile = new File();
                        tempFile.name = file.largeUrl;
                        tempFile.width = file.width;
                        tempFile.height = file.height;
                        step.ImageNames.Add(tempFile);
                    }
                    if (step.stepIndex == 0)
                    {
                        if (step.ImageNames.Count == 0)
                        {
                            CoverImage = COVER_EMPTY_IMAGE;
                        }
                        else
                        {
                            CoverImage = step.ImageNames[0].name;
                        }
                    }
                }
            }
        }

        public async Task<CreateResult> SaveCloudInstructable()
        {
            IsSaving = true;
            await InstructablesDataService.DataServiceSingleton.DiscoverActiveUploadsAsync();
            IsSaving = false;
            if (Instructable.id == null || Instructable.id == string.Empty)
                return await InstructablesDataService.DataServiceSingleton.NewInstructable(Instructable);
            else
                return await InstructablesDataService.DataServiceSingleton.SaveInstructable(Instructable);
        }

        public async Task<CreateResult> PublishCloudInstructable()
        {
            await InstructablesDataService.DataServiceSingleton.DiscoverActiveUploadsAsync();

            if (Instructable.id == null || Instructable.id == string.Empty)
            {
                var result = await InstructablesDataService.DataServiceSingleton.NewInstructable(Instructable);
                if (result == null)
                {
                    return null;
                }
                Instructable.id = result.id;
            }
            return await InstructablesDataService.DataServiceSingleton.PublishInstructable(Instructable);
        }

        public void CleanUpData()
        {
            _isSaving = false;
            _visualSteps.Clear();
            //_stepImages.Clear();
        }

        // Obolete codes below -----------------------

        //public void PrepareInstructableForPreview()
        //{
        //    if (VisualSteps.Count > 0)
        //    {
        //        // The first step will be introduction.
        //        this._instructable.title = VisualSteps[0].title;
        //        this._instructable.body = VisualSteps[0].body;
        //        this._instructable.files = VisualSteps[0].files;

        //        this._instructable.StepImages.Clear();
        //        this._instructable.StepImages.AddRange(VisualSteps[0].StepImages);

        //        // The rest steps will be appended.
        //        this._instructable.steps.Clear();
        //        this._instructable.steps.AddRange(VisualSteps.Skip(0));

        //        if (!InstructablesDataService.DataServiceSingleton.isLogin())
        //        {
        //            foreach (var step in this._instructable.steps)
        //            {
        //                step.files.Clear();
        //                foreach (var image in step.StepImages)
        //                {
        //                    File file = new File();
        //                    BitmapImage bitmap = GetBitmapImage(image);
        //                    if (bitmap != null)
        //                    {
        //                        file.height = bitmap.PixelHeight;
        //                        file.width = bitmap.PixelWidth;
        //                    }
        //                    file.mediumUrl = image;
        //                    file.imageNotes = null;
        //                    step.files.Add(file);
        //                }
        //            }
        //        }
        //    }
        //}

        //   private BitmapImage GetBitmapImage(string name)
        //   {
        //       foreach (StorageFile file in _stepImages)
        //       {
        //           if (file.DisplayName == name || file.Name == name)
        //           {
        //               var bitmapImage = new BitmapImage();
        //               var stream = file.GetThumbnailAsync(ThumbnailMode.PicturesView, 120, ThumbnailOptions.UseCurrentScale).AsTask();
        //               stream.Wait();
        //               bitmapImage.SetSource(stream.Result);
        //               return bitmapImage;
        //           }
        //       }
        //       return null;
        //   }

        //   private byte[] imageBytes;
        //   private async Task<bool> GetByteImage(string name)
        //   {
        //       foreach (StorageFile file in _stepImages)
        //       {
        //           if (file.DisplayName == name || file.Name == name)
        //           {
        //               using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
        //               {
        //                   var dataReader = new DataReader(stream.GetInputStreamAt(0));
        //                   uint size = (uint)stream.Size;
        //                   await dataReader.LoadAsync(size);

        //                   imageBytes = new byte[size];
        //                   dataReader.ReadBytes(imageBytes);
        //                   return true;
        //               }
        //           }
        //       }
        //       return false;
        //   }

        //   public async Task<StorageFolder> getInstructableFolder(Instructable instructable)
        //   {
        //       StorageFolder ibles = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ibles", CreationCollisionOption.OpenIfExists);

        //       instructable.localPath = generateLocalPath(instructable);
        //       StorageFolder photosFolder = await ibles.CreateFolderAsync(instructable.localPath, CreationCollisionOption.OpenIfExists);

        //       return photosFolder;
        //   }

        //   public string generateLocalPath(Instructable instructable)
        //   {
        //       if (instructable.localPath != null && instructable.localPath != string.Empty)
        //       {
        //           return instructable.localPath;
        //       }
        //       else if (instructable.id != null && instructable.id != string.Empty)
        //       {
        //           return instructable.id;
        //       }
        //       else
        //       {
        //           return Guid.NewGuid().ToString();
        //       }
        //   }

        //   public async void SaveLocalInstructable()
        //   {
        //       StorageFolder ibles = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ibles", CreationCollisionOption.OpenIfExists);
        //       if (ibles != null)
        //       {
        //           _instructable.localPath = generateLocalPath(_instructable);
        //           StorageFolder folder = await ibles.CreateFolderAsync(_instructable.localPath, CreationCollisionOption.OpenIfExists);

        //           // 1. save instructable data
        //           StorageFile file = await folder.CreateFileAsync("instructable.json", CreationCollisionOption.ReplaceExisting);
        //           if (file != null)
        //           {
        //               try
        //               {
        //                   string json = SerializationHelper.Serialize<Instructable>(this._instructable);
        //                   await FileIO.WriteTextAsync(file, json);
        //               }
        //               catch (Exception ex)
        //               {
        //                   Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
        //               }
        //           }
        //           // 2. copy instructable images
        //           foreach (var image in StepImages)
        //           {
        //               if (!image.Path.StartsWith(folder.Path))
        //               {
        //                   await image.CopyAsync(folder, image.Name, NameCollisionOption.ReplaceExisting);
        //               }
        //           }
        //       }
        //   }

        //   public void LoadLocalInstructable(Instructable instructable)
        //   {
        //       string fullpath = string.Format(@"ibles\{0}", instructable.localPath);
        //       Task<StorageFolder> draftTask = ApplicationData.Current.LocalFolder.GetFolderAsync(fullpath).AsTask();
        //       draftTask.Wait();

        //       StorageFolder draft = draftTask.Result;

        //       Task<IReadOnlyList<StorageFile>> filesTask = draft.GetFilesAsync().AsTask();
        //       filesTask.Wait();

        //       var files = filesTask.Result;
        //       foreach (var file in files)
        //       {
        //           if (file.Name != "instructable.json")
        //           {
        //               StepImages.Add(file);
        //           }
        //       }
        //   }

        //   public async void DeleteLocalInstructable(Instructable instructable)
        //   {
        //       if (instructable.localPath == null || instructable.localPath == string.Empty)
        //           return;

        //       string fullpath = string.Format(@"ibles\{0}", instructable.localPath);
        //       StorageFolder draft = await ApplicationData.Current.LocalFolder.GetFolderAsync(fullpath);
        //       await draft.DeleteAsync();
        //   }

        //   public async void GetLocalInstructables(InstructableList instructableList)
        //   {
        //       try
        //       {
        //           StorageFolder ibles = await ApplicationData.Current.LocalFolder.GetFolderAsync("ibles");
        //           IReadOnlyList<StorageFolder> drafts = await ibles.GetFoldersAsync();

        //           var filo = drafts.Reverse();
        //           foreach (var draft in filo)
        //           {
        //               try
        //               {
        //                   var file = await draft.GetFileAsync("instructable.json");
        //                   string json = await FileIO.ReadTextAsync(file);
        //                   Instructable ible = SerializationHelper.Deserialize<Instructable>(json);
        //                   if (ible != null)
        //                       instructableList.instructables.Add(ible);
        //               }
        //               catch(Exception ex)
        //               {
        //               }
        //           }
        //       }catch(Exception ex)
        //       {

        //       }
        //   }
        //}
    }
}
