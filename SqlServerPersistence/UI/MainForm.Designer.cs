namespace SqlServerPersistence.UI;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tlpMain = new TableLayoutPanel();
        connectionControl = new ConnectionControl();
        grpExclusions = new GroupBox();
        lstExclusions = new ListBox();
        btnClearExclusions = new Button();
        btnRemoveExclusion = new Button();
        lblExclusionHint = new Label();
        txtExcludeSchema = new TextBox();
        txtExcludeTable = new TextBox();
        btnAddExclusion = new Button();
        grpExport = new GroupBox();
        btnChooseExportLocation = new Button();
        lblExportPath = new Label();
        lblQueryTimeout = new Label();
        nudQueryTimeout = new NumericUpDown();
        btnExport = new Button();
        btnCancel = new Button();
        prgExport = new ProgressBar();
        grpLog = new GroupBox();
        rtbLog = new RichTextBox();
        tlpMain.SuspendLayout();
        grpExclusions.SuspendLayout();
        grpExport.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudQueryTimeout).BeginInit();
        grpLog.SuspendLayout();
        SuspendLayout();
        //
        // tlpMain
        //
        tlpMain.ColumnCount = 2;
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
        tlpMain.Controls.Add(connectionControl, 0, 0);
        tlpMain.Controls.Add(grpExclusions, 1, 0);
        tlpMain.Controls.Add(grpExport, 0, 1);
        tlpMain.Controls.Add(grpLog, 1, 1);
        tlpMain.Dock = DockStyle.Fill;
        tlpMain.Location = new Point(0, 0);
        tlpMain.Name = "tlpMain";
        tlpMain.Padding = new Padding(8);
        tlpMain.RowCount = 2;
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 260F));
        tlpMain.Size = new Size(1180, 690);
        tlpMain.TabIndex = 0;
        //
        // connectionControl
        //
        connectionControl.Dock = DockStyle.Fill;
        connectionControl.Location = new Point(11, 11);
        connectionControl.Name = "connectionControl";
        connectionControl.Size = new Size(599, 408);
        connectionControl.TabIndex = 0;
        connectionControl.CheckConnectionClick += connectionControl_CheckConnectionClick;
        connectionControl.ConnectionChanged += connectionControl_ConnectionChanged;
        //
        // grpExclusions
        //
        grpExclusions.Controls.Add(lstExclusions);
        grpExclusions.Controls.Add(btnClearExclusions);
        grpExclusions.Controls.Add(btnRemoveExclusion);
        grpExclusions.Controls.Add(lblExclusionHint);
        grpExclusions.Controls.Add(txtExcludeSchema);
        grpExclusions.Controls.Add(txtExcludeTable);
        grpExclusions.Controls.Add(btnAddExclusion);
        grpExclusions.Dock = DockStyle.Fill;
        grpExclusions.Location = new Point(616, 11);
        grpExclusions.Name = "grpExclusions";
        grpExclusions.Size = new Size(553, 408);
        grpExclusions.TabIndex = 1;
        grpExclusions.TabStop = false;
        grpExclusions.Text = "Exclude Tables";
        //
        // lstExclusions
        //
        lstExclusions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstExclusions.HorizontalScrollbar = true;
        lstExclusions.IntegralHeight = false;
        lstExclusions.Location = new Point(15, 30);
        lstExclusions.Name = "lstExclusions";
        lstExclusions.Size = new Size(523, 250);
        lstExclusions.TabIndex = 0;
        //
        // btnClearExclusions
        //
        btnClearExclusions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnClearExclusions.Location = new Point(15, 292);
        btnClearExclusions.Name = "btnClearExclusions";
        btnClearExclusions.Size = new Size(80, 30);
        btnClearExclusions.TabIndex = 1;
        btnClearExclusions.Text = "Clear";
        btnClearExclusions.UseVisualStyleBackColor = true;
        btnClearExclusions.Click += btnClearExclusions_Click;
        //
        // btnRemoveExclusion
        //
        btnRemoveExclusion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnRemoveExclusion.Location = new Point(101, 292);
        btnRemoveExclusion.Name = "btnRemoveExclusion";
        btnRemoveExclusion.Size = new Size(140, 30);
        btnRemoveExclusion.TabIndex = 2;
        btnRemoveExclusion.Text = "Remove selected";
        btnRemoveExclusion.UseVisualStyleBackColor = true;
        btnRemoveExclusion.Click += btnRemoveExclusion_Click;
        //
        // lblExclusionHint
        //
        lblExclusionHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblExclusionHint.AutoSize = true;
        lblExclusionHint.Location = new Point(15, 335);
        lblExclusionHint.Name = "lblExclusionHint";
        lblExclusionHint.Size = new Size(226, 19);
        lblExclusionHint.TabIndex = 3;
        lblExclusionHint.Text = "Schema / Table name (exact names)";
        //
        // txtExcludeSchema
        //
        txtExcludeSchema.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        txtExcludeSchema.Location = new Point(15, 362);
        txtExcludeSchema.Name = "txtExcludeSchema";
        txtExcludeSchema.Size = new Size(120, 25);
        txtExcludeSchema.TabIndex = 4;
        txtExcludeSchema.Text = "dbo";
        //
        // txtExcludeTable
        //
        txtExcludeTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtExcludeTable.Location = new Point(141, 362);
        txtExcludeTable.Name = "txtExcludeTable";
        txtExcludeTable.Size = new Size(311, 25);
        txtExcludeTable.TabIndex = 5;
        //
        // btnAddExclusion
        //
        btnAddExclusion.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnAddExclusion.Location = new Point(458, 360);
        btnAddExclusion.Name = "btnAddExclusion";
        btnAddExclusion.Size = new Size(80, 30);
        btnAddExclusion.TabIndex = 6;
        btnAddExclusion.Text = "Add";
        btnAddExclusion.UseVisualStyleBackColor = true;
        btnAddExclusion.Click += btnAddExclusion_Click;
        //
        // grpExport
        //
        grpExport.Controls.Add(btnChooseExportLocation);
        grpExport.Controls.Add(lblExportPath);
        grpExport.Controls.Add(lblQueryTimeout);
        grpExport.Controls.Add(nudQueryTimeout);
        grpExport.Controls.Add(btnExport);
        grpExport.Controls.Add(btnCancel);
        grpExport.Controls.Add(prgExport);
        grpExport.Dock = DockStyle.Fill;
        grpExport.Location = new Point(11, 425);
        grpExport.Name = "grpExport";
        grpExport.Size = new Size(599, 254);
        grpExport.TabIndex = 2;
        grpExport.TabStop = false;
        grpExport.Text = "Export";
        //
        // btnChooseExportLocation
        //
        btnChooseExportLocation.Location = new Point(15, 30);
        btnChooseExportLocation.Name = "btnChooseExportLocation";
        btnChooseExportLocation.Size = new Size(190, 30);
        btnChooseExportLocation.TabIndex = 0;
        btnChooseExportLocation.Text = "Choose Export Location";
        btnChooseExportLocation.UseVisualStyleBackColor = true;
        btnChooseExportLocation.Click += btnChooseExportLocation_Click;
        //
        // lblExportPath
        //
        lblExportPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblExportPath.AutoEllipsis = true;
        lblExportPath.Location = new Point(15, 68);
        lblExportPath.Name = "lblExportPath";
        lblExportPath.Size = new Size(569, 40);
        lblExportPath.TabIndex = 1;
        lblExportPath.Text = "Export location: NONE";
        //
        // lblQueryTimeout
        //
        lblQueryTimeout.AutoSize = true;
        lblQueryTimeout.Location = new Point(15, 128);
        lblQueryTimeout.Name = "lblQueryTimeout";
        lblQueryTimeout.Size = new Size(163, 19);
        lblQueryTimeout.TabIndex = 2;
        lblQueryTimeout.Text = "Query timeout (seconds):";
        //
        // nudQueryTimeout
        //
        nudQueryTimeout.Location = new Point(185, 126);
        nudQueryTimeout.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
        nudQueryTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudQueryTimeout.Name = "nudQueryTimeout";
        nudQueryTimeout.Size = new Size(85, 25);
        nudQueryTimeout.TabIndex = 3;
        nudQueryTimeout.Value = new decimal(new int[] { 300, 0, 0, 0 });
        //
        // btnExport
        //
        btnExport.Location = new Point(15, 165);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(90, 30);
        btnExport.TabIndex = 4;
        btnExport.Text = "Export";
        btnExport.UseVisualStyleBackColor = true;
        btnExport.Click += btnExport_Click;
        //
        // btnCancel
        //
        btnCancel.Location = new Point(111, 165);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(90, 30);
        btnCancel.TabIndex = 5;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        //
        // prgExport
        //
        prgExport.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        prgExport.Location = new Point(15, 208);
        prgExport.Name = "prgExport";
        prgExport.Size = new Size(569, 23);
        prgExport.TabIndex = 6;
        //
        // grpLog
        //
        grpLog.Controls.Add(rtbLog);
        grpLog.Dock = DockStyle.Fill;
        grpLog.Location = new Point(616, 425);
        grpLog.Name = "grpLog";
        grpLog.Padding = new Padding(8);
        grpLog.Size = new Size(553, 254);
        grpLog.TabIndex = 3;
        grpLog.TabStop = false;
        grpLog.Text = "Logger";
        //
        // rtbLog
        //
        rtbLog.BackColor = SystemColors.Window;
        rtbLog.DetectUrls = false;
        rtbLog.Dock = DockStyle.Fill;
        rtbLog.Font = new Font("Consolas", 9F);
        rtbLog.Location = new Point(8, 26);
        rtbLog.Name = "rtbLog";
        rtbLog.ReadOnly = true;
        rtbLog.Size = new Size(537, 220);
        rtbLog.TabIndex = 0;
        rtbLog.Text = "";
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1180, 690);
        Controls.Add(tlpMain);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1050, 690);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SQL Server Persistence";
        FormClosing += MainForm_FormClosing;
        tlpMain.ResumeLayout(false);
        grpExclusions.ResumeLayout(false);
        grpExclusions.PerformLayout();
        grpExport.ResumeLayout(false);
        grpExport.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudQueryTimeout).EndInit();
        grpLog.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tlpMain;
    private ConnectionControl connectionControl;
    private GroupBox grpExclusions;
    private ListBox lstExclusions;
    private Button btnClearExclusions;
    private Button btnRemoveExclusion;
    private Label lblExclusionHint;
    private TextBox txtExcludeSchema;
    private TextBox txtExcludeTable;
    private Button btnAddExclusion;
    private GroupBox grpExport;
    private Button btnChooseExportLocation;
    private Label lblExportPath;
    private Label lblQueryTimeout;
    private NumericUpDown nudQueryTimeout;
    private Button btnExport;
    private Button btnCancel;
    private ProgressBar prgExport;
    private GroupBox grpLog;
    private RichTextBox rtbLog;
}
