using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using Mono.Api.Hatena.Extensions;
using Mono.Api.Hatena.Model;
using Mono.Framework.Common.Extensions;

namespace Mono.Api.Hatena
{
    public class HatenaClient
    {
        private const string HotEntryUrl = "http://feedproxy.google.com/hatena/b/hotentry";

        private IObservable<IEnumerable<HatebuItem>> GetObservableSyndication<T>(string method)
        {
            var wc = new WebClient();
            var observable = wc.GetHatebuObservable();
            var url = string.Concat(HotEntryUrl, method);
            wc.OpenReadAsync(new Uri(url));
            return observable;
        }

        public IObservable<IEnumerable<HatebuItem>> GetHotEntry()
        {
            return GetObservableSyndication<HatebuItem>("");
        }
    }
}