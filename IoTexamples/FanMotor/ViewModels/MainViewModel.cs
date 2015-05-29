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
        #region Fan
        private FanModel fan = new FanModel() { FirstPin = 18, SecondPin= 23};

        public FanModel Fan
        {
            get { return fan; }
            set { if (fan != value) { fan = value; NotifyPropertyChanged(); } }
        }

        #endregion

        #region MainHardwareView
        public MainHardwareView MainHardwareView { get; set; }
        #endregion

        public MainViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            MainHardwareView = new MainHardwareView();
            MainHardwareView.FirstPin = Fan.FirstPin;
            MainHardwareView.SecondPin = Fan.SecondPin;

            if (await MainHardwareView.InitializeComponent())
            {
                MainHardwareView.Specific();
            }
        }

    }
}
