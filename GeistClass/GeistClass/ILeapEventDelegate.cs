using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    interface ILeapEventDelegate
    {
        void LeapEventNotification(string EventName);
    }
}
