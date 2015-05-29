using Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanMotor.Models
{
    public class FanModel : Model
    {
        public int FirstPin { get; set; }
        public int SecondPin { get; set; }
    }
}
