using DownloadSorter.Data;
using Newtonsoft.Json;

namespace DownloadSorter.Services
{
	public class SortManager
	{
		public string configFile = "./SortConfig.json";

		public SortConfiguration? CurrentConfig = null;

		public List<string> SortFileList = [];


		public Dictionary<string, SortRule>? extensionMap = null;

		public bool CheckConfig()
		{
			if (File.Exists(configFile))
			{
				return true;
			}
			return false;
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

		public bool CreateConfig(string downloadLocation)
		{
			if (!CheckLocationIsValid(downloadLocation)) { return false; }

			List<SortRule> SortRule = [];
			SortConfiguration rootStructure = new()
			{
				DownloadLocation = downloadLocation,
				SortRule = SortRule
			};
			string json = JsonConvert.SerializeObject(rootStructure, Formatting.Indented);
			File.WriteAllText(configFile, json);

			return true;
		}

		public bool LoadConfig()
		{
			if (!File.Exists(configFile))
			{
				return false;
			}
			string existingJson = File.ReadAllText(configFile);
			CurrentConfig = JsonConvert.DeserializeObject<SortConfiguration>(existingJson);
			BuildExtensionMap();
			LoggerService.LogInformation($"Config loaded successfully from {configFile}");
			return true;
		}

		public bool SaveConfig()
		{
			if (!File.Exists(configFile) || CurrentConfig == null)
			{
				LoggerService.LogWarning($"Failed to save config. Config file does not exist or CurrentConfig is null.");
				return false;
			}

			string updatedJson = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
			File.WriteAllText(configFile, updatedJson);

			LoggerService.LogInformation($"Config saved successfully to {configFile}");
			return true;
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
			if (!CheckLocationIsValid(location) || !CheckFileExtensionIsValid(fileExtension) || CurrentConfig == null)
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

				CurrentConfig.SortRule.Add(newRule);
				BuildExtensionMap();
				return SaveConfig();
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
				var ruleToEdit = CurrentConfig!.SortRule.FirstOrDefault(rule => rule.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

				if (ruleToEdit == null)
				{
					LoggerService.LogWarning($"No sort rule found with name: {targetName}");
					return false;
				}

				newLocation = newLocation.Trim('"').Replace("\\", "/");

				ruleToEdit.Name = newName;
				ruleToEdit.Location = newLocation;
				ruleToEdit.FileExtension = newExtension;

				BuildExtensionMap();
				return SaveConfig();
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
				var ruleToEdit = CurrentConfig!.SortRule.FirstOrDefault(rule => rule.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

				if (ruleToEdit == null)
				{
					LoggerService.LogWarning($"No sort rule found with name: {targetName}");
					return false;
				}

				var getIndex = CurrentConfig!.SortRule.IndexOf(ruleToEdit!);

				CurrentConfig.SortRule.RemoveAt(getIndex);

				BuildExtensionMap();
				return SaveConfig();
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
				CurrentConfig!.DownloadLocation = newDirectory;
				return SaveConfig();
			}
			catch
			{
				LoggerService.LogError($"Failed to change download directory to: {newDirectory}");
				return false;
			}
		}

		public int RunSorter()
		{
			SortFileList.Clear();
			string sourceFolder = CurrentConfig!.DownloadLocation;
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
				if (extensionMap != null && extensionMap.TryGetValue(fileExtension, out var rule))
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
							SortFileList.Add($"{fileName} -> {matchingRule.Location}");
							LoggerService.LogInformation($"Moved {fileName} → {matchingRule.Location}");
							filesMovedCount++;
						}
					}
					catch
					{
						SortFileList.Add($"Failed to move file: {fileName} to {matchingRule.Location}");
						LoggerService.LogError($"Failed to move file: {fileName} to {matchingRule.Location}");
						continue;
					}
				}
			}

			return filesMovedCount;
		}

		private void BuildExtensionMap()
		{
			extensionMap = new Dictionary<string, SortRule>(StringComparer.OrdinalIgnoreCase);
			if (CurrentConfig?.SortRule != null)
			{
				foreach (var rule in CurrentConfig.SortRule)
				{
					string normalizedExtension = rule.FileExtension.ToLower();
					extensionMap[normalizedExtension] = rule;
				}
			}
		}
	}
}
