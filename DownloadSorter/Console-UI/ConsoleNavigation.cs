using DownloadSorter.Data;
using DownloadSorter.Services;
using static System.Net.Mime.MediaTypeNames;

namespace DownloadSorter.Console_UI
{
	public class ConsoleNavigation
	{
		private static readonly ConsoleMenu consoleMenu = new();
		private readonly ConsoleMenu Menu = consoleMenu;
		public static readonly ConfigService configService = new();
		private static readonly SortManager sortManager = new(configService: configService);

		private readonly List<Option> MainMenuOptions;

		private readonly List<Option> ManageSortRuleMenuOptions;

		private readonly List<Option> ReturnExitOptions;

		public ConsoleNavigation()
		{
			MainMenuOptions =
			[
				new("Create new Sort Rule", () => { CreateSortRuleHandler(); }, "1"),
				new("Manage Sort Rules", () => { ManageSortRules(); }, "2"),
				new("Show all sorts", () => { ListSortRuleHandler(); }, "3"),
				new("Start Sorter", () => { RunSorterHandler(); }, "4"),
				new("Change Download Directory", () => {ChangeDownloadHandler();}, "5" ),
				new("Sort List History", () => {SortFileHistoryHandler();}, "6"),
				new("Exit", () => { Environment.Exit(0); }, "E")
			];

			ManageSortRuleMenuOptions =
			[
				new("Edit Sort Rule", () => {}, "1"),
				new("Remove Sort Rule", () => {}, "2"),
				new("Return", () => {}, "R")
			];

			ReturnExitOptions =
			[
				new("Return to Main Menu", () => {}, "R"),
				new("Exit", () => {Environment.Exit(0);}, "E")
			];
		}

		public void MainNavigation()
		{
			while (true)
			{
				Console.Clear();

				ConsoleHelper.ConsoleWriteLine(ConsoleHelper.TitleArt(), ConsoleColor.Cyan);
				ConsoleHelper.ConsoleWriteLine(ConsoleHelper.Version(), ConsoleColor.DarkGray);
				Console.WriteLine("");

				ConsoleHelper.ConsoleWrite("Current Download Directory: ");
				ConsoleHelper.ConsoleWriteLine(configService.CurrentConfig != null ? configService.CurrentConfig.DownloadLocation : "Not Set", ConsoleColor.Blue);

				int index = ConsoleMenu.OptionsMenu(MainMenuOptions);

				Console.Clear();
				MainMenuOptions[index].Function.Invoke();
				Console.WriteLine("");

				ConsoleMenu.ListOptions(ReturnExitOptions);

				var getReturnExitKeys = ConsoleMenu.GetKeys(ReturnExitOptions);
				var getReturnExitOption = ConsoleMenu.GetResponse(getReturnExitKeys);


				if (getReturnExitOption == "E")
				{
					ReturnExitOptions[1].Function.Invoke();
				}
			}
		}

		public static void CreateSortRuleHandler()
		{
			while (true)
			{
				Console.Clear();
				Console.WriteLine("=== Create a Sort Rule ===");

				ConsoleHelper.ConsoleWriteLine("Enter a name for the sort rule:", ConsoleColor.Yellow);
				string name = ConsoleMenu.GetResponse();
				Console.WriteLine("");

				ConsoleHelper.ConsoleWriteLine("Enter The Destination of the file after being Sorted:", ConsoleColor.Yellow);
				string destinationLocation = ConsoleMenu.GetResponse();
				Console.WriteLine("");


				ConsoleHelper.ConsoleWriteLine("Enter The File extension of Sort (eg. .exe)", ConsoleColor.Yellow);
				string fileExtension = ConsoleMenu.GetResponse();
				Console.WriteLine("");

				if (configService.extensionMap!.ContainsKey(fileExtension))
				{
					ConsoleHelper.ConsoleWriteLine("A sort rule with this file extension already exists.", ConsoleColor.Red);
					ConsoleHelper.ConsoleWriteLine("Please choose a different file extension.", ConsoleColor.Red);
					Thread.Sleep(2000);
					continue;
				}
				if (string.Empty == name)
				{
					ConsoleHelper.ConsoleWriteLine("Name is Empty", ConsoleColor.Red);
					ConsoleHelper.ConsoleWriteLine("AutoGenerating Name");
					name = SortManager.GenerateName(4);
					Console.WriteLine(name);
				}

				if (sortManager.AddNewSortRule(name, destinationLocation, fileExtension))
				{
					ConsoleHelper.ConsoleWriteLine("Sort rule created successfully.", ConsoleColor.Green);
					Thread.Sleep(2000);
					break;
				}
				ConsoleHelper.ConsoleWriteLine("Failed to create sort rule. Please try again.", ConsoleColor.Red);
				Console.WriteLine("Press any key to try again...");
				Console.ReadKey();
				Console.Clear();
			}
		}

