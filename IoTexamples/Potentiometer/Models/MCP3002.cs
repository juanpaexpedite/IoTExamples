using Potentiometer.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Potentiometer.Models
{
    public class MCP3002 : IAnalogDigitalConverter
    {
        public Int32 ChipSelect0 => 0;
        public Int32 ChipSelect1 => 1;

        public SpiConnectionSettings SpiConnectionSettings => new SpiConnectionSettings(ChipSelect0)
            {
                ClockFrequency = 500000,
                Mode = SpiMode.Mode0
            };


        public Int32 Channel { get; set; }

        private byte[] readbuffer = new byte[2];
        public byte[] ReadBuffer
        {
            get { return readbuffer; }
            set
            {
                readbuffer = value;
            }
        }

        public byte[] WriteBuffer
        {
            get
            {
                switch (Channel)
                {
                    case 0:
                        return new byte[2] { 0x68, 0x00 };
                    case 1:
                        return new byte[2] { 0x70, 0x00 };
                    default:
                        return new byte[2] { 0x68, 0x00 };
                }
            }
        }

        public Int32 Convert(byte[] bytes)
        {
            int result = bytes[0];
            result <<= 8; // == result * 2^8
            result += bytes[1]; // 0 - 255
            return result; //min = 0, max = 1023
        }
    }
}
