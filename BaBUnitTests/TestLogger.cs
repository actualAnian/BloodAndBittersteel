using LanceSystem.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaBUnitTests
{
    public class TestLogger : ILanceLogger
    {
        public int WarningCalls { get; private set; }

        public string? LastMessage { get; private set; }

        public void Warning(string message)
        {
            WarningCalls++;
            LastMessage = message;
        }
    }
}
