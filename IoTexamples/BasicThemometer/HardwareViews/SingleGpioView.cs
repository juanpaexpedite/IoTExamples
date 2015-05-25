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
    public class SingleGpioView : HardwareView
    {
        #region Standard Implementation
        public SingleGpioView()
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
        public Int32 Pin { get; set; }

        public GpioStatus InitializeGpios()
        {
            Gpio = InitGpioOutput(Pin);
            if (Gpio == null)
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
            Gpio.ValueChanged += Gpio_ValueChanged;
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
            if (discharging)
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
        private void Charge()
        {
            Gpio.SetDriveMode(GpioPinDriveMode.Output);
            Gpio.Write(GpioPinValue.Low);
        }

        private void Discharge()
        {
            pulseLength.Restart();
            Gpio.SetDriveMode(GpioPinDriveMode.Input);
            discharging = true;
        }
    }
}
