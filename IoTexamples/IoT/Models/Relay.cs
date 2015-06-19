using IoT.Hardwares.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoT.Models
{
    public class Relay
    {
        private int relayidx;

        private GpioPin Gpio => Hardware.Gpios[relayidx];

        public Relay(int Relayidx)
        {
            relayidx = Relayidx;
        }

        public void SwitchOn()
        {
            Gpio.Write(GpioPinValue.Low);
        }

        public void SwitchOff()
        {
            Gpio.Write(GpioPinValue.High);
        }
    }
}
