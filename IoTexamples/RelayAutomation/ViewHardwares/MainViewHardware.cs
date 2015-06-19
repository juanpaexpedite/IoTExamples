using IoT.Hardwares.Base;
using IoT.Models;
using IoT.Servers;
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
using Windows.ApplicationModel.AppService;

namespace RelayAutomation.ViewHardwares
{
    public class MainViewHardware : INotifyPropertyChanged
    {
        #region MainHardware
        private Hardware mainhardware;
        public Hardware MainHardware
        {
            get { return mainhardware; }
            set
            {
                mainhardware = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Relay
        public Relay Relay { get; set; }
        #endregion

        #region Constructor & Initialize
        public MainViewHardware()
        {
            if(SystemService.IsRuntime)
                Initialize();
        }

        private async void Initialize()
        {
            if (SystemService.IsIoTDevice)
            {
                MainHardware = new Hardware(new int[] { 18 }); //Relay Gpio Pin

                if (await MainHardware.InitializeGpioController())
                {
                    Relay = new Relay(0);
                }

                InitializeModel();

                InitializeServer();
            }
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
                if(switchcommand == null)
                {
                    InitializeSwitchCommand();
                }
                return switchcommand;
            }
        }

        private void InitializeSwitchCommand()
        {
            switchcommand = new Command(() =>
            {
                On = !On;

                if (On)
                {
                    Relay.SwitchOn();
                }
                else
                {
                    Relay.SwitchOff();
                }

                Lamps.First().On = On;
            });
        }
        #endregion

        #region Model
        public List<Lamp> Lamps { get; set; }

        public void InitializeModel()
        {
            Lamps = new List<Lamp>()
            {
                 new Lamp() { Id = 0, On = false }
            };
        }
        #endregion

        #region Server
        RestServer server;
        string GetLamps = "getlamps";
        string SwitchLamp = "switchlamp";
        public void InitializeServer()
        {
            server = new RestServer(8000, "RelayAutomation");

            server.GetContentRequestData = (method, data) =>
            {
                if (method == GetLamps)
                {
                    return JsonConvert.SerializeObject(Lamps);
                }
                else if(method == SwitchLamp)
                {
                    SwitchCommand.Execute(null);
                    return JsonConvert.SerializeObject(Lamps.First());
                }

                return String.Empty;
            };

            server.StartServer();
        }
        #endregion

        #region NotifyPropertyChanged
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
