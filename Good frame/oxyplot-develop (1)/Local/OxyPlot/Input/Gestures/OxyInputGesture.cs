namespace OxyPlot
{
    using System;
    public abstract class OxyInputGesture : IEquatable<OxyInputGesture>
    {
        public abstract bool Equals(OxyInputGesture other);
    }
}