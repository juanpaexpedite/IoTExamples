using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Models
{
    public class Thermistor1K25C
    {
        public double Resistance;

        private double A;
        private double B;
        private double C;

        public Thermistor1K25C(double a = 0.00288301, double b = -0.00002028, double c = 0.00000190)
        {
            A = a; B = b; C = c;
        }

        public double GetTemperatureCelsius()
        {
            var lnR = Math.Log(Resistance);
            var invTemperature = A + B * lnR + C * Math.Pow(lnR, 3);
            return 1 / invTemperature - 273.15;
        }

        public double GetTemperatureKelvin()
        {
            return GetTemperatureCelsius() + 273.15;
        }

        public double GetTemperatureFarenheit()
        {
            return GetTemperatureCelsius() * 1.8 + 32.00;
        }
    }

}
