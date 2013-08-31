using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;

//using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Mono.Api.ReactivePinboard;
using Mono.Api.ReactivePinboard.Model;

namespace Mono.Api.ReactivePinboard.Helper
{
    internal static class PinboarObservableHelper
    {
        private static Rss10FeedFormatter formatter;

        public static IObservable<IEnumerable<Post>> SelectPosts(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Descendants("post"))
                             .Select(statuses => statuses.Select(s => Post.Parse(s)));
        }

        public static IObservable<DateTime> SelectTimeAttribute(this IObservable<XElement> observable)
        {
            return observable.Select(x => DateTime.Parse(x.Attribute("time").Value));
        }

        public static IObservable<IEnumerable<PinDate>> SelectDateAttribute(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Descendants("date"))
                             .Select(xs => xs.Select(x => PinDate.Parse(x)));
        }

        public static IObservable<Suggested> SelectSuggested(this IObservable<XElement> observable)
        {
            return observable.Select(x => Suggested.Parse(x));
        }

        public static IObservable<IEnumerable<Tag>> SelectTags(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Descendants("tag"))
                             .Select(xs => xs.Select(x => Tag.Parse(x)));
        }

        public static IObservable<string> SelectCodeAttribute(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Attribute("code").Value);
        }

        public static IObservable<string> SelectValue(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Value);
        }

        public static IObservable<XElement> GetResultObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                        {
                            if (e.Error != null)
                            {
                                throw new PinboardException(e.Error.Message);
                            }

                            return XElement.Load(e.Result);
                        });
            return o;
        }

        public static IObservable<SyndicationFeed> GetSydicationObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                        {
                            if (e.Error != null)
                            {
                                throw new PinboardException(e.Error.Message);
                            }

                            if (formatter == null)
                            {
                                formatter = new Rss10FeedFormatter();
                            }
                            formatter.ReadFrom(XmlReader.Create(e.Result));
                            var feed = formatter.Feed;
                            return feed;
                        }
                    );
            return o;
        }
    }
}