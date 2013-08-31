using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Mono.Api.ReactivePinboard;
using Mono.Api.ReactivePinboard.Extensions;
using Mono.Api.ReactivePinboard.Model;
using Mono.Framework.Common.Extensions;
using Mono.Framework.Common.Lib;

namespace Mono.Api.ReactivePinboard
{
    public class PinboardClient
    {
        private const string ROOT_URL_BASE = "https://api.pinboard.in/v1/";
        private const string RSS_ROOT = "http://feeds.pinboard.in/rss/";
        private const string POSTS_ALL = "posts/all";
        private const string UPDATE = "posts/update";
        private const string ADD = "posts/add";
        private const string DELETE = "posts/delete";
        private const string GET = "posts/get";
        private const string RECENT = "posts/recent";
        private const string DATES = "posts/dates";
        private const string SUGGEST = "posts/suggest";
        private const string GET_TAG = "tags/get";
        private const string DELETE_TAG = "tags/delete";
        private const string RENAME_TAG = "tags/rename";
        private const string SECRET = "user/secret";

        private const string NETWORK = "secret:{0}/u:{1}/network";
        private const string POPULAR = "popular";
        private const string RECENT_RSS = "recent";
        private const string USER = "u:{0}";
        private const string TAG_SEARCH = "t:{0}";

        private NetworkCredential nc;
        private string user;

        public PinboardClient(string user, string password)
        {
            this.user = user;
#if SILVERLIGHT
            WebRequest.RegisterPrefix("https://", System.Net.Browser.WebRequestCreator.ClientHttp);
            //WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);

#endif
            nc = new NetworkCredential(user, password);
        }

        public void GetAllBookmarksAsync(Action<IEnumerable<Post>> callback = null)
        {
            Get(callback, ParsePosts, POSTS_ALL);
        }

        public IObservable<IEnumerable<Post>> GetAllBookmarks()
        {
            return GetObservableResult(POSTS_ALL).SelectPosts();
        }

        public void UpdateAsync(Action<DateTime> callback = null)
        {
            Get(callback, ParseTimeAttribute, UPDATE);
        }

        public IObservable<DateTime> Update()
        {
            return GetObservableResult(UPDATE).SelectTimeAttribute();
        }

        public void AddPostAsync(Post post, Action<string> callback = null)
        {
            var method = CreateAddPostParameter(post);
            Get(callback, ParseCodeAttribute, method);
        }

        public IObservable<string> AddPost(Post post)
        {
            var method = CreateAddPostParameter(post);
            return GetObservableResult(method).SelectCodeAttribute();
        }

