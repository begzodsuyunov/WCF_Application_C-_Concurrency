using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Gate
    {
        //Gate class with properties the gate number and availability status
        //By default, a gate is available, indicated by the available property set to true
        public String number { get; set; }
        public Boolean available { get; set; } = true;
    }
}
