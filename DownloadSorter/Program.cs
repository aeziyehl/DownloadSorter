// See https://aka.ms/new-console-template for more information
using DownloadSorter.Console_UI;
using System.Runtime.InteropServices;

namespace DownloadSorter
{
	internal class Program
	{
		static void Main(string[] args)
		{
			UIManager uiManager = new UIManager();
			//if (args.Contains("Console", StringComparer.OrdinalIgnoreCase))
			//{
				uiManager.ConsoleDisplay();
			//}
		}
	}
}