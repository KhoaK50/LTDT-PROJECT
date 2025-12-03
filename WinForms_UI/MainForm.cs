using Core.Algorithms;
using Core.Export;
using Core.Graph;
using Core.Models;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinForms_UI
{
    public partial class MainForm : Form
    {
        // ============ TRẠNG THÁI CHUNG ============

        // Panel input thu / mở
        private int _inputPanelExpandedWidth;
        private bool _isCollapsed = false;

        // Dữ liệu logic
        private List<MonThi> _dsMon = new List<MonThi>();
        // Lưu xung đột theo MaMon (để dùng chung với GraphBuilder + DSatur)
        private List<(string maA, string maB)> _dsXungDot = new List<(string, string)>();

        // Đồ thị + kết quả DSATUR
        private Graph _graph;
        private DSaturResult _dsaturResult;
        private List<ColoringStep> _steps = new List<ColoringStep>();
        private int _currentStepIndex = -1;

        // Layout vẽ
        private PointF[] _vertexPositions;
        private readonly int _vertexRadius = 18;

        // Timer cho nút Play
        private readonly Timer _animationTimer;

        public MainForm()
        {
            InitializeComponent();

            // Ghi nhớ width ban đầu của panel input
            _inputPanelExpandedWidth = pnlInput.Width;

            // Sự kiện form
            pnGraph.Paint += pnGraph_Paint;
            pnGraph.Resize += pnGraph_Resize;

            // Timer animation
            _animationTimer = new Timer();
            _animationTimer.Interval = 700; // ms
            _animationTimer.Tick += AnimationTimer_Tick;

            // Trạng thái ban đầu của nút
            btnExportExcel.Enabled = false;
            btnPlay.Enabled = false;
            btnNextStep.Enabled = false;
            btnReset.Enabled = false;
        }

        // =========================================================
        // 1. THU / MỞ PANEL INPUT (3 GẠCH BÊN TRÊN)
        // =========================================================
        private void btnToggleSidebar_Click(object sender, EventArgs e)
        {
            if (_isCollapsed)
            {
                pnlInput.Width = _inputPanelExpandedWidth;
                _isCollapsed = false;
            }
            else
            {
                pnlInput.Width = 0;
                _isCollapsed = true;
            }

            pnGraph.Invalidate();
        }

        // =========================================================
        // 2. IMPORT CSV CHO DANH SÁCH MÔN
        //    Định dạng:  Id,MaMon,TenMon  (có thể có header)
        // =========================================================
        private void btnImportSubjectsCsv_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                ofd.Title = "Chọn file CSV danh sách môn";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var lines = System.IO.File.ReadAllLines(ofd.FileName)
                                              .Where(l => !string.IsNullOrWhiteSpace(l))
                                              .ToList();
                    if (lines.Count == 0)
                        throw new Exception("File rỗng.");

                    var list = new List<MonThi>();
                    bool hasHeader = lines[0].IndexOf("MaMon", StringComparison.OrdinalIgnoreCase) >= 0;

                    int lineIndex = 0;
                    if (hasHeader) lineIndex = 1;

                    int autoId = 0;
                    for (; lineIndex < lines.Count; lineIndex++)
                    {
                        string line = lines[lineIndex].Trim();
                        if (line.Length == 0) continue;

                        var parts = line.Split(',');
                        if (parts.Length < 2) continue;

                        string ma, ten;

                        // 3 cột: Id,MaMon,TenMon
                        if (parts.Length >= 3)
                        {
                            ma = parts[1].Trim();
                            ten = parts[2].Trim();
                        }
                        else
                        {
                            // 2 cột: MaMon,TenMon
                            ma = parts[0].Trim();
                            ten = parts[1].Trim();
                        }

                        list.Add(new MonThi(autoId, ma, ten));
                        autoId++;
                    }

                    _dsMon = list;
                    RefreshTextFromData();

                    MessageBox.Show(
                        $"Đã đọc {list.Count} môn từ CSV.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc CSV môn:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // 3. IMPORT CSV CHO DANH SÁCH XUNG ĐỘT
        //    Định dạng ưu tiên: FromId,ToId (Id là chỉ số dòng của môn)
        //    Ví dụ:
        //      FromId,ToId
        //      0,1
        //      1,4
        //    Nếu gặp chữ (CTDL,LTDT) thì xem như MaMon luôn.
        // =========================================================
        private void btnImportConflictsCsv_Click(object sender, EventArgs e)
        {
            if (_dsMon.Count == 0)
            {
                MessageBox.Show("Hãy nhập danh sách môn trước.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                ofd.Title = "Chọn file CSV danh sách xung đột";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var lines = System.IO.File.ReadAllLines(ofd.FileName)
                                              .Where(l => !string.IsNullOrWhiteSpace(l))
                                              .ToList();
                    if (lines.Count == 0)
                        throw new Exception("File rỗng.");

                    var list = new List<(string, string)>();
                    bool hasHeader = lines[0].IndexOf("From", StringComparison.OrdinalIgnoreCase) >= 0;

                    int lineIndex = 0;
                    if (hasHeader) lineIndex = 1;

                    for (; lineIndex < lines.Count; lineIndex++)
                    {
                        string line = lines[lineIndex].Trim();
                        if (line.Length == 0) continue;

                        // hỗ trợ dạng "0-1" -> "0,1"
                        line = line.Replace("-", ",");

                        var parts = line.Split(',');
                        if (parts.Length < 2) continue;

                        string tokenA = parts[0].Trim();
                        string tokenB = parts[1].Trim();
                        if (tokenA.Length == 0 || tokenB.Length == 0) continue;

                        string maA = TryMapTokenToMaMon(tokenA);
                        string maB = TryMapTokenToMaMon(tokenB);

                        if (string.IsNullOrEmpty(maA) || string.IsNullOrEmpty(maB))
                            continue;

                        // Chuẩn hóa: MaMonA < MaMonB để tránh trùng
                        if (string.Compare(maA, maB, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            (maA, maB) = (maB, maA);
                        }

                        if (!list.Contains((maA, maB)))
                            list.Add((maA, maB));
                    }

                    _dsXungDot = list;
                    RefreshTextFromData();

                    MessageBox.Show(
                        $"Đã đọc {list.Count} cặp xung đột từ CSV.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc CSV xung đột:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // 4. VẼ ĐỒ THỊ + TÍNH DSATUR (NÚT: Vẽ đồ thị + tô màu DSATUR)
        // =========================================================
        private void btnDrawGraph_Click(object sender, EventArgs e)
        {
            // Parse lại từ textbox (trong trường hợp user chỉnh tay)
            ParseSubjectsFromTextBox();
            ParseConflictsFromTextBox();

            if (_dsMon.Count == 0)
            {
                MessageBox.Show("Chưa có môn nào.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xây dựng đồ thị
            _graph = GraphBuilder.BuildFromConflicts(_dsMon, _dsXungDot);

            // Chạy DSATUR
            _dsaturResult = DSaturAlgorithm.Colorize(_graph);
            _steps = _dsaturResult.Steps ?? new List<ColoringStep>();
            _currentStepIndex = -1;

            // Layout lại vị trí đỉnh
            RecalculateVertexPositions();

            // Bật các nút animation + xuất Excel
            btnExportExcel.Enabled = true;
            btnPlay.Enabled = _steps.Count > 0;
            btnNextStep.Enabled = _steps.Count > 0;
            btnReset.Enabled = _steps.Count > 0;

            pnGraph.Invalidate();

            MessageBox.Show(
                $"Đã dựng đồ thị với {_graph.VertexCount} môn và {_graph.EdgeCount} cạnh xung đột.\n" +
                $"Đã tô màu DSATUR (hiển thị bằng animation các bước).",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =========================================================
        // 5. PLAY / NEXT / RESET ANIMATION
        // =========================================================
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (_steps == null || _steps.Count == 0) return;

            // nếu đang chạy thì dừng
            if (_animationTimer.Enabled)
            {
                _animationTimer.Stop();
                btnPlay.Text = "Play";
            }
            else
            {
                _animationTimer.Start();
                btnPlay.Text = "Pause";
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            StepForward();
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            StepForward();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _animationTimer.Stop();
            btnPlay.Text = "Play";
            _currentStepIndex = -1;
            pnGraph.Invalidate();
        }

        private void StepForward()
        {
            if (_steps == null || _steps.Count == 0) return;

            if (_currentStepIndex < _steps.Count - 1)
            {
                _currentStepIndex++;
                pnGraph.Invalidate();
            }
            else
            {
                _animationTimer.Stop();
                btnPlay.Text = "Play";
            }
        }

        // =========================================================
        // 6. XUẤT EXCEL
        // =========================================================
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_dsMon.Count == 0 || _dsaturResult == null)
            {
                MessageBox.Show("Chưa có dữ liệu để xuất.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel file (*.xlsx)|*.xlsx";
                dlg.Title = "Chọn nơi lưu file lịch thi";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    int soCaMoiNgay = 4;

                    // Khung giờ chuẩn như bạn yêu cầu
                    string[] khungGio = new string[]
                    {
                        "7:00 - 9:00",
                        "9:30 - 11:30",
                        "13:00 - 15:00",
                        "15:30 - 17:30"
                    };

                    // Ngày bắt đầu lấy từ DateTimePicker (dd-MM-yyyy)
                    DateTime startDate = dtpStartDate.Value.Date;

                    ExcelExporter.Export(
                        dlg.FileName,
                        _dsMon,
                        _dsaturResult,
                        soCaMoiNgay,
                        khungGio,
                        startDate);

                    MessageBox.Show("Xuất file Excel lịch thi thành công.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Lỗi xuất Excel:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // 7. VẼ LÊN pnGraph
        // =========================================================
        private void pnGraph_Resize(object sender, EventArgs e)
        {
            RecalculateVertexPositions();
            pnGraph.Invalidate();
        }

        private void RecalculateVertexPositions()
        {
            if (_graph == null || _graph.VertexCount == 0)
            {
                _vertexPositions = null;
                return;
            }

            int n = _graph.VertexCount;
            _vertexPositions = new PointF[n];

            // Vẽ đỉnh trên vòng tròn
            var rect = pnGraph.ClientRectangle;
            float cx = rect.Width / 2f;
            float cy = rect.Height / 2f;
            float radius = Math.Min(rect.Width, rect.Height) * 0.35f;

            for (int i = 0; i < n; i++)
            {
                double angle = 2 * Math.PI * i / n - Math.PI / 2;
                float x = cx + (float)(radius * Math.Cos(angle));
                float y = cy + (float)(radius * Math.Sin(angle));
                _vertexPositions[i] = new PointF(x, y);
            }
        }

        private void pnGraph_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.White);

            if (_graph == null || _vertexPositions == null)
                return;

            int n = _graph.VertexCount;

            // 1. Vẽ cạnh
            using (var penEdge = new Pen(Color.Gray, 2))
            {
                for (int u = 0; u < n; u++)
                {
                    for (int v = u + 1; v < n; v++)
                    {
                        if (_graph.AdjMatrix[u, v])
                        {
                            e.Graphics.DrawLine(
                                penEdge,
                                _vertexPositions[u],
                                _vertexPositions[v]);
                        }
                    }
                }
            }

            // 2. Tính màu hiện tại của từng đỉnh theo step
            int[] colors = new int[n];
            for (int i = 0; i < n; i++) colors[i] = -1;

            if (_steps != null && _currentStepIndex >= 0)
            {
                for (int s = 0; s <= _currentStepIndex && s < _steps.Count; s++)
                {
                    var step = _steps[s];
                    if (step.Vertex >= 0 && step.Vertex < n)
                    {
                        colors[step.Vertex] = step.Color;
                    }
                }
            }

            // 3. Vẽ đỉnh
            for (int i = 0; i < n; i++)
            {
                Color fillColor = Color.White;
                if (colors[i] >= 0)
                {
                    fillColor = ColorUtils.GetColorForIndex(colors[i]);
                }

                DrawVertex(e.Graphics, i, _vertexPositions[i], fillColor);
            }
        }

        private void DrawVertex(Graphics g, int index, PointF pos, Color fillColor)
        {
            float r = _vertexRadius;
            var rect = new RectangleF(pos.X - r, pos.Y - r, 2 * r, 2 * r);

            using (var brush = new SolidBrush(fillColor))
            using (var pen = new Pen(Color.Black, 2))
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);

                string text = index.ToString();
                var size = g.MeasureString(text, font);
                var textPos = new PointF(
                    pos.X - size.Width / 2,
                    pos.Y - size.Height / 2);
                g.DrawString(text, font, textBrush, textPos);
            }
        }

        // =========================================================
        // 8. HÀM PHỤ: PARSE TỪ TEXTBOX & ĐỒNG BỘ HIỂN THỊ
        // =========================================================
        private void ParseSubjectsFromTextBox()
        {
            var list = new List<MonThi>();
            var lines = txtSubjects.Text
                                   .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int autoId = 0;
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                var parts = line.Split(',');
                if (parts.Length < 2) continue;

                string ma, ten;

                // Cho phép: Id,MaMon,TenMon
                if (parts.Length >= 3 && int.TryParse(parts[0].Trim(), out _))
                {
                    ma = parts[1].Trim();
                    ten = parts[2].Trim();
                }
                else
                {
                    // MaMon,TenMon
                    ma = parts[0].Trim();
                    ten = parts[1].Trim();
                }

                list.Add(new MonThi(autoId, ma, ten));
                autoId++;
            }

            if (list.Count > 0)
            {
                _dsMon = list;
            }
        }

        private void ParseConflictsFromTextBox()
        {
            var list = new List<(string, string)>();
            var lines = txtConflicts.Text
                                   .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                line = line.Replace("-", ",");

                var parts = line.Split(',');
                if (parts.Length < 2) continue;

                string tokenA = parts[0].Trim();
                string tokenB = parts[1].Trim();
                if (tokenA.Length == 0 || tokenB.Length == 0) continue;

                string maA = TryMapTokenToMaMon(tokenA);
                string maB = TryMapTokenToMaMon(tokenB);

                if (string.IsNullOrEmpty(maA) || string.IsNullOrEmpty(maB))
                    continue;

                if (string.Compare(maA, maB, StringComparison.OrdinalIgnoreCase) > 0)
                    (maA, maB) = (maB, maA);

                if (!list.Contains((maA, maB)))
                    list.Add((maA, maB));
            }

            _dsXungDot = list;
        }

        /// <summary>
        /// Chuyển một token (có thể là Id hoặc MaMon) thành MaMon.
        /// </summary>
        private string TryMapTokenToMaMon(string token)
        {
            if (int.TryParse(token, out int id))
            {
                // Token là Id -> lấy MaMon theo chỉ số dòng
                if (id >= 0 && id < _dsMon.Count)
                    return _dsMon[id].MaMon;
                return null;
            }

            // Token là MaMon luôn
            return token;
        }

        /// <summary>
        /// Cập nhật lại textbox từ dữ liệu _dsMon + _dsXungDot,
        /// hiển thị môn dạng:  Id,MaMon,TenMon
        /// và xung đột dạng:  FromId,ToId
        /// </summary>
        private void RefreshTextFromData()
        {
            // Mon
            txtSubjects.Clear();
            for (int i = 0; i < _dsMon.Count; i++)
            {
                var m = _dsMon[i];
                txtSubjects.AppendText($"{i},{m.MaMon},{m.TenMon}{Environment.NewLine}");
            }

            // Xung đột (convert MaMon -> Id để hiển thị)
            txtConflicts.Clear();
            foreach (var pair in _dsXungDot)
            {
                string maA = pair.maA;
                string maB = pair.maB;

                int idA = FindMonIndexByMaMon(maA);
                int idB = FindMonIndexByMaMon(maB);

                if (idA >= 0 && idB >= 0)
                {
                    txtConflicts.AppendText($"{idA},{idB}{Environment.NewLine}");
                }
                else
                {
                    // fallback – nếu ko map được thì in nguyên MaMon
                    txtConflicts.AppendText($"{maA},{maB}{Environment.NewLine}");
                }
            }
        }

        private int FindMonIndexByMaMon(string ma)
        {
            for (int i = 0; i < _dsMon.Count; i++)
            {
                if (string.Equals(_dsMon[i].MaMon, ma,
                        StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }
    }
}
