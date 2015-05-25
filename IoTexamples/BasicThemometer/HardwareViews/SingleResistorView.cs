using BasicThermometer.Common;
using BasicThermometer.HardwareViews.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;

namespace BasicThemometer.HardwareViews
{
    public class SingleResistorView : HardwareView
    {
        #region Standard Implementation
        public SingleResistorView()
        {

        }

        #region Status
        public GpioStatus status = GpioStatus.Default;
        public GpioStatus Status
        {
            get { return status; }
            set { if (status != value) { status = value; NotifyPropertyChanged(); } }
        }
        #endregion

        #region Controller
        private GpioController gpiocontroller;
        public GpioController GpioController
        {
            get
            {
                return gpiocontroller;
            }
            set
            {
                if (gpiocontroller != value)
                {
                    gpiocontroller = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Gpios and Pins
        public GpioPin Gpio;
        public GpioPin ReadGpio;
        public Int32 Pin { get; set; }
        public Int32 ReadPin { get; set; }


        public GpioStatus InitializeGpios()
        {
            Gpio = InitGpioOutput(Pin);
            if (Gpio == null)
                return GpioStatus.NoPin;

            ReadGpio = InitGpioInput(ReadPin);
            if (ReadGpio == null)
                return GpioStatus.NoPin;

            return GpioStatus.Success;
        }

        public GpioPin InitGpioOutput(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);

            return pin;
        }

        public GpioPin InitGpioInput(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Input);
            return pin;
        }
        #endregion


        public async Task<bool> InitializeComponent()
        {
            var devicefamily = GetForCurrentView().QualifierValues["DeviceFamily"];
            if (devicefamily != "Universal")
                return false;

            var controller = GpioController.GetDefault();

            if (controller == null)
            {
                Gpio = null;
                Status = GpioStatus.NoGPIO;
            }
            else
            {
                Status = GpioStatus.Initialized;
                await Task.Delay(200);
                GpioController = controller;
                Status = InitializeGpios();
                await Task.Delay(200);
            }

            Specific();

            return true;
        }

        #endregion Standard Implementation


        Stopwatch pulseLength;
        public void Specific()
        {
            pulseLength = new Stopwatch();
            ReadGpio.ValueChanged += Gpio_ValueChanged;
            Loop();
        }

        private async void Loop()
        {
            Charge();
            await Task.Delay(250);
            Discharge();
            await Task.Delay(250);
            Loop();
        }

        public Action<double> Update { get; set; }

        private async void Gpio_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if(discharging)
            {
                pulseLength.Stop();
                discharging = false;
                await App.rootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (Update != null)
                        Update.Invoke(pulseLength.Elapsed.TotalMilliseconds);
                });
            }

        }

        bool discharging = false;
        private void Discharge()
        {
            pulseLength.Restart();
            Gpio.Write(GpioPinValue.Low);
            discharging = true;
        }
        
        
        private void Charge()
        {
            Gpio.Write(GpioPinValue.High);
        }
    }

    public class Thermistor
    {
        public double Resistance;

        private double A;
        private double B;
        private double C;

        public Thermistor(double a, double b, double c)
        {
            A = a; B = b; C = c;
        }

        public double GetTemperatureCelsius()
        {
            var lnR = Math.Log(Resistance);
            var invTemperature = A + B * lnR + C * Math.Pow(lnR, 3);
            return 1 / invTemperature - 273.15;
        }

        public double GetTemperatureKelvin()
        {
            return GetTemperatureCelsius() + 273.15;
        }

        public double GetTemperatureFarenheit()
        {
            return GetTemperatureCelsius() * 1.8 + 32.00;
        }
    }

    public class RCCircuit
    {
        public static double GetResistor(double milliseconds)
        {
            return 386.92 * Math.Pow(milliseconds, 1.1909);

        }
    }
}
