using Hardwares.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Potentiometer.Hardwares
{
    public class TrimPotHardware : Hardware
    {
        #region Gpios, SPI and Constructor
        //In this case there are no Gpios in use
        public TrimPotHardware() : base(null)
        {

        }
        #endregion Standard Implementation

        

        public void Update()
        {

        }

    }
}
