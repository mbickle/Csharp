using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStream.Service
{
    public class TweetDto
    {
        public string id { get; set; }
        public List<string> edit_history_tweet_ids { get; set; }
        public string text { get; set; }
    }
}
