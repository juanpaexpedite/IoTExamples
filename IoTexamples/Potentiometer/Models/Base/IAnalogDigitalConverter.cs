using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Potentiometer.Models.Base
{
    public interface IAnalogDigitalConverter
    {
        Int32 Convert(byte[] bytes);

        Int32 Channel { get; set; }

        byte[] WriteBuffer { get; }

        byte[] ReadBuffer { get; set; }

        SpiConnectionSettings SpiConnectionSettings { get; }
        int[] ChipSelect { get; }

    }
}
