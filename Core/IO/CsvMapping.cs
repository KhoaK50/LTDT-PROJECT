using System;

namespace Core.IO
{
    /// <summary>
    /// Định nghĩa tên cột chuẩn & hàm tiện ích để tìm index cột
    /// trong file CSV môn học (MaMon, TenMon, Conflicts, ...).
    /// </summary>
    public static class CsvMapping
    {
        // Tên cột chuẩn trong file CSV
        public const string Col_MaMon = "MaMon";
        public const string Col_TenMon = "TenMon";
        public const string Col_Conflicts = "Conflicts";

        /// <summary>
        /// Tìm vị trí cột theo tên cột trong dòng header.
        /// Trả về -1 nếu không thấy.
        /// </summary>
        public static int IndexOf(string[] header, string columnName)
        {
            if (header == null || columnName == null)
                return -1;

            for (int i = 0; i < header.Length; i++)
            {
                var col = header[i];
                if (col == null)
                    continue;

                if (string.Equals(col.Trim(), columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Lấy index 3 cột chính: MaMon, TenMon, Conflicts.
        /// </summary>
        public static bool TryGetIndices(
            string[] header,
            out int idxMaMon,
            out int idxTenMon,
            out int idxConflicts)
        {
            idxMaMon = IndexOf(header, Col_MaMon);
            idxTenMon = IndexOf(header, Col_TenMon);
            idxConflicts = IndexOf(header, Col_Conflicts);

            return idxMaMon >= 0 && idxTenMon >= 0 && idxConflicts >= 0;
        }
    }
}
