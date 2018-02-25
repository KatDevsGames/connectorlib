namespace ConnectorLibDemo
{
    partial class FormMain
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
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupConnector = new System.Windows.Forms.GroupBox();
            this.tableGame = new System.Windows.Forms.TableLayoutPanel();
            this.buttonConnectorTest = new System.Windows.Forms.Button();
            this.comboConnector = new System.Windows.Forms.ComboBox();
            this.buttonConnectorStartStop = new System.Windows.Forms.Button();
            this.tabBottom = new System.Windows.Forms.TabControl();
            this.tabPageStatus = new System.Windows.Forms.TabPage();
            this.listStatus = new System.Windows.Forms.ListBox();
            this.tabPageConnection = new System.Windows.Forms.TabPage();
            this.tableConnection = new System.Windows.Forms.TableLayoutPanel();
            this.groupGameOptionssd2snesCOMPort = new System.Windows.Forms.GroupBox();
            this.combosd2snesCOMPort = new System.Windows.Forms.ComboBox();
            this.groupGameOptionsLuaConnectorSocketType = new System.Windows.Forms.GroupBox();
            this.comboLuaConnectorSocketType = new System.Windows.Forms.ComboBox();
            this.tabPageDebug = new System.Windows.Forms.TabPage();
            this.tableMain.SuspendLayout();
            this.groupConnector.SuspendLayout();
            this.tableGame.SuspendLayout();
            this.tabBottom.SuspendLayout();
            this.tabPageStatus.SuspendLayout();
            this.tabPageConnection.SuspendLayout();
            this.tableConnection.SuspendLayout();
            this.groupGameOptionssd2snesCOMPort.SuspendLayout();
            this.groupGameOptionsLuaConnectorSocketType.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMain
            // 
            this.tableMain.AutoSize = true;
            this.tableMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMain.ColumnCount = 1;
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Controls.Add(this.groupConnector, 0, 0);
            this.tableMain.Controls.Add(this.tabBottom, 0, 1);
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.Location = new System.Drawing.Point(0, 0);
            this.tableMain.Name = "tableMain";
            this.tableMain.RowCount = 2;
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableMain.Size = new System.Drawing.Size(328, 506);
            this.tableMain.TabIndex = 0;
            // 
            // groupConnector
            // 
            this.groupConnector.AutoSize = true;
            this.groupConnector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupConnector.Controls.Add(this.tableGame);
            this.groupConnector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupConnector.Location = new System.Drawing.Point(3, 3);
            this.groupConnector.Name = "groupConnector";
            this.groupConnector.Size = new System.Drawing.Size(322, 48);
            this.groupConnector.TabIndex = 1;
            this.groupConnector.TabStop = false;
            this.groupConnector.Text = "Connector";
            // 
            // tableGame
            // 
            this.tableGame.AutoSize = true;
            this.tableGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableGame.ColumnCount = 3;
            this.tableGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableGame.Controls.Add(this.buttonConnectorTest, 2, 0);
            this.tableGame.Controls.Add(this.comboConnector, 0, 0);
            this.tableGame.Controls.Add(this.buttonConnectorStartStop, 1, 0);
            this.tableGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableGame.Location = new System.Drawing.Point(3, 16);
            this.tableGame.Name = "tableGame";
            this.tableGame.RowCount = 1;
            this.tableGame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableGame.Size = new System.Drawing.Size(316, 29);
            this.tableGame.TabIndex = 0;
            // 
            // buttonConnectorTest
            // 
            this.buttonConnectorTest.AutoSize = true;
            this.buttonConnectorTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonConnectorTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonConnectorTest.Location = new System.Drawing.Point(242, 3);
            this.buttonConnectorTest.MinimumSize = new System.Drawing.Size(71, 0);
            this.buttonConnectorTest.Name = "buttonConnectorTest";
            this.buttonConnectorTest.Size = new System.Drawing.Size(71, 23);
            this.buttonConnectorTest.TabIndex = 0;
            this.buttonConnectorTest.Text = "Test";
            this.buttonConnectorTest.UseVisualStyleBackColor = true;
            this.buttonConnectorTest.Click += new System.EventHandler(this.buttonConnectorTest_Click);
            // 
            // comboConnector
            // 
            this.comboConnector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboConnector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConnector.FormattingEnabled = true;
            this.comboConnector.Items.AddRange(new object[] {
            "Lua Connector",
            "SD2SNES (Web Socket)"});
            this.comboConnector.Location = new System.Drawing.Point(3, 3);
            this.comboConnector.Name = "comboConnector";
            this.comboConnector.Size = new System.Drawing.Size(156, 21);
            this.comboConnector.TabIndex = 0;
            this.comboConnector.SelectedIndexChanged += new System.EventHandler(this.comboConnector_SelectedIndexChanged);
            // 
            // buttonConnectorStartStop
            // 
            this.buttonConnectorStartStop.AutoSize = true;
            this.buttonConnectorStartStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonConnectorStartStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonConnectorStartStop.Location = new System.Drawing.Point(165, 3);
            this.buttonConnectorStartStop.MinimumSize = new System.Drawing.Size(71, 0);
            this.buttonConnectorStartStop.Name = "buttonConnectorStartStop";
            this.buttonConnectorStartStop.Size = new System.Drawing.Size(71, 23);
            this.buttonConnectorStartStop.TabIndex = 1;
            this.buttonConnectorStartStop.Text = "Start";
            this.buttonConnectorStartStop.UseVisualStyleBackColor = true;
            this.buttonConnectorStartStop.Click += new System.EventHandler(this.buttonConnectorStartStop_Click);
            // 
            // tabBottom
            // 
            this.tabBottom.Controls.Add(this.tabPageStatus);
            this.tabBottom.Controls.Add(this.tabPageConnection);
            this.tabBottom.Controls.Add(this.tabPageDebug);
            this.tabBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabBottom.Location = new System.Drawing.Point(3, 57);
            this.tabBottom.Name = "tabBottom";
            this.tabBottom.SelectedIndex = 0;
            this.tabBottom.Size = new System.Drawing.Size(322, 446);
            this.tabBottom.TabIndex = 3;
            // 
            // tabPageStatus
            // 
            this.tabPageStatus.Controls.Add(this.listStatus);
            this.tabPageStatus.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatus.Name = "tabPageStatus";
            this.tabPageStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStatus.Size = new System.Drawing.Size(314, 420);
            this.tabPageStatus.TabIndex = 0;
            this.tabPageStatus.Text = "Status";
            this.tabPageStatus.UseVisualStyleBackColor = true;
            // 
            // listStatus
            // 
            this.listStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listStatus.FormattingEnabled = true;
            this.listStatus.Location = new System.Drawing.Point(3, 3);
            this.listStatus.Name = "listStatus";
            this.listStatus.Size = new System.Drawing.Size(308, 414);
            this.listStatus.TabIndex = 0;
            // 
            // tabPageConnection
            // 
            this.tabPageConnection.Controls.Add(this.tableConnection);
            this.tabPageConnection.Location = new System.Drawing.Point(4, 22);
            this.tabPageConnection.Name = "tabPageConnection";
            this.tabPageConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConnection.Size = new System.Drawing.Size(314, 420);
            this.tabPageConnection.TabIndex = 1;
            this.tabPageConnection.Text = "Connection";
            this.tabPageConnection.UseVisualStyleBackColor = true;
            // 
            // tableConnection
            // 
            this.tableConnection.AutoSize = true;
            this.tableConnection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableConnection.ColumnCount = 1;
            this.tableConnection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableConnection.Controls.Add(this.groupGameOptionssd2snesCOMPort, 0, 1);
            this.tableConnection.Controls.Add(this.groupGameOptionsLuaConnectorSocketType, 0, 0);
            this.tableConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableConnection.Location = new System.Drawing.Point(3, 3);
            this.tableConnection.Name = "tableConnection";
            this.tableConnection.RowCount = 3;
            this.tableConnection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableConnection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableConnection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableConnection.Size = new System.Drawing.Size(308, 414);
            this.tableConnection.TabIndex = 0;
            // 
            // groupGameOptionssd2snesCOMPort
            // 
            this.groupGameOptionssd2snesCOMPort.AutoSize = true;
            this.groupGameOptionssd2snesCOMPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupGameOptionssd2snesCOMPort.Controls.Add(this.combosd2snesCOMPort);
            this.groupGameOptionssd2snesCOMPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupGameOptionssd2snesCOMPort.Location = new System.Drawing.Point(3, 49);
            this.groupGameOptionssd2snesCOMPort.Name = "groupGameOptionssd2snesCOMPort";
            this.groupGameOptionssd2snesCOMPort.Size = new System.Drawing.Size(302, 40);
            this.groupGameOptionssd2snesCOMPort.TabIndex = 0;
            this.groupGameOptionssd2snesCOMPort.TabStop = false;
            this.groupGameOptionssd2snesCOMPort.Text = "COM Port";
            // 
            // combosd2snesCOMPort
            // 
            this.combosd2snesCOMPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.combosd2snesCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combosd2snesCOMPort.Enabled = false;
            this.combosd2snesCOMPort.FormattingEnabled = true;
            this.combosd2snesCOMPort.Location = new System.Drawing.Point(3, 16);
            this.combosd2snesCOMPort.Name = "combosd2snesCOMPort";
            this.combosd2snesCOMPort.Size = new System.Drawing.Size(296, 21);
            this.combosd2snesCOMPort.TabIndex = 0;
            this.combosd2snesCOMPort.SelectedIndexChanged += new System.EventHandler(this.combosd2snesCOMPort_SelectedIndexChanged);
            // 
            // groupGameOptionsLuaConnectorSocketType
            // 
            this.groupGameOptionsLuaConnectorSocketType.AutoSize = true;
            this.groupGameOptionsLuaConnectorSocketType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupGameOptionsLuaConnectorSocketType.Controls.Add(this.comboLuaConnectorSocketType);
            this.groupGameOptionsLuaConnectorSocketType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupGameOptionsLuaConnectorSocketType.Location = new System.Drawing.Point(3, 3);
            this.groupGameOptionsLuaConnectorSocketType.Name = "groupGameOptionsLuaConnectorSocketType";
            this.groupGameOptionsLuaConnectorSocketType.Size = new System.Drawing.Size(302, 40);
            this.groupGameOptionsLuaConnectorSocketType.TabIndex = 1;
            this.groupGameOptionsLuaConnectorSocketType.TabStop = false;
            this.groupGameOptionsLuaConnectorSocketType.Text = "Socket Type";
            // 
            // comboLuaConnectorSocketType
            // 
            this.comboLuaConnectorSocketType.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboLuaConnectorSocketType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLuaConnectorSocketType.FormattingEnabled = true;
            this.comboLuaConnectorSocketType.Items.AddRange(new object[] {
            "Local",
            "Internet"});
            this.comboLuaConnectorSocketType.Location = new System.Drawing.Point(3, 16);
            this.comboLuaConnectorSocketType.Name = "comboLuaConnectorSocketType";
            this.comboLuaConnectorSocketType.Size = new System.Drawing.Size(296, 21);
            this.comboLuaConnectorSocketType.TabIndex = 0;
            this.comboLuaConnectorSocketType.SelectedIndexChanged += new System.EventHandler(this.comboLuaConnectorSocketType_SelectedIndexChanged);
            // 
            // tabPageDebug
            // 
            this.tabPageDebug.Location = new System.Drawing.Point(4, 22);
            this.tabPageDebug.Name = "tabPageDebug";
            this.tabPageDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDebug.Size = new System.Drawing.Size(314, 420);
            this.tabPageDebug.TabIndex = 2;
            this.tabPageDebug.Text = "Debug";
            this.tabPageDebug.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 506);
            this.Controls.Add(this.tableMain);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "ConnectorLib Demo";
            this.tableMain.ResumeLayout(false);
            this.tableMain.PerformLayout();
            this.groupConnector.ResumeLayout(false);
            this.groupConnector.PerformLayout();
            this.tableGame.ResumeLayout(false);
            this.tableGame.PerformLayout();
            this.tabBottom.ResumeLayout(false);
            this.tabPageStatus.ResumeLayout(false);
            this.tabPageConnection.ResumeLayout(false);
            this.tabPageConnection.PerformLayout();
            this.tableConnection.ResumeLayout(false);
            this.tableConnection.PerformLayout();
            this.groupGameOptionssd2snesCOMPort.ResumeLayout(false);
            this.groupGameOptionsLuaConnectorSocketType.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableMain;
        private System.Windows.Forms.GroupBox groupConnector;
        private System.Windows.Forms.ListBox listStatus;
        private System.Windows.Forms.TableLayoutPanel tableGame;
        private System.Windows.Forms.ComboBox comboConnector;
        private System.Windows.Forms.Button buttonConnectorStartStop;
        private System.Windows.Forms.GroupBox groupGameOptionsLuaConnectorSocketType;
        private System.Windows.Forms.ComboBox comboLuaConnectorSocketType;
        private System.Windows.Forms.GroupBox groupGameOptionssd2snesCOMPort;
        private System.Windows.Forms.ComboBox combosd2snesCOMPort;
        private System.Windows.Forms.Button buttonConnectorTest;
        private System.Windows.Forms.TabControl tabBottom;
        private System.Windows.Forms.TabPage tabPageStatus;
        private System.Windows.Forms.TabPage tabPageConnection;
        private System.Windows.Forms.TableLayoutPanel tableConnection;
        private System.Windows.Forms.TabPage tabPageDebug;
    }
}