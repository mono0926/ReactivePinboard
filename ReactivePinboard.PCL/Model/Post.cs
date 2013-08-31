using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Mono.Api.ReactivePinboard.Helper;

namespace Mono.Api.ReactivePinboard.Model
{
    public class Post : ItemBase
    {
        public Post()
            : base()
        {
        }

        [Column]
        public bool? ToReade { get; set; }

        [Column]
        public bool? Shared { get; set; }

        [Column]
        public bool? Replace { get; set; }

        public bool? IncludeMeta { get; set; }

        [Column]
        public string Meta { get; set; }

        [Column]
        public int? Others { get; set; }

        public static Post Parse(XElement x)
        {
            var toread = x.Attribute("toread");
            var shared = x.Attribute("shared");
            var others = x.Attribute("others");
            var meta = x.Attribute("meta");
            var tags = x.Attribute("tag").Value.Split(' ');
            var desc = x.Attribute("extended").Value;
            if (tags.Count() == 1 && string.IsNullOrWhiteSpace(tags[0]))
            {
                tags = new string[0];
            }
            return new Post
                        {
                            Url = x.Attribute("href").Value,
                            Time = DateTime.Parse(x.Attribute("time").Value),
                            Title = x.Attribute("description").Value,
                            Description = desc.Substring(0, Math.Min(500, desc.Length)),
                            Tags = tags,
                            Id = x.Attribute("hash").Value,
                            ToReade = (toread != null && toread.Value == "yes") ? true : false,
                            Shared = (shared != null && shared.Value == "no") ? false : true,
                            Others = (others != null) ? int.Parse(others.Value) : 0,
                            Meta = (meta != null) ? x.Attribute("meta").Value : null,
                        };
        }

        public override string ToString()
        {
            return string.Format("Title: {0}({1}), description: {2}, tag: {3}, time: {4}, shared: {5}", Title, Url, Description, string.Join(" ", Tags), TimeStr, Shared);
        }
    }
}