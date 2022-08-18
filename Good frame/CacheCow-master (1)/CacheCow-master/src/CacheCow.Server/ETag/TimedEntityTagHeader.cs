using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Server
{
	public class TimedEntityTagHeaderValue
	{
		public DateTimeOffset? LastModified { get; set; }
        public EntityTagHeaderValue ETag { get; }

        public TimedEntityTagHeaderValue(string tag, bool isWeak = false)
		{
            if (tag == null)
                throw new ArgumentNullException("tag");

            if (!tag.StartsWith("\""))
                tag = "\"" + tag;

            if (!tag.EndsWith("\""))
                tag = tag + "\"";

            ETag = new EntityTagHeaderValue(tag, isWeak);
		}

        public TimedEntityTagHeaderValue(EntityTagHeaderValue entityTagHeaderValue)
        {
            ETag = entityTagHeaderValue;
        }

        public TimedEntityTagHeaderValue(DateTimeOffset lastModified)
            : this((EntityTagHeaderValue)null)
        {
            LastModified = lastModified;
        }
	}
}
