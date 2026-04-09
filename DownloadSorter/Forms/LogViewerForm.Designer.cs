namespace DownloadSorter.Forms
{
	partial class LogViewerForm
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
			richTextBox1 = new RichTextBox();
			SuspendLayout();
			// 
			// richTextBox1
			// 
			richTextBox1.BackColor = SystemColors.Window;
			richTextBox1.Dock = DockStyle.Fill;
			richTextBox1.Font = new Font("Cascadia Mono", 10.8F);
			richTextBox1.Location = new Point(0, 0);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(1347, 517);
			richTextBox1.TabIndex = 0;
			richTextBox1.Text = "";
			richTextBox1.TextChanged += richTextBox1_TextChanged;
			// 
			// LogViewerForm
			// 
			AutoScaleDimensions = new SizeF(12F, 30F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1347, 517);
			Controls.Add(richTextBox1);
			MinimumSize = new Size(800, 500);
			Name = "LogViewerForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "LogViewer";
			Load += LogViewerForm_Load;
			ResumeLayout(false);
		}

		#endregion

		private RichTextBox richTextBox1;
	}
}