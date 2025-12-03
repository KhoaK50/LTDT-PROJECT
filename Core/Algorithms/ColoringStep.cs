using System;

namespace Core.Algorithms
{
    /// <summary>
    /// Một bước tô màu trong thuật toán DSATUR.
    /// Vertex: đỉnh được tô ở bước này.
    /// Color : màu vừa tô.
    /// SnapshotColors: snapshot mảng màu tại thời điểm sau khi tô xong đỉnh đó.
    /// </summary>
    public class ColoringStep
    {
        public int Vertex { get; set; }
        public int Color { get; set; }
        public int[] SnapshotColors { get; set; }

        public ColoringStep()
        {
        }

        public ColoringStep(int vertex, int color, int[] snapshot)
        {
            Vertex = vertex;
            Color = color;
            SnapshotColors = snapshot;
        }
    }
}
