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
			if (args.Contains("Console", StringComparer.OrdinalIgnoreCase))
			{
				uiManager.ConsoleDisplay();
			}
		}
	}
}
//NavigationManager Navman = new NavigationManager();
//SortManagment sortMan = new SortManagment();

//Console.Title = "DownloadSorter";
//Console.WriteLine("Download Sorter" + "[" + "v" + Assembly.GetExecutingAssembly().GetName().Version + "]");
//Thread.Sleep(1000);
//if(!File.Exists("Sorts.json"))
//{
//    sortMan.DownloadManager(true);
//}
//Navman.MainNavigation(true);
//Console.ReadLine();