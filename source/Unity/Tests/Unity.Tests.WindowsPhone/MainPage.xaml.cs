using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.Unity.Tests.Resources;
using Microsoft.VisualStudio.TestPlatform.Core;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using vstest_executionengine_platformbridge;

namespace Microsoft.Practices.Unity.Tests
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var wrapper = new TestExecutorServiceWrapper();
            new Thread(new ServiceMain((param0, param1) => wrapper.SendMessage((ContractName)param0, param1)).Run).Start();
        }
    }
}