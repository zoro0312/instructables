using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using Instructables.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentsPage : Instructables.Common.LayoutAwarePage
    {
        public CommentsPage()
        {
            try 
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return;
            }

            Comments.IsEnabled = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                if (vm.CommentsList != null)
                    vm.CommentsList = null;
                _defferedText = String.Empty;
            }
        }

        private string _urlString;
        public static string _defferedText = String.Empty;

        async protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _urlString = navigationParameter as string;
            await LoadComments();
        }

        private async Task LoadComments()
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            var vm = this.DataContext as InstructableDetailViewModel;
            CommentsListView.Visibility = Visibility.Collapsed;
            BottomAppBar.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            CommentText.Text = _defferedText;
            if (_defferedText != String.Empty && SessionManager.IsLoginSuccess() == true)
            {
                Comment comment = new Comment();
                comment.body = _defferedText;
                comment.instructableId = vm.SelectedInstructable.id;
                comment.IMadeIt = false;
                string jsonPara = SerializationHelper.Serialize<Comment>(comment);
                var result = await dataService.PostComment(jsonPara);
                if (result.isSucceeded)
                {
                    CommentText.Text = CommentText.Text.Remove(0, CommentText.Text.Count());
                }
            }

            if (vm != null && !String.IsNullOrEmpty(_urlString))
            {
                await vm.LoadComments(_urlString);
            }

            if (vm.CommentsList.Count <= 0)
            {
                No_Comment.Visibility = Visibility.Visible;
            }
            else
            {
                No_Comment.Visibility = Visibility.Collapsed;
            }


            CommentsListView.Visibility = Visibility.Visible;
            BottomAppBar.Visibility = Visibility.Visible;
            LoadingPanel.Visibility = Visibility.Collapsed;
           
        }

        private void AppBarButton_Comments_Click(object sender, RoutedEventArgs e)
        {
            SendComment();
        }

        private async void SendComment()
        {
            if (CommentText.Text.Count() == 0)
                return;
            var dataService = InstructablesDataService.DataServiceSingleton;
            bool ifLogin = await dataService.EnsureLogin();
            if (ifLogin != true)
            {
                _defferedText = CommentText.Text;
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                Comments.IsEnabled = false;

                InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
                //string jsonPara = "{approved:false,body:" + CommentText.Text + ",children:[],deleted:false,files:[],IMadeIt:false,images:[],instructableId:" + vm.SelectedInstructable.id + ",limbo:false,quarantine:false,sticky:false}";
                Comment comment = new Comment();
                comment.body = CommentText.Text;
                comment.instructableId = vm.SelectedInstructable.id;
                comment.IMadeIt = false;
                string jsonPara = SerializationHelper.Serialize<Comment>(comment);
                //string jsonPara = "{body:" + CommentText.Text + ",IMadeIt:false,files:[],instructableId:" + vm.SelectedInstructable.id + "}";
                var result = await dataService.PostComment(jsonPara);
                GoogleAnalyticsTracker.SendEvent("comment", "comment", "textComments");
                if (result.isSucceeded)
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_comment", "operation", "succeed");
                    CommentText.Text = CommentText.Text.Remove(0, CommentText.Text.Count());
                    await LoadComments();
                }
                else
                {
                    GoogleAnalyticsTracker.SendEvent("Ible_comment", "operation", "error");
                }
            }
           
        }

        private void CommentText_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if( e.Key == VirtualKey.Enter )
            {
                Comments.Focus(FocusState.Pointer);
                SendComment();
            }
            else if(e.Key == VirtualKey.Back)
            {
                if(CommentText.Text.Length<=1)
                {
                    Comments.IsEnabled = false;
                }
            }
            else
            {
                if (CommentText.Text.Length >= 0)
                {
                    Comments.IsEnabled = true;
                }
            }
            /*if(CommentText.Text.Length>=0)
            {
                Comments.IsEnabled = true;
                
            }
            else
            {
                Comments.IsEnabled = false;
            }*/
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            //var item = sender;
            var item = sender as DependencyObject;
            ContentPresenter itemTemplate = null;
            do
            {
                var item2 = VisualTreeHelper.GetParent(item);
                item = item2;
                itemTemplate = item as ContentPresenter;
                if (itemTemplate != null)
                {
                    break;
                }
            } while (item != null);

            var Comment = itemTemplate.Content as Comment;
            if (itemTemplate != null && Comment!=null)
            {
                var userProfileVM = ViewModelLocator.Instance.UserProfileVM;
                //List<string> param = new List<string>();
                //param.Add(Comment.author);
                //userProfileVM.InitData = param;
                var param = new UserProfileViewModel.ProfileInitData();
                param.screenName = Comment.author;
                userProfileVM.InitData.Add(param);
                userProfileVM.CurrentInitData += 1;
                this.Frame.Navigate(typeof(UserProfilePage));
            }
        }

    }
}
