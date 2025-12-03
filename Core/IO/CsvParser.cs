using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Models;

namespace Core.IO
{
    /// <summary>
    /// Đọc / phân tích CSV cho môn và xung đột.
    /// Quy ước:
    ///  - MonHoc.csv:  Id,MaMon,TenMon
    ///  - XungDot.csv: FromId,ToId  (tham chiếu tới cột Id của MonHoc.csv)
    /// </summary>
    public static class CsvParser
    {
        // ====================== MÔN HỌC (ĐỈNH) ======================

        /// <summary>
        /// Đọc file môn học: Id,MaMon,TenMon.
        /// </summary>
        public static List<MonThi> ReadSubjectsCsv(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Không tìm thấy file môn.", filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
                throw new InvalidDataException("File môn rỗng.");

            var header = SplitCsvLine(lines[0]);
            int colId = IndexOf(header, "Id");
            int colMa = IndexOf(header, "MaMon");
            int colTen = IndexOf(header, "TenMon");

            if (colId < 0 || colMa < 0 || colTen < 0)
                throw new InvalidDataException(
                    "Header file môn phải có đủ 3 cột: Id, MaMon, TenMon.");

            var result = new List<MonThi>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = SplitCsvLine(line);
                if (cols.Length <= Math.Max(colTen, Math.Max(colId, colMa)))
                    continue;

                if (!int.TryParse(
                        cols[colId],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int id))
                {
                    throw new InvalidDataException(
                        $"Id không hợp lệ ở dòng {i + 1}: \"{cols[colId]}\".");
                }

                string ma = cols[colMa].Trim();
                string ten = cols[colTen].Trim();

                if (string.IsNullOrEmpty(ma))
                    throw new InvalidDataException(
                        $"MaMon trống ở dòng {i + 1}.");

                result.Add(new MonThi(id, ma, ten));
            }

            return result;
        }

        /// <summary>
        /// Phân tích chuỗi nhập tay cho danh sách môn.
        /// Mỗi dòng: Id,MaMon,TenMon.
        /// Nếu người dùng không nhập header, hàm sẽ tự coi toàn bộ là data.
        /// </summary>
        public static List<MonThi> ParseSubjectsFromText(string text)
        {
            var rawLines = new List<string>();

            using (var reader = new StringReader(text ?? string.Empty))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        rawLines.Add(line);
                }
            }

            if (rawLines.Count == 0)
                return new List<MonThi>();

            // Nếu dòng đầu không chứa "Id" thì coi như không có header,
            // tự chèn "Id,MaMon,TenMon" vào đầu.
            if (rawLines[0].IndexOf("Id", StringComparison.OrdinalIgnoreCase) < 0)
            {
                rawLines.Insert(0, "Id,MaMon,TenMon");
            }

            string tmp = Path.GetTempFileName();
            File.WriteAllLines(tmp, rawLines);
            try
            {
                return ReadSubjectsCsv(tmp);
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        // ====================== XUNG ĐỘT (CẠNH) ======================

        /// <summary>
        /// Đọc file xung đột: FromId,ToId.
        /// Trả về list (MaMonA, MaMonB) để dùng chung với GraphBuilder.BuildFromConflicts.
        /// </summary>
        public static List<(string maA, string maB)> ReadConflictsCsv(
            string filePath,
            List<MonThi> dsMon)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Không tìm thấy file xung đột.", filePath);

            if (dsMon == null || dsMon.Count == 0)
                throw new ArgumentException(
                    "Danh sách môn trống, hãy đọc môn trước.",
                    nameof(dsMon));

            // Map Id -> MaMon
            var maById = dsMon.ToDictionary(m => m.Id, m => m.MaMon);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
                throw new InvalidDataException("File xung đột rỗng.");

            var header = SplitCsvLine(lines[0]);
            int colFrom = IndexOf(header, "FromId");
            int colTo = IndexOf(header, "ToId");

            if (colFrom < 0 || colTo < 0)
                throw new InvalidDataException(
                    "Header file xung đột phải có các cột: FromId, ToId.");

            var result = new List<(string, string)>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = SplitCsvLine(line);
                if (cols.Length <= Math.Max(colFrom, colTo))
                    continue;

                if (!int.TryParse(
                        cols[colFrom],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int idA) ||
                    !int.TryParse(
                        cols[colTo],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int idB))
                {
                    throw new InvalidDataException(
                        $"FromId/ToId không hợp lệ ở dòng {i + 1}.");
                }

                if (!maById.TryGetValue(idA, out string maA))
                    throw new InvalidDataException(
                        $"Không tìm thấy môn có Id = {idA} (dòng {i + 1}).");

                if (!maById.TryGetValue(idB, out string maB))
                    throw new InvalidDataException(
                        $"Không tìm thấy môn có Id = {idB} (dòng {i + 1}).");

                if (string.Equals(maA, maB, StringComparison.OrdinalIgnoreCase))
                    continue; // bỏ tự xung đột

                // Chuẩn hoá A < B để tránh trùng (1-2 & 2-1)
                if (string.Compare(maA, maB, StringComparison.OrdinalIgnoreCase) > 0)
                    (maA, maB) = (maB, maA);

                if (!result.Contains((maA, maB)))
                    result.Add((maA, maB));
            }

            return result;
        }

        /// <summary>
        /// Phân tích text xung đột nhập tay.
        /// Hỗ trợ dạng:
        ///  - "1,2"
        ///  - "1-2"
        /// Mỗi dòng một cặp.
        /// </summary>
        public static List<(string maA, string maB)> ParseConflictsFromText(
            string text,
            List<MonThi> dsMon)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(text ?? string.Empty))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Cho phép 1-2 -> 1,2
                    line = line.Replace("-", ",");
                    lines.Add(line);
                }
            }

            if (lines.Count == 0)
                return new List<(string, string)>();

            // Nếu dòng đầu không phải header, tự chèn "FromId,ToId"
            if (lines[0].IndexOf("FromId", StringComparison.OrdinalIgnoreCase) < 0)
            {
                lines.Insert(0, "FromId,ToId");
            }

            string tmp = Path.GetTempFileName();
            File.WriteAllLines(tmp, lines);
            try
            {
                return ReadConflictsCsv(tmp, dsMon);
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        // ====================== HÀM PHỤ ======================

        private static string[] SplitCsvLine(string line)
        {
            return (line ?? string.Empty)
                .Split(',')
                .Select(s => s.Trim())
                .ToArray();
        }

        private static int IndexOf(string[] header, string name)
        {
            for (int i = 0; i < header.Length; i++)
            {
                if (string.Equals(header[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }
    }
}
