using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mono.Api.ReactivePinboard.Model
{
    public class Suggested
    {
        public IEnumerable<string> Populars { get; set; }

        public IEnumerable<string> Recommendeds { get; set; }

        public static Suggested Parse(XElement x)
        {
            return new Suggested
            {
                Populars = from p in x.Descendants("popular")
                           select p.Value,
                Recommendeds = from r in x.Descendants("recommended")
                               select r.Value,
            };
        }
    }
}