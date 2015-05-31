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
    public class MainHardware : Hardware
    {
        #region Gpios and Pins
        public GpioPin FirstGpio => Gpios[0];
        public GpioPin SecondGpio => Gpios[1];

        #endregion Standard Implementation

        public MainHardware(Int32[] pins) : base(pins)
        {

        }

        public void Specific()
        {
            Loop();

            //SoftwarePWMLoop();
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
