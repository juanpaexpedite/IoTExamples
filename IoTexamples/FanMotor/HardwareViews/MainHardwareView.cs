using Common;
using HardwareViews.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;

namespace FanMotor.HardwareViews
{
    public class MainHardwareView : HardwareView
    {
        #region Standard Implementation
        public MainHardwareView()
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
            FirstGpio = InitGpioOutput(FirstPin);
            if (FirstGpio == null)
                return GpioStatus.NoPin;

            SecondGpio = InitGpioOutput(SecondPin);
            if (SecondGpio == null)
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
                FirstGpio = null;
                SecondGpio = null;
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

        public void Specific()
        {
            //Loop();

            SoftwarePWMLoop();
        }

        private async void SoftwarePWMLoop()
        {
            Forward();
            await Task.Delay(1);
            Stop();
            await Task.Delay(190);
            SoftwarePWMLoop();
        }

        private async void Loop()
        {
            Forward();
            await Task.Delay(2500);
            Stop();
            await Task.Delay(2500);
            Reverse();
            await Task.Delay(2500);
            Stop();
            await Task.Delay(2500);
            Loop();
        }

        public void Stop()
        {
            FirstGpio.Write(GpioPinValue.Low);
            SecondGpio.Write(GpioPinValue.Low);
        }

        public void Forward()
        {
            FirstGpio.Write(GpioPinValue.High);
            SecondGpio.Write(GpioPinValue.Low);
        }

        public void Reverse()
        {
            FirstGpio.Write(GpioPinValue.Low);
            SecondGpio.Write(GpioPinValue.High);
        }
    }


}
