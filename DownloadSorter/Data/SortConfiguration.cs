namespace DownloadSorter.Data
{
	public class SortRules
	{
		public required List<SortRule> SortRule { get; set; }

		public SortRules()
		{
			SortRule = [];
		}

	}
	public class SortRule
	{
		public required string Name { get; set; }
		public required string Location { get; set; }
		public required string FileExtension { get; set; }

		public SortRule()
		{
			Name = string.Empty;
			Location = string.Empty;
			FileExtension = string.Empty;
		}
	}
	public class SortConfiguration
	{
		public required bool AutoSort { get; set; }
		public required bool AutoStartup { get; set; }

		public required string DownloadLocation { get; set; }
		public required List<SortRule> SortRule { get; set; }

		public SortConfiguration()
		{
			SortRule = [];
			DownloadLocation = string.Empty;
		}
	}
}
