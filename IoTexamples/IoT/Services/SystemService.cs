using IoT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;

namespace IoT.Services
{
    public class SystemService
    {
        public static bool IsDesign => DesignMode.DesignModeEnabled;

        public static bool IsRuntime => !DesignMode.DesignModeEnabled;
        public static bool IsIoTDevice => GetForCurrentView().QualifierValues["DeviceFamily"] == "Universal";

        public static DisplayInformation DisplayInformation => DisplayInformation.GetForCurrentView();

        public static Frame DisplayFrame => Window.Current.Content == null ? null : Window.Current.Content as Frame;

        public static Families Family
        {
            get
            {
                var family = GetForCurrentView().QualifierValues["DeviceFamily"];

                if (family == "Mobile")
                    return Families.Mobile;
                else if (family == "Universal")
                    return Families.IoT;
                return Families.Desktop;
            }
        }
    }
}
