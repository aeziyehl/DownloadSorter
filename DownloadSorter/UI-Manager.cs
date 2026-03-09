using DownloadSorter.Services;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DownloadSorter.Console_UI
{
	public class UIManager
	{
		public void ConsoleDisplay()
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			static extern bool AllocConsole();

			AllocConsole();

			ConsoleNavigation navigation = new ConsoleNavigation();

			Console.Title = "Download Sorter";
			Thread.Sleep(1000);

			ConsoleNavigation.InitializeConfig();
			navigation.MainNavigation();
			Console.ReadLine();
		}

		public void WinformsDisplay()
		{
		}
	}
}
