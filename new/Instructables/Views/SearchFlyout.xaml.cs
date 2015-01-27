using System;
using System.Collections.Generic;
using System.IO;
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
using Windows.System;
using Windows.UI.Popups;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchFlyout : SettingsFlyout
    {
        public delegate void SearchHandle(String queryText);
        public SearchHandle searchHandle;

        public SearchFlyout()
        {
            this.InitializeComponent();
        }

        private void Query_Submitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (searchHandle != null)
                searchHandle.Invoke(SearchKeyWord.QueryText);
        }
        public void handleSearch(String queryText)
        {
            searchHandle -= handleSearch;
            if(queryText != string.Empty)
                Hide();
        }

        private  void OnLostFocus(object sender, RoutedEventArgs e)
        {
          //  Hide();
        }

        public void clearInput()
        {
            SearchKeyWord.QueryText = string.Empty;
        }


    }
    
}