		public static void ListSortRuleHandler(bool isInEditMode = false)
		{
			if (!isInEditMode) { Console.WriteLine("=== List of Sort Rules ==="); }

			foreach (var sortRule in configService.CurrentConfig!.SortRule)
			{
				ConsoleHelper.ConsoleWrite("Name:");
				ConsoleHelper.ConsoleWriteLine(sortRule.Name, ConsoleColor.Green);

				ConsoleHelper.ConsoleWrite("Destination:");
				ConsoleHelper.ConsoleWriteLine(sortRule.Location, ConsoleColor.Green);

				ConsoleHelper.ConsoleWrite("File Extension:");
				ConsoleHelper.ConsoleWriteLine(sortRule.FileExtension, ConsoleColor.Green);

				Console.WriteLine("-------------------------");
			}

		}

		public void ManageSortRules()
		{
			while (true)
			{
				Console.Clear();
				Console.WriteLine("=== Manage Sort Rules ===");
				ListSortRuleHandler(true);
				Console.WriteLine("");

				ConsoleHelper.ConsoleWriteLine("Enter the name of the Sort Rule to Manage", ConsoleColor.Yellow);
				ConsoleHelper.ConsoleWriteLine("Press [ENTER] to return to Main Menu", ConsoleColor.Yellow);
				string name = ConsoleMenu.GetResponse(returnByEnterKey: true);

				if (name == "") { break; }

				var sortRule = configService.CurrentConfig!.SortRule.Find(x => x.Name == name);

				if (sortRule != null)
				{
					ManageSortRule(sortRule);
				}
				else
				{
					ConsoleHelper.ConsoleWriteLine("Sort rule not found. Please try again.", ConsoleColor.Red);
				}

				Thread.Sleep(1000);
			}

		}

		public void ManageSortRule(SortRule sortRule)
		{
			while (true)
			{
				Console.WriteLine();
				ConsoleHelper.ConsoleWrite("Managing Sort Rule: ");
				ConsoleHelper.ConsoleWriteLine(sortRule.Name, ConsoleColor.Green);

				int index = ConsoleMenu.OptionsMenu(ManageSortRuleMenuOptions);

				Console.Clear();
				if (index == 0)
				{
					EditSortRuleHandler(sortRule);
					break;
				}
				if (index == 1)
				{
					RemoveSortRuleHandler(sortRule.Name);
					break;
				}
				if (index == 2)
				{
					break;
				}
				ManageSortRuleMenuOptions[index].Function.Invoke();
			}
		}

		public static void EditSortRuleHandler(SortRule sortRule)
		{
			while (true)
			{
				if (sortRule != null)
				{
					ConsoleHelper.ConsoleWriteLine("Enter new name for the sort rule (leave blank to keep current):", ConsoleColor.Yellow);
					string newName = ConsoleMenu.GetResponse();
					if (string.IsNullOrWhiteSpace(newName)) { newName = sortRule.Name; }

					ConsoleHelper.ConsoleWriteLine("Enter new destination for the sort rule (leave blank to keep current):", ConsoleColor.Yellow);
					string newLocation = ConsoleMenu.GetResponse();
					if (string.IsNullOrWhiteSpace(newLocation)) { newLocation = sortRule.Location; }

					ConsoleHelper.ConsoleWriteLine("Enter new file extension for the sort rule (leave blank to keep current):", ConsoleColor.Yellow);
					string newExtension = ConsoleMenu.GetResponse();
					if (string.IsNullOrWhiteSpace(newExtension)) { newExtension = sortRule.FileExtension; }



					if (sortManager.EditSortRule(sortRule.Name, newName, newLocation, newExtension))
					{
						ConsoleHelper.ConsoleWriteLine("Sort rule edited successfully.", ConsoleColor.Green);
						break;
					}
					else
					{
						ConsoleHelper.ConsoleWriteLine("Failed to edit sort rule. Please try again.", ConsoleColor.Red);
					}
				}
				else
				{
					ConsoleHelper.ConsoleWriteLine("Sort rule not found. Please try again.", ConsoleColor.Red);
				}
				Console.Clear();
			}
		}

