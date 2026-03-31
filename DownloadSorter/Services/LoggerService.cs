using Serilog;
using Serilog.Formatting.Display;

namespace DownloadSorter.Services
{
	internal class LoggerService : IDisposable
	{

		public static void InitLogger(RichTextBox? richTextBox = null)
		{

			if (richTextBox != null)
			{
				Log.Logger = new LoggerConfiguration()
				.WriteTo.RichTextBox(richTextBox)
				.WriteTo.File(@"logs/LOG-.txt", rollingInterval: Serilog.RollingInterval.Day, rollOnFileSizeLimit: true)
				.CreateLogger();
			}
			else
			{
				Log.Logger = new LoggerConfiguration()
				 .WriteTo.File(@"logs/LOG-.txt", rollingInterval: Serilog.RollingInterval.Day, rollOnFileSizeLimit: true)
				 .CreateLogger();
			}

			Log.Information("Download Sorter v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
			Log.Information("Log Service has Started");
		}

		public static void AddSortHistory(string fileName, string destination)
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string logMessage = $"[{timestamp}] MOVED: {fileName} -> {destination}";
			Log.Information(logMessage);
		}

		public static void LogInformation(string message)
		{
			Log.Information(message);
		}

		public static void LogWarning(string message)
		{
			Log.Warning(message);
		}

		public static void LogError(string message, Exception? exception = null)
		{
			Log.Error(exception, message);
		}

		public void Dispose()
		{
			Log.CloseAndFlush();
		}

	}
}
