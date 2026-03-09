using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DownloadSorter.Data
{
	public class Option
	{
		public string Name { get; }

		public string Key { get; }

		public Action Function { get; }

		public Option(string name, Action function, string key)
		{
			Name = name;
			Function = function;
			Key = key;
		}
	}
}
