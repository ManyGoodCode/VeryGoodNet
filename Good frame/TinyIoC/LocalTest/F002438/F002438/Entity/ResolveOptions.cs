using F002438.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Entity
{
    public sealed class ResolveOptions
    {
        private static readonly ResolveOptions _default = new ResolveOptions();
        private static readonly ResolveOptions failUnregisteredAndNameNotFound = new ResolveOptions()
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };

        private static readonly ResolveOptions failUnregisteredOnly = new ResolveOptions()
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };

        private static readonly ResolveOptions failNameNotFoundOnly = new ResolveOptions()
        {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve
        };

        private UnregisteredResolutionActions unregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve;
        public UnregisteredResolutionActions UnregisteredResolutionAction
        {
            get { return unregisteredResolutionAction; }
            set { unregisteredResolutionAction = value; }
        }

        private NamedResolutionFailureActions namedResolutionFailureAction = NamedResolutionFailureActions.Fail;
        public NamedResolutionFailureActions NamedResolutionFailureAction
        {
            get { return namedResolutionFailureAction; }
            set { namedResolutionFailureAction = value; }
        }


        public static ResolveOptions Default
        {
            get { return _default; }
        }

        public static ResolveOptions FailNameNotFoundOnly
        {
            get { return failNameNotFoundOnly; }
        }

        public static ResolveOptions FailUnregisteredAndNameNotFound
        {
            get { return failUnregisteredAndNameNotFound; }
        }

        public static ResolveOptions FailUnregisteredOnly
        {
            get { return failUnregisteredOnly; }
        }
    }
}
