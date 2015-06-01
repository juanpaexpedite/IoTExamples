using Common;
using Hardwares.Base;
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

namespace FanMotor.Hardwares
{
    public class FanMotorHardware : Hardware
    {
        #region Gpios and Pins
        public GpioPin FirstGpio => Gpios[0];
        public GpioPin SecondGpio => Gpios[1];

        public FanMotorHardware(Int32[] pins) : base(pins)
        {

        }
        #endregion Standard Implementation


        public void Specific()
        {
            //Long await
            Loop();

            //Mininum await
            //SoftwarePWMLoop();

            //Thread with for wait
            //ThreadLoop();
        }

        //Core of the thread Loop
        private async void ForPWMLoop()
        {
            Forward();
            for (int i = 0; i < 140000; i++)
            {

            }
            Stop();
            await Task.Delay(150);
            ForPWMLoop();
        }

        //Another Thread Loop
        private async void ThreadLoop()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                ForPWMLoop();
            }, TaskCreationOptions.LongRunning);

            await t;

        }

        //Minimum Software PWM Loop
        private async void SoftwarePWMLoop()
        {
            Forward();
            await Task.Delay(1);
            Stop();
            await Task.Delay(190);
            SoftwarePWMLoop();
        }

        //Basic Loop
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

            e.FanMotorMode = Modes.Stop;
            CallOnChanged();
        }

        public void Forward()
        {
            FirstGpio.Write(GpioPinValue.High);
            SecondGpio.Write(GpioPinValue.Low);

            e.FanMotorMode = Modes.Forward;
            CallOnChanged();
        }

        public void Reverse()
        {
            FirstGpio.Write(GpioPinValue.Low);
            SecondGpio.Write(GpioPinValue.High);

            e.FanMotorMode = Modes.Reverse;
            CallOnChanged();
        }

        #region Event implementation
        //This allows to subscribe (+=) to the event when the fan motor mode changes and make something like turn on a led
        public delegate void LoopEndHandler(object sender, FanMotorEventArgs e);
        public event LoopEndHandler OnChanged;
        public FanMotorEventArgs e = new FanMotorEventArgs() { FanMotorMode = Modes.Stop };

        private void CallOnChanged()
        {
            if (OnChanged != null)
            {
                OnChanged(this, e);
            }
        }

        public enum Modes
        {
            Stop,
            Forward,
            Reverse
        }

        public class FanMotorEventArgs : EventArgs
        {
            public Modes FanMotorMode;
        }
        #endregion
    }


}
