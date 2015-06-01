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

namespace Hardwares.Base
{
    public class Hardware : INotifyPropertyChanged
    {

        public static List<Int32> GpioPins;
        public static List<GpioPin> Gpios;
        public Hardware(Int32[] pins)
        {
            if (GpioPins == null)
            {
                GpioPins = new List<int>(pins);
                Gpios = new List<GpioPin>();
            }
            else
            {
                foreach(var pin in pins)
                {
                    GpioPins.Add(pin);
                    Gpios.Add(InitGpioOutput(pin));
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

        #region Status
        public GpioStatus status = GpioStatus.Default;
        public GpioStatus Status
        {
            get { return status; }
            set { if (status != value) { status = value; NotifyPropertyChanged(); } }
        }
        #endregion

        public async Task<bool> InitializeComponent()
        {
            var devicefamily = GetForCurrentView().QualifierValues["DeviceFamily"];
            if (devicefamily != "Universal")
                return false;

            if(gpiocontroller==null)
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

        #region GpioPins
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
}
