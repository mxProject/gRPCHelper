namespace Examples.GrpcClient
{
    partial class DataSearchForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtDataCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.grpMethods = new System.Windows.Forms.GroupBox();
            this.rdoMethodUnary = new System.Windows.Forms.RadioButton();
            this.rdoMethodDuplexStream = new System.Windows.Forms.RadioButton();
            this.rdoMethodUnaryAsync = new System.Windows.Forms.RadioButton();
            this.rdoMethodServerStream = new System.Windows.Forms.RadioButton();
            this.rdoMethodClientStream = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHeartbeat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHttpGateway = new System.Windows.Forms.ToolStripMenuItem();
            this.rdoMethodServerPush = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.grpMethods.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.txtDataCount);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnExecute);
            this.panel1.Controls.Add(this.grpMethods);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(488, 173);
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(371, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 26);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtDataCount
            // 
            this.txtDataCount.Location = new System.Drawing.Point(371, 29);
            this.txtDataCount.Name = "txtDataCount";
            this.txtDataCount.Size = new System.Drawing.Size(100, 19);
            this.txtDataCount.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "Expecting data count";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(265, 130);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(100, 26);
            this.btnExecute.TabIndex = 11;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // grpMethods
            // 
            this.grpMethods.Controls.Add(this.rdoMethodServerPush);
            this.grpMethods.Controls.Add(this.rdoMethodUnary);
            this.grpMethods.Controls.Add(this.rdoMethodDuplexStream);
            this.grpMethods.Controls.Add(this.rdoMethodUnaryAsync);
            this.grpMethods.Controls.Add(this.rdoMethodServerStream);
            this.grpMethods.Controls.Add(this.rdoMethodClientStream);
            this.grpMethods.Location = new System.Drawing.Point(12, 12);
            this.grpMethods.Name = "grpMethods";
            this.grpMethods.Size = new System.Drawing.Size(234, 155);
            this.grpMethods.TabIndex = 10;
            this.grpMethods.TabStop = false;
            this.grpMethods.Text = "Methods";
            // 
            // rdoMethodUnary
            // 
            this.rdoMethodUnary.AutoSize = true;
            this.rdoMethodUnary.Location = new System.Drawing.Point(6, 18);
            this.rdoMethodUnary.Name = "rdoMethodUnary";
            this.rdoMethodUnary.Size = new System.Drawing.Size(143, 16);
            this.rdoMethodUnary.TabIndex = 5;
            this.rdoMethodUnary.TabStop = true;
            this.rdoMethodUnary.Text = "Search teams ( Unary )";
            this.rdoMethodUnary.UseVisualStyleBackColor = true;
            // 
            // rdoMethodDuplexStream
            // 
            this.rdoMethodDuplexStream.AutoSize = true;
            this.rdoMethodDuplexStream.Location = new System.Drawing.Point(6, 106);
            this.rdoMethodDuplexStream.Name = "rdoMethodDuplexStream";
            this.rdoMethodDuplexStream.Size = new System.Drawing.Size(194, 16);
            this.rdoMethodDuplexStream.TabIndex = 9;
            this.rdoMethodDuplexStream.TabStop = true;
            this.rdoMethodDuplexStream.Text = "Search players ( Duplex Stream )";
            this.rdoMethodDuplexStream.UseVisualStyleBackColor = true;
            // 
            // rdoMethodUnaryAsync
            // 
            this.rdoMethodUnaryAsync.AutoSize = true;
            this.rdoMethodUnaryAsync.Location = new System.Drawing.Point(6, 40);
            this.rdoMethodUnaryAsync.Name = "rdoMethodUnaryAsync";
            this.rdoMethodUnaryAsync.Size = new System.Drawing.Size(179, 16);
            this.rdoMethodUnaryAsync.TabIndex = 6;
            this.rdoMethodUnaryAsync.TabStop = true;
            this.rdoMethodUnaryAsync.Text = "Search teams ( Unary Async )";
            this.rdoMethodUnaryAsync.UseVisualStyleBackColor = true;
            // 
            // rdoMethodServerStream
            // 
            this.rdoMethodServerStream.AutoSize = true;
            this.rdoMethodServerStream.Location = new System.Drawing.Point(6, 84);
            this.rdoMethodServerStream.Name = "rdoMethodServerStream";
            this.rdoMethodServerStream.Size = new System.Drawing.Size(192, 16);
            this.rdoMethodServerStream.TabIndex = 8;
            this.rdoMethodServerStream.TabStop = true;
            this.rdoMethodServerStream.Text = "Search players ( Server Stream )";
            this.rdoMethodServerStream.UseVisualStyleBackColor = true;
            // 
            // rdoMethodClientStream
            // 
            this.rdoMethodClientStream.AutoSize = true;
            this.rdoMethodClientStream.Location = new System.Drawing.Point(6, 62);
            this.rdoMethodClientStream.Name = "rdoMethodClientStream";
            this.rdoMethodClientStream.Size = new System.Drawing.Size(189, 16);
            this.rdoMethodClientStream.TabIndex = 7;
            this.rdoMethodClientStream.TabStop = true;
            this.rdoMethodClientStream.Text = "Search players ( Client Stream )";
            this.rdoMethodClientStream.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 197);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(488, 329);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtLog);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Size = new System.Drawing.Size(488, 329);
            this.splitContainer1.SplitterDistance = 241;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(480, 233);
            this.dataGridView1.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(4, 4);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(480, 76);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(488, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHeartbeat,
            this.mnuHttpGateway});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.optionsToolStripMenuItem.Text = "options";
            // 
            // mnuHeartbeat
            // 
            this.mnuHeartbeat.Name = "mnuHeartbeat";
            this.mnuHeartbeat.Size = new System.Drawing.Size(167, 22);
            this.mnuHeartbeat.Text = "Heartbeat";
            // 
            // mnuHttpGateway
            // 
            this.mnuHttpGateway.Name = "mnuHttpGateway";
            this.mnuHttpGateway.Size = new System.Drawing.Size(167, 22);
            this.mnuHttpGateway.Text = "use Http Gateway";
            // 
            // rdoMethodServerPush
            // 
            this.rdoMethodServerPush.AutoSize = true;
            this.rdoMethodServerPush.Location = new System.Drawing.Point(6, 128);
            this.rdoMethodServerPush.Name = "rdoMethodServerPush";
            this.rdoMethodServerPush.Size = new System.Drawing.Size(179, 16);
            this.rdoMethodServerPush.TabIndex = 10;
            this.rdoMethodServerPush.TabStop = true;
            this.rdoMethodServerPush.Text = "Push players ( Client Stream )";
            this.rdoMethodServerPush.UseVisualStyleBackColor = true;
            // 
            // DataSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 526);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataSearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gRPC Example";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grpMethods.ResumeLayout(false);
            this.grpMethods.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdoMethodDuplexStream;
        private System.Windows.Forms.RadioButton rdoMethodServerStream;
        private System.Windows.Forms.RadioButton rdoMethodClientStream;
        private System.Windows.Forms.RadioButton rdoMethodUnaryAsync;
        private System.Windows.Forms.RadioButton rdoMethodUnary;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox grpMethods;
        private System.Windows.Forms.TextBox txtDataCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuHeartbeat;
        private System.Windows.Forms.ToolStripMenuItem mnuHttpGateway;
        private System.Windows.Forms.RadioButton rdoMethodServerPush;
    }
}

