using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TestingSDCard.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestingSDCard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ThirdCase();
        }

        public string Temperature = String.Empty;

        public void SimpleCase()
        {
            DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
            dt.Tick += async (s, e) =>
            {
                var file = await DataLoggerService.FindFile($"data_{DataLoggerService.GetDateTime()}.dat", true);

                if (file != null)
                {
                    await DataLoggerService.WriteFile(file, $"{Temperature}_{DataLoggerService.GetDateTime()}");
                }
            };
            dt.Start();
        }

        public async void SecondCase()
        {
            DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

            var file = await DataLoggerService.FindFile($"data_{DataLoggerService.GetDate()}.dat", true);
            dt.Tick += async (s, e) =>
            {
                if (file != null)
                {
                    await DataLoggerService.AddContentToFile(file, $"{Temperature}_{DataLoggerService.GetDateTime()}");
                }
            };
            dt.Start();
        }

        public async void ThirdCase()
        {

            
            var path =await DataLoggerService.CreateDatabase("db.dat");

            var connection = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);

        }

    }

    public class TemperatureModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Lightness { get; set; }
        public double Humidity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
