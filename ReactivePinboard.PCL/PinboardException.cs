using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Api.ReactivePinboard
{
    public class PinboardException : Exception
    {
        public PinboardException(string message)
            : base(message)
        {
        }
    }
}