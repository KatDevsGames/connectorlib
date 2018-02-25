using System.ComponentModel;
using System.Windows.Forms;

namespace ConnectorLibDemo
{
    partial class BugReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.groupDescription = new System.Windows.Forms.GroupBox();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.groupState = new System.Windows.Forms.GroupBox();
            this.textState = new System.Windows.Forms.TextBox();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.tableMain.SuspendLayout();
            this.groupDescription.SuspendLayout();
            this.groupState.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMain
            // 
            this.tableMain.ColumnCount = 2;
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Controls.Add(this.groupDescription, 0, 0);
            this.tableMain.Controls.Add(this.groupState, 1, 0);
            this.tableMain.Controls.Add(this.flowButtons, 0, 1);
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.Location = new System.Drawing.Point(0, 0);
            this.tableMain.Name = "tableMain";
            this.tableMain.RowCount = 2;
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.Size = new System.Drawing.Size(834, 382);
            this.tableMain.TabIndex = 0;
            // 
            // groupDescription
            // 
            this.groupDescription.Controls.Add(this.textDescription);
            this.groupDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDescription.Location = new System.Drawing.Point(3, 3);
            this.groupDescription.Name = "groupDescription";
            this.groupDescription.Size = new System.Drawing.Size(411, 341);
            this.groupDescription.TabIndex = 0;
            this.groupDescription.TabStop = false;
            this.groupDescription.Text = "Your Description";
            this.groupDescription.Visible = false;
            // 
            // textDescription
            // 
            this.textDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textDescription.Location = new System.Drawing.Point(3, 16);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(405, 322);
            this.textDescription.TabIndex = 0;
            this.textDescription.Text = "Please enter a description of the problem. If you can, describe what you were doi" +
    "ng immediately before the problem occurred.";
            // 
            // groupState
            // 
            this.groupState.Controls.Add(this.textState);
            this.groupState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupState.Location = new System.Drawing.Point(420, 3);
            this.groupState.Name = "groupState";
            this.groupState.Size = new System.Drawing.Size(411, 341);
            this.groupState.TabIndex = 1;
            this.groupState.TabStop = false;
            this.groupState.Text = "Information We Are Also Collecting";
            // 
            // textState
            // 
            this.textState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textState.Location = new System.Drawing.Point(3, 16);
            this.textState.Multiline = true;
            this.textState.Name = "textState";
            this.textState.ReadOnly = true;
            this.textState.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textState.Size = new System.Drawing.Size(405, 322);
            this.textState.TabIndex = 1;
            // 
            // flowButtons
            // 
            this.flowButtons.AutoSize = true;
            this.flowButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableMain.SetColumnSpan(this.flowButtons, 2);
            this.flowButtons.Controls.Add(this.buttonCopy);
            this.flowButtons.Controls.Add(this.buttonExit);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowButtons.Location = new System.Drawing.Point(3, 350);
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.Size = new System.Drawing.Size(828, 29);
            this.flowButtons.TabIndex = 2;
            // 
            // buttonCopy
            // 
            this.buttonCopy.AutoSize = true;
            this.buttonCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCopy.Location = new System.Drawing.Point(3, 3);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(100, 23);
            this.buttonCopy.TabIndex = 3;
            this.buttonCopy.Text = "Copy to Clipboard";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.AutoSize = true;
            this.buttonExit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonExit.Location = new System.Drawing.Point(109, 3);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(34, 23);
            this.buttonExit.TabIndex = 1;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // BugReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 382);
            this.Controls.Add(this.tableMain);
            this.MaximumSize = new System.Drawing.Size(850, 420);
            this.MinimumSize = new System.Drawing.Size(850, 420);
            this.Name = "BugReport";
            this.Text = "Bug Report";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BugReport_Load);
            this.Shown += new System.EventHandler(this.BugReport_Shown);
            this.tableMain.ResumeLayout(false);
            this.tableMain.PerformLayout();
            this.groupDescription.ResumeLayout(false);
            this.groupDescription.PerformLayout();
            this.groupState.ResumeLayout(false);
            this.groupState.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableMain;
        private GroupBox groupDescription;
        private TextBox textDescription;
        private GroupBox groupState;
        private TextBox textState;
        private FlowLayoutPanel flowButtons;
        private Button buttonExit;
        private Button buttonCopy;
    }
}