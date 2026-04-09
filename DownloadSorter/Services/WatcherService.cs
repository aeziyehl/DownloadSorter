using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadSorter.Services
{
	public class WatcherService(SortManager sortManager)
	{
		private readonly FileSystemWatcher? _watcher;
		private readonly ConfigService configService = new();

		public void StartWatcher()
		{
			string path = configService.CurrentConfig!.DownloadLocation;

			using var watcher = new FileSystemWatcher(path);

			watcher.EnableRaisingEvents = true;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Created += OnFileCreated;


			LoggerService.LogInformation($"Watching folder: {path}");
		}

		public void StopWatcher()
		{
			if (_watcher != null)
			{
				_watcher.EnableRaisingEvents = false;
				_watcher.Dispose();
				LoggerService.LogInformation("File watcher stopped.");
			}
		}

		private void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			LoggerService.LogInformation($"Detected new file: {e.Name}");
			int moved = sortManager.RunSorter();
			LoggerService.LogInformation($"Auto-sort complete. Files moved: {moved}");
		}
	}
}
