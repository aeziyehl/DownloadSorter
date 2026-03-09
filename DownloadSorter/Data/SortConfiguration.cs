using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSorter.Data
{
    public class SortRules
	{
        public required List<SortRule> SortRule { get; set; }

        public SortRules()
        {
           SortRule = new List<SortRule>();
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
        public required string DownloadLocation { get; set; }
        public required List<SortRule> SortRule { get; set; }

        public SortConfiguration()
        {
            SortRule = new List<SortRule>();
            DownloadLocation = string.Empty;
        }
    }
}
