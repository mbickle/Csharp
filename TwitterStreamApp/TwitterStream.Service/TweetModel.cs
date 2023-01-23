using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Interfaces;

namespace TwitterStream.Service
{
    public class TweetModel : ITweet
    {
        public long Id { get; set; }
        public string Content { get; set; }

        public TweetModel(long tweetId, string content)
        {
            Id = tweetId;
            Content = content;
        }
    }
}
