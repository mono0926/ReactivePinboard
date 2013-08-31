using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using Mono.Api.ReactivePinboard.Extensions;

namespace Mono.Api.ReactivePinboard.Model
{
    [Table]
    [InheritanceMapping(Code = "P", Type = typeof(Post))]
    [InheritanceMapping(Code = "R", Type = typeof(RecentItem))]
    [InheritanceMapping(Code = "PO", Type = typeof(PopularItem))]
    [InheritanceMapping(Code = "N", Type = typeof(NetworkItem))]
    [InheritanceMapping(Code = "I", Type = typeof(ItemBase), IsDefault = true)]
    public class ItemBase
    {
        [Column(IsPrimaryKey = true)]
        public string Id { get; set; }

        [Column(IsDiscriminator = true)]
        public string Type;

        [Column]
        public string Title { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string Url { get; set; }

        private string tagString;

        [Column]
        public string TagString
        {
            get
            {
                return tagString;
            }
            set
            {
                this.tagString = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.tags = new string[0];
                }
                else
                {
                    this.tags = value.Split(' ');
                }
            }
        }

        private string[] tags;

        public string[] Tags
        {
            get
            {
                return tags;
            }
            set
            {
                this.tags = value;
                this.tagString = string.Join(" ", value);
            }
        }

        [Column]
        public DateTime? Time { get; set; }

        public string TimeStr { get { return Time.HasValue ? Time.Value.ToUTCDateTimeStr() : string.Empty; } }

        public string DateStr { get { return Time.HasValue ? Time.Value.ToUTCDateStr() : string.Empty; } }
    }
}