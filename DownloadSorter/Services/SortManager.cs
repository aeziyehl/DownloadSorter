using DownloadSorter.Data;
using Newtonsoft.Json;

namespace DownloadSorter.Services
{
	public class SortManager(ConfigService? configService)
	{
		public List<string> SortFileHistoryList = [];

		private readonly ConfigService configService = configService ?? new();

		public void LoadSortFileHistory()
		{
			string historyFile = "./SortHistory.json";
			if (File.Exists(historyFile))
			{
				try
				{
					string existingJson = File.ReadAllText(historyFile);
					SortFileHistoryList = JsonConvert.DeserializeObject<List<string>>(existingJson) ?? [];
					LoggerService.LogInformation("Successfully loaded sort file history.");
				}
				catch (Exception ex)
				{
					LoggerService.LogError($"Failed to load sort file history. Error deserializing JSON.", ex);
					SortFileHistoryList = [];
				}
			}
			else
			{
				LoggerService.LogInformation("No existing sort file history found. Starting with an empty history.");
				SortFileHistoryList = [];
			}
		}

		public void SaveSortFileHistory()
		{
			string historyFile = "./SortHistory.json";
			try
			{
				string json = JsonConvert.SerializeObject(SortFileHistoryList, Formatting.Indented);
				File.WriteAllText(historyFile, json);
			}
			catch (Exception ex)
			{
				LoggerService.LogError($"Failed to save sort file history. Error serializing JSON.", ex);
			}
		}

		public static string GenerateName(int len)
		{
			Random r = new();
			string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
			string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
			string Name = "";
			Name += consonants[r.Next(consonants.Length)].ToUpper();
			Name += vowels[r.Next(vowels.Length)];
			int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
			while (b < len)
			{
				Name += consonants[r.Next(consonants.Length)];
				b++;
				Name += vowels[r.Next(vowels.Length)];
				b++;
			}

			return Name;
		}

		public static bool CheckNameIsValid(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				LoggerService.LogWarning("Invalid name: Name cannot be null or empty.");
				return false;
			}
			return true;
		}

		public static bool CheckLocationIsValid(string location)
		{
			location = location.Trim('"').Replace("\\", "/");
			if (Directory.Exists(location))
			{
				return true;
			}
			LoggerService.LogWarning($"Invalid location: Directory does not exist at path '{location}'.");
			return false;

		}

		public static bool CheckFileExtensionIsValid(string fileExtension)
		{
			if (!Path.HasExtension(fileExtension) || string.IsNullOrEmpty(fileExtension))
			{
				LoggerService.LogWarning($"Invalid file extension: '{fileExtension}' is not a valid file extension.");
				return false;
			}
			return true;
		}

		public bool AddNewSortRule(string name, string location, string fileExtension)
		{
			if (!CheckLocationIsValid(location) || !CheckFileExtensionIsValid(fileExtension) || configService.CurrentConfig == null)
			{
				LoggerService.LogWarning("Failed to add new sort rule due to invalid input or null configuration.");
				return false;
			}

			try
			{
				location = location.Trim('"').Replace("\\", "/");
				SortRule newRule = new()
				{
					Name = name,
					Location = location,
					FileExtension = fileExtension
				};

				configService.CurrentConfig.SortRule.Add(newRule);
				configService.BuildExtensionMap();
				return configService.SaveConfig();
			}
			catch
			{
				LoggerService.LogError("An error occurred while adding a new sort rule. :(");
				return false;
			}
		}

		public bool EditSortRule(string targetName, string newName, string newLocation, string newExtension)
		{
			try
			{
				var ruleToEdit = configService.CurrentConfig!.SortRule.FirstOrDefault(rule => rule.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

				if (ruleToEdit == null)
				{
					LoggerService.LogWarning($"No sort rule found with name: {targetName}");
					return false;
				}

				newLocation = newLocation.Trim('"').Replace("\\", "/");

				ruleToEdit.Name = newName;
				ruleToEdit.Location = newLocation;
				ruleToEdit.FileExtension = newExtension;

				configService.BuildExtensionMap();
				return configService.SaveConfig();
			}
			catch
			{
				LoggerService.LogError($"Failed to edit sort rule with name: {targetName}");
				return false;
			}
		}

		public bool RemoveSortRule(string targetName)
		{
			try
			{
				var ruleToEdit = configService.CurrentConfig!.SortRule.FirstOrDefault(rule => rule.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

				if (ruleToEdit == null)
				{
					LoggerService.LogWarning($"No sort rule found with name: {targetName}");
					return false;
				}

				var getIndex = configService.CurrentConfig!.SortRule.IndexOf(ruleToEdit!);

				configService.CurrentConfig.SortRule.RemoveAt(getIndex);

				configService.BuildExtensionMap();
				return configService.SaveConfig();
			}
			catch
			{
				return false;
			}
		}

		public bool ChangeDownloadDirectory(string newDirectory)
		{
			if (!CheckLocationIsValid(newDirectory))
			{
				LoggerService.LogError($"Failed to change download directory. Invalid directory: {newDirectory}");
				return false;
			}
			try
			{
				configService.CurrentConfig!.DownloadLocation = newDirectory;
				return configService.SaveConfig();
			}
			catch
			{
				LoggerService.LogError($"Failed to change download directory to: {newDirectory}");
				return false;
			}
		}

		public int RunSorter()
		{
			SortFileHistoryList.Clear();

			string sourceFolder = configService.CurrentConfig!.DownloadLocation;
			if (!Directory.Exists(sourceFolder))
			{
				LoggerService.LogError($"Source folder does not exist: {sourceFolder}");
				return -1;
			}

			int filesMovedCount = 0;

			string[] allFiles = Directory.GetFiles(sourceFolder);

			foreach (string currentFile in allFiles)
			{
				string fileExtension = Path.GetExtension(currentFile).ToLower();

				SortRule? matchingRule = null;
				if (configService.extensionMap != null && configService.extensionMap.TryGetValue(fileExtension, out var rule))
				{
					matchingRule = rule;
				}

				if (matchingRule != null)
				{
					if (!Directory.Exists(matchingRule.Location))
					{
						Directory.CreateDirectory(matchingRule.Location);
					}

					string fileName = Path.GetFileName(currentFile);
					string destinationPath = Path.Combine(matchingRule.Location, fileName);

					try
					{
						if (!File.Exists(destinationPath))
						{
							File.Move(currentFile, destinationPath);
							SortFileHistoryList.Add($"{fileName} -> {matchingRule.Location}");
							LoggerService.LogInformation($"Moved {fileName} → {matchingRule.Location}");
							filesMovedCount++;
						}
						else
						{
							LoggerService.LogWarning($"Skipped duplicate: {fileName}");
						}
					}
					catch
					{
						SortFileHistoryList.Add($"Failed to move file: {fileName} to {matchingRule.Location}");
						LoggerService.LogError($"Failed to move file: {fileName} to {matchingRule.Location}");
						continue;
					}
				}
			}

			SaveSortFileHistory();
			return filesMovedCount;
		}

	}
}
