using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using Mono.Api.ReactivePinboard.Helper;

namespace Mono.Api.ReactivePinboard.Model
{
    public class FeedItem : ItemBase
    {
        public FeedItem()
            : base()
        { }

        [Column]
        public string Account { get; set; }

        [Column]
        public string Author { get; set; }

        [Column]
        public string Source { get; set; }
    }

    public class RecentItem : FeedItem
    {
    }

    public class NetworkItem : FeedItem
    {
    }

    public class PopularItem : FeedItem
    {
    }
}