using RGBLed.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace RGBLed.Models
{
    public class RGBLedModel : Model
    {
        private Color color;
        public Color Color
        {
            get { return color; }
            set { if (color != value) { color = value; NotifyPropertyChanged(); } }
        }

        public int RedPin { get; set; }
        public int GreenPin { get; set; }
        public int BluePin { get; set; }

    }
}
