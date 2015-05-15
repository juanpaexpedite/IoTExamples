using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicThermometer.Common
{
    public enum GpioStatus
    {
        Default,
        NoGPIO,
        Initialized,
        NoPin,
        Success
    }
}
