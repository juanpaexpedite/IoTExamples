using FanMotor.HardwareViews;
using FanMotor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewModels.Base;
using Windows.UI;
using Windows.UI.Xaml;

namespace FanMotor.ViewModels
{
    public class MainViewModel : ViewModel
    {
        #region MainHardwareView
        public MainHardware MainHardwareView { get; set; }
        #endregion

        public MainViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            MainHardwareView = new MainHardware(new int[] { 18, 23 });

            if (await MainHardwareView.InitializeComponent())
            {
                MainHardwareView.Specific();
            }
        }

    }
}
