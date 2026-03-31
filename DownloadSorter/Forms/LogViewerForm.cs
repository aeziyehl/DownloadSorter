using DownloadSorter.Services;

namespace DownloadSorter.Forms
{
	public partial class LogViewerForm : Form
	{
		public LogViewerForm()
		{
			InitializeComponent();
		}

		private void LogViewerForm_Load(object sender, EventArgs e)
		{
			LoggerService.InitLogger(this.richTextBox1);
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
