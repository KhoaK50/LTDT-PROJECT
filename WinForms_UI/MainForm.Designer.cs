using System.Windows.Forms;

namespace WinForms_UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // ==== TOP BAR ====
        private Panel pnlTopBar;
        private Button btnToggleSidebar;
        private Label lblTitle;

        // ==== LEFT INPUT PANEL ====
        private Panel pnlInput;
        private GroupBox grpSubjects;
        private Label lblSubjectsHint;
        private TextBox txtSubjects;
        private Button btnImportSubjectsCsv;

        private GroupBox grpConflicts;
        private Label lblConflictsHint;
        private TextBox txtConflicts;
        private Button btnImportConflictsCsv;

        private Label lblStartDate;
        private DateTimePicker dtpStartDate;

        private Button btnDrawGraph;
        private Button btnExportExcel;

        private Button btnPlay;
        private Button btnNextStep;
        private Button btnReset;

        // ==== RIGHT GRAPH PANEL ====
        private Panel pnGraph;   // panel vẽ đồ thị

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.btnToggleSidebar = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.grpSubjects = new System.Windows.Forms.GroupBox();
            this.lblSubjectsHint = new System.Windows.Forms.Label();
            this.btnImportSubjectsCsv = new System.Windows.Forms.Button();
            this.txtSubjects = new System.Windows.Forms.TextBox();
            this.grpConflicts = new System.Windows.Forms.GroupBox();
            this.lblConflictsHint = new System.Windows.Forms.Label();
            this.btnImportConflictsCsv = new System.Windows.Forms.Button();
            this.txtConflicts = new System.Windows.Forms.TextBox();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.btnDrawGraph = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnGraph = new System.Windows.Forms.Panel();
            this.pnlTopBar.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.grpSubjects.SuspendLayout();
            this.grpConflicts.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlTopBar.Controls.Add(this.btnToggleSidebar);
            this.pnlTopBar.Controls.Add(this.lblTitle);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pnlTopBar.Size = new System.Drawing.Size(1200, 50);
            this.pnlTopBar.TabIndex = 2;
            // 
            // btnToggleSidebar
            // 
            this.btnToggleSidebar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleSidebar.FlatAppearance.BorderSize = 0;
            this.btnToggleSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSidebar.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.btnToggleSidebar.Location = new System.Drawing.Point(10, 5);
            this.btnToggleSidebar.Name = "btnToggleSidebar";
            this.btnToggleSidebar.Size = new System.Drawing.Size(45, 40);
            this.btnToggleSidebar.TabIndex = 0;
            this.btnToggleSidebar.Text = "☰";
            this.btnToggleSidebar.UseVisualStyleBackColor = true;
            this.btnToggleSidebar.Click += new System.EventHandler(this.btnToggleSidebar_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(70, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(263, 21);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Xếp lịch thi bằng tô màu DSATUR";
            // 
            // pnlInput
            // 
            this.pnlInput.BackColor = System.Drawing.SystemColors.Control;
            this.pnlInput.Controls.Add(this.grpSubjects);
            this.pnlInput.Controls.Add(this.grpConflicts);
            this.pnlInput.Controls.Add(this.lblStartDate);
            this.pnlInput.Controls.Add(this.dtpStartDate);
            this.pnlInput.Controls.Add(this.btnDrawGraph);
            this.pnlInput.Controls.Add(this.btnExportExcel);
            this.pnlInput.Controls.Add(this.btnPlay);
            this.pnlInput.Controls.Add(this.btnNextStep);
            this.pnlInput.Controls.Add(this.btnReset);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlInput.Location = new System.Drawing.Point(0, 50);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Padding = new System.Windows.Forms.Padding(10);
            this.pnlInput.Size = new System.Drawing.Size(380, 600);
            this.pnlInput.TabIndex = 1;
            // 
            // grpSubjects
            // 
            this.grpSubjects.Controls.Add(this.lblSubjectsHint);
            this.grpSubjects.Controls.Add(this.btnImportSubjectsCsv);
            this.grpSubjects.Controls.Add(this.txtSubjects);
            this.grpSubjects.Location = new System.Drawing.Point(10, 10);
            this.grpSubjects.Name = "grpSubjects";
            this.grpSubjects.Size = new System.Drawing.Size(350, 200);
            this.grpSubjects.TabIndex = 0;
            this.grpSubjects.TabStop = false;
            this.grpSubjects.Text = "Danh sách môn (mỗi dòng là 1 môn)";
            // 
            // lblSubjectsHint
            // 
            this.lblSubjectsHint.AutoSize = true;
            this.lblSubjectsHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubjectsHint.Location = new System.Drawing.Point(10, 18);
            this.lblSubjectsHint.Name = "lblSubjectsHint";
            this.lblSubjectsHint.Size = new System.Drawing.Size(173, 30);
            this.lblSubjectsHint.TabIndex = 0;
            this.lblSubjectsHint.Text = "Mỗi dòng: Id,MaMon,TenMon  \r\n(ví dụ: 0,CTDL,Cấu trúc dữ liệu)";
            // 
            // btnImportSubjectsCsv
            // 
            this.btnImportSubjectsCsv.Location = new System.Drawing.Point(240, 18);
            this.btnImportSubjectsCsv.Name = "btnImportSubjectsCsv";
            this.btnImportSubjectsCsv.Size = new System.Drawing.Size(95, 25);
            this.btnImportSubjectsCsv.TabIndex = 1;
            this.btnImportSubjectsCsv.Text = "Nhập từ CSV...";
            this.btnImportSubjectsCsv.UseVisualStyleBackColor = true;
            this.btnImportSubjectsCsv.Click += new System.EventHandler(this.btnImportSubjectsCsv_Click);
            // 
            // txtSubjects
            // 
            this.txtSubjects.AcceptsReturn = true;
            this.txtSubjects.Location = new System.Drawing.Point(10, 54);
            this.txtSubjects.Multiline = true;
            this.txtSubjects.Name = "txtSubjects";
            this.txtSubjects.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSubjects.Size = new System.Drawing.Size(325, 140);
            this.txtSubjects.TabIndex = 2;
            // 
            // grpConflicts
            // 
            this.grpConflicts.Controls.Add(this.lblConflictsHint);
            this.grpConflicts.Controls.Add(this.btnImportConflictsCsv);
            this.grpConflicts.Controls.Add(this.txtConflicts);
            this.grpConflicts.Location = new System.Drawing.Point(10, 220);
            this.grpConflicts.Name = "grpConflicts";
            this.grpConflicts.Size = new System.Drawing.Size(350, 200);
            this.grpConflicts.TabIndex = 1;
            this.grpConflicts.TabStop = false;
            this.grpConflicts.Text = "Danh sách các xung đột (cạnh kề)";
            // 
            // lblConflictsHint
            // 
            this.lblConflictsHint.AutoSize = true;
            this.lblConflictsHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblConflictsHint.Location = new System.Drawing.Point(10, 22);
            this.lblConflictsHint.Name = "lblConflictsHint";
            this.lblConflictsHint.Size = new System.Drawing.Size(192, 30);
            this.lblConflictsHint.TabIndex = 0;
            this.lblConflictsHint.Text = "Mỗi dòng: FromId,ToId  (ví dụ: 0,1)\r\nhoặc 1-2 cho cạnh giữa môn 1 và 2";
            // 
            // btnImportConflictsCsv
            // 
            this.btnImportConflictsCsv.Location = new System.Drawing.Point(240, 18);
            this.btnImportConflictsCsv.Name = "btnImportConflictsCsv";
            this.btnImportConflictsCsv.Size = new System.Drawing.Size(95, 25);
            this.btnImportConflictsCsv.TabIndex = 1;
            this.btnImportConflictsCsv.Text = "Nhập từ CSV...";
            this.btnImportConflictsCsv.UseVisualStyleBackColor = true;
            this.btnImportConflictsCsv.Click += new System.EventHandler(this.btnImportConflictsCsv_Click);
            // 
            // txtConflicts
            // 
            this.txtConflicts.AcceptsReturn = true;
            this.txtConflicts.Location = new System.Drawing.Point(10, 60);
            this.txtConflicts.Multiline = true;
            this.txtConflicts.Name = "txtConflicts";
            this.txtConflicts.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConflicts.Size = new System.Drawing.Size(325, 125);
            this.txtConflicts.TabIndex = 2;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(10, 430);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(142, 15);
            this.lblStartDate.TabIndex = 7;
            this.lblStartDate.Text = "Ngày bắt đầu (dd-MM-yyyy):";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "dd-MM-yyyy";
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(160, 426);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(200, 23);
            this.dtpStartDate.TabIndex = 8;
            // 
            // btnDrawGraph
            // 
            this.btnDrawGraph.Location = new System.Drawing.Point(10, 460);
            this.btnDrawGraph.Name = "btnDrawGraph";
            this.btnDrawGraph.Size = new System.Drawing.Size(350, 35);
            this.btnDrawGraph.TabIndex = 2;
            this.btnDrawGraph.Text = "Vẽ đồ thị + tô màu DSATUR";
            this.btnDrawGraph.UseVisualStyleBackColor = true;
            this.btnDrawGraph.Click += new System.EventHandler(this.btnDrawGraph_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Enabled = false;
            this.btnExportExcel.Location = new System.Drawing.Point(10, 500);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(350, 30);
            this.btnExportExcel.TabIndex = 3;
            this.btnExportExcel.Text = "Xuất file Excel lịch thi";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Enabled = false;
            this.btnPlay.Location = new System.Drawing.Point(10, 545);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(90, 30);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnNextStep
            // 
            this.btnNextStep.Enabled = false;
            this.btnNextStep.Location = new System.Drawing.Point(110, 545);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(120, 30);
            this.btnNextStep.TabIndex = 5;
            this.btnNextStep.Text = "Bước tiếp theo";
            this.btnNextStep.UseVisualStyleBackColor = true;
            this.btnNextStep.Click += new System.EventHandler(this.btnNextStep_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(240, 545);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 30);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset bước";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // pnGraph
            // 
            this.pnGraph.BackColor = System.Drawing.Color.White;
            this.pnGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGraph.Location = new System.Drawing.Point(380, 50);
            this.pnGraph.Name = "pnGraph";
            this.pnGraph.Size = new System.Drawing.Size(820, 600);
            this.pnGraph.TabIndex = 0;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1200, 650);
            this.Controls.Add(this.pnGraph);
            this.Controls.Add(this.pnlInput);
            this.Controls.Add(this.pnlTopBar);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xếp lịch thi - DSATUR";
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.grpSubjects.ResumeLayout(false);
            this.grpSubjects.PerformLayout();
            this.grpConflicts.ResumeLayout(false);
            this.grpConflicts.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
