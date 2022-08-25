using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    internal class PriorityComparisonCallback : SharpDX.CallbackBase, PriorityComparison
    {
        public Manager.PriorityComparisonDelegate Delegate;

        public bool HasPriority(Storyboard scheduledStoryboard, Storyboard newStoryboard, PriorityEffect priorityEffect)
        {
            if (Delegate != null)
                return Delegate.Invoke(scheduledStoryboard, newStoryboard, priorityEffect);
            return false;
        }
    }
}