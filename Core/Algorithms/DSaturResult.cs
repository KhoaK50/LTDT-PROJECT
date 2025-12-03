using System.Collections.Generic;

namespace Core.Algorithms
{
    /// <summary>
    /// Kết quả sau khi tô màu DSATUR.
    /// Colors[i] = màu (ca thi) của đỉnh i.
    /// Steps     = danh sách các bước để animation.
    /// </summary>
    public class DSaturResult
    {
        public int[] Colors { get; private set; }
        public List<ColoringStep> Steps { get; private set; }

        public DSaturResult(int[] colors, List<ColoringStep> steps)
        {
            Colors = colors;
            Steps = steps ?? new List<ColoringStep>();
        }
    }
}
