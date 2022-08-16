using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Common
{
    public enum CacheValidationStatus
    {
        None = 0,
        GetIfModifiedSince = 1,
        GetIfNoneMatch = 2,
        PutPatchDeleteIfUnModifiedSince = 3,
        PutPatchDeleteIfMatch = 4
    }
}
