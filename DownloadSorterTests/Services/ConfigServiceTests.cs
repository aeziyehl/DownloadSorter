using DownloadSorter.Data;
using DownloadSorter.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DownloadSorterTests.Services
{
	[TestClass()]
	public class ConfigServiceTests
	{
		private string testFileName = null!;
		private ConfigService configService = null!;

		// -------------------------------------------------------------------------
		// Helpers
		// -------------------------------------------------------------------------

		/// <summary>
		/// Writes a well-formed config JSON to the test file.
		/// </summary>
		private void WriteValidConfig(
			string? downloadLocation = null,
			bool autoStartup = false,
			bool autoSort = false,
			List<SortRule>? rules = null)
		{
			SortConfiguration config = new()
			{
				AutoStartup = autoStartup,
				AutoSort = autoSort,
				DownloadLocation = downloadLocation ?? Path.GetTempPath(),
				SortRule = rules ?? []
			};
			File.WriteAllText(testFileName, JsonConvert.SerializeObject(config, Formatting.Indented));
		}


		[TestInitialize]
		public void Setup()
		{
			// Each test gets its own isolated file — prevents parallel-run interference
			testFileName = Path.Combine(Path.GetTempPath(), $"TestConfig_{Guid.NewGuid()}.json");

			configService = new ConfigService
			{
				configFile = testFileName,
				CurrentConfig = null,
			};
		}

		[TestCleanup]
		public void Cleanup()
		{
			if (File.Exists(testFileName))
				File.Delete(testFileName);
		}


		[TestMethod]
		public void CreateConfig_ValidLocation_ReturnsTrue()
		{
			// Act
			bool result = configService.CreateConfig(Path.GetTempPath());

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CreateConfig_ValidLocation_CreatesFile()
		{
			// Act
			configService.CreateConfig(Path.GetTempPath());

			// Assert
			Assert.IsTrue(File.Exists(testFileName));
		}

		[TestMethod]
		public void CreateConfig_ValidLocation_WritesValidJson()
		{
			// Act
			configService.CreateConfig(Path.GetTempPath());

			// Assert — file must be parseable and have expected defaults
			string json = File.ReadAllText(testFileName);
			var config = JsonConvert.DeserializeObject<SortConfiguration>(json);

			Assert.IsNotNull(config);
			Assert.IsFalse(config.AutoStartup);
			Assert.IsFalse(config.AutoSort);
			Assert.IsNotNull(config.SortRule);
			Assert.AreEqual(0, config.SortRule.Count);
		}

		[TestMethod]
		public void CreateConfig_ValidLocation_SetsDownloadLocation()
		{
			// Arrange
			string expectedPath = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			// Act
			configService.CreateConfig(Path.GetTempPath());
			string json = File.ReadAllText(testFileName);
			var config = JsonConvert.DeserializeObject<SortConfiguration>(json);

			// Assert
			Assert.IsNotNull(config);
			StringAssert.Contains(config.DownloadLocation, expectedPath);
		}

		[TestMethod]
		public void CreateConfig_InvalidLocation_ReturnsFalse()
		{
			// Act
			bool result = configService.CreateConfig("Z:/this/does/not/exist");

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CreateConfig_InvalidLocation_DoesNotCreateFile()
		{
			// Act
			configService.CreateConfig("Z:/this/does/not/exist");

			// Assert
			Assert.IsFalse(File.Exists(testFileName));
		}

		[TestMethod]
		public void CreateConfig_EmptyLocation_ReturnsFalse()
		{
			// Act
			bool result = configService.CreateConfig(string.Empty);

			// Assert
			Assert.IsFalse(result);
		}

		// =========================================================================
		// LoadConfig
		// =========================================================================

		[TestMethod]
		public void LoadConfig_ValidFile_ReturnsTrue()
		{
			// Arrange
			WriteValidConfig();

			// Act
			bool result = configService.LoadConfig();

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void LoadConfig_ValidFile_PopulatesCurrentConfig()
		{
			// Arrange
			WriteValidConfig();

			// Act
			configService.LoadConfig();

			// Assert
			Assert.IsNotNull(configService.CurrentConfig);
		}

		[TestMethod]
		public void LoadConfig_ValidFile_BuildsExtensionMap()
		{
			// Arrange
			WriteValidConfig(rules:
			[
				new SortRule { Name = "Docs", Location = Path.GetTempPath(), FileExtension = ".txt" }
			]);

			// Act
			configService.LoadConfig();

			// Assert
			Assert.IsNotNull(configService.extensionMap);
			Assert.IsTrue(configService.extensionMap.ContainsKey(".txt"));
		}

		[TestMethod]
		public void LoadConfig_MissingFile_ReturnsFalse()
		{
			// Arrange — no file written, configFile points to nowhere
			configService.configFile = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid()}.json");

			// Act
			bool result = configService.LoadConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void LoadConfig_MissingFile_CurrentConfigRemainsNull()
		{
			// Arrange
			configService.configFile = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid()}.json");

			// Act
			configService.LoadConfig();

			// Assert
			Assert.IsNull(configService.CurrentConfig);
		}

		[TestMethod]
		public void LoadConfig_MalformedJson_ReturnsFalse()
		{
			// Arrange
			File.WriteAllText(testFileName, "{ this is not valid json }}}");

			// Act
			bool result = configService.LoadConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void LoadConfig_EmptyFile_ReturnsFalse()
		{
			// Arrange
			File.WriteAllText(testFileName, string.Empty);

			// Act
			bool result = configService.LoadConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void LoadConfig_ValidConfig_PreservesAutoStartup()
		{
			// Arrange
			WriteValidConfig(autoStartup: true);

			// Act
			configService.LoadConfig();

			// Assert
			Assert.IsTrue(configService.CurrentConfig!.AutoStartup);
		}

		[TestMethod]
		public void LoadConfig_ValidConfig_PreservesAutoSort()
		{
			// Arrange
			WriteValidConfig(autoSort: true);

			// Act
			configService.LoadConfig();

			// Assert
			Assert.IsTrue(configService.CurrentConfig!.AutoSort);
		}

		// =========================================================================
		// SaveConfig
		// =========================================================================

		[TestMethod]
		public void SaveConfig_ValidConfig_ReturnsTrue()
		{
			// Arrange
			WriteValidConfig();
			configService.LoadConfig();

			// Act
			bool result = configService.SaveConfig();

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SaveConfig_ValidConfig_FileExists()
		{
			// Arrange
			WriteValidConfig();
			configService.LoadConfig();

			// Act
			configService.SaveConfig();

			// Assert
			Assert.IsTrue(File.Exists(testFileName));
		}

		[TestMethod]
		public void SaveConfig_ValidConfig_ContentRoundTrips()
		{
			// Arrange
			WriteValidConfig(autoStartup: true, autoSort: true);
			configService.LoadConfig();
			configService.CurrentConfig!.AutoStartup = true;
			configService.CurrentConfig.AutoSort = true;

			// Act
			configService.SaveConfig();
			string savedJson = File.ReadAllText(testFileName);
			var reloaded = JsonConvert.DeserializeObject<SortConfiguration>(savedJson);

			// Assert
			Assert.IsNotNull(reloaded);
			Assert.IsTrue(reloaded.AutoStartup);
			Assert.IsTrue(reloaded.AutoSort);
		}

		[TestMethod]
		public void SaveConfig_NullCurrentConfig_ReturnsFalse()
		{
			// Arrange
			WriteValidConfig();
			configService.CurrentConfig = null;

			// Act
			bool result = configService.SaveConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SaveConfig_MissingFile_ReturnsFalse()
		{
			// Arrange — do NOT write the file; set a config so the null check passes
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule = []
			};

			// Act
			bool result = configService.SaveConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SaveConfig_PersistsSortRules()
		{
			// Arrange
			WriteValidConfig();
			configService.LoadConfig();
			configService.CurrentConfig!.SortRule.Add(
				new SortRule { Name = "Images", Location = Path.GetTempPath(), FileExtension = ".png" });

			// Act
			configService.SaveConfig();
			string json = File.ReadAllText(testFileName);
			var reloaded = JsonConvert.DeserializeObject<SortConfiguration>(json);

			// Assert
			Assert.AreEqual(1, reloaded!.SortRule.Count);
			Assert.AreEqual("Images", reloaded.SortRule[0].Name);
		}

		// =========================================================================
		// CheckConfig
		// =========================================================================

		[TestMethod]
		public void CheckConfig_ValidFile_ReturnsTrue()
		{
			// Arrange
			WriteValidConfig();

			// Act
			bool result = configService.CheckConfig();

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CheckConfig_ValidFile_NoErrorMessages()
		{
			// Arrange
			WriteValidConfig();

			// Act
			configService.CheckConfig();

			// Assert
			Assert.AreEqual(0, configService.errorMessages.Count);
		}

		[TestMethod]
		public void CheckConfig_EmptyFile_ReturnsFalse()
		{
			// Arrange
			File.WriteAllText(testFileName, string.Empty);

			// Act
			bool result = configService.CheckConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CheckConfig_MalformedJson_ReturnsFalse()
		{
			// Arrange
			File.WriteAllText(testFileName, "{ bad json !!!");

			// Act
			bool result = configService.CheckConfig();

			// Assert
			Assert.IsFalse(result);
		}

		// =========================================================================
		// BuildExtensionMap
		// =========================================================================

		[TestMethod]
		public void BuildExtensionMap_SingleRule_MapsExtensionCorrectly()
		{
			// Arrange
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule =
				[
					new SortRule { Name = "Docs", Location = Path.GetTempPath(), FileExtension = ".txt" }
				]
			};

			// Act
			configService.BuildExtensionMap();

			// Assert
			Assert.IsNotNull(configService.extensionMap);
			Assert.IsTrue(configService.extensionMap.ContainsKey(".txt"));
			Assert.AreEqual("Docs", configService.extensionMap[".txt"].Name);
		}

		[TestMethod]
		public void BuildExtensionMap_ExtensionLookupIsCaseInsensitive()
		{
			// Arrange
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule =
				[
					new SortRule { Name = "Docs", Location = Path.GetTempPath(), FileExtension = ".TXT" }
				]
			};

			// Act
			configService.BuildExtensionMap();

			// Assert — lookup by lowercase should still work
			Assert.IsTrue(configService.extensionMap!.ContainsKey(".txt"));
		}

		[TestMethod]
		public void BuildExtensionMap_MultipleRules_AllExtensionsMapped()
		{
			// Arrange
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule =
				[
					new SortRule { Name = "Docs",   Location = Path.GetTempPath(), FileExtension = ".txt" },
					new SortRule { Name = "Images", Location = Path.GetTempPath(), FileExtension = ".png" },
					new SortRule { Name = "Zips",   Location = Path.GetTempPath(), FileExtension = ".zip" }
				]
			};

			// Act
			configService.BuildExtensionMap();

			// Assert
			Assert.AreEqual(3, configService.extensionMap!.Count);
			Assert.IsTrue(configService.extensionMap.ContainsKey(".txt"));
			Assert.IsTrue(configService.extensionMap.ContainsKey(".png"));
			Assert.IsTrue(configService.extensionMap.ContainsKey(".zip"));
		}

		[TestMethod]
		public void BuildExtensionMap_NoRules_ProducesEmptyMap()
		{
			// Arrange
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule = []
			};

			// Act
			configService.BuildExtensionMap();

			// Assert
			Assert.IsNotNull(configService.extensionMap);
			Assert.AreEqual(0, configService.extensionMap.Count);
		}

		[TestMethod]
		public void BuildExtensionMap_NullCurrentConfig_ProducesEmptyMap()
		{
			// Arrange
			configService.CurrentConfig = null;

			// Act
			configService.BuildExtensionMap();

			// Assert — should not throw; map should exist but be empty
			Assert.IsNotNull(configService.extensionMap);
			Assert.AreEqual(0, configService.extensionMap.Count);
		}

		[TestMethod]
		public void BuildExtensionMap_DuplicateExtension_LastRuleWins()
		{
			// Arrange — two rules for same extension; last one should win (dict overwrite)
			configService.CurrentConfig = new SortConfiguration
			{
				AutoStartup = false,
				AutoSort = false,
				DownloadLocation = Path.GetTempPath(),
				SortRule =
				[
					new SortRule { Name = "First",  Location = Path.GetTempPath(), FileExtension = ".txt" },
					new SortRule { Name = "Second", Location = Path.GetTempPath(), FileExtension = ".txt" }
				]
			};

			// Act
			configService.BuildExtensionMap();

			// Assert
			Assert.AreEqual("Second", configService.extensionMap![".txt"].Name);
		}

		// =========================================================================
		// TryFixConfig
		// =========================================================================

		[TestMethod]
		public void TryFixConfig_NullCurrentConfig_ReturnsFalse()
		{
			// Arrange
			WriteValidConfig();
			configService.CurrentConfig = null;

			// Act
			bool result = configService.TryFixConfig();

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryFixConfig_NullSortRules_SetsEmptyList()
		{
			// Arrange
			WriteValidConfig();
			configService.LoadConfig();
			configService.CurrentConfig!.SortRule = null!;

			// Act
			configService.TryFixConfig();

			// Assert
			Assert.IsNotNull(configService.CurrentConfig.SortRule);
		}

		[TestMethod]
		public void TryFixConfig_ValidConfig_ReturnsTrue()
		{
			// Arrange
			WriteValidConfig();
			configService.LoadConfig();

			// Act
			bool result = configService.TryFixConfig();

			// Assert
			Assert.IsTrue(result);
		}
	}
}