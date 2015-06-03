using Hardwares.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Potentiometer.Hardwares
{
    public class MoistureSensorHardware : Hardware
    {
        #region Gpios, SPI and Constructor
        //In this case there are no Gpios in use
        public MoistureSensorHardware() : base(null)
        {

        }
        #endregion Standard Implementation

        #region Spi
        public void Update()
        {
            Humidity = DigitalToHumidity(SpiDigitalValue);
            Level = DigitalToLevel(SpiDigitalValue);
        }
        //public void ReadSpi()
        //{
        //    SpiDevice.TransferFullDuplex(WriteBuffer, ReadBuffer);

        //    SpiData = $"{ReadBuffer[0]},{ReadBuffer[1]}";
        //    SpiDigitalValue = convertToInt(ReadBuffer);


        //}
        //private int convertToInt(byte[] data)
        //{
        //    int result = data[0];
        //    result <<= 8;
        //    result += data[1];
        //    return result;
        //}
        #endregion

        #region Specific
        private double humidity;

        public double Humidity
        {
            get { return humidity; }
            set
            {
                humidity = value;
                NotifyPropertyChanged();
            }
        }

        private double DigitalToHumidity(double value)
        {
            if (value > 340)
                return 100;
            else
                return value * 100 / 340;
                
        }

        private double level;

        public double Level
        {
            get { return level; }
            set
            {
                level = value;
                NotifyPropertyChanged();
            }
        }

        private double DigitalToLevel(double value)
        {
            double min = 500.0;
            double max = 715.0;

            if (value < min)
                return 0;
            else
            {
                double b = 100.0 / (1.0 - max / min);
                double a = -b / min;


                return value * a + b;
            }
        }
        #endregion

    }
}
