using RGBLed.HardwareViews;
using RGBLed.Models;
using RGBLed.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;

namespace RGBLed.ViewModels
{
    public class MainViewModel : ViewModel
    {
        #region Led
        private RGBLedModel led = new RGBLedModel();

        public RGBLedModel Led
        {
            get { return led; }
            set { if (led != value) { led = value; NotifyPropertyChanged(); } }
        }
        #endregion

        #region Colors
        private double red = 0;
        public double Red
        {
            get { return red; }
            set
            {
                    red = value;
                    Led.Color = new Color() { R = (byte)red, G = Led.Color.G, B = Led.Color.B, A = 255};
                    SetNewColor();
                    NotifyPropertyChanged();
            }
        }

        private double green = 0;
        public double Green
        {
            get { return green; }
            set
            {
                    green = value;
                    Led.Color = new Color() { R = Led.Color.R, G = (byte)green, B = Led.Color.B, A = 255 };
                    SetNewColor();
                    NotifyPropertyChanged();
            }
        }

        private double blue = 0;
        public double Blue
        {
            get { return blue; }
            set
            {
                    blue = value;
                    Led.Color = new Color() { R = Led.Color.R, G = Led.Color.G, B = (byte)blue, A = 255 };
                    SetNewColor();
                    NotifyPropertyChanged();
            }
        }
        #endregion

        CancellationTokenSource cts = new CancellationTokenSource();
        private void SetNewColor()
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
            MainHardwareView.SetColors(cts.Token, red == 255, green == 255, blue == 255);
        }

        #region MainHardwareView
        public MainHardwareView MainHardwareView { get; set; }
        #endregion

        public MainViewModel()
        {
            MainHardwareView = new MainHardwareView();
            Red = 255;
            Green = 0;
            Blue = 0;
        }


    }
}
