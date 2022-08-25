namespace SharpDX.Animation
{
    [Shadow(typeof(PriorityComparisonShadow))]
    internal partial interface PriorityComparison
    {
        bool HasPriority( SharpDX.Animation.Storyboard scheduledStoryboard, SharpDX.Animation.Storyboard newStoryboard, SharpDX.Animation.PriorityEffect priorityEffect);
    }
}