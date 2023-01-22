using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStream.Data.Models
{
    public class Hashtag
    {
        public Guid Id { get; set; }
        public long TweetId { get; set; }
        public string Content { get; set; }
        public int Count { get; set; }
    }
}
