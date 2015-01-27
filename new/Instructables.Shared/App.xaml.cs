using Instructables.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Instructables.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
//using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
//using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GoogleAnalytics.Core;
using Windows.Storage;
using Instructables.DataModel;
using Instructables.DataServices;
using System.Diagnostics;
//using Facebook.Client;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace Instructables
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    /// 

    public class Constants
    {
        public static readonly string FacebookAppId = "840371729341091";
    }

    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

        internal static string AccessToken = String.Empty;
        internal static string FacebookId = String.Empty;
        public static bool isAuthenticated = false;
        //public static FacebookSessionClient FacebookSessionClient = new FacebookSessionClient(Constants.FacebookAppId);
        private static string _apiKey = "%fj)t6dWQpOj2FGJfgknf&cFcPq";
        public static string ApiKey
        {
            get 
            {
                return _apiKey;
            }
        }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                /*rootFrame.Background = new ImageBrush()
                    {
                        Stretch = Stretch.UniformToFill,
                        ImageSource = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/DesignerBackground.jpg") }
                    };*/

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                await SessionManager.ReadSession();

                // Hide the status bar
                StatusBar statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();

                if (SessionManager.IsShowLicenseAgreement() == true)
                {
                    if (!rootFrame.Navigate(typeof(LicenseAgreement)))
                    {
                        throw new Exception("Failed to create LicenseAgreement page");
                    }
                }
                else if (SessionManager.IsFirstTime() == true)
                {
                    if (!rootFrame.Navigate(typeof(WalkThroughs)))
                    {
                        throw new Exception("Failed to create WalkThroughs page");
                    }
                } 
                else 
                {
                    if (!rootFrame.Navigate(typeof(LandingPage), "init"))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }

#else
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                await SessionManager.ReadSession();
                string launchParam = "AllGroups";
                if (args.TileId != "App")
                    launchParam = args.TileId;
                if (!rootFrame.Navigate(typeof(LandingPage), launchParam))
                {
                    throw new Exception("Failed to create initial page");
                }
#endif
            }
            // Ensure the current window is active
            //if (args.PreviousExecutionState == ApplicationExecutionState.Running && args.Kind == ActivationKind.Launch)
            //    rootFrame.Navigate(typeof (InstructableDetail), args.TileId);
            //else
                Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }


        //public async void onPrivacyCommand(IUICommand command)
        //{
        //    var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("http://usa.autodesk.com/adsk/servlet/item?siteID=123112&id=21292079",
        //                                                       UriKind.Absolute));
        //}
        //Ethan temp
        /*void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
         {
             UICommandInvokedHandler privacyHandler = new UICommandInvokedHandler(onPrivacyCommand);

             SettingsCommand privacyCommand = new SettingsCommand("Privacy Statement", "Privacy Statement", privacyHandler);
             eventArgs.Request.ApplicationCommands.Add(privacyCommand);

         }

         protected override void OnWindowCreated(WindowCreatedEventArgs args)
         {
             base.OnWindowCreated(args);
             SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
             Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted += App_QuerySubmitted;
             SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
         }

         void App_QuerySubmitted(Windows.ApplicationModel.Search.SearchPane sender, Windows.ApplicationModel.Search.SearchPaneQuerySubmittedEventArgs args)
         {
             var previousContent = Window.Current.Content;
             var frame = previousContent as Frame;
             frame.Navigate(typeof(SearchResultsPage), args.QueryText);
         }*/

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                /*frame.Background = new ImageBrush()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/DesignerBackground.jpg") }
                };*/
                Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await Common.SuspensionManager.RestoreAsync();
                    }
                    catch (Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Handle OnActivated event to deal with File Open/Save continuation activation kinds
        /// </summary>
        /// <param name="e">Application activated event arguments, it can be casted to proper sub-type based on ActivationKind</param>
        /// 
        ContinuationManager continuationManager;
       
        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            continuationManager = new ContinuationManager();

            await RestoreStatusAsync(e.PreviousExecutionState);

            Frame currentFrame = Window.Current.Content as Frame;

            var DataService = InstructablesDataService.DataServiceSingleton;
            await DataService.EnsureLogin();

            if (currentFrame.Content == null)
            {
                currentFrame.Navigate(typeof(LandingPage));
            }

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                // Call ContinuationManager to handle continuation activation
                continuationManager.Continue(continuationEventArgs, currentFrame);
            }

            Window.Current.Activate();
        }
#endif
    }
}


