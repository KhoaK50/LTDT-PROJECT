using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.Graph
{
    /// <summary>
    /// Xây dựng đồ thị từ danh sách môn và xung đột theo:
    ///  (1) Mã môn: (string maA, string maB)
    ///  (2) Id môn:  (int fromId, int toId)
    /// </summary>
    public static class GraphBuilder
    {
        // ============================================================
        // 1) Build từ mã môn (CTDL, LTDT, …)
        // ============================================================
        public static Graph BuildFromConflicts(
            List<MonThi> dsMon,
            List<(string maA, string maB)> dsXungDot)
        {
            if (dsMon == null) throw new ArgumentNullException(nameof(dsMon));
            if (dsXungDot == null) throw new ArgumentNullException(nameof(dsXungDot));

            // Map mã môn -> Id
            var indexOf = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < dsMon.Count; i++)
            {
                dsMon[i].Id = i;
                indexOf[dsMon[i].MaMon] = i;
            }

            var graph = new Graph(dsMon);

            foreach (var (maA, maB) in dsXungDot)
            {
                if (!indexOf.ContainsKey(maA) || !indexOf.ContainsKey(maB))
                    continue;

                graph.AddEdge(indexOf[maA], indexOf[maB]);
            }

            return graph;
        }

        // ============================================================
        // 2) Build từ ID môn (dạng 1-2, 3-5, …)
        // ============================================================
        public static Graph BuildFromIdConflicts(
            List<MonThi> dsMon,
            List<(int fromId, int toId)> dsXungDotId)
        {
            if (dsMon == null) throw new ArgumentNullException(nameof(dsMon));
            if (dsXungDotId == null) throw new ArgumentNullException(nameof(dsXungDotId));

            // Gán Id = đúng vị trí trong list
            for (int i = 0; i < dsMon.Count; i++)
            {
                dsMon[i].Id = i;
            }

            var graph = new Graph(dsMon);

            // fromId, toId là ZERO–BASED từ CSV luôn
            foreach (var (u, v) in dsXungDotId)
            {
                if (u < 0 || u >= dsMon.Count) continue;
                if (v < 0 || v >= dsMon.Count) continue;

                graph.AddEdge(u, v);
            }

            return graph;
        }
    }
}
