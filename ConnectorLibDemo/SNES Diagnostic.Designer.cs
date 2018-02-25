namespace ConnectorLibDemo
{
    partial class SNESDiagnostic
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SNESDiagnostic));
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupMemory = new System.Windows.Forms.GroupBox();
            this.tableMemory = new System.Windows.Forms.TableLayoutPanel();
            this.buttonMemoryWrite = new System.Windows.Forms.Button();
            this.numericAddress = new System.Windows.Forms.NumericUpDown();
            this.numericValue = new System.Windows.Forms.NumericUpDown();
            this.buttonMemoryRead = new System.Windows.Forms.Button();
            this.groupMessage = new System.Windows.Forms.GroupBox();
            this.tableMessage = new System.Windows.Forms.TableLayoutPanel();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.buttonMessageSend = new System.Windows.Forms.Button();
            this.tableMain.SuspendLayout();
            this.groupMemory.SuspendLayout();
            this.tableMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericValue)).BeginInit();
            this.groupMessage.SuspendLayout();
            this.tableMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMain
            // 
            this.tableMain.AutoSize = true;
            this.tableMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMain.ColumnCount = 1;
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Controls.Add(this.groupMemory, 0, 0);
            this.tableMain.Controls.Add(this.groupMessage, 0, 1);
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.Location = new System.Drawing.Point(0, 0);
            this.tableMain.Name = "tableMain";
            this.tableMain.RowCount = 3;
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Size = new System.Drawing.Size(377, 109);
            this.tableMain.TabIndex = 0;
            // 
            // groupMemory
            // 
            this.groupMemory.AutoSize = true;
            this.groupMemory.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupMemory.Controls.Add(this.tableMemory);
            this.groupMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupMemory.Location = new System.Drawing.Point(3, 3);
            this.groupMemory.Name = "groupMemory";
            this.groupMemory.Size = new System.Drawing.Size(371, 48);
            this.groupMemory.TabIndex = 0;
            this.groupMemory.TabStop = false;
            this.groupMemory.Text = "Memory";
            // 
            // tableMemory
            // 
            this.tableMemory.AutoSize = true;
            this.tableMemory.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMemory.ColumnCount = 4;
            this.tableMemory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMemory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableMemory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableMemory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableMemory.Controls.Add(this.buttonMemoryWrite, 3, 0);
            this.tableMemory.Controls.Add(this.numericAddress, 0, 0);
            this.tableMemory.Controls.Add(this.numericValue, 1, 0);
            this.tableMemory.Controls.Add(this.buttonMemoryRead, 2, 0);
            this.tableMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMemory.Location = new System.Drawing.Point(3, 16);
            this.tableMemory.Margin = new System.Windows.Forms.Padding(0);
            this.tableMemory.Name = "tableMemory";
            this.tableMemory.RowCount = 1;
            this.tableMemory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMemory.Size = new System.Drawing.Size(365, 29);
            this.tableMemory.TabIndex = 1;
            // 
            // buttonMemoryWrite
            // 
            this.buttonMemoryWrite.AutoSize = true;
            this.buttonMemoryWrite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMemoryWrite.Location = new System.Drawing.Point(309, 3);
            this.buttonMemoryWrite.Name = "buttonMemoryWrite";
            this.buttonMemoryWrite.Size = new System.Drawing.Size(53, 23);
            this.buttonMemoryWrite.TabIndex = 3;
            this.buttonMemoryWrite.Text = "Write";
            this.buttonMemoryWrite.UseVisualStyleBackColor = true;
            this.buttonMemoryWrite.Click += new System.EventHandler(this.buttonMemoryWrite_Click);
            // 
            // numericAddress
            // 
            this.numericAddress.AutoSize = true;
            this.numericAddress.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericAddress.Hexadecimal = true;
            this.numericAddress.Location = new System.Drawing.Point(3, 3);
            this.numericAddress.Maximum = new decimal(new int[] {
            16777215,
            0,
            0,
            0});
            this.numericAddress.Name = "numericAddress";
            this.numericAddress.Size = new System.Drawing.Size(166, 20);
            this.numericAddress.TabIndex = 0;
            // 
            // numericValue
            // 
            this.numericValue.AutoSize = true;
            this.numericValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericValue.Hexadecimal = true;
            this.numericValue.Location = new System.Drawing.Point(175, 3);
            this.numericValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(69, 20);
            this.numericValue.TabIndex = 1;
            // 
            // buttonMemoryRead
            // 
            this.buttonMemoryRead.AutoSize = true;
            this.buttonMemoryRead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMemoryRead.Location = new System.Drawing.Point(250, 3);
            this.buttonMemoryRead.Name = "buttonMemoryRead";
            this.buttonMemoryRead.Size = new System.Drawing.Size(53, 23);
            this.buttonMemoryRead.TabIndex = 2;
            this.buttonMemoryRead.Text = "Read";
            this.buttonMemoryRead.UseVisualStyleBackColor = true;
            this.buttonMemoryRead.Click += new System.EventHandler(this.buttonMemoryRead_Click);
            // 
            // groupMessage
            // 
            this.groupMessage.AutoSize = true;
            this.groupMessage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupMessage.Controls.Add(this.tableMessage);
            this.groupMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupMessage.Location = new System.Drawing.Point(3, 57);
            this.groupMessage.Name = "groupMessage";
            this.groupMessage.Size = new System.Drawing.Size(371, 48);
            this.groupMessage.TabIndex = 1;
            this.groupMessage.TabStop = false;
            this.groupMessage.Text = "Message";
            // 
            // tableMessage
            // 
            this.tableMessage.AutoSize = true;
            this.tableMessage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMessage.ColumnCount = 2;
            this.tableMessage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMessage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableMessage.Controls.Add(this.textMessage, 0, 0);
            this.tableMessage.Controls.Add(this.buttonMessageSend, 1, 0);
            this.tableMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMessage.Location = new System.Drawing.Point(3, 16);
            this.tableMessage.Name = "tableMessage";
            this.tableMessage.RowCount = 1;
            this.tableMessage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMessage.Size = new System.Drawing.Size(365, 29);
            this.tableMessage.TabIndex = 0;
            // 
            // textMessage
            // 
            this.textMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textMessage.Location = new System.Drawing.Point(3, 3);
            this.textMessage.Name = "textMessage";
            this.textMessage.Size = new System.Drawing.Size(300, 20);
            this.textMessage.TabIndex = 0;
            // 
            // buttonMessageSend
            // 
            this.buttonMessageSend.AutoSize = true;
            this.buttonMessageSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonMessageSend.Location = new System.Drawing.Point(309, 3);
            this.buttonMessageSend.Name = "buttonMessageSend";
            this.buttonMessageSend.Size = new System.Drawing.Size(53, 23);
            this.buttonMessageSend.TabIndex = 1;
            this.buttonMessageSend.Text = "Send";
            this.buttonMessageSend.UseVisualStyleBackColor = true;
            this.buttonMessageSend.Click += new System.EventHandler(this.buttonMessageSend_Click);
            // 
            // SNESDiagnostic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 109);
            this.Controls.Add(this.tableMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SNESDiagnostic";
            this.Text = "SNES Diagnostic";
            this.tableMain.ResumeLayout(false);
            this.tableMain.PerformLayout();
            this.groupMemory.ResumeLayout(false);
            this.groupMemory.PerformLayout();
            this.tableMemory.ResumeLayout(false);
            this.tableMemory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericValue)).EndInit();
            this.groupMessage.ResumeLayout(false);
            this.groupMessage.PerformLayout();
            this.tableMessage.ResumeLayout(false);
            this.tableMessage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableMain;
        private System.Windows.Forms.GroupBox groupMemory;
        private System.Windows.Forms.NumericUpDown numericAddress;
        private System.Windows.Forms.TableLayoutPanel tableMemory;
        private System.Windows.Forms.Button buttonMemoryWrite;
        private System.Windows.Forms.NumericUpDown numericValue;
        private System.Windows.Forms.Button buttonMemoryRead;
        private System.Windows.Forms.GroupBox groupMessage;
        private System.Windows.Forms.TableLayoutPanel tableMessage;
        private System.Windows.Forms.TextBox textMessage;
        private System.Windows.Forms.Button buttonMessageSend;
    }
}