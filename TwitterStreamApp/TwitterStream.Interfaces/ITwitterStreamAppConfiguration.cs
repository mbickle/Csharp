using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStream.Interfaces
{
    public interface ITwitterStreamAppConfiguration
    {
        string BearerToken { get; set; }
        int RetryWaitSeconds { get; set; }
        int RefreshSeconds { get; set; }
        string TwitterUrl { get; set; }
    }
}
