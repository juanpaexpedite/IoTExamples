using IoT.Common;
using IoT.Hardwares.Base;
using IoT.Models;
using IoT.ViewHardwares.Base;
using Potentiometer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;

namespace SPIChildsQuick.ViewHardwares
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
            MainHardware = new Hardware(new int[] { 18 });

            if (await MainHardware.InitializeGpioController() )
            {
                if (SpiStatus.Success == await MainHardware.InitalizeSpi(new MCP3002(), 0))
                {
                    Loop();
                }
                else
                {
                    CS0CH0 = 435;
                    FirstResistance = 4.248;
                    CS1CH0 = 679;
                    SecondResistance = 67.8;
                }
            }
        }

        private async void Loop()
        {
            if (MainHardware.SpiChipSelect == 0)
            {
                Hardware.Gpios[0].Write(GpioPinValue.High);
            }
            else
            {
                Hardware.Gpios[0].Write(GpioPinValue.Low);
            }

            MainHardware.ReadSpi(MainHardware.SpiChipSelect);
            Update();

            //MainHardware.SpiChipSelect = MainHardware.SpiChipSelect == 0 ? 1 : 0;

            MainHardware.SpiDevices[0].ConnectionSettings.ChipSelectLine = MainHardware.SpiChipSelect;

            await Task.Delay(1000);
            Loop();
        }

        private void Update()
        {
            if(MainHardware.SpiChipSelect == 0)
            {
                UpdateFirstResistance(MainHardware.SpiDigitalValue);

            }
            else if (MainHardware.SpiChipSelect == 1)
            {
                UpdateSecondResistance(MainHardware.SpiDigitalValue);
            }
        }

        #region FirstResistance
        private double cs0ch0;
        public double CS0CH0
        {
            get { return cs0ch0; }
            set
            {
                cs0ch0 = value;
                NotifyPropertyChanged();
            }
        }

        private double firstresistance;

        public double FirstResistance
        {
            get { return firstresistance; }
            set
            {
                firstresistance = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateFirstResistance(double value)
        {
            CS0CH0 = value;
            FirstResistance = DigitalToFirstResistance(value);
            Temperature = ResistanceToTemperature(FirstResistance);
        }

        private double DigitalToFirstResistance(double value)
        {
            var Vr = value * 5 / 1024;

            var I = Vr / 1000;
            var Vt = 5 - Vr;

            var Rt = Vt / I;

            return Rt;
        }

        private double temperature;
        public double Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                NotifyPropertyChanged();
            }
        }

        public Thermistor1K25C Thermistor = new Thermistor1K25C();
        private double ResistanceToTemperature(double value)
        {
            Thermistor.Resistance = value;
            return Thermistor.GetTemperatureCelsius();
        }
        #endregion

        #region SecondResistance
        private double cs1ch0;
        public double CS1CH0
        {
            get { return cs1ch0; }
            set
            {
                cs1ch0 = value;
                NotifyPropertyChanged();
            }
        }

        private double secondresistance;

        public double SecondResistance
        {
            get { return secondresistance; }
            set
            {
                secondresistance = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateSecondResistance(double value)
        {
            CS1CH0 = value;
            SecondResistance = DigitalToSecondResistance(value);
        }

        private double DigitalToSecondResistance(double value)
        {
            return value * 100.0 / 1024.0;
        }
        #endregion
    }
 
}
