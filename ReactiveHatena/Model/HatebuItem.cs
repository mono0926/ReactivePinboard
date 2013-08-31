using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Api.Hatena.Model
{
    public class HatebuItem
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int HatebuCount { get; set; }

        public string Subject { get; set; }

        public DateTime Date { get; set; }
    }
}