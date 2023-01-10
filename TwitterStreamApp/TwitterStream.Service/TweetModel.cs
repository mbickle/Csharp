using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Interfaces;

namespace TwitterStream.Service
{
    public class TweetModel : ITweetModel
    {
        public string TweetId { get; set; }
        public string Content { get; set; }

        public TweetModel(string tweetId, string content)
        {
            TweetId = tweetId;
            Content = content;
        }
    }
}
