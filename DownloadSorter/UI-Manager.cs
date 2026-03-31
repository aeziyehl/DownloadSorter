using DownloadSorter.Services;
using System.Runtime.InteropServices;

namespace DownloadSorter.Console_UI
{
	public class UIManager
	{
		public static void ConsoleDisplay()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				[DllImport("kernel32.dll", SetLastError = true)]
				[return: MarshalAs(UnmanagedType.Bool)]
				static extern bool AllocConsole();

				AllocConsole();
			}	
			ConsoleNavigation navigation = new();


			Console.Title = "Download Sorter";
			Console.Clear();

			ConsoleNavigation.InitializeConfig();
			navigation.MainNavigation();
			Console.ReadLine();
		}

		public void WinformsDisplay()
		{
		}
	}
}
