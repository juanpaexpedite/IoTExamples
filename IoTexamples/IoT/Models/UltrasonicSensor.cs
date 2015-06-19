using IoT.Hardwares.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoT.Models
{
    //http://stackoverflow.com/questions/30124861/ultrasonic-sensor-raspberry-pi-2-c-sharp-net

    public class UltrasonicSensor 
    {

        private int triggeridx;
        private int echopidx;

        private GpioPin TriggerGpio => Hardware.Gpios[triggeridx];
        private GpioPin EchoGpio => Hardware.Gpios[echopidx];

        public UltrasonicSensor(int TriggerIdx, int EchoIdx)
        {
            triggeridx = TriggerIdx;
            echopidx = EchoIdx;

            
        }

        public void Initialize(Action<double> ondistance = null)
        {
            OnDistance = ondistance;

            EchoGpio.SetDriveMode(GpioPinDriveMode.Input);

            EchoGpio.ValueChanged += EchoGpio_ValueChanged;
        }

        private void EchoGpio_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if(args.Edge == GpioPinEdge.RisingEdge)
            {
                pulseLength.Start();
            }
            else
            {
                pulseLength.Stop();

                TimeSpan timeBetween = pulseLength.Elapsed;
                var distance = timeBetween.TotalSeconds * 17000;

                if (OnDistance != null)
                    OnDistance.Invoke(distance);
            }
        }

        public Action<double> OnDistance { get; set; }

        Stopwatch pulseLength = new Stopwatch();
        public void GetDistance()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            pulseLength.Restart();

            //Send pulse
            TriggerGpio.Write(GpioPinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            TriggerGpio.Write(GpioPinValue.Low);

            ////Recieve pulse
            //while (EchoGpio.Read() == GpioPinValue.Low)
            //{
            //}
            //pulseLength.Start();


            //while (EchoGpio.Read() == GpioPinValue.High)
            //{
            //}
            //pulseLength.Stop();

            //Calculating distance
            //TimeSpan timeBetween = pulseLength.Elapsed;
            //return timeBetween.TotalSeconds * 17000;
        }

    }

}
