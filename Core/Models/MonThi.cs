using System;

namespace Core.Models
{
    /// <summary>
    /// Môn thi trong đồ án xếp lịch thi.
    /// Id     : chỉ số (0..n-1), dùng để map với ma trận kề.
    /// MaMon  : mã môn (ví dụ "CTDL").
    /// TenMon : tên đầy đủ (ví dụ "Cấu trúc dữ liệu").
    /// Mau    : màu tô trong thuật toán DSATUR (ca thi) - có thể null khi chưa tô.
    /// </summary>
    public class MonThi
    {
        /// <summary>
        /// Chỉ số môn (0..n-1). Dùng trong DSATUR và ma trận kề.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mã môn, ví dụ "CTDL".
        /// </summary>
        public string MaMon { get; set; }

        /// <summary>
        /// Tên môn, ví dụ "Cấu trúc dữ liệu và giải thuật".
        /// </summary>
        public string TenMon { get; set; }

        /// <summary>
        /// Màu (mã ca thi) sau khi tô màu đồ thị.
        /// null nếu chưa tô.
        /// </summary>
        public int? Mau { get; set; }

        public MonThi(int id, string maMon, string tenMon, int? mau = null)
        {
            Id = id;
            MaMon = maMon ?? throw new ArgumentNullException(nameof(maMon));
            TenMon = tenMon ?? throw new ArgumentNullException(nameof(tenMon));
            Mau = mau;
        }

        public override string ToString()
        {
            // Ví dụ: "CTDL - Cấu trúc dữ liệu (Id=0, Mau=2)"
            return $"{MaMon} - {TenMon} (Id={Id}, Mau={Mau?.ToString() ?? "null"})";
        }
    }
}
