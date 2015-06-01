using FanMotor.Hardwares;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewHardwares.Base;
using Windows.UI;
using Windows.UI.Xaml;

namespace FanMotor.ViewHardwares
{
    public class MainViewHardware : ViewHardware
    {
        #region Hardware
        public FanMotorHardware FanMotorHardware { get; set; }
        public LedHardware LedHardware { get; set; }
        #endregion

        public MainViewHardware()
        {
            Initialize();
        }

        private async void Initialize()
        {
            FanMotorHardware = new FanMotorHardware(new int[] { 18, 23 });

            if (await FanMotorHardware.InitializeComponent())
            {
                //Initialized FanMotorHardware
                //Now I can initialize more hardware in the same RPi2

                LedHardware = new LedHardware(new int[] { 25 });

                //Because Hardware was initialized once I do not need to initialize again just the pin

                FanMotorHardware.OnChanged += (s, f) =>
                {
                    if(f.FanMotorMode == FanMotorHardware.Modes.Stop)
                    {
                        LedHardware.TurnOff();
                    }
                    else
                    {
                        LedHardware.TurnOn();
                    }
                };

                //To be pure HVVH you should extract the Specific Method to here but I wanted to show how to implement
                //one event and read from here
                FanMotorHardware.Specific();
            }
        }
    }
}
