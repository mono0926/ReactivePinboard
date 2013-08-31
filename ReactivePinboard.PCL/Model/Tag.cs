using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mono.Api.ReactivePinboard.Model
{
    [Table]
    public class Tag
    {
        [Column]
        public int Count { get; set; }

        [Column]
        public string Name { get; set; }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        public static Tag Parse(XElement x)
        {
            return new Tag
            {
                Count = int.Parse(x.Attribute("count").Value),
                Name = x.Attribute("tag").Value,
            };
        }
    }
}