        private string CreateAddPostParameter(Post post)
        {
            var parameters = new List<object>();
            parameters.Add(Parameter("url", post.Url));
            parameters.Add(Parameter("description", post.Title));
            if (!string.IsNullOrWhiteSpace(post.Description))
            {
                parameters.Add(Parameter("extended", post.Description));
            }
            if (post.Tags.Length > 0)
            {
                parameters.Add(Parameter("tags", string.Join(" ", post.Tags)));
            }
            if (post.Time.HasValue)
            {
                parameters.Add(Parameter("dt", post.TimeStr));
            }
            if (post.Replace.HasValue)
            {
                parameters.Add(Parameter("replace", post.Replace.Value ? "yes" : "no"));
            }
            if (post.Shared.HasValue)
            {
                parameters.Add(Parameter("shared", post.Shared.Value ? "yes" : "no"));
            }
            if (post.ToReade.HasValue)
            {
                parameters.Add(Parameter("toread", post.ToReade.Value ? "yes" : "no"));
            }
            var method = string.Format("{0}?{1}", ADD, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void DeletePostAsync(string url, Action<string> callback = null)
        {
            var method = string.Format("{0}?{1}", DELETE, Parameter("url", url));
            Get(callback, ParseCodeAttribute, method);
        }

        public IObservable<string> DeletePost(string url)
        {
            var method = string.Format("{0}?{1}", DELETE, Parameter("url", url));
            return GetObservableResult(method).SelectCodeAttribute();
        }

        public void GetPostAsync(string url = null, string[] tags = null, DateTime? date = null, bool includeMeta = false, Action<IEnumerable<Post>> callback = null)
        {
            var method = CreateGetPostMethod(url, tags, date, includeMeta);
            Get(callback, ParsePosts, method);
        }

        public IObservable<IEnumerable<Post>> GetPost(string url, string[] tags, DateTime? date, bool includeMeta)
        {
            var method = CreateGetPostMethod(url, tags, date, includeMeta);
            return GetObservableResult(method).SelectPosts();
        }

        private string CreateGetPostMethod(string url, string[] tags, DateTime? date, bool includeMeta)
        {
            var parameters = new List<object>();
            if (url != null)
            {
                parameters.Add(Parameter("url", url));
            }
            if (tags != null && tags.Length > 0)
            {
                parameters.Add(Parameter("tag", string.Join(" ", tags)));
            }
            if (date.HasValue)
            {
                parameters.Add(Parameter("dt", date.Value.ToUTCDateStr()));
            }
            parameters.Add(Parameter("meta", includeMeta ? "yes" : "no"));

            var method = string.Format("{0}?{1}", GET, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void GetRecentAsync(string[] tags = null, int? count = null, Action<IEnumerable<Post>> callback = null)
        {
            var method = CreateGetRecentMethod(tags, count);
            Get(callback, ParsePosts, method);
        }

        public IObservable<IEnumerable<Post>> GetRecent(string[] tags = null, int? count = null)
        {
            var method = CreateGetRecentMethod(tags, count);
            return GetObservableResult(method).SelectPosts();
        }

        private IObservable<XElement> GetObservableResult(string method)
        {
            var wc = new WebClient();
            wc.Credentials = nc;
            var observable = wc.GetResultObservable();
            wc.OpenReadAsync(new Uri(string.Concat(ROOT_URL_BASE, method)));
            return observable;
        }

        [Obsolete]
        private IObservable<XElement> GetObservableResultForSecret(string method)
        {
            var wc = new WebClient();
            wc.Credentials = nc;
            var observable = wc.GetResultObservableFoSecret();
            wc.OpenReadAsync(new Uri(string.Concat(ROOT_URL_BASE, method)));
            return observable;
        }

        private IObservable<IEnumerable<T>> GetObservableSyndication<T>(string method) where T : FeedItem, new()
        {
            var wc = new WebClient();
            wc.Credentials = nc;
            var observable = wc.GetSydicationObservable()
                .Select(feed =>
            {
                return from item in feed.Items
                       select item.Parse<T>();
            });
            var url = string.Concat(RSS_ROOT, method);
            wc.OpenReadAsync(new Uri(url));
            return observable;
        }

        private string CreateGetRecentMethod(string[] tags = null, int? count = null)
        {
            var parameters = new List<object>();
            if (tags != null && tags.Length > 0)
            {
                parameters.Add(Parameter("tag", string.Join(" ", tags)));
            }
            if (count != null)
            {
                parameters.Add(Parameter("count", count.Value.ToString()));
            }
            var method = string.Format("{0}?{1}", RECENT, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void GetDatesAsync(string[] tags = null, Action<IEnumerable<PinDate>> callback = null)
        {
            var method = CreateGetDatesMethod(tags);
            Get(callback, ParseDateAttribute, method);
        }

        public IObservable<IEnumerable<PinDate>> GetDates(string[] tags = null)
        {
            var method = CreateGetDatesMethod(tags);
            return GetObservableResult(method).SelectDateAttribute();
        }

        private string CreateGetDatesMethod(string[] tags)
        {
            var parameters = new List<object>();
            if (tags != null && tags.Length > 0)
            {
                parameters.Add(Parameter("tag", string.Join(" ", tags)));
            }

            var method = string.Format("{0}?{1}", DATES, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void GetSuggestedAsync(string url, Action<Suggested> callback = null)
        {
            var method = CreateSuggestedMethod(url);
            Get(callback, ParseSuggest, method);
        }

        public IObservable<Suggested> GetSuggested(string url)
        {
            var method = CreateSuggestedMethod(url);
            return GetObservableResult(method).SelectSuggested();
        }

        private string CreateSuggestedMethod(string url)
        {
            var parameters = new List<object>();
            parameters.Add(Parameter("url", url));
            var method = string.Format("{0}?{1}", SUGGEST, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void GetTagsAsync(Action<IEnumerable<Tag>> callback = null)
        {
            Get(callback, ParseTags, GET_TAG);
        }

        public IObservable<IEnumerable<Tag>> GetTags()
        {
            return GetObservableResult(GET_TAG).SelectTags();
        }

        public void DeleteTagAsync(string tag, Action<string> callback = null)
        {
            var method = CreateDeleteTagMethod(tag);
            Get(callback, ParseValue, method);
        }

        public IObservable<string> DeleteTag(string tag)
        {
            var method = CreateDeleteTagMethod(tag);
            return GetObservableResult(method).SelectValue();
        }

        private string CreateDeleteTagMethod(string tag)
        {
            var parameters = new List<object>();
            parameters.Add(Parameter("tag", tag));
            var method = string.Format("{0}?{1}", DELETE_TAG, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void RenameTagAsync(string oldName, string newName, Action<string> callback = null)
        {
            var method = CreateRenameTagMethod(oldName, newName);
            Get(callback, ParseValue, method);
        }

        public IObservable<string> RenameTag(string oldName, string newName)
        {
            var method = CreateRenameTagMethod(oldName, newName);
            return GetObservableResult(method).SelectValue();
        }

        private string CreateRenameTagMethod(string oldName, string newName)
        {
            var parameters = new List<object>();
            parameters.Add(Parameter("old", oldName));
            parameters.Add(Parameter("new", newName));
            var method = string.Format("{0}?{1}", RENAME_TAG, string.Join("&", parameters.ToArray()));
            return method;
        }

        public void GetUserSecretAsync(Action<string> callback)
        {
            Get(callback, ParseValue, SECRET);
        }

        public IObservable<string> GetUserSecret()
        {
            return GetObservableResultForSecret(SECRET).SelectValue().Catch((Exception e) => new BehaviorSubject<string>(null));
        }

        public void GetNetworkAsync(Action<IEnumerable<FeedItem>> callback = null)
        {
            GetUserSecret().Subscribe(secret =>
            {
                var method = string.Format(NETWORK, secret, user);
                GetNetworkAsync(method, callback);
            });
        }

        public void GetNetworkAsync(string method, Action<IEnumerable<FeedItem>> callback)
        {
            GetSyndication(method, callback);
        }

        public void GetPopularAsync(Action<IEnumerable<FeedItem>> callback)
        {
            GetSyndication(POPULAR, callback);
        }

        public IObservable<IEnumerable<NetworkItem>> GetNetwork()
        {
            return GetUserSecret().SelectMany(secret =>
                {
                    return GetNetwork(secret);
                });
        }

        public IObservable<IEnumerable<NetworkItem>> GetNetwork(string secret)
        {
            var method = string.Format(NETWORK, secret, user);
            return GetObservableSyndication<NetworkItem>(method);
        }

        public IObservable<IEnumerable<PopularItem>> GetPopular()
        {
            return GetObservableSyndication<PopularItem>(POPULAR);
        }

        public void GetRecentAllAsync(Action<IEnumerable<FeedItem>> callback)
        {
            GetSyndication(RECENT_RSS, callback);
        }

        public IObservable<IEnumerable<RecentItem>> GetRecentAll()
        {
            return GetObservableSyndication<RecentItem>(RECENT_RSS);
        }

        public void GetUserFeedAsync(string account, Action<IEnumerable<FeedItem>> callback)
        {
            var method = string.Format(USER, account);
            GetSyndication(method, callback);
        }

        public IObservable<IEnumerable<FeedItem>> GetUserFeed(string account)
        {
            var method = string.Format(USER, account);
            return GetObservableSyndication<FeedItem>(method);
        }

        public void GetTagFeedAsync(string tag, Action<IEnumerable<FeedItem>> callback)
        {
            var method = string.Format(TAG_SEARCH, tag);
            GetSyndication(method, callback);
        }

        public IObservable<IEnumerable<FeedItem>> GetTagFeed(string tag)
        {
            var method = string.Format(TAG_SEARCH, tag);
            return GetObservableSyndication<FeedItem>(method);
        }

        private string Parameter(string key, string value)
        {
            return string.Format("{0}={1}", key, Uri.EscapeUriString(value));
        }

        private void Get<T>(Action<T> callback, Func<Stream, T> parser, string method)
        {
            var client = new WebClient();
            client.Credentials = nc;

            client.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    throw new PinboardException(e.Error.Message);
                }
                ProcessApi<T>(callback, parser, e);
            };
            var url = string.Concat(ROOT_URL_BASE, method);
            client.OpenReadAsync(new Uri(url));
        }

        private void GetSyndication(string method, Action<IEnumerable<FeedItem>> callback)
        {
            var client = new WebClient();
            client.Credentials = nc;
            client.OpenReadCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    throw new PinboardException(args.Error.Message);
                }
                ProcessSyndication(callback, args);
            };
            var url = string.Concat(RSS_ROOT, method);
            client.OpenReadAsync(new Uri(url));
        }

        private static void ProcessApi<T>(Action<T> callback, Func<Stream, T> parser, OpenReadCompletedEventArgs args)
        {
            using (Stream s = args.Result)
            {
                var result = parser(args.Result);
                if (callback != null)
                {
                    callback(result);
                }
            }
        }

        private static void ProcessSyndication<T>(Action<IEnumerable<T>> callback, OpenReadCompletedEventArgs args) where T : FeedItem, new()
        {
            using (var reader = XmlReader.Create(args.Result))
            {
                var formatter = new Rss10FeedFormatter();
                formatter.ReadFrom(reader);
                SyndicationFeed feed = formatter.Feed;
                var items = feed.Parse<T>();
                callback(items);
            }
        }

        private IEnumerable<Post> ParsePosts(Stream response)
        {
            var doc = XElement.Load(response);
            var posts = from p in doc.Descendants("post")
                        select Post.Parse(p);
            return posts;
        }

        private IEnumerable<PinDate> ParseDateAttribute(Stream response)
        {
            var doc = XElement.Load(response);
            var dates = from d in doc.Descendants("date")
                        select PinDate.Parse(d);
            return dates;
        }

        private Suggested ParseSuggest(Stream response)
        {
            var doc = XElement.Load(response);
            return Suggested.Parse(doc);
        }

        private IEnumerable<Tag> ParseTags(Stream response)
        {
            var doc = XElement.Load(response);
            var tags = from p in doc.Descendants("tag")
                       select Tag.Parse(p);
            return tags;
        }

        private DateTime ParseTimeAttribute(Stream response)
        {
            return DateTime.Parse(XElement.Load(response).Attribute("time").Value);
        }

        private string ParseCodeAttribute(Stream response)
        {
            return XElement.Load(response).Attribute("code").Value;
        }

        private string ParseValue(Stream response)
        {
            return XElement.Load(response).Value;
        }
    }
}