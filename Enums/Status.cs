using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeInformationSystem.Enums
{
    public enum Status : ulong
    {
        Opened = 1,
        Ready = 2,
        Issued = 3,
        Closed = 4
    }
}