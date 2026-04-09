using DownloadSorter.Console_UI;
using DownloadSorter.Forms;
using DownloadSorter.Services;
using System.Diagnostics;

namespace DownloadSorter
{
	internal class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetHighDpiMode(HighDpiMode.SystemAware);

			ConfigService configService = new();


#if DEBUG
			var form = new LogViewerForm();

			Thread consoleThread = new(() => UIManager.ConsoleDisplay())
			{
				IsBackground = true,
				Name = "ConsoleThread"
			};
			consoleThread.Start();
			Application.Run(form);
#else
			LoggerService logger = new();
			LoggerService.InitLogger();
			configService.LoadConfig();

			Thread consoleThread = new Thread(() => UIManager.ConsoleDisplay());
			consoleThread.IsBackground = true;
			consoleThread.Start();

			// Keep the app alive
			Thread.Sleep(Timeout.Infinite);
#endif
		}
	}
}