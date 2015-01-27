using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.UI.Core;
using Instructables.DataServices;
using System.Diagnostics;
using Windows.UI.Popups;
//using Facebook.Client;
//using Facebook;
using Windows.Security.Authentication.Web;
using System.Threading.Tasks;
using Instructables.ViewModels;
using Instructables.DataModel;
using Instructables.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentsFlyout : SettingsFlyout
    {
        private static LoginFlyout _loginFlyout = null;
        public CommentsFlyout(string urlString)
        {
            this.InitializeComponent();
            _urlString = urlString;
        }

        private string _urlString;
        public string UrlString
        {
            set
            {
                _urlString = value;
            }
        }
        public static string _defferedText = String.Empty;

        private async Task LoadComments()
        {
            var dataService = InstructablesDataService.DataServiceSingleton;
            var vm = this.DataContext as InstructableDetailViewModel;
            CommentsListView.Visibility = Visibility.Collapsed;
            Comments.Visibility = Visibility.Collapsed;
            Edge.Visibility = Visibility.Collapsed;
            LoadingPanel2.Visibility = Visibility.Visible;
            if (_defferedText != String.Empty)
            {
                Comment comment = new Comment();
                comment.body = _defferedText;
                comment.instructableId = vm.SelectedInstructable.id;
                comment.IMadeIt = false;
                string jsonPara = SerializationHelper.Serialize<Comment>(comment);
                var result = await dataService.PostComment(jsonPara);
                if (result != null && result.isSucceeded)
                {
                    CommentText.Text = CommentText.Text.Remove(0, CommentText.Text.Count());
                }
            }

            if (vm != null && !String.IsNullOrEmpty(_urlString))
            {
                await vm.LoadComments(_urlString);
            }

            //add
            if(vm.CommentsList.Count <= 1)
            {
                CommentTitle.Text = "comment";
            }
            else
            {
                CommentTitle.Text = "comments";
            }

            if (vm.CommentsList.Count <= 0)
            {
                No_Comment.Visibility = Visibility.Visible;
            }
            else
            {
                No_Comment.Visibility = Visibility.Collapsed;
            }
            Edge.Visibility = Visibility.Visible;
            Comments.Visibility = Visibility.Visible;
            CommentsListView.Visibility = Visibility.Visible;
            LoadingPanel2.Visibility = Visibility.Collapsed;
            _defferedText = String.Empty;

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
                if (_loginFlyout == null)
                {
                    _loginFlyout = new LoginFlyout();
                }
                _loginFlyout.Show();
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
            if (e.Key == VirtualKey.Enter)
            {
                Comments.Focus(FocusState.Pointer);
                //SendComment();
            }
            else if (e.Key == VirtualKey.Back)
            {
                if (CommentText.Text.Length <= 1)
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
            if (itemTemplate != null && Comment != null)
            {
                (Window.Current.Content as Frame).Navigate(typeof(UserProfilePage), Comment.author);
            }
            this.Hide();
        }

        private async void OnCommentsLoaded(object sender, RoutedEventArgs e)
        {
            await LoadComments();
        }

        private void OnCommentsUnloaded(object sender, RoutedEventArgs e)
        {
            InstructableDetailViewModel vm = this.DataContext as InstructableDetailViewModel;
            if (vm.CommentsList != null)
                vm.CommentsList = null;
        }
    }
}
