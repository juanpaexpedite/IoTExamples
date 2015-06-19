using IoT.Common;
using IoT.Services;
using RelayAutomation.ViewHardwares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RelayAutomation.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();

            this.Loaded += (s, e) =>
            {
                if (SystemService.Family == Families.IoT)
                {
                    this.DataContext = new MainViewHardware();
                }
                else
                {
                    this.DataContext = new PCMainViewHardware();
                }
            };
        }

        private async void Switch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            await Task.Delay(5000);
            (sender as Button).IsEnabled = true;
        }
       
    }
}
