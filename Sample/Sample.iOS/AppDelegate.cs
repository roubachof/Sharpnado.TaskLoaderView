using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;

using ObjCRuntime;

using UIKit;

using Xamarin.Forms;

namespace Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            Xamarin.Forms.Forms.ViewInitialized += Forms_ViewInitialized;

            return base.FinishedLaunching(app, options);
        }

        private void Forms_ViewInitialized(object sender, Xamarin.Forms.ViewInitializedEventArgs e)
        {
            if (e.View is Page)
            {
                UpdateStatusBar();
            }
        }

        private void UpdateStatusBar()
        {
            var statusBarColor = UIColor.Black; // #2254A8

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                foreach (var window in UIApplication.SharedApplication.Windows)
                {
                    const int statusBarTag = 38482;

                    if (window.ViewWithTag(statusBarTag) != null) continue;

                    var statusBar = new UIView(UIApplication.SharedApplication.StatusBarFrame);
                    statusBar.Tag = 38482;
                    statusBar.BackgroundColor = statusBarColor;
                    window.AddSubview(statusBar);
                }
            }
            else
            {
                var statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
                if (statusBar.RespondsToSelector(new Selector("setBackgroundColor:")))
                {
                    statusBar.BackgroundColor = statusBarColor;
                }
            }
        }
    }
}
