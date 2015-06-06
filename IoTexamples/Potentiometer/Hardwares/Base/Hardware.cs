using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using static Windows.ApplicationModel.Resources.Core.ResourceContext;
using System.Reflection;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using Potentiometer.Models.Base;
using Potentiometer.Models;

namespace Hardwares.Base
{
    public class Hardware : INotifyPropertyChanged
    {
        public Hardware() : this(null)
        {

        }

        public Hardware(Int32[] pins)
        {
            if (GpioPins == null)
            {
                GpioPins = pins == null ? new List<int>() : new List<int>(pins);
                Gpios = new List<GpioPin>();
            }
            else
            {
                if (pins != null)
                {
                    foreach (var pin in pins)
                    {
                        GpioPins.Add(pin);
                        Gpios.Add(InitGpioOutput(pin));
                    }
                }
            }
        }

        #region Controller
        private static GpioController gpiocontroller;
        public GpioController GpioController
        {
            get
            {
                return gpiocontroller;
            }
            set
            {
                if (gpiocontroller != value)
                {
                    gpiocontroller = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Gpio
        public GpioStatus status = GpioStatus.Default;
        public GpioStatus Status
        {
            get { return status; }
            set { if (status != value) { status = value; NotifyPropertyChanged(); } }
        }
        #endregion

        #region Initialize Gpio
        public async Task<bool> InitializeGpioController()
        {
            var devicefamily = GetForCurrentView().QualifierValues["DeviceFamily"];
            if (devicefamily != "Universal")
                return false;

            if (gpiocontroller == null)
                gpiocontroller = GpioController.GetDefault();

            if (gpiocontroller == null)
            {
                if (Gpios != null)
                {
                    for (int i = 0; i < Gpios.Count(); i++)
                    {
                        Gpios[i] = null;
                    }
                }
                Status = GpioStatus.NoGPIO;
            }
            else
            {
                Status = GpioStatus.Initialized;
                await Task.Delay(200);
                Status = InitializeGpios();
                await Task.Delay(200);
            }

            return true;
        }

        public GpioStatus InitializeGpios()
        {
            if (Gpios != null && GpioPins != null)
            {
                for (int i = 0; i < GpioPins.Count(); i++)
                {
                    Gpios.Add(InitGpioOutput(GpioPins[i]));
                    if (Gpios[i] == null)
                        return GpioStatus.NoPin;
                }
            }

            return GpioStatus.Success;
        }
        #endregion

        #region GpioPins
        public static List<Int32> GpioPins;
        public static List<GpioPin> Gpios;
        public GpioPin InitGpioOutput(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);

            return pin;
        }

        public GpioPin InitGpioInput(int PIN)
        {
            var pin = GpioController.OpenPin(PIN);
            pin.SetDriveMode(GpioPinDriveMode.Input);
            return pin;
        }
        #endregion

        #region SPI Device
        private static SpiDevice spidevice;
        public SpiDevice SpiDevice
        {
            get
            {
                return spidevice;
            }
            set
            {
                if (spidevice != value)
                {
                    spidevice = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region SPI Analog Digital Converter
        private IAnalogDigitalConverter adconverter;
        public IAnalogDigitalConverter ADConverter
        {
            get { return adconverter; }
            set
            {
                adconverter = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Initialize SPI
        private List<String> Controllers => new List<String>() { "SPI0", "SPI1"};

        public async Task<SpiStatus> InitalizeSpi(IAnalogDigitalConverter adc, int Channel = 0)
        {
            ADConverter = adc;
            adc.Channel = Channel;
            string spiAqs = SpiDevice.GetDeviceSelector(Controllers[0]);
            var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
            SpiDevice = await SpiDevice.FromIdAsync(deviceInfo[0].Id,adc.SpiConnectionSettings);

            if (SpiDevice == null)
                return SpiStatus.NoSPI;
            else
                return SpiStatus.Success;
        }
        #endregion

        #region SpiChannel
        private int spichannel;

        public int SpiChannel
        {
            get { return spichannel; }
            set
            {
                spichannel = value;
                if (ADConverter != null)
                    ADConverter.Channel = spichannel;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region SpiDigitalValue
        private int spidigitalvalue;

        public int SpiDigitalValue
        {
            get
            {
                return spidigitalvalue;
            }
            set
            {
                spidigitalvalue = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region SpiData
        private string spidata;

        public string SpiData
        {
            get
            {
                return spidata;
            }
            set
            {
                spidata = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public int ReadSpi()
        {
            SpiDevice.TransferFullDuplex(adconverter.WriteBuffer, adconverter.ReadBuffer);
            SpiData = $"{adconverter.ReadBuffer[0]},{adconverter.ReadBuffer[1]}";
            SpiDigitalValue = adconverter.Convert(adconverter.ReadBuffer);
            return SpiDigitalValue;

        }
        
        //private int convertToInt(byte[] data)
        //{
        //    int result = data[0];
        //    result <<= 8;
        //    result += data[1];
        //    return result;
        //}


        #region NotifyPropertyChanged
        public void NotifyPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    //public enum ADCs
    //{
    //    MCP3002,
    //    MCP3008
    //}

    //public class RPI2SPISettings
    //{
       
    //    private static Int32 ChipSelect0 => 0;
    //    private static Int32 ChipSelect1 => 1;

    //    public static SpiConnectionSettings MCP3002_ChipSelect0
    //    {
    //        get
    //        {
    //            return new SpiConnectionSettings(ChipSelect0)
    //            {
    //                ClockFrequency = 500000,
    //                Mode = SpiMode.Mode0
    //            };
    //        }
    //    }
    //}

    //public class RPI2SPIDevices
    //{
    //    private static string Controller0 => "SPI0";
    //    private static string Controller1 => "SPI1";

    //    public static async Task<SpiDevice> MCP3002Channel0()
    //    {
    //        try
    //        {
    //            string spiAqs = SpiDevice.GetDeviceSelector(Controller0);
    //            var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
    //            return await SpiDevice.FromIdAsync(deviceInfo[0].Id, RPI2SPISettings.MCP3002_ChipSelect0);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("SPI Initialization Failed", ex);
    //        }
    //    }

    //    public static async Task<SpiDevice> MCP3002Channel1()
    //    {
    //        try
    //        {
    //            string spiAqs = SpiDevice.GetDeviceSelector(Controller0);
    //            var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
    //            return await SpiDevice.FromIdAsync(deviceInfo[0].Id, RPI2SPISettings.MCP3002_ChipSelect0);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("SPI Initialization Failed", ex);
    //        }
    //    }
    //}
}
