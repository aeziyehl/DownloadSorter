using Download_Sorter_UI.Models;
using Newtonsoft.Json;

namespace Download_Sorter_UI.Forms
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();
            Hide();
            ShowInTaskbar = false;
            Notification_Informer.BalloonTipTitle = " The App is in System tray";
            Notification_Informer.BalloonTipText = "To disable the app from starting, uncheck Start with Windows checkbox";
            Notification_Informer.Visible = true;
            Notification_Informer.ShowBalloonTip(500);
            Sort_File();
        }
        public List<string> SortLocationList = new List<string>();
        public List<string> SortfileList = new List<string>();
        public InformationStructure GetInformation(string fileName)
        {
            string json;
            using (var reader = File.OpenText(fileName))
            {
                json = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<InformationStructure>(json);
        }
        public void Sort_File()
        {

            var information = GetInformation("Sorts.json");
            string Downloadlocation;
            Console.WriteLine("============================");
            try
            {
                foreach (var informarionset in information.Information)
                {
                    SortLocationList.Add(informarionset.Location);
                    SortfileList.Add(informarionset.Sortfile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.Source);
            }
            using (StreamReader r = new StreamReader(@"Sorts.json"))
            {
                string json = r.ReadToEnd();
                RootStructure structure = JsonConvert.DeserializeObject<RootStructure>(json);
                Downloadlocation = structure.DownloadLocation;

            }
            Console.WriteLine(string.Join(" + ", Downloadlocation));
            Console.WriteLine(string.Join(" + ", SortLocationList));
            Console.WriteLine(string.Join(" + ", SortfileList));
            foreach (var sortfileextention in SortfileList)
            {
                Console.WriteLine("TEST");
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sortNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sort_File();
        }
    }
}
