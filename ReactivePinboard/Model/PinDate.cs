using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mono.Api.ReactivePinboard.Model
{
    public class PinDate
    {
        public int Count { get; set; }

        public DateTime Date { get; set; }

        public static PinDate Parse(XElement x)
        {
            return new PinDate
            {
                Count = int.Parse(x.Attribute("count").Value),
                Date = DateTime.Parse(x.Attribute("date").Value),
            };
        }
    }
}