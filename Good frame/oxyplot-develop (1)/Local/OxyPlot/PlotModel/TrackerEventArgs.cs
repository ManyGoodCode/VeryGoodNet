// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackerEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for the tracker event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot
{
    using System;

    /// <summary>
    /// Provides data for the tracker event.
    /// </summary>
    public class TrackerEventArgs : EventArgs
    {
        public TrackerHitResult HitResult { get; set; }
    }
}