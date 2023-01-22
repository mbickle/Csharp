using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStream.Interfaces
{
    public interface IHashtag
    {
        Guid Id { get; set; }
        long TweetId { get; set; }
        string Content { get; set; }
        int Count { get; set; }
    }
}
