using Common;
using Hardwares.Base;
using Potentiometer.Hardwares;
using Potentiometer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewHardwares.Base;
using Windows.UI;
using Windows.UI.Xaml;

namespace Potentiometer.ViewHardwares
{
    public class MainViewHardware : ViewHardware
    {
        #region MainHardware
        private Hardware mainhardware;
        public Hardware MainHardware
        {
            get { return mainhardware; }
            set
            {
                mainhardware = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public MainViewHardware()
        {
            Initialize();
        }

        private async void Initialize()
        {
            MainHardware = new Hardware();

            if (SpiStatus.Success == await MainHardware.InitalizeSpi(new MCP3002(), 0))
            {
                DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
                dt.Tick += (s, e) =>
                {
                    MainHardware.ReadSpi();
                    Update();

                    MainHardware.SpiChannel = MainHardware.SpiChannel== 0 ? 1 : 0;
                };
                dt.Start();
             
            }
        }

        private void Update()
        {
            if(MainHardware.ADConverter.Channel == 0)
            {
                UpdateResistance(MainHardware.SpiDigitalValue);

            }
            else if (MainHardware.ADConverter.Channel == 1)
            {
                UpdateMoisture(MainHardware.SpiDigitalValue);
            }
        }

        #region Resistance
        private double ch0;
        public double CH0
        {
            get { return ch0; }
            set
            {
                ch0 = value;
                NotifyPropertyChanged();
            }
        }

        private double resistance;

        public double Resistance
        {
            get { return resistance; }
            set
            {
                resistance = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateResistance(double value)
        {
            CH0 = value;
            Resistance = DigitalToResistance(value);
        }

        private double DigitalToResistance(double value)
        {
            return value * 10.0 / 1024.0;

        }
        #endregion

        #region Moisture Sensor
        

        private double ch1;
        public double CH1
        {
            get { return ch1; }
            set
            {
                ch1 = value;
                NotifyPropertyChanged();
            }
        }

        private double humidity;

        public double Humidity
        {
            get { return humidity; }
            set
            {
                humidity = value;
                NotifyPropertyChanged();
            }
        }

        private double DigitalToHumidity(double value)
        {
            if (value > 340)
                return 100;
            else
                return value * 100 / 340;

        }

        private double level;

        public double Level
        {
            get { return level; }
            set
            {
                level = value;
                NotifyPropertyChanged();
            }
        }

        private double DigitalToLevel(double value)
        {
            double min = 500.0;
            double max = 715.0;

            if (value < min)
                return 0;
            else
            {
                double b = 100.0 / (1.0 - max / min);
                double a = -b / min;


                return value * a + b;
            }
        }

        public void UpdateMoisture(double value)
        {
            CH1 = value;
            Humidity = DigitalToHumidity(value);
            Level = DigitalToLevel(value);
        }
        #endregion
    }
}
