using Microsoft.VisualStudio.TestTools.UnitTesting;
using DownloadSorter.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadSorterTests.Data
{
	[TestClass()]
	public class GetDownloadFolderTest
	{
		[TestMethod()]
		public void GetDownload()
		{
			string downloadsPath =  GetDownloadFolder.GetDownloadsFolderPath();
			Assert.IsFalse(string.IsNullOrEmpty(downloadsPath), "Downloads folder path should not be null or empty.");
			Assert.IsTrue(System.IO.Directory.Exists(downloadsPath), $"Downloads folder path '{downloadsPath}' should exist.");
		}
	}
}
