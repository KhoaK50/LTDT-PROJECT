using Core.Graph;
using DocumentFormat.OpenXml.Office.CustomXsn;
using System;
using System.Collections.Generic;

namespace Core.Algorithms
{
    /// <summary>
    /// Thuật toán tô màu DSATUR trên đồ thị vô hướng, đơn.
    /// </summary>
    public static class DSaturAlgorithm
    {
        public static DSaturResult Colorize(Core.Graph.Graph g)
        {
            if (g == null) throw new ArgumentNullException("g");

            int n = g.VertexCount;

            // Màu của từng đỉnh, -1 = chưa tô
            int[] color = new int[n];
            for (int i = 0; i < n; i++) color[i] = -1;

            int[] degree = new int[n];
            int[] saturation = new int[n];

            // Tính bậc ban đầu và khởi tạo độ bão hoà
            for (int u = 0; u < n; u++)
            {
                int d = 0;
                foreach (int v in g.GetNeighbors(u))
                    d++;

                degree[u] = d;
                saturation[u] = 0;
            }

            List<ColoringStep> steps = new List<ColoringStep>();

            for (int iter = 0; iter < n; iter++)
            {
                // 1. Chọn đỉnh có độ bão hoà cao nhất (tie-break theo bậc)
                int chosenVertex = -1;
                int bestSat = -1;
                int bestDeg = -1;

                for (int i = 0; i < n; i++)
                {
                    if (color[i] != -1) continue; // đã tô

                    if (saturation[i] > bestSat ||
                        (saturation[i] == bestSat && degree[i] > bestDeg))
                    {
                        bestSat = saturation[i];
                        bestDeg = degree[i];
                        chosenVertex = i;
                    }
                }

                if (chosenVertex == -1)
                    break; // an toàn

                // 2. Chọn màu nhỏ nhất không trùng hàng xóm
                bool[] used = new bool[n];
                foreach (int v in g.GetNeighbors(chosenVertex))
                {
                    int c = color[v];
                    if (c >= 0 && c < n) used[c] = true;
                }

                int chosenColor = 0;
                while (chosenColor < n && used[chosenColor]) chosenColor++;

                color[chosenVertex] = chosenColor;

                // 3. Cập nhật độ bão hoà cho hàng xóm chưa tô
                for (int v = 0; v < n; v++)
                {
                    if (color[v] != -1) continue;
                    if (!g.HasEdge(chosenVertex, v)) continue;

                    bool[] neighborColors = new bool[n];
                    foreach (int w in g.GetNeighbors(v))
                    {
                        int c = color[w];
                        if (c >= 0 && c < n) neighborColors[c] = true;
                    }

                    int sat = 0;
                    for (int c = 0; c < n; c++)
                        if (neighborColors[c]) sat++;

                    saturation[v] = sat;
                }

                // 4. Lưu step cho animation
                int[] snapshot = new int[n];
                Array.Copy(color, snapshot, n);

                ColoringStep step = new ColoringStep();
                step.Vertex = chosenVertex;
                step.Color = chosenColor;
                step.SnapshotColors = snapshot;

                steps.Add(step);
            }

            return new DSaturResult(color, steps);
        }
    }
}
