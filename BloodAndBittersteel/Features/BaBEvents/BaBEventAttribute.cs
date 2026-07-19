using System;

namespace BloodAndBittersteel.Features.BaBEvents
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal class BaBEventAttribute : Attribute
    {
    }
}