using IoT.Clients;
using IoT.Services;
using IoT.ViewHardwares.Input;
using Newtonsoft.Json;
using RelayAutomation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RelayAutomation.ViewHardwares
{
    public class PCMainViewHardware : INotifyPropertyChanged
    {
        #region Constructor
        public PCMainViewHardware()
        {
            if (SystemService.IsRuntime && !SystemService.IsIoTDevice)
                Initialize();
        }
        #endregion

        #region Initialize
        RestClient client;
        string GetLamps = "getlamps";
        string SwitchLamp = "switchlamp";

        public void Initialize()
        {
            client = new RestClient("http://192.168.0.204:8000");

            DispatcherTimer UITimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };

            UITimer.Tick += (s, e) =>
            {
                CheckLamps();
            };
            UITimer.Start();
            CheckLamps();
        }

        private async void CheckLamps()
        {
            var lamps = await client.Request<List<Lamp>>(GetLamps);
            if (lamps != null)
                On = lamps.First().On;
        }
        #endregion


        #region On / Off
        private bool on = false;
        public bool On
        {
            get { return on; }
            set
            {
                on = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region SwitchCommand
        private Command switchcommand;
        public Command SwitchCommand
        {
            get
            {
                if (switchcommand == null)
                {
                    InitializeSwitchCommand();
                }
                return switchcommand;
            }
        }
        private void InitializeSwitchCommand()
        {
            switchcommand = new Command(async () =>
            {
                var lamp = await client.Request(SwitchLamp,new Lamp() { Id = 0 });
                On = lamp.On;
            });
        }
        #endregion

        #region INotifyPropertyChanged
        public async void NotifyPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null && App.rootFrame != null)
            {
                await App.rootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(caller));
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
