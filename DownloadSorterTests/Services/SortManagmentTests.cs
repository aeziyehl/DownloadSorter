using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DownloadSorter.Services.Tests
{
    [TestClass()]
    public class SortManagmentTests
    {
        [TestMethod()]
        public void Sort_FileTest()
        {
            SortManager sortManagment = new SortManager();
            try
            {
                sortManagment.Sort_File();
            }
            catch (Exception ex)
            {
                ConsoleDisplay.WriteLine(ex.ToString());
            }
            //Console.WriteLine("FAILED");

        }

        [TestMethod()]
        public void ListSortsTest()
        {
            SortManager sortManagment = new SortManager();
            try
            {
                sortManagment.ListSorts(false);
            }
            catch (Exception ex)
            {
                ConsoleDisplay.WriteLine(ex.ToString());
            }


        }
    }
}