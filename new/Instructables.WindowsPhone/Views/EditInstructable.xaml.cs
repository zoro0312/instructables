using System;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.FileProperties;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.DataServices;
using Instructables.Utils;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditInstructable : Instructables.Common.LayoutAwarePage, IFileOpenPickerContinuable
    {
        
        //public class StepTemplateStruct
        //{
        //    public TextBox textBox = null;
        //    public TranslateTransform transform = null;
        //}

        //private List<StepTemplateStruct> _stepDataList = new List<StepTemplateStruct>();

        

        private sealed class ItemSelector : DataTemplateSelector
        {
            protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            {
                var step = item as Step;
                if (step.stepIndex == -1)
                {
                    return Current.Resources["overviewItemTemplate"] as DataTemplate;
                }

                else if (step.stepIndex == -2)
                {
                    return Current.Resources["publishItemTemplate"] as DataTemplate;
                }
                else
                {
                    return Current.Resources["stepItemTemplate"] as DataTemplate;
                }
            }
        }

        private static int selectedIndex = 0;

        private TextBox description;
        private TextBox keyword;

        private TranslateTransform scrollUpTranslateDesc = null;
        private TranslateTransform scrollUpTranslateKeyw = null;

        public static EditInstructable Current;

        public EditInstructable()
        {
            this.InitializeComponent();

            Current = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = DataContext as CreateViewModel;
            StepsView.ItemsSource = vm.VisualSteps;
            StepsView.ItemTemplateSelector = new ItemSelector();

            if (e.NavigationMode == NavigationMode.Back)
            {
                StepsView.SelectedIndex = selectedIndex;
            }
            else
            {
                StepsView.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property provides the group to be displayed.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            selectedIndex = StepsView.SelectedIndex;
            //selectedIndex = 0;
            //var vm = DataContext as CreateViewModel;
            //vm.CleanUpData();

            if(description != null)
            {
                RemoveInputPanelElement(description);
                scrollUpTranslateDesc.Y = 0;
            }

            if (keyword != null)
            {
                RemoveInputPanelElement(keyword);
                scrollUpTranslateKeyw.Y = 0;
            }
                
        }

        protected async override void OnHardwareBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;            
            if(CategoryMenu.Visibility == Visibility.Visible)
            {
                CategoryMenu.Visibility = Visibility.Collapsed;
            }
            else if (ChannelMenu.Visibility == Visibility.Visible)
            {
                ChannelMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageDialog dialog = new MessageDialog("", "");
                dialog.Title = "Leave?";
                dialog.Content = "Would you like to save the instructable before leaving?";
                dialog.Commands.Add(new UICommand("Save", async (command) =>
                {
                    var vm = DataContext as CreateViewModel;
                    await vm.SaveCloudInstructable();
                    this.GoBack(this, new RoutedEventArgs());
                }));
                dialog.Commands.Add(new UICommand("Discard", (command) =>
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_Create", "operation", "remove save");
                    this.GoBack(this, new RoutedEventArgs());
                }));
                await dialog.ShowAsync();
            }
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                while (this.Frame.CanGoBack &&
                       this.Frame.SourcePageType != typeof(LandingPage) &&
                       this.Frame.SourcePageType != typeof(UserProfilePage)&&
                       this.Frame.SourcePageType != typeof(DraftsListPage))
                    this.Frame.GoBack();
            }
        }

        private void AddPhoto_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //MessageDialog dialog = new MessageDialog("", "add photos from");

            //dialog.Commands.Add(new UICommand("camera", (command) =>
            //{
            //    takePhotos();
            //}));
            //dialog.Commands.Add(new UICommand("photo album", (command) =>
            //{
            //    pickPhotos();
            //}));

            //dialog.DefaultCommandIndex = 0;
            //dialog.CancelCommandIndex = 2;

            //await dialog.ShowAsync();

            var vm = DataContext as CreateViewModel;
            vm.IsSaving = true;
            LoadingPanel.Visibility = Visibility.Visible;            
            
            pickPhotos();
        }

        private void takePhotos()
        {
            var step = StepsView.SelectedItem as Step;
            if (step!= null)
            {
                this.Frame.Navigate(typeof(PhotoSequenceCapture), step);
            }
        }

        private void pickPhotos()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            // Launch file open picker and caller app is suspended and may be terminated if required
            openPicker.PickMultipleFilesAndContinue();
        }

        /// <summary>
        /// Handle the returned files from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            StepView_SelectionChanged(this, null); //Reactive the input pane show/hide events

            var step = StepsView.SelectedItem as Step;
            var vm = DataContext as CreateViewModel;
            if (vm != null && step != null && args.Files.Count > 0)
            {
                List<StorageFile> newImages = vm.AddStepImages(step, args.Files);
                await InstructablesDataService.DataServiceSingleton.UploadPhotos(step, newImages);
            }
            LoadingPanel.Visibility = Visibility.Collapsed;            
            vm.IsSaving = false;
        }

        private void DelPhotoButton_Click(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            var file = button.DataContext as File;
            string imageName =  file.name;

            var step = StepsView.SelectedItem as Step;
            var vm = DataContext as CreateViewModel;
            if (vm != null && step != null && imageName != null)
            {
                vm.DeleteStepImage(step, imageName);
            }

            e.Handled = true;
            StepsView.Focus(FocusState.Pointer); //Prevent focus goto description textbox.
        }

        private void AppBarOverviewButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;
            StepsView.SelectedIndex = vm.VisualSteps.Count - 2;
        }

        private void AppBarAddStepButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;
            var step = vm.CreateStep();
            StepsView.SelectedIndex = vm.VisualStepIndex(step);
            //StepTemplateStruct ts = new StepTemplateStruct();
            //_stepDataList.Add(ts);
        }

        private async void DelStepButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Permanently remove this step?", "Delete Step");
            dialog.Commands.Add(new UICommand("Delete", (command) =>
            {
                Button button = sender as Button;
                var step = button.DataContext as Step;

                var vm = DataContext as CreateViewModel;
                vm.DeleteStep(step);

                // stay at the overview step
                StepsView.SelectedIndex = vm.VisualSteps.Count - 2;
            }));
            dialog.Commands.Add(new UICommand("Cancel", (command) => { }));
            await dialog.ShowAsync();
        }

        private void Overview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;
            var grid = sender as GridView;
            var step = grid.SelectedItem as Step;
            int index = vm.VisualStepIndex(step);
            StepsView.SelectedIndex = index;
        }

        private void StepsView_Loaded(object sender, RoutedEventArgs e)
        {
            //var StepsView = sender as FlipView;
            //if (StepsView != null && _defferedSetIndex >= 0 && _defferedSetIndex < StepsView.Items.Count)
            //{
            //    var obj = StepsView.ContainerFromIndex(_defferedSetIndex);
            //    FlipViewItem item = obj as FlipViewItem;
            //    if (item != null)
            //    {
            //        Grid grid = item.ContentTemplateRoot as Grid;
            //        if (grid != null)
            //        {
            //            description = grid.FindName("Description") as TextBox;

            //            if (description != null)
            //            {
            //                scrollUpTranslateDesc = null;
            //                scrollUpTranslateDesc = grid.FindName("ScrollUpTranslate") as TranslateTransform;
            //                AddInputPanelElement(description);
            //            }

            //            keyword = grid.FindName("keywordInput") as TextBox;
            //            if (keyword != null)
            //            {
            //                scrollUpTranslateKeyw = null;
            //                scrollUpTranslateKeyw = grid.FindName("keywordInput") as TranslateTransform;
            //                AddInputPanelElement(keyword);
            //            }
            //        }
            //        _defferedSetIndex = -1;
            //    }
            //}
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InstructableDetail), "creating");
        }

        private void EnterContestsButton_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalyticsTracker.SendEvent("enter contest", "operation", "enter contest");
            this.Frame.Navigate(typeof(VotingContests), "creating");
        }

        private async void AppBarSaveComButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;
            vm.IsSaving = true;
            var result = await vm.SaveCloudInstructable();
            vm.IsSaving = false;

            MessageDialog dialog = new MessageDialog("", "");
            dialog.Title = "Saved!";
            if (result != null)
            {
                vm.Instructable.id = result.id;
                dialog.Content = "Your Instructable has been saved to Instructables.com and you will still need to publish it!";
            }
            else
            {
                dialog.Content = "Sorry, there're something wrong during saving to Instructables.com. Please try it later!";
            }

            dialog.Commands.Add(new UICommand("OK", (command) => { }));
            await dialog.ShowAsync();
        }

        private async void AppBarPublishButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;
            if (StepsView.SelectedIndex != vm.VisualSteps.Count - 1)
            {
                // go to publish
                StepsView.SelectedIndex = vm.VisualSteps.Count - 1;
            }
            else
            {
                // start to publish
                if (vm.Instructable.keywords.Count == 0)
                {
                    MessageDialog dialog = new MessageDialog("Please enter keywords.", "Keywords Required");
                    dialog.Commands.Add(new UICommand("OK", (command) => { return; }));
                    await dialog.ShowAsync();
                }
                else if (vm.Instructable.channel == null || vm.Instructable.channel == string.Empty)
                {
                    MessageDialog dialog = new MessageDialog("Please select category and channel for your Instructable.", "Error");
                    dialog.Commands.Add(new UICommand("OK", (command) => { return; }));
                    await dialog.ShowAsync();
                }
                else if (vm.Instructable.steps[0].body == null || vm.Instructable.steps[0].body == string.Empty)
                {
                    MessageDialog dialog = new MessageDialog("Please write the description for introducing your Instructable.", "Description Required");
                    dialog.Commands.Add(new UICommand("OK", (command) => { return; }));
                    await dialog.ShowAsync();
                }
                else if (vm.ThumbImages.Count == 0)
                {
                    MessageDialog dialog = new MessageDialog("Please add at least one image to your Instructable's intro step to publish it.", "Images Required");
                    dialog.Commands.Add(new UICommand("OK", (command) => { return; }));
                    await dialog.ShowAsync();
                }
                else
                {
                    SavingText.Text = "Publishing...";
                    bottomBar.Visibility = Visibility.Collapsed;
                    vm.IsSaving = true;
                    var result = await vm.PublishCloudInstructable();                    
                    vm.IsSaving = false;
                    bottomBar.Visibility = Visibility.Visible;
                    GoogleAnalyticsTracker.SendEvent("publish", "publish", "publish");
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Published!";
                    if (result != null)
                    {
                        dialog.Content = "Your Instructable has been published, but it may take 24 hours to appear while browsing.";
                    }
                    else
                    {
                        dialog.Content = "Sorry, there're something wrong during publishing to Instructables.com. Please try it later!";
                    }
                    dialog.Commands.Add(new UICommand("OK", (command) => {
                        if (result != null)
                        {
                            this.GoBack(this, new RoutedEventArgs());
                        }
                    }));
                    await dialog.ShowAsync();
                }
            }
        }

        //----------------------------------------------------------------------------
        //keyboard show hide handler
        private double displacement = 0;
        private InputPaneHelper inputPaneHelper = new InputPaneHelper();

        public void AddInputPanelElement(FrameworkElement element)
        {
            inputPaneHelper.SubscribeToKeyboard(true);
            inputPaneHelper.AddShowingHandler(element, new InputPaneShowingHandler(CustomKeyboardHandler));
            inputPaneHelper.SetHidingHandler(new InputPaneHidingHandler(InputPaneHiding));
        }

        public void RemoveInputPanelElement(FrameworkElement element)
        {
            inputPaneHelper.SubscribeToKeyboard(false);
            inputPaneHelper.RemoveShowingHandler(element);
            inputPaneHelper.SetHidingHandler(null);
        }

        private void CustomKeyboardHandler(object sender, InputPaneVisibilityEventArgs e)
        {
            // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
            // will move on its own. You should make sure the input element doesn't get occluded by the bar
            FrameworkElement element = sender as FrameworkElement;
            Point poppoint = element.TransformToVisual(this).TransformPoint(new Point(0, 0));
            displacement = 0.0;
            if (element.Name == "keywordInput")
                displacement = e.OccludedRect.Y - (poppoint.Y + element.ActualHeight);
                //displacement = - (Window.Current.Bounds.Height / 4);
            else
                displacement = e.OccludedRect.Y - (poppoint.Y + element.ActualHeight - 180);
                //displacement = - (Window.Current.Bounds.Height / 3);
            //bottomOfList = MiddleScroller.VerticalOffset + MiddleScroller.ActualHeight;


            // Be careful with this property. Once it has been set, the framework will
            // do nothing to help you keep the focused element in view.
            e.EnsuredFocusedElementInView = true;

            if (displacement > 0)
            {
                displacement = 0;
            }

            if (element.Name == "keywordInput")
            {
                if (scrollUpTranslateKeyw != null)
                {
                    scrollUpTranslateKeyw.Y = displacement;
                }
            }
            else
            {
                if (scrollUpTranslateDesc != null)
                {
                    scrollUpTranslateDesc.Y = displacement;
                }
            }
            
        }

        private void InputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            if (displacement != 0.0 && scrollUpTranslateDesc != null)
            {
                scrollUpTranslateDesc.Y = 0;
            }

            if (displacement != 0.0 && scrollUpTranslateKeyw != null)
            {
                scrollUpTranslateKeyw.Y = 0;
            }
        }

        private int _defferedSetIndex = -1;

        private void StepView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as CreateViewModel;

            if (StepsView.Items.Count != vm.Instructable.steps.Count + 2)
                return;
            if (StepsView.SelectedIndex == StepsView.Items.Count - 1)       // Publish Page
            {
                AddStep.Visibility = Visibility.Collapsed;
                Overview.Visibility = Visibility.Collapsed;
                SaveCloud.Visibility = Visibility.Collapsed;
            }
            else if (StepsView.SelectedIndex == StepsView.Items.Count - 2)  // Overview Page
            {
                AddStep.Visibility = Visibility.Visible;
                Overview.Visibility = Visibility.Collapsed;
                SaveCloud.Visibility = Visibility.Visible;
            }
            else
            {
                AddStep.Visibility = Visibility.Visible;
                Overview.Visibility = Visibility.Visible;
                SaveCloud.Visibility = Visibility.Visible;
            }

            if (description != null)
            {
                RemoveInputPanelElement(description);
                scrollUpTranslateDesc.Y = 0;
            }
            if(keyword != null)
            {
                RemoveInputPanelElement(keyword);
                scrollUpTranslateKeyw.Y = 0;
            }

            if (StepsView.SelectedIndex < 0)
                return;

            var obj = StepsView.ContainerFromIndex(StepsView.SelectedIndex);
            FlipViewItem item = obj as FlipViewItem;
            if (item != null)
            {
                Grid grid = item.ContentTemplateRoot as Grid;
                if (grid != null)
                {
                    description = grid.FindName("Description") as TextBox;
                    
                    if (description != null)
                    {
                        scrollUpTranslateDesc = null;
                        scrollUpTranslateDesc = grid.FindName("ScrollUpTranslate") as TranslateTransform;
                        AddInputPanelElement(description);
                    }

                    keyword = grid.FindName("keywordInput") as TextBox;
                    if (keyword != null)
                    {
                        scrollUpTranslateKeyw = null;
                        scrollUpTranslateKeyw = grid.FindName("ScrollUpTranslate") as TranslateTransform;
                        AddInputPanelElement(keyword);
                    }
                }
                _defferedSetIndex = -1;
            }
            else
            {
                _defferedSetIndex = StepsView.SelectedIndex;
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        // ------------------------------------------------------------------
        private string category;
        private string channel;
        private bool _isSelectingCategory = false;
        private bool _isSelectingChannel = false;
        private Button categoryButton;

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryMenuList.SelectedIndex = -1;
            _isSelectingCategory = true;
            categoryButton = sender as Button;
            categoryButton.Focus(FocusState.Pointer);
            CategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Channel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelectingChannel == false)
                return;

            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            var vm = DataContext as CreateViewModel;
            var evm = ViewModelLocator.Instance.ExploreVM;
            if (evm != null && categoryButton != null)
            {
                Category cate = evm.SelectedCollectionCategory;
                Channel ch = evm.SelectedChannel;
                category = cate.CategoryName;
                channel = ch.display;

                if (channel.ToLower() == "all")
                {
                    categoryButton.Content = category;
                }
                else
                {
                    categoryButton.Content = channel;
                }
                vm.Instructable.category = category;
                vm.Instructable.channel = channel;
            }
            _isSelectingChannel = false;
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelectingCategory == false)
                return;
            CategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            _isSelectingCategory = false;
            _isSelectingChannel = true;
        }

        private void OnKeywordInputLoaded(object sender, RoutedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;
            //AddInputPanelElement(textBox);
            //if (textBox.Name == "keywordInput")
            //    keyword = textBox;
        }

        private void OnDescriptionLoaded(object sender, RoutedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;
            //AddInputPanelElement(textBox);
            //if (textBox.Name == "Description")
            //{
            //    //if(StepsView.SelectedIndex < _stepDataList.Count)
            //    //{
            //    //    _stepDataList[StepsView.SelectedIndex].textBox = textBox;
            //    //}
                
            //    description = textBox;
            //}
                
        }

        private void OnGridLoaded(object sender, RoutedEventArgs e)
        {
            //var grid = sender as Grid;
            //if(grid != null)
            //{
            //    //scrollUpTranslateDesc = null;
            //    //scrollUpTranslateKeyw = null;
            //    if ( grid.FindName("Description") != null)
            //    {
            //        //if (StepsView.SelectedIndex < _stepDataList.Count)
            //        //{
            //        //    _stepDataList[StepsView.SelectedIndex].transform = grid.FindName("ScrollUpTranslate") as TranslateTransform;
            //        //}
            //        scrollUpTranslateDesc = grid.FindName("ScrollUpTranslate") as TranslateTransform;
            //    }
            //    else if (grid.FindName("keywordInput") != null)
            //    {
            //        scrollUpTranslateKeyw = grid.FindName("ScrollUpTranslate") as TranslateTransform;
            //    }
                
            //}
            if (StepsView != null && _defferedSetIndex >= 0 && _defferedSetIndex < StepsView.Items.Count)
            {
                var obj = StepsView.ContainerFromIndex(_defferedSetIndex);
                FlipViewItem item = obj as FlipViewItem;
                if (item != null)
                {
                    Grid grid = item.ContentTemplateRoot as Grid;
                    if (grid != null)
                    {
                        description = null;
                        description = grid.FindName("Description") as TextBox;

                        if (description != null)
                        {
                            scrollUpTranslateDesc = null;
                            scrollUpTranslateDesc = grid.FindName("ScrollUpTranslate") as TranslateTransform;
                            AddInputPanelElement(description);
                        }

                        keyword = null;
                        keyword = grid.FindName("keywordInput") as TextBox;
                        if (keyword != null)
                        {
                            scrollUpTranslateKeyw = null;
                            scrollUpTranslateKeyw = grid.FindName("ScrollUpTranslate") as TranslateTransform;
                            AddInputPanelElement(keyword);
                        }
                    }
                    _defferedSetIndex = -1;
                }
            }
        }

        private Color AddPhotoColorOriginal = Color.FromArgb(0xFF, 0x95, 0x95, 0x95);
        private Color AddPhotoColorNew = Color.FromArgb(0xFF, 0x5E, 0x5E, 0x5E);

        private void OnAddPhotoPressed(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush newBrush = new SolidColorBrush();
            newBrush.Color = AddPhotoColorNew;

            var grid = sender as Grid;
            if (grid != null)
            {
                Path Icon1 = grid.FindName("AddPhotoIcon1") as Path;
                if (Icon1 != null)
                {
                    Icon1.Fill = newBrush;
                }

                Ellipse Icon2 = grid.FindName("AddPhotoIcon2") as Ellipse;
                if (Icon2 != null)
                {
                    Icon2.Fill = newBrush;
                }

                Path Icon3 = grid.FindName("AddPhotoIcon3") as Path;
                if (Icon3 != null)
                {
                    Icon3.Fill = newBrush;
                }

                TextBlock textBlock = grid.FindName("AddPhotoText") as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Foreground = newBrush;
                }
            }
        }

        private void OnAddPhotoLoosed(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush originalBrush = new SolidColorBrush();
            originalBrush.Color = AddPhotoColorOriginal;

            var grid = sender as Grid;
            if (grid != null)
            {
                Path Icon1 = grid.FindName("AddPhotoIcon1") as Path;
                if (Icon1 != null)
                {
                    Icon1.Fill = originalBrush;
                }

                Ellipse Icon2 = grid.FindName("AddPhotoIcon2") as Ellipse;
                if (Icon2 != null)
                {
                    Icon2.Fill = originalBrush;
                }

                Path Icon3 = grid.FindName("AddPhotoIcon3") as Path;
                if (Icon3 != null)
                {
                    Icon3.Fill = originalBrush;
                }

                TextBlock textBlock = grid.FindName("AddPhotoText") as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Foreground = originalBrush;
                }
            }
        }

        private void Grid_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {

        }

        private void OnAddPhotoLost(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush originalBrush = new SolidColorBrush();
            originalBrush.Color = AddPhotoColorOriginal;

            var grid = sender as Grid;
            if (grid != null)
            {
                Path Icon1 = grid.FindName("AddPhotoIcon1") as Path;
                if (Icon1 != null)
                {
                    Icon1.Fill = originalBrush;
                }

                Ellipse Icon2 = grid.FindName("AddPhotoIcon2") as Ellipse;
                if (Icon2 != null)
                {
                    Icon2.Fill = originalBrush;
                }

                Path Icon3 = grid.FindName("AddPhotoIcon3") as Path;
                if (Icon3 != null)
                {
                    Icon3.Fill = originalBrush;
                }

                TextBlock textBlock = grid.FindName("AddPhotoText") as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Foreground = originalBrush;
                }
            }
        }

        private void OnAddPhotoReleased(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
