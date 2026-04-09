namespace DownloadSorter.Data
{
	public class Option(string name, Action function, string key)
	{
		public string Name { get; } = name;

		public string Key { get; } = key;

		public Action Function { get; } = function;
	}
}
