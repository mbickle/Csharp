using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStream.Interfaces
{
    public interface IReport
    {
        Task Report(CancellationToken cancellation);
    }
}
