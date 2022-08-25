using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    public partial class Manager
    {
        private static readonly Guid ManagerGuid = new Guid("4C1FC63A-695C-47E8-A339-1A194BE3D0B8");
        private ManagerEventHandlerCallback statusEventHandler;

        public Manager()
        {
            Utilities.CreateComInstance(ManagerGuid, Utilities.CLSCTX.ClsctxInprocServer, Utilities.GetGuidFromType(typeof(Manager)), this);
        }

        public delegate void StatusChangedDelegate(ManagerStatus newStatus, ManagerStatus previousStatus);
        public delegate bool PriorityComparisonDelegate(Storyboard scheduledStoryboard, Storyboard newStoryboard, PriorityEffect priorityEffect);
        public event StatusChangedDelegate StatusChanged
        {
            add
            {
                if (statusEventHandler == null)
                {
                    statusEventHandler = new ManagerEventHandlerCallback();
                    SetManagerEventHandler(statusEventHandler);
                }

                statusEventHandler.Delegates += value;
            }

            remove
            {
                if (statusEventHandler == null) 
                    return;
                statusEventHandler.Delegates -= value;
                if (statusEventHandler.Delegates.GetInvocationList().Length == 0)
                {
                    SetManagerEventHandler(null);
                    statusEventHandler.Dispose();
                    statusEventHandler = null;
                }
            }
        }

        private PriorityComparisonCallback cancelPriorityComparisonCallback;
        public PriorityComparisonDelegate CancelPriorityComparison
        {
            set
            {
                if (value != null)
                {
                    if (cancelPriorityComparisonCallback == null)
                    {
                        cancelPriorityComparisonCallback = new PriorityComparisonCallback { Delegate = value };
                        SetCancelPriorityComparison(cancelPriorityComparisonCallback);
                    }
                    cancelPriorityComparisonCallback.Delegate = value;
                }
                else if (cancelPriorityComparisonCallback != null)
                {
                    SetCancelPriorityComparison(null);
                    cancelPriorityComparisonCallback.Dispose();
                    cancelPriorityComparisonCallback = null;
                }
            }
        }

        private PriorityComparisonCallback trimPriorityComparisonCallback;

        public PriorityComparisonDelegate TrimPriorityComparison
        {
            set
            {
                if (value != null)
                {
                    if (trimPriorityComparisonCallback == null)
                    {
                        trimPriorityComparisonCallback = new PriorityComparisonCallback { Delegate = value };
                        SetTrimPriorityComparison(trimPriorityComparisonCallback);
                    }
                    trimPriorityComparisonCallback.Delegate = value;
                }
                else if (trimPriorityComparisonCallback != null)
                {
                    SetTrimPriorityComparison(null);
                    trimPriorityComparisonCallback.Dispose();
                    trimPriorityComparisonCallback = null;
                }
            }
        }

        private PriorityComparisonCallback compressPriorityComparisonCallback;
        public PriorityComparisonDelegate CompressPriorityComparison
        {
            set
            {
                if (value != null)
                {
                    if (compressPriorityComparisonCallback == null)
                    {
                        compressPriorityComparisonCallback = new PriorityComparisonCallback { Delegate = value };
                        SetCompressPriorityComparison(compressPriorityComparisonCallback);
                    }
                    compressPriorityComparisonCallback.Delegate = value;
                }
                else if (compressPriorityComparisonCallback != null)
                {
                    SetCompressPriorityComparison(null);
                    compressPriorityComparisonCallback.Dispose();
                    compressPriorityComparisonCallback = null;
                }
            }
        }

        private PriorityComparisonCallback concludePriorityComparisonCallback;
        public PriorityComparisonDelegate ConcludePriorityComparison
        {
            set
            {
                if (value != null)
                {
                    if (concludePriorityComparisonCallback == null)
                    {
                        concludePriorityComparisonCallback = new PriorityComparisonCallback { Delegate = value };
                        SetConcludePriorityComparison(concludePriorityComparisonCallback);
                    }
                    concludePriorityComparisonCallback.Delegate = value;
                }
                else if (concludePriorityComparisonCallback != null)
                {
                    SetConcludePriorityComparison(null);
                    concludePriorityComparisonCallback.Dispose();
                    concludePriorityComparisonCallback = null;
                }
            }
        }

        public SharpDX.Animation.Variable GetVariableFromTag(int id, object tagObject = null)
        {
            var tagObjectHandle = GCHandle.Alloc(tagObject);
            try
            {
                return GetVariableFromTag(GCHandle.ToIntPtr(tagObjectHandle), id);
            } 
            finally
            {
                tagObjectHandle.Free();
            }
        }

        public SharpDX.Animation.Storyboard GetStoryboardFromTag(int id, object tagObject = null)
        {
            var tagObjectHandle = GCHandle.Alloc(tagObject);
            try
            {
                return GetStoryboardFromTag(GCHandle.ToIntPtr(tagObjectHandle), id);
            }
            finally
            {
                tagObjectHandle.Free();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (statusEventHandler != null)
            {
                SetManagerEventHandler(null);
                statusEventHandler.Dispose();
                statusEventHandler = null;
            }

            if (cancelPriorityComparisonCallback != null)
            {
                SetCancelPriorityComparison(null);
                cancelPriorityComparisonCallback.Dispose();
                cancelPriorityComparisonCallback = null;
            }

            if (concludePriorityComparisonCallback != null)
            {
                SetConcludePriorityComparison(null);
                concludePriorityComparisonCallback.Dispose();
                concludePriorityComparisonCallback = null;
            }

            if (compressPriorityComparisonCallback != null)
            {
                SetCompressPriorityComparison(null);
                compressPriorityComparisonCallback.Dispose();
                compressPriorityComparisonCallback = null;
            }

            if (concludePriorityComparisonCallback != null)
            {
                SetConcludePriorityComparison(null);
                concludePriorityComparisonCallback.Dispose();
                concludePriorityComparisonCallback = null;
            }

            base.Dispose(disposing);
        }
    }
}