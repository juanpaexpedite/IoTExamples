using BasicThermometer.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicThemometer.Models
{
    public class ThermometerModel : Model
    {
        private double temperature;
        public double Temperature
        {
            get { return temperature; }
            set { if (temperature != value) { temperature = value; NotifyPropertyChanged(); } }
        }

        private double averagetemperature;
        public double AverageTemperature
        {
            get { return averagetemperature; }
            set { if (averagetemperature != value) { averagetemperature = value; NotifyPropertyChanged(); } }
        }

        private double linear;
        public double Linear
        {
            get { return linear; }
            set { if (linear != value) { linear = value; NotifyPropertyChanged(); } }
        }

        public int FirstPin { get; set; }
        public int SecondPin { get; set; }
    }
}
