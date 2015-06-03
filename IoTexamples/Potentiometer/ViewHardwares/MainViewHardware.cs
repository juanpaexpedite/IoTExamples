using Common;
using Hardwares.Base;
using Potentiometer.Hardwares;
using Potentiometer.Models;
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

namespace Potentiometer.ViewHardwares
{
    public class MainViewHardware : ViewHardware
    {
        #region Hardware
        private TrimPotHardware hardware;
        public TrimPotHardware Hardware
        {
            get { return hardware; }
            set
            {
                hardware = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public MainViewHardware()
        {
            Initialize();
        }

        private async void Initialize()
        {
            Hardware = new TrimPotHardware();

            if (SpiStatus.Success == await Hardware.InitalizeSpi(new MCP3002(), 1))
            {
                DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
                dt.Tick += (s, e) =>
                {
                    Hardware.ReadSpi();
                    Hardware.Update();
                };
                dt.Start();
             
            }
        }
    }
}
