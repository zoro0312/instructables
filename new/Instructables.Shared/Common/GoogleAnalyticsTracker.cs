using GoogleAnalytics;
using GoogleAnalytics.Core;
using System;
using Windows.UI.Xaml.Data;

namespace Instructables.Common
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class GoogleAnalyticsTracker
    {
        static public Tracker Tracker
        { 
            get
            {
                return GoogleAnalytics.EasyTracker.GetTracker();            
            }
        }

        //static public bool _GAGlobalSwitch = true;
        
        static public void SendEvent(string Category, string Action, string Lable)
        {
            if (SessionManager.IsGAEnable() == false)
                return;
            Tracker.SendEvent(Category, Action, Lable, 0);
        }
    }
}
