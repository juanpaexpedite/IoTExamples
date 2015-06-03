using BasicThermometer.HardwareViews.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Devices.Gpio;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;
using BasicThermometer.Common;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace BasicThemometer.HardwareViews
{
    public class SecondHardwareView : HardwareView
    {
        public SecondHardwareView()
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
        public GpioPin FirstGpio;
        public GpioPin SecondGpio;

        public Int32 FirstPin { get; set; }
        public Int32 SecondPin { get; set; }

        public GpioStatus InitializeGpios()
        {
            FirstGpio = InitGpio(FirstPin);
            if (FirstGpio == null)
                return GpioStatus.NoPin;

            SecondGpio = InitGpio(SecondPin);
            if (SecondGpio == null)
                return GpioStatus.NoPin;

            return GpioStatus.Success;
        }

        public GpioPin InitGpio(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);

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
                FirstGpio = null; SecondGpio = null;
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

            return true;
        }

        //public void Charge()
        //{
        //    FirstGpio.SetDriveMode(GpioPinDriveMode.Output);
        //    SecondGpio.SetDriveMode(GpioPinDriveMode.Input);

        //    FirstGpio.Write(GpioPinValue.High);
        //}

            public Action Update { get; set; }

        Stopwatch pulseLength;
        bool firstime = true;
        bool charging = false;
        public void Charge()
        {
            charging = true;

            pulseLength = new Stopwatch();
            pulseLength.Start();
            FirstGpio.SetDriveMode(GpioPinDriveMode.Output);
            SecondGpio.SetDriveMode(GpioPinDriveMode.Input);

            if (firstime)
            {
                SecondGpio.ValueChanged += SecondGpio_ValueChanged;
                firstime = false;
            }
            FirstGpio.Write(GpioPinValue.High);

        }

        private async void SecondGpio_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (charging)
            {
                pulseLength.Stop();

                await App.rootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                 {
                     Temperature = pulseLength.Elapsed.TotalMilliseconds;
                     if (Update != null)
                         Update.Invoke();
                 });
            }

            charging = false;
        }

        private void Discharge()
        {
            FirstGpio.SetDriveMode(GpioPinDriveMode.Input);
            SecondGpio.SetDriveMode(GpioPinDriveMode.Output);
            SecondGpio.Write(GpioPinValue.Low);
        }

        public async void Initialize()
        {
            Discharge();
            await Task.Delay(500);
            Charge();
            await Task.Delay(500);
            Initialize();
        }

        public double Temperature { get; set; }
    }
}
