using DownloadSorter.Data;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace DownloadSorter.Services
{
	public class ConfigService
	{
		public bool failedtoLoadConfig = false;

		public string configFile = "./SortConfig.json";

		public SortConfiguration? CurrentConfig = null;

		public Dictionary<string, SortRule>? extensionMap = null;

		public IList<string> errorMessages = [];

		private readonly JSchemaGenerator generator = new();

		public bool CreateConfig(string downloadLocation)
		{
			if (!SortManager.CheckLocationIsValid(downloadLocation)) { return false; }

			List<SortRule> SortRule = [];
			SortConfiguration rootStructure = new()
			{
				AutoStartup = false,
				AutoSort = false,
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
				LoggerService.LogError($"Failed to load config. Config file is missing");
				return false;
			}
			try
			{
				string existingJson = File.ReadAllText(configFile);
				CurrentConfig = JsonConvert.DeserializeObject<SortConfiguration>(existingJson);
			}
			catch (Exception ex)
			{
				LoggerService.LogError($"Failed to load config file. Error deserializing JSON.", ex);
				return false;
			}
			if (!CheckConfig())
			{
				LoggerService.LogError($"Failed to load config file.");
				return false;
			}
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

		public bool CheckConfig()
		{
			JSchema schema = generator.Generate(typeof(SortConfiguration));

			string configtext = File.ReadAllText(configFile);
			if (configtext.Length == 0) return false;
			bool valid;
			try
			{
				JObject config = JObject.Parse(configtext);
				valid = config.IsValid(schema, out errorMessages);

			}
			catch (JsonReaderException ex)
			{
				LoggerService.LogError($"Failed to parse config file. Invalid JSON format.", ex);
				return false;
			}

			if (errorMessages.Count == 0)
			{
				LoggerService.LogInformation($"Config file {configFile} is valid.");
				return valid;
			}
			{
				LoggerService.LogError($"Config file is invalid. Errors:");
				foreach (var error in errorMessages)
				{
					LoggerService.LogError($"{error}");
				}
				return false;
			}
		}

		public bool TryFixConfig()
		{
			LoggerService.LogWarning($"Config file is invalid. Attempting to fix...");
			if (CurrentConfig == null) return false;

			if (CurrentConfig.AutoStartup == default)
			{
				CurrentConfig.AutoStartup = false;
				LoggerService.LogWarning("AutoStartup was missing or invalid. Set to default value: false");
			}
			if (CurrentConfig.AutoSort == default)
			{
				CurrentConfig.AutoSort = false;
				LoggerService.LogWarning("EnableAutoSort was missing or invalid. Set to default value: false");
			}

			if (CurrentConfig.DownloadLocation == "")
			{
				CurrentConfig.DownloadLocation = GetDownloadFolder.GetDownloadsFolderPath();
				LoggerService.LogWarning($"DownloadLocation was missing or invalid. Set to default value: {CurrentConfig.DownloadLocation}");
			}

			if (CurrentConfig.SortRule == null)
			{
				CurrentConfig.SortRule = [];
				LoggerService.LogWarning("SortRules was missing or invalid. Set to default value: empty list");
			}

			SaveConfig();

			if (CheckConfig())
			{
				return true;
			}
			failedtoLoadConfig = true;
			return false;

		}

		public void BuildExtensionMap()
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

