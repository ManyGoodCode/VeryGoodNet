// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleFlags.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleLibrary
{
    using System;

    /// <summary>
    /// Properties of an example.
    /// </summary>
    [Flags]
    public enum ExampleFlags
    {
        /// <summary>
        /// 将轴转置，使水平轴变成垂直，反之亦然。 
        /// </summary>
        Transpose = 1,

        /// <summary>
        /// 反转坐标轴，使它们的开始和结束位置反映在情节区域内。 
        /// </summary>
        Reverse = 2,
    }
}
