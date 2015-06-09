using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Potentiometer.Models
{
    public class PotentiometerModel
    {
        public double TotalResistance = 10000; //10 KOhms

        public double UpperResistance = 0;
        public double DownResistance => TotalResistance - UpperResistance;

        public double Voltage;

        public double Current => Voltage / TotalResistance;

        public double UpperVoltage => UpperResistance * Current;

        public double DownVoltage => DownResistance * Current;
    }
    

}
