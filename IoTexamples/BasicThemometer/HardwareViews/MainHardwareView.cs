using BasicThermometer.Common;
using BasicThermometer.HardwareViews.Base;
using BasicThermometer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;

namespace BasicThermometer.HardwareViews
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
                FirstGpio= null; SecondGpio = null;
                Status = GpioStatus.NoGPIO;
            }
            else
            {
                Status = GpioStatus.Initialized;
                await Task.Delay(200);
                GpioController = controller;
                Status = InitializePins();
                await Task.Delay(200);
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


        #region Gpios and Pins
        public GpioPin FirstGpio;
        public GpioPin SecondGpio;

        public Int32 FirstPin { get; set; }
        public Int32 SecondPin { get; set; }

        public GpioStatus InitializePins()
        {
            FirstGpio = InitPin(FirstPin);
            if (FirstGpio == null)
                return GpioStatus.NoPin;

            SecondGpio = InitPin(SecondPin);
            if (SecondGpio == null)
                return GpioStatus.NoPin;

            //FirstGpio.ValueChanged += async (s, e) =>
            //{
            //    Debug.WriteLine($"First {e.Edge}");

            //    await Task.Delay(100);

            //    if(e.Edge == GpioPinEdge.RisingEdge)
            //    {
            //        //Charged
            //    }

            //    //if (e.Edge == GpioPinEdge.RisingEdge)
            //    //{
            //    //    T1 = DateTime.Now;
            //    //    Debug.WriteLine($"time {(T1 - T0).TotalMilliseconds}");

            //    //    var total = (T1 - T0).TotalMilliseconds * 1000;
            //    //    Temperature = ReadResistance(total);
            //    //    await Task.Delay(500);
            //    //    Discharge();
            //    //}
            //};

            //SecondGpio.ValueChanged += async (s, e) =>
            //{
            //    Debug.WriteLine($"Second {e.Edge}");

            //    if (e.Edge == GpioPinEdge.RisingEdge)
            //    {
            //        T1 = DateTime.Now;

            //        var total = (T1 - T0).TotalMilliseconds * 1000;
            //        Temperature = Convert(ReadResistance(total));

            //        await Task.Delay(100);
            //        Initialize();
            //    }
            //};

            return GpioStatus.Success;
        }



        public GpioPin InitPin(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);

            return pin;
        }
        #endregion


        #region Capacitor
        public async Task<double> Recharge()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            Stopwatch pulseLength = new Stopwatch();

            FirstGpio.SetDriveMode(GpioPinDriveMode.Input);
            SecondGpio.SetDriveMode(GpioPinDriveMode.Output);

            SecondGpio.Write(GpioPinValue.Low);

            await Task.Delay(10);

            SecondGpio.SetDriveMode(GpioPinDriveMode.Input);
            FirstGpio.SetDriveMode(GpioPinDriveMode.Output);

            pulseLength.Start();
            FirstGpio.Write(GpioPinValue.High);

            while (SecondGpio.Read() == GpioPinValue.Low)
            {

            }
            pulseLength.Stop();

            TimeSpan timeBetween = pulseLength.Elapsed;
            var microseconds = timeBetween.TotalMilliseconds * 1000;
            var resistance = ReadResistance(microseconds);
            return Convert(resistance);
           
        }

        public double Linear = 3.8;

        public double ReadResistance(double time)
        {
            return time * Linear - 939;
        }

        public double Convert(double resistance)
        {
            var b = 3800.0;
            var R0 = 1000.0;
            var t0 = 273.15;
            var t25 = t0 + 25.0;
            var inv_T = 1 / t25 + 1 / b * Math.Log(resistance / R0);
            var T = 1 / inv_T - t0;
            return T * 0.9;
        }

        public double Temperature { get; set; }
        public double AverageTemperature { get; set; }


        public async void Initialize()
        {
            var temperature = 0.0;
            for (int i = 0; i < 20; i++)
            {
                Temperature = await Recharge();
                temperature += Temperature;
                await Task.Delay(500);
            }
            AverageTemperature = temperature / 20;
            //Linear+=0.05;
            await Task.Delay(500);
            Initialize();
        }
        #endregion

    }
}
