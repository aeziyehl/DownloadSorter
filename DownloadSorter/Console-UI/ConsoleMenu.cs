using DownloadSorter.Data;
using DownloadSorter.Services;

using System.Reflection;

namespace DownloadSorter.Console_UI
{
	public class ConsoleMenu
	{

		public SortManager sortMan = new SortManager();

		public static List<Option>? options;


		public static string GetResponse(List<string>? options = null, string exceptionReason = "Invalid option, please try again.", bool returnByEnterKey = false)
		{
			while (true)
			{
				Console.Write("Enter the value: ");
				string? input = Console.ReadLine();

				if (input == null && returnByEnterKey == true)
				{
					return "";
				}


				if (options != null && !options.Contains(input!))
				{
					ConsoleHelper.ConsoleWriteLine(exceptionReason, ConsoleColor.Red);
					Console.WriteLine();
					continue;
				}


				return input!;
			}
		}

		public List<string> GetKeys(List<Option> options)
		{
			List<string> keys = [];
			foreach (Option currentOption in options)
			{
				keys.Add(currentOption.Key);
			}

			return keys;
		}

		public static void ListOptions(List<Option> options, bool ShowSelectOptionText = true)
		{
			if (ShowSelectOptionText)
			{
				Console.WriteLine("=== Select an Option ===");
			}

			Console.WriteLine("");

			foreach (Option currentOption in options)
			{
				ConsoleHelper.ConsoleWrite("[" + currentOption.Key + "] ", ConsoleColor.Green);
				Console.WriteLine(currentOption.Name);
			}
		}

		public int OptionsMenu(List<Option> options)
		{
			ListOptions(options);
			var getkeys = GetKeys(options);

			var getOption = GetResponse(getkeys);

			Console.WriteLine("\n-------------------------\n");

			return getkeys.IndexOf(getOption);
		}

		public void ReturnOptionFunction()
		{
			Console.Clear();
			Console.WriteLine("Returning to the Main Menu ......");
		}
	}
}
