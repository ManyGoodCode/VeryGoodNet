using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    public class DefaultTimedETagExtractor : ITimedETagExtractor
    {
        private readonly ISerialiser serialiser;
        private readonly IHasher hasher;

        public DefaultTimedETagExtractor(ISerialiser serialiser, IHasher hasher)
        {
            this.serialiser = serialiser;
            this.hasher = hasher;
        }

        public TimedEntityTagHeaderValue Extract(object viewModel)
        {
            ICacheResource resource = viewModel as ICacheResource;
            if (resource != null)
                return resource.GetTimedETag();

            return new TimedEntityTagHeaderValue(hasher.ComputeHash(bytes: serialiser.Serialise(viewModel)));
        }
    }


    public class DefaultTimedETagExtractor<TViewModel> : DefaultTimedETagExtractor, ITimedETagExtractor<TViewModel>
    {
        public DefaultTimedETagExtractor(ISerialiser serialiser, IHasher hasher) : base(serialiser, hasher)
        {
        }

        public TimedEntityTagHeaderValue Extract(TViewModel viewModel)
        {
            return base.Extract(viewModel);
        }
    }

}
