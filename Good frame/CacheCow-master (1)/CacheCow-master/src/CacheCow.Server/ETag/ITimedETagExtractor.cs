using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    public interface ITimedETagExtractor
    {
        TimedEntityTagHeaderValue Extract(object viewModel);
    }

    public interface ITimedETagExtractor<TViewModel> : ITimedETagExtractor
    {
        TimedEntityTagHeaderValue Extract(TViewModel viewModel);
    }
}
