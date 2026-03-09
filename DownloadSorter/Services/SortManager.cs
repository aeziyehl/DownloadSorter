using DownloadSorter.Console_UI;
using DownloadSorter.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace DownloadSorter.Services
{
	public class SortManager
	{
		public string fileName = "./Sorts.json";

		public SortConfiguration? CurrentConfig = null;

		public Dictionary<string, SortRule>? extensionMap = null;

		public bool CheckConfig()
		{
			if (File.Exists(fileName))
			{
				return true;
			}
			return false;
		}

		public static string GenerateName(int len)
		{
			Random r = new Random();
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
			if (CheckLocationIsValid(downloadLocation)) { return false; }

			List<SortRule> SortRule = [];
			SortConfiguration rootStructure = new()
			{
				DownloadLocation = downloadLocation,
				SortRule = SortRule
			};
			string json = JsonConvert.SerializeObject(rootStructure, Formatting.Indented);
			File.WriteAllText(fileName, json);

			return true;
		}

		public bool LoadConfig()
		{
			if (!File.Exists(fileName))
			{
				return false;
			}
			string existingJson = File.ReadAllText(fileName);
			CurrentConfig = JsonConvert.DeserializeObject<SortConfiguration>(existingJson);
			BuildExtensionMap();
			return true;
		}

		public bool SaveConfig()
		{
			if (!File.Exists(fileName) || CurrentConfig == null)
			{
				return false;
			}

			string updatedJson = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
			File.WriteAllText(fileName, updatedJson);

			return true;
		}


		public static bool CheckNameIsValid(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
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
			return false;

		}

		public static bool CheckFileExtensionIsValid(string fileExtension)
		{
			if (!Path.HasExtension(fileExtension) || string.IsNullOrEmpty(fileExtension))
			{
				return false;
			}
			return true;
		}

		public bool AddNewSortRule(string name, string location, string fileExtension)
		{
			if (CheckLocationIsValid(location) && CheckFileExtensionIsValid(fileExtension) || CurrentConfig == null)
			{
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
					return false;
				}

				_ = newLocation.Trim('"').Replace("\\", "/");

				ruleToEdit.Name = newName;
				ruleToEdit.Location = newLocation;
				ruleToEdit.FileExtension = newExtension;

				BuildExtensionMap();
				return SaveConfig();
			}
			catch
			{
				return false;
			}
		}

		public bool RemoveSortRule(string targetName)
		{
			try
			{
				var ruleToEdit = CurrentConfig!.SortRule.FirstOrDefault(rule => rule.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

				if (ruleToEdit == null)
				{ return false; }

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
			if (CheckLocationIsValid(newDirectory)) { return false; }
			try
			{
				CurrentConfig!.DownloadLocation = newDirectory;
				return SaveConfig();
			}
			catch { return false; }
		}

		public int RunSorter()
		{
			string sourceFolder = CurrentConfig!.DownloadLocation;
			if (!Directory.Exists(sourceFolder))
			{
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
							filesMovedCount++;
						}
					}
					catch
					{
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
