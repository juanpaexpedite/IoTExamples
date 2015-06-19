using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Models
{
    public class Photoresistor
    {
        public double Resistance;

        public double GetIlluminance()
        {
            return  255.84 * Math.Pow(Resistance, -10 / 9);
        }

        public Lightness GetLightness()
        {
            if (Resistance > 100000)
                return Lightness.Dark;
            if (Resistance > 50000)
                return Lightness.Dim;
            if (Resistance > 7000)
                return Lightness.Light;
            if (Resistance > 3000)
                return Lightness.Bright;
            else
                return Lightness.Blazing;
        }

        public enum Lightness
        {
            Dark,
            Dim,
            Light,
            Bright,
            Blazing
        }
    }
}
