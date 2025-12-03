using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using Core.Algorithms;
using Core.Models;

namespace Core.Export
{
    public static class ExcelExporter
    {
        /// <summary>
        /// Hàm cũ (không truyền ngày) – mặc định dùng ngày hôm nay.
        /// </summary>
        public static void Export(
            string filePath,
            List<MonThi> dsMon,
            DSaturResult dsaturResult,
            int soCaMoiNgay,
            string[] khungGioTheoCa = null)
        {
            Export(filePath, dsMon, dsaturResult, soCaMoiNgay, khungGioTheoCa, DateTime.Today);
        }

        /// <summary>
        /// Hàm mới: có truyền ngày bắt đầu, đặt tên sheet theo dd-MM-yyyy.
        /// </summary>
        public static void Export(
            string filePath,
            List<MonThi> dsMon,
            DSaturResult dsaturResult,
            int soCaMoiNgay,
            string[] khungGioTheoCa,
            DateTime startDate)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Đường dẫn file không hợp lệ.");

            if (dsMon == null || dsaturResult == null)
                throw new ArgumentNullException();

            if (soCaMoiNgay <= 0)
                throw new ArgumentException("Số ca mỗi ngày phải > 0.");

            int n = dsMon.Count;
            int[] colors = dsaturResult.Colors;

            if (colors == null || colors.Length != n)
                throw new ArgumentException("Mảng màu DSaturResult không hợp lệ.");

            // ===== 0. Khung giờ mặc định nếu không truyền vào =====
            if (khungGioTheoCa == null || khungGioTheoCa.Length < soCaMoiNgay)
            {
                string[] macDinh = new string[soCaMoiNgay];

                if (soCaMoiNgay >= 1) macDinh[0] = "7:00 - 9:00";
                if (soCaMoiNgay >= 2) macDinh[1] = "9:30 - 11:30";
                if (soCaMoiNgay >= 3) macDinh[2] = "13:00 - 15:00";
                if (soCaMoiNgay >= 4) macDinh[3] = "15:30 - 17:30";

                khungGioTheoCa = macDinh;
            }

            // ===== 1. Gom môn theo ngày & ca =====
            Dictionary<int, Dictionary<int, List<MonThi>>> groups =
                new Dictionary<int, Dictionary<int, List<MonThi>>>();

            int maxNgay = 0;

            for (int i = 0; i < n; i++)
            {
                int colorId = colors[i];
                if (colorId < 0) continue;

                CaThi ct = new CaThi(colorId, soCaMoiNgay, khungGioTheoCa);

                if (!groups.ContainsKey(ct.Ngay))
                    groups[ct.Ngay] = new Dictionary<int, List<MonThi>>();

                if (!groups[ct.Ngay].ContainsKey(ct.Ca))
                    groups[ct.Ngay][ct.Ca] = new List<MonThi>();

                groups[ct.Ngay][ct.Ca].Add(dsMon[i]);

                if (ct.Ngay > maxNgay)
                    maxNgay = ct.Ngay;
            }

            // ===== 2. Tạo file Excel =====
            using (XLWorkbook wb = new XLWorkbook())
            {
                int soNgay = maxNgay + 1;

                for (int ngay = 0; ngay < soNgay; ngay++)
                {
                    // Tên sheet: dd-MM-yyyy
                    string sheetName = startDate.AddDays(ngay).ToString("dd-MM-yyyy");
                    var ws = wb.Worksheets.Add(sheetName);

                    Dictionary<int, List<MonThi>> byCa;
                    if (!groups.TryGetValue(ngay, out byCa))
                        byCa = new Dictionary<int, List<MonThi>>();

                    BuildDayTable(ws, byCa, soCaMoiNgay, khungGioTheoCa);
                }

                wb.SaveAs(filePath);
            }
        }

        private static void BuildDayTable(
            IXLWorksheet ws,
            Dictionary<int, List<MonThi>> byCa,
            int soCaMoiNgay,
            string[] khungGioTheoCa)
        {
            // ===== Header =====
            ws.Cell(1, 1).Value = "Ca thi";
            ws.Cell(1, 2).Value = "Khung giờ";
            ws.Cell(1, 3).Value = "Tên môn";
            ws.Cell(1, 4).Value = "Ghi chú";

            var header = ws.Range(1, 1, 1, 4);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            // ===== Build từng ca =====
            for (int ca = 0; ca < soCaMoiNgay; ca++)
            {
                List<MonThi> list;
                if (!byCa.TryGetValue(ca, out list))
                    list = new List<MonThi>();

                int start = row;

                if (list.Count == 0)
                {
                    ws.Cell(row, 3).Value = "";
                    ws.Cell(row, 4).Value = "";
                    row++;
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ws.Cell(row, 3).Value = list[i].TenMon;
                        ws.Cell(row, 4).Value = "";
                        row++;
                    }
                }

                int end = row - 1;
                if (end < start) end = start;

                // Merge Ca thi
                var rCa = ws.Range(start, 1, end, 1);
                rCa.Merge();
                ws.Cell(start, 1).Value = ca + 1;

                // Merge Khung giờ
                var rKg = ws.Range(start, 2, end, 2);
                rKg.Merge();

                string kg = "";
                if (khungGioTheoCa != null && ca < khungGioTheoCa.Length)
                    kg = khungGioTheoCa[ca];

                ws.Cell(start, 2).Value = kg;

                // Tô vàng ca 1 và ca 3 (ca = 0,2)
                if (ca == 0 || ca == 2)
                {
                    ws.Range(start, 1, end, 4)
                      .Style.Fill.BackgroundColor = XLColor.Yellow;
                }
            }

            int lastRow = row - 1;

            // ===== Border + Align =====
            var tbl = ws.Range(1, 1, lastRow, 4);

            // Viền
            tbl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tbl.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Tất cả center + middle trước
            tbl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            tbl.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Cột Tên môn + Ghi chú: left nhưng vẫn middle
            ws.Range(2, 3, lastRow, 3).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Left;

            ws.Range(2, 4, lastRow, 4).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Left;

            // Tự căn độ rộng
            ws.Columns(1, 4).AdjustToContents();
            ws.SheetView.FreezeRows(1);
        }
    }
}
