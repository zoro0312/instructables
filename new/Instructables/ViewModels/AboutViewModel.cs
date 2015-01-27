using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;
using Instructables.DataModel;
using Windows.Storage;
using System.Diagnostics;

namespace Instructables.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        //private const string _aboutText = "<b>© 2014 Autodesk, Inc. All rights reserved.</b>        \r\n\r\n<b>PLEASE SEE DATA COLLECTION NOTICE AT END OF THIS ABOUT SECTION.</b>        \r\n\r\nAll use of this Software is subject to the Instructables Terms and Conditions of Use (<a href=\"http://www.instructables.com/tos.html\">http://www.instructables.com/tos.html</a>) \r\n\r\n<b>Trademarks</b>        \r\n\r\nAutodesk and Instructables are registered trademarks or trademarks of Autodesk, Inc., and/or its subsidiaries and/or affiliates. \r\n\r\nAndroid, Google Play, and Google are registered trademarks or service marks of Google Inc. \r\n\r\nAll other brand names, product names or trademarks belong to their respective holders. \r\n\r\n<b>Third-Party Software Credits and Attributions</b>        \r\n\r\n<b>AndroidQuery</b>        \r\n<a href=\"http://androidquery.com\">http://androidquery.com</a>        \r\nCopyright 2011 - AndroidQuery.com (tinyeeliu@gmail.com)    \r\n\r\n<b>PullToRefresh</b>        \r\n<a href=\"https://github.com/chrisbanes/Android-PullToRefresh\">https://github.com/chrisbanes/Android-PullToRefresh</a>        \r\nCopyright 2011, 2012 Chris Banes.     \r\n\r\n<b>ActionBarSherlock</b>        \r\n<a href=\"http://actionbarsherlock.com/\">http://actionbarsherlock.com/</a>        \r\nCopyright 2012 Jake Wharton \r\n\r\n<b>ViewPagerIndicator</b>        \r\n<a href=\"http://viewpagerindicator.com/\">http://viewpagerindicator.com/</a>        \r\nCopyright 2012 Jake Wharton     \r\nCopyright 2011 Patrik Åkerfeldt  \r\nCopyright 2011 Francisco Figueiredo Jr.    \r\n\r\n<b>Facebook SDK3.6 for Android</b>        \r\n<a href=\"https://developers.facebook.com/docs/android/\">https://developers.facebook.com/docs/android/</a>        \r\nCopyright 2010-present Facebook     \r\n\r\nAll licensed under the Apache License, Version 2.0 (the \"License\"); you may not use this file except in compliance with the License. You may obtain a copy of the License at \r\n\r\n<a href=\"http://www.apache.org/licenses/LICENSE-2.0\">http://www.apache.org/licenses/LICENSE-2.0</a>        \r\n\r\nUnless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: \r\n\r\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. \r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE  \r\n\r\n<b>DATA COLLECTION NOTICE</b>        \r\n\r\nAutodesk respects your privacy.  For details, please see our Privacy Statement located at <a href=\"http://usa.autodesk.com/privacy/\">http://usa.autodesk.com/privacy/</a> and specific Privacy Notices and Data Collection Notices in our applications. \r\n\r\nIn order to support ongoing product improvement efforts, Autodesk receives aggregated product usage information about this application.  This information consists of: product launch and closure which allows us to calculate length and number of application sessions; whether a connection was successful or failed; general geographic location (to city level); general application commands and operations used (e.g., which functions are used more often);  as well as general mobile device information such as OS version and device type. \r\n\r\nWe use the Google Analytics SDK, which collects information and then presents the data to us in aggregated form.  The Google Analytics software agent collects the following data: agent version, platform, SDK version, platform timestamp, API key (identifier for application), application version, device identifier, Model (non-iOS), manufacture (non-iOS) and OS version of device (non-iOS), session start/stop time, events, errors and page views (for example, clicking on buttons or links, and whether or not it worked, all of which is reported to us in aggregate), locale (specific location where a given language is spoken), time zone, and network status (WiFi, etc.). The device identifiers (if applicable), IMEI (if applicable), MAC address (if applicable), and platform are hashed to a Google ID.  Note that Google will see Internet Protocol (IP) address through the HTTP request to send information.  For clarity, Autodesk does not receive geo-location data (i.e. GPS coordinates). \r\n\r\nIf you would like to opt-out so that data is not sent to Google and then aggregated for Autodesk, please take the following steps depending on what type of device you are using.  iOS device: Go to the in-app settings and switch the in-application toggle to \"off\" to prevent data being sent to Google.  Alternatively, if the app does not have in-app settings, go to device settings and switch the in-application toggle to “off” to prevent data being sent to Google.   Android device:  Go to the in-app menu settings and uncheck the “approve data collection” box.  Note that changing the toggle to “off” (iOS) or unchecking the box (Android) mid-session will prevent data being sent to Google as of the next use session.  You can review Google’s privacy policy at <a href=\"www.google.com/policies/privacy/\">www.google.com/policies/privacy/.</a></string>";
        private StorageFile aboutTextFile = null;
        private string _aboutText = String.Empty;
        public string AboutText
        {
            get { return _aboutText; }
            set
            {
                _aboutText = value;
                OnPropertyChanged();
            }

        }
        
        public async void LoadAboutTextFile()
        {
            if(aboutTextFile == null)
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AboutText.txt"));
                try 
                { 
                    AboutText = await FileIO.ReadTextAsync(file);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }
                
            }
        }


    }
}