		public static void RemoveSortRuleHandler(String Name)
		{
			while (true)
			{
				Console.Clear();
				ConsoleHelper.ConsoleWrite("Are you sure you want to delete ", ConsoleColor.Red);
				ConsoleHelper.ConsoleWriteLine(Name);
				ConsoleHelper.ConsoleWrite("[YES, NO] ");
				string Conformation = ConsoleMenu.GetResponse(["YES", "NO"]);

				if (Conformation == "YES")
				{
					ConsoleHelper.ConsoleWriteLine("Deleting Sort Rule");
					if (sortManager.RemoveSortRule(Name))
					{
						ConsoleHelper.ConsoleWriteLine($"Delete {Name}");
						break;
					}
					else
					{
						ConsoleHelper.ConsoleWrite($"Error in Deleting {Name}");
					}

				}
				else
				{
					Console.WriteLine("Returning to Sort Rule List");
					break;
				}
			}
		}

		public static void RunSorterHandler()
		{
			Console.WriteLine("=== Running Sorter ===");
			int filesSorted = sortManager.RunSorter();
			if (filesSorted == -1)
			{
				ConsoleHelper.ConsoleWriteLine("An Error has occured, Please check your configuration.", ConsoleColor.Red);
				return;
			}
			if (filesSorted == 0)
			{
				ConsoleHelper.ConsoleWriteLine("No files to sort.", ConsoleColor.Yellow);
				return;
			}

			foreach (string number in sortManager.SortFileHistoryList)
			{
				Console.WriteLine(number);
			}

			ConsoleHelper.ConsoleWriteLine($"Sorted {filesSorted} files successfully.", ConsoleColor.Green);

		}

		public static void ChangeDownloadHandler(bool isInit = false)
		{
			while (true)
			{
				if (isInit == false && configService.CurrentConfig != null)
				{
					if (configService.CurrentConfig.DownloadLocation != "")
					{
						ConsoleHelper.ConsoleWrite("Current Download location: ");
						ConsoleHelper.ConsoleWriteLine(configService.CurrentConfig!.DownloadLocation, ConsoleColor.DarkGreen);
					}

				}

				ConsoleHelper.ConsoleWriteLine("Type the download loction, press enter to use User Download Folder ", ConsoleColor.Yellow);
				var input = ConsoleMenu.GetResponse(returnByEnterKey: true);

				if (input == "")
				{
					input = GetDownloadFolder.GetDownloadsFolderPath();
				}

				bool function;
				if (isInit)
				{
					function = configService.CreateConfig(input);
				}
				else
				{
					function = sortManager.ChangeDownloadDirectory(input);
				}


				if (function)
				{
					ConsoleHelper.ConsoleWriteLine("Download Location saved to configuration file", ConsoleColor.Green);
					break;
				}
				ConsoleHelper.ConsoleWriteLine("Failed to save download loacation to configuration file. Please try again.", ConsoleColor.Red);
				Thread.Sleep(1000);
				Console.Clear();
			}
		}


		public static void SortFileHistoryHandler()
		{
			sortManager.LoadSortFileHistory();
			ConsoleHelper.ConsoleWriteLine("=== Sort List History ===");
			foreach (string SortFile in sortManager.SortFileHistoryList)
			{
				Console.WriteLine(SortFile);
			}
			Console.WriteLine("");
		}

		public static void InitializeConfig()
		{
			if (configService.LoadConfig())
			{
				ConsoleHelper.ConsoleWriteLine("Configuration loaded successfully.", ConsoleColor.Green);
				return;
			}
			if (configService.errorMessages.Count > 0)
			{

				ConsoleHelper.ConsoleWriteLine("Errors in configuration file:", ConsoleColor.Red);
				foreach (var error in configService.errorMessages)
				{
					ConsoleHelper.ConsoleWriteLine(error, ConsoleColor.Red);
				}
				ConsoleHelper.ConsoleWriteLine("Fixing the errors in the config file", ConsoleColor.Red);
				Thread.Sleep(2000);
				if (configService.TryFixConfig())
				{
					ConsoleHelper.ConsoleWriteLine("Config file fixed successfully.", ConsoleColor.Green);
					Thread.Sleep(2000);
					return;
				}
				else
				{
					ConsoleHelper.ConsoleWriteLine("Failed to fix config file", ConsoleColor.Red);
					Thread.Sleep(2000);
					Console.Clear();
				}
			}

			if (configService.failedtoLoadConfig)
			{
				ConsoleHelper.ConsoleWriteLine("Failed to load configuration. Creating new Configuration", ConsoleColor.Red);
				Thread.Sleep(2000);
				Console.Clear();
			}

			ConsoleHelper.ConsoleWriteLine("Creating new configuration...", ConsoleColor.Yellow);
			Thread.Sleep(2000);

			ConsoleHelper.ConsoleWriteLine("Please enter your download location:");

			ChangeDownloadHandler(true);

			ConsoleHelper.ConsoleWriteLine("Configuration created successfully.", ConsoleColor.Green);
			configService.LoadConfig();
			Thread.Sleep(2000);
			Console.Clear();
		}
	}
}
