using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
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
using Windows.Storage;
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
    public sealed partial class WalkThroughs : Instructables.Common.LayoutAwarePage
    {
        private DispatcherTimer EnableTimer = new DispatcherTimer();
        private DispatcherTimer DisableTimer = new DispatcherTimer();
        private double ENABLE_TIME_GAP = 500.0;
        private double DISABLE_TIME_GAP = 200.0;
        private DispatcherTimer AnimationTimer = new DispatcherTimer();
        private double ANIMATION_TIME_GAP = 2000.0;
        private DispatcherTimer AnimationDelayTimer = new DispatcherTimer();
        private double ANIMATION_DELAY_GAP = 500.0;
        private bool animationFlag = false;
        private DispatcherTimer GobackTimer = new DispatcherTimer();
        //private double GOBACK_TIME_GAP = 500.0;
        public WalkThroughs()
        {
                this.InitializeComponent();
                TimeSpan enableTimeGap = TimeSpan.FromMilliseconds(ENABLE_TIME_GAP);
                TimeSpan disableTimeGap = TimeSpan.FromMilliseconds(DISABLE_TIME_GAP);
                EnableTimer.Interval = enableTimeGap;
                DisableTimer.Interval = disableTimeGap;
                EnableTimer.Tick += (sender, args) =>
                {
                    AnimationTimer.Stop();
                    EnableTimer.Stop();
                    WalkThroughPages.IsEnabled = false;
                    DisableTimer.Start();
                };

                DisableTimer.Tick += (sender, args) =>
                {
                    AnimationTimer.Start();
                    DisableTimer.Stop();
                    WalkThroughPages.IsEnabled = true;
                    animationFlag = true;
                };

                TimeSpan animationTimeGap = TimeSpan.FromMilliseconds(ANIMATION_TIME_GAP);
                AnimationTimer.Interval = animationTimeGap;
                AnimationTimer.Tick += (sender, args) =>
                {
                    AnimationTimer.Stop();
                    //WalkThroughPages.IsEnabled = false;
                    if (WalkThroughPages.SelectedIndex <= WalkThroughPages.Items.Count - 2)
                    {
                        WalkThroughPages.SelectedIndex += 1;
                    }
                    else
                    {
                        WalkThroughPages.SelectedIndex = 1;
                    }
                    AnimationDelayTimer.Start();
                    WalkThroughPages.IsEnabled = false;
                };

                TimeSpan animationDelayTimeGap = TimeSpan.FromMilliseconds(ANIMATION_DELAY_GAP);
                AnimationDelayTimer.Tick += (sender, args) =>
                {
                    AnimationDelayTimer.Stop();
                    WalkThroughPages.IsEnabled = true;
                    AnimationTimer.Start();
                };
                /*bool? result = new bool?();
                try 
                {
                    result = true;
                    WalkThroughPages.IsSynchronizedWithCurrentItem = result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                    return;
                }*/
                WalkThroughPages.SelectedIndex = 1;
                //WalkThroughPages.Focus(FocusState.Unfocused);
                //WalkThroughPages.IsEnabled = false;
                AnimationTimer.Start();
                animationFlag = true;

            
        }


        protected override void OnHardwareBackPressed(object sender, BackPressedEventArgs e)
        {
            return;
            //e.Handled = true;
            //GoBack(this, new RoutedEventArgs());
        }

        private void OnPageChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private string GetPageKeyword(int index)
        {
            string result = "";
            switch (WalkThroughPages.SelectedIndex)
            {
                case 0:
                    result = "music";
                    break;
                case 1:
                    result = "costumes";
                    break;
                case 2:
                    result = "cloths";
                    break;
                case 3:
                    result = "furniture";
                    break;
                case 4:
                    result = "engines";
                    break;
                case 5:
                    result = "dinner";
                    break;
                case 6:
                    result = "music";
                    break;
                case 7:
                    result = "costumes";
                    break;
            }

            return result;
        }

        private bool jumpFlag = false;

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WalkThroughPages == null)
                return;
            //WalkThroughPages.IsEnabled = true;
            PageCategory.Text = GetPageKeyword(WalkThroughPages.SelectedIndex);
            if(WalkThroughPages.SelectedIndex == 0)
            {
                jumpFlag = true;
                WalkThroughPages.SelectedIndex = WalkThroughPages.Items.Count - 2;
            }
            else if(WalkThroughPages.SelectedIndex == 1)
            {
                if (jumpFlag == false)
                {
                    LastPage.Visibility = Visibility.Visible;
                    FirstPage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    jumpFlag = false;
                }
            }
            else if(WalkThroughPages.SelectedIndex == WalkThroughPages.Items.Count - 1)
            {
                jumpFlag = true;
                if (animationFlag == false)
                {
                    WalkThroughPages.SelectedIndex = 1;
                }
                /*else
                {
                    AnimationTimer.Stop();
                    AnimationDelayTimer.Start();
                }*/
                    
            }
            else if(WalkThroughPages.SelectedIndex == WalkThroughPages.Items.Count - 2)
            {
                if (jumpFlag == false)
                {
                    LastPage.Visibility = Visibility.Collapsed;
                    FirstPage.Visibility = Visibility.Visible;
                }
                else
                {
                    jumpFlag = false;
                }
            }
        }

        private async Task UpdateSettings()
        {
            SessionManager.NotFirstTime();
            await SessionManager.WriteSession();
        }

        private async void StartExploring(object sender, RoutedEventArgs e)
        {
            await UpdateSettings();
            this.Frame.Navigate(typeof(LandingPage), "init");
        }

        private async void SearchKeyword(object sender, RoutedEventArgs e)
        {
            await UpdateSettings();
            string keyWord = GetPageKeyword(WalkThroughPages.SelectedIndex);
            this.Frame.Navigate(typeof(SearchResultsPage), keyWord);
        }


        private void pointerCapterLost(object sender, PointerRoutedEventArgs e)
        {
            //WalkThroughPages.IsEnabled = false;
            //DisableTimer.Start();
            EnableTimer.Start();
            animationFlag = false;
        }

    }
}
