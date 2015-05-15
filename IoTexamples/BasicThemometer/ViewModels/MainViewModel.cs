using BasicThemometer.Models;
using BasicThermometer.Common;
using BasicThermometer.HardwareViews;
using BasicThermometer.Models;
using BasicThermometer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace BasicThermometer.ViewModels
{
    public class MainViewModel : ViewModel
    {
        #region Thermometer
        private ThermometerModel thermometer = new ThermometerModel() { FirstPin = 18, SecondPin= 23};

        public ThermometerModel Thermometer
        {
            get { return thermometer; }
            set { if (thermometer != value) { thermometer = value; NotifyPropertyChanged(); } }
        }
        #endregion

        #region MainHardwareView
        public MainHardwareView MainHardwareView { get; set; }
        #endregion

        public MainViewModel()
        {
            Values = new ObservableCollection<Value>();
            Initialize();
        }

        public ObservableCollection<Value> Values { get; set; }

        double last = -1;
        private async void Initialize()
        {
            MainHardwareView = new MainHardwareView();
            MainHardwareView.FirstPin = Thermometer.FirstPin;
            MainHardwareView.SecondPin = Thermometer.SecondPin;

            if (await MainHardwareView.InitializeComponent())
            {
                MainHardwareView.Initialize();

                DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
                timer.Tick += (s, t) =>
                {
                    Thermometer.Temperature =  MainHardwareView.Temperature;

                    Thermometer.AverageTemperature = MainHardwareView.AverageTemperature;
                    Thermometer.Linear = MainHardwareView.Linear;

                    if (last != Thermometer.Linear)
                    {
                        Values.Add(new Value { Key = Thermometer.AverageTemperature.ToString(), Val = Thermometer.Linear.ToString() });
                        last = Thermometer.Linear;
                    }

                };
                timer.Start();

               
            }
        }

        public class Value
        {
            public string Key { get; set; }
            public string Val { get; set; }
        }

    }
}
