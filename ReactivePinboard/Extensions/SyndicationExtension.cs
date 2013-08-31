using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Api.ReactivePinboard.Model;

namespace Mono.Api.ReactivePinboard.Extensions
{
    internal static class SyndicationExtension
    {
        public static string GetAccount(this TextSyndicationContent content)
        {
            var str = Regex.Match(content.Text, @"\(.+?\)").Value;
            if (content.Text.StartsWith("(") && str != null && str.Length > 1)
            {
                return str.Substring(1, str.Length - 2);
            }
            else
            {
                return null;
            }
        }

        public static string GetTitle(this TextSyndicationContent content)
        {
            var str = Regex.Match(content.Text, @"\s.+").Value;
            if (content.Text.StartsWith("(") && str != null && str.Length > 0)
            {
                return str.Substring(1);
            }
            else
            {
                return content.Text;
            }
        }

        public static T Parse<T>(this SyndicationItem item) where T : FeedItem, new()
        {
            var preText = item.Title;
            var summary = item.Summary;
            var tags = new List<string>();

            item.Categories.ToList().ForEach(c => tags.Add(c.Name));
            string source = null;
            if (item.Links.Count > 1)
            {
                source = item.Links[1].Uri.ToString();
            }
            string author = null;
            if (item.Authors.Count > 0)
            {
                author = item.Authors[0].Name;
            }

            return new T
            {
                Title = preText.GetTitle(),
                Description = summary != null ? summary.Text.Substring(0, Math.Min(500, summary.Text.Length)) : null,
                Url = item.Links[0].Uri.ToString(),
                Account = preText.GetAccount(),
                Time = DateTime.Parse(item.PublishDate.ToString()),
                Author = author,
                Tags = tags.ToArray(),
                Id = item.Id + typeof(T).ToString(),
                Source = source,
            };
        }

        public static IEnumerable<T> Parse<T>(this SyndicationFeed feed) where T : FeedItem, new()
        {
            return from item in feed.Items
                   select item.Parse<T>();
        }
    }
}