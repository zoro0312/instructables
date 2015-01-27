using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Instructables.Common;

namespace Instructables.DataModel
{
    public class UserProfile
    {
        public string id { get; set; }
        public string screenName { get; set; }
        public string tinyUrl { get; set; }
        public string square2Url { get; set; }
        public string square3Url { get; set; }
        public int age { get; set; }
        public string zip { get; set; }
        public string signup { get; set; }

        private string _about = string.Empty;
        public string about { 
            get {
                return System.Net.WebUtility.HtmlDecode(_about);
            }
            set
            {
                _about = value;
            }
        }
        public string interests { get; set; }
        public string location { get; set; }
        public string gender { get; set; }

        public int views { get; set; }
        public int featuredCount { get; set; }
        public int followersCount { get; set; }
        public int subscriptionsCount { get; set; }
        public int instructablesCount { get; set; }
        public int favoritesCount { get; set; }
        public int draftsCount { get; set; }

        public ObservableCollection<Author> subscriptions { get; set; }
        public ObservableCollection<Author> subscriptionsForLoginUser { get; set; }
        public ObservableCollection<Author> followers { get; set; }
        public ObservableCollection<Instructable> favorites { get; set; }
        public ObservableCollection<Instructable> instructables { get; set; }
        public ObservableCollection<Instructable> drafts { get; set; }

        public string avatarId { get; set; }
    }
}
