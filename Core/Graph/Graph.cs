using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.Graph
{
    /// <summary>
    /// Đồ thị vô hướng, đơn (không đa cạnh, không có khuyên),
    /// dùng để biểu diễn xung đột giữa các môn thi.
    /// Đỉnh: danh sách MonThi
    /// Cạnh: ma trận kề bool[,]
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Danh sách đỉnh (môn thi).
        /// Index trong list chính là Id dùng cho ma trận kề.
        /// </summary>
        public List<MonThi> Vertices { get; }

        /// <summary>
        /// Ma trận kề. AdjMatrix[u, v] = true nếu có cạnh giữa u và v.
        /// </summary>
        public bool[,] AdjMatrix { get; }

        /// <summary>
        /// Số đỉnh của đồ thị.
        /// </summary>
        public int VertexCount
        {
            get { return Vertices.Count; }
        }

        /// <summary>
        /// Số cạnh của đồ thị (tính cho đồ thị vô hướng).
        /// </summary>
        public int EdgeCount { get; private set; }

        /// <summary>
        /// Khởi tạo đồ thị từ danh sách môn thi.
        /// Mặc định chưa có cạnh nào (ma trận kề toàn false).
        /// </summary>
        public Graph(List<MonThi> vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            Vertices = vertices;
            int n = vertices.Count;
            AdjMatrix = new bool[n, n];
            EdgeCount = 0;
        }

        /// <summary>
        /// Trả về môn thi tại vị trí index.
        /// </summary>
        public MonThi GetVertex(int index)
        {
            if (index < 0 || index >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return Vertices[index];
        }

        /// <summary>
        /// Thêm cạnh giữa u và v (vô hướng).
        /// Bỏ qua nếu là khuyên (u == v) hoặc cạnh đã tồn tại.
        /// </summary>
        public void AddEdge(int u, int v)
        {
            if (u < 0 || u >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(u));
            if (v < 0 || v >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(v));

            if (u == v)
                return; // không thêm khuyên

            if (!AdjMatrix[u, v])
            {
                AdjMatrix[u, v] = true;
                AdjMatrix[v, u] = true;
                EdgeCount++;
            }
        }

        /// <summary>
        /// Kiểm tra có cạnh giữa u và v không.
        /// </summary>
        public bool HasEdge(int u, int v)
        {
            if (u < 0 || u >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(u));
            if (v < 0 || v >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(v));

            return AdjMatrix[u, v];
        }

        /// <summary>
        /// Liệt kê các đỉnh kề với u.
        /// </summary>
        public IEnumerable<int> GetNeighbors(int u)
        {
            if (u < 0 || u >= VertexCount)
                throw new ArgumentOutOfRangeException(nameof(u));

            for (int v = 0; v < VertexCount; v++)
            {
                if (AdjMatrix[u, v])
                    yield return v;
            }
        }
    }
}
