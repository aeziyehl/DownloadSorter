using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DownloadSorter.Data
{
	public class GetDownloadFolder
	{
		public static class KnownFolder
		{
			public static readonly Guid Downloads = new("374DE290-123F-4565-9164-39C4925E467B");
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);



		public static string GetDownloadsFolderPath()
		{
			_ = SHGetKnownFolderPath(KnownFolder.Downloads, 0, IntPtr.Zero, out string downloads);
			return downloads;
		}
	}
}
