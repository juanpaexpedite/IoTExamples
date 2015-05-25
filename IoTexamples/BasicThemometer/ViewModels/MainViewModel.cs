using BasicThemometer.HardwareViews;
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

        public Thermistor Thermistor = new Thermistor(0.00288301, -0.00002028, 0.00000190);
        #endregion

        #region MainHardwareView
        public SingleResistorView MainHardwareView { get; set; }
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
            MainHardwareView = new SingleResistorView();
            MainHardwareView.Pin = Thermometer.FirstPin;
            MainHardwareView.ReadPin = Thermometer.SecondPin;
            MainHardwareView.Update = Refresh;


            if (await MainHardwareView.InitializeComponent())
            {
                MainHardwareView.Specific();
            }
        }

        int i = 0;
        double average = 0;
        int times = 40;
        public Action<double> Refresh => (value) =>
        {
            Thermistor.Resistance = RCCircuit.GetResistor(value);
            Thermometer.Temperature = Thermistor.GetTemperatureCelsius();

            average += Thermometer.Temperature;

            if (++i > (times-1))
            {
                average = average / times;
                Values.Insert(0,new Value { Key = "average", Val = average.ToString() });
                average = 0;
                i = 0;
            }

        };


        public class Value
        {
            public string Key { get; set; }
            public string Val { get; set; }
        }

    }
}
