using System;

namespace Core.Models
{
    /// <summary>
    /// Đại diện cho một ca thi được suy ra từ màu trong DSATUR.
    /// ColorId : số màu DSATUR (0,1,2,...)
    /// Ngay    : ngày thi (tính từ 0, khi xuất Excel +1)
    /// Ca      : ca trong ngày (0..soCaMoiNgay-1)
    /// KhungGio: giờ thi tương ứng (nếu có)
    /// </summary>
    public class CaThi
    {
        /// <summary>
        /// Số màu DSATUR ứng với ca thi.
        /// </summary>
        public int ColorId { get; set; }

        /// <summary>
        /// Ngày thi (0 = ngày 1, 1 = ngày 2...)
        /// </summary>
        public int Ngay { get; set; }

        /// <summary>
        /// Ca thi trong ngày (0 = ca 1, 1 = ca 2...)
        /// </summary>
        public int Ca { get; set; }

        /// <summary>
        /// Chuỗi khung giờ (ví dụ 7h30 - 9h30)
        /// </summary>
        public string KhungGio { get; set; }

        /// <summary>
        /// Khởi tạo ca thi từ mã màu và tổng số ca trong ngày.
        /// </summary>
        public CaThi(int colorId, int soCaMoiNgay, string[] khungGioTheoCa = null)
        {
            if (soCaMoiNgay <= 0)
                throw new ArgumentException("Số ca mỗi ngày phải > 0", nameof(soCaMoiNgay));

            ColorId = colorId;

            // Phân rã ColorId thành Ngày & Ca
            // VD: soCa = 4
            //     ColorId=0 -> Ngày 0, Ca 0
            //     ColorId=3 -> Ngày 0, Ca 3
            //     ColorId=4 -> Ngày 1, Ca 0
            Ngay = colorId / soCaMoiNgay;
            Ca = colorId % soCaMoiNgay;

            // Gán khung giờ nếu có
            if (khungGioTheoCa != null &&
                Ca >= 0 &&
                Ca < khungGioTheoCa.Length)
            {
                KhungGio = khungGioTheoCa[Ca];
            }
            else
            {
                KhungGio = string.Empty;
            }
        }

        /// <summary>
        /// Chuỗi mô tả dễ đọc (dùng cho debug, UI, Excel)
        /// </summary>
        public override string ToString()
        {
            // Ví dụ: "Ngày 1 - Ca 2 (7h30 - 9h30)"
            string baseStr = $"Ngày {Ngay + 1} - Ca {Ca + 1}";
            return string.IsNullOrWhiteSpace(KhungGio)
                ? baseStr
                : $"{baseStr} ({KhungGio})";
        }
    }
}
