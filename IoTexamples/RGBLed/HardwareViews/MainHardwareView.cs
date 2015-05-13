using RGBLed.Common;
using RGBLed.HardwareViews.Base;
using RGBLed.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;

namespace RGBLed.HardwareViews
{
    public class MainHardwareView : HardwareView
    {
        #region Constructor and Initialization
        public MainHardwareView()
        {
           
        }

        public async Task<bool> InitializeComponent()
        {
            var devicefamily = GetForCurrentView().QualifierValues["DeviceFamily"];
            if (devicefamily != "Universal")
                return false;

            var controller = GpioController.GetDefault();

            if (controller == null)
            {
                RedPin = null; GreenPin = null; BluePin = null;
                Status = GpioStatus.NoGPIO;
            }
            else
            {
                Status = GpioStatus.Initialized;
                await Task.Delay(100);
                GpioController = controller;
                Status = InitializePins();
            }

            return true;
        }
        #endregion

        #region Status
        public GpioStatus status = GpioStatus.Default;
        public GpioStatus Status
        {
            get { return status; }
            set { if(status != value) { status = value; NotifyPropertyChanged(); }}
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

        #region Pins
        public GpioPin RedPin;
        public GpioPin GreenPin;
        public GpioPin BluePin;

        public GpioStatus InitializePins()
        {
            RedPin = InitPin(18);
            if (RedPin == null)
                return GpioStatus.NoPin;

            GreenPin = InitPin(23);
            if (GreenPin == null)
                return GpioStatus.NoPin;

            BluePin = InitPin(24);
            if (BluePin == null)
                return GpioStatus.NoPin;

            return GpioStatus.Success;
        }

        public GpioPin InitPin(int PIN)
        {
            var pin =  GpioController.OpenPin(PIN);
            if (pin != null)
            {
                pin.Write(GpioPinValue.Low);
                pin.SetDriveMode(GpioPinDriveMode.Output);
            }
            return pin;
        }
        #endregion

        #region Colors
        private bool RedOn = false;
        private bool GreenOn = false;
        private bool BlueOn = false;
        public async void SetColors(CancellationToken token, bool red, bool green, bool blue)
        {
            await Task.Delay(500);

            if (!token.IsCancellationRequested)
            {
                RedOn = red; GreenOn = green; BlueOn = blue;

                RedPin.Write(RedOn ? GpioPinValue.High : GpioPinValue.Low);
                GreenPin.Write(GreenOn ? GpioPinValue.High : GpioPinValue.Low);
                BluePin.Write(BlueOn ? GpioPinValue.High : GpioPinValue.Low);
            }
        }
        #endregion

    }
}
