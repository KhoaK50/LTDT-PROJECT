using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.Utils
{
    public static class Validator
    {
        public static (bool IsValid, string ErrorMessage) ValidateAll(
            IList<MonThi> dsMon,
            IList<(string maA, string maB)> dsXungDot)
        {
            var monResult = ValidateMonThiList(dsMon);
            if (!monResult.IsValid)
                return monResult;

            var xdResult = ValidateConflicts(dsMon, dsXungDot);
            if (!xdResult.IsValid)
                return xdResult;

            return (true, "");
        }

        public static (bool IsValid, string ErrorMessage) ValidateMonThiList(
            IList<MonThi> dsMon)
        {
            if (dsMon == null || dsMon.Count == 0)
                return (false, "Danh sách môn đang rỗng.");

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int index = 0;

            foreach (var m in dsMon)
            {
                index++;

                if (m == null)
                    return (false, $"Môn thứ {index} bị null.");

                if (string.IsNullOrWhiteSpace(m.MaMon))
                    return (false, $"Môn thứ {index} thiếu mã môn.");

                var ma = m.MaMon.Trim();
                if (!set.Add(ma))
                    return (false, $"Trùng mã môn: '{ma}'.");
            }

            return (true, "");
        }

        public static (bool IsValid, string ErrorMessage) ValidateConflicts(
            IList<MonThi> dsMon,
            IList<(string maA, string maB)> dsXungDot)
        {
            if (dsXungDot == null || dsXungDot.Count == 0)
                return (true, "");

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var m in dsMon)
            {
                if (m != null && !string.IsNullOrWhiteSpace(m.MaMon))
                    set.Add(m.MaMon.Trim());
            }

            int index = 0;

            foreach (var p in dsXungDot)
            {
                index++;

                var a = p.maA == null ? "" : p.maA.Trim();
                var b = p.maB == null ? "" : p.maB.Trim();

                if (a.Length == 0 || b.Length == 0)
                    return (false, $"Cặp xung đột thứ {index} thiếu mã.");

                if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
                    return (false, $"Cặp xung đột thứ {index} có hai mã trùng nhau: '{a}'.");

                if (!set.Contains(a))
                    return (false, $"Mã '{a}' trong cặp {index} không tồn tại.");

                if (!set.Contains(b))
                    return (false, $"Mã '{b}' trong cặp {index} không tồn tại.");
            }

            return (true, "");
        }

        public static bool TryGetMonByMa(
            IList<MonThi> dsMon,
            string maMon,
            out MonThi mon)
        {
            mon = null;

            if (dsMon == null || string.IsNullOrWhiteSpace(maMon))
                return false;

            var target = maMon.Trim();

            foreach (var m in dsMon)
            {
                if (m != null &&
                    !string.IsNullOrWhiteSpace(m.MaMon) &&
                    string.Equals(m.MaMon.Trim(), target, StringComparison.OrdinalIgnoreCase))
                {
                    mon = m;
                    return true;
                }
            }

            return false;
        }
    }
}
