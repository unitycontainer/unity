// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.NUnit.UI;
using MonoTouch.UIKit;

namespace Microsoft.Practices.Unity.Tests.Xamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;
        private TouchRunner runner;

        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);
            this.runner = new TouchRunner(this.window);

            // register every tests included in the main application/assembly
            this.runner.Add(System.Reflection.Assembly.GetExecutingAssembly());

            this.window.RootViewController = new UINavigationController(this.runner.GetViewController());

            // make the window visible
            this.window.MakeKeyAndVisible();

            return true;
        }
    }
}