using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodAndBittersteel.Features.BaBEvents
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal class BaBEventAttribute : Attribute
    {
    }
}
