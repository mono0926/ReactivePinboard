using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using Mono.Api.ReactivePinboard.Model;

namespace Mono.Api.ReactivePinboard.Extensions
{
    public static class PinboardObservableExtension
    {
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
    }
}