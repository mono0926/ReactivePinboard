using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Mono.Api.Hatena.Model;

namespace Mono.Api.Hatena.Extensions
{
    public static class HatebuExtension
    {
        public static IObservable<IEnumerable<HatebuItem>> GetHatebuObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }

                        var xml = XElement.Load(e.Result);
                        XNamespace ns = "http://purl.org/rss/1.0/";
                        XNamespace cont = "http://purl.org/rss/1.0/modules/content/";
                        return xml.Descendants(ns + "item")
                            .Select(x => new HatebuItem { Title = x.Element("title").Value });
                    });
            return o.Take(1);
        }

        public static string GetImageUrl(string source)
        {
            var pre = @"<cite><img src=""";
            var post = @"""";
            var regex = new Regex(@"<cite><img src="".+""");
            var m = regex.Match(source, 0);
            return m.Value.Replace(pre, "").Replace(post, "");
            //            return System.Text.RegularExpressions.Regex.Replace(
            //   source, @"<cite><img src="".+""", "$1");
        }
    }
}