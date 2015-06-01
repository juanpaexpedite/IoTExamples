using Hardwares.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace FanMotor.Hardwares
{
    public class LedHardware : Hardware
    {
        #region Gpios and Pins
        public GpioPin Gpio => Gpios[2];

        public LedHardware(Int32[] pins) : base(pins)
        {
            
        }
        #endregion Standard Implementation

        public void TurnOn()
        {
            Gpio.Write(GpioPinValue.High);
        }

        public void TurnOff()
        {
            Gpio.Write(GpioPinValue.Low);
        }
    }
}
