using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Accord.Video.FFMPEG;

namespace Metro
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
       
        bool exludedExtension = false;
        string outputFolder { get; set; }
        private int numberOfFiles { get; set; }
        List<string> filePath = new List<string>();
        List<string> fileName = new List<string>();
        List<Int32> VideoDurations = new List<Int32>();
        private int extractedFrames { get; set; }
        private bool enableVideoPlayer { get; set; }
        public static int FPS { get; private set; }
        public Form1()
        {
            InitializeComponent();
            this.StyleManager = metroStyleManager1;
        }
        #region file_processing
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (metroComboBox1.SelectedIndex)
            {
                case 0:metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
                    break;
                case 1:
                    metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
                    break;
                default:
                    break;
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            metroComboBox1.SelectedIndex = 0;
            PreviewB.Enabled = true;
            addExclusionB.Enabled = true;
            extcludeToggle_CheckedChanged(sender, e);
            replaceToggle_CheckedChanged(sender, e);
            enableProcessB();
            replaceT.Text = "";
            replace2T.Text = "";
            metroProgressSpinner1.Visible = false;
            //Thread timeThread = new Thread(() => Clock());
            //timeThread.Start();
        }
        public void enableProcessB()
        {
            if (originalListView.Items.Count == 0)
            {
                Process.Enabled = false;
            }
            else
            {
                Process.Enabled = true;
            }
        }
        private void BrowseFiles_CLick(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = false;
                    fbd.Description = "Browse Dir";
                    fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = fbd.SelectedPath;
                        loadFiles(textBox1.Text, originalListView, BrowseFiles.Name);
                     
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


       public  void loadFiles(string lokasi, ListView Listview, string btnName)
        {
                if (lokasi == "")
                    return;
                try
                {
                if (btnName == "BrowseFiles")
                {
                    Listview.Items.Clear();
                    RenamedListView.Items.Clear();
                    foreach (string filenya in Directory.GetFiles(lokasi))
                    {
                        exludedExtension = false;
                        for (int x = 0; x < exlusioinList.Items.Count; x++)
                        {
                            if (exlusioinList.Items[x].ToString().Contains(Path.GetExtension(filenya)) == true)
                            {
                                exludedExtension = true; break;
                            }
                        }
                        if (exludedExtension == false)
                        {
                            Listview.Items.Add((Listview.Items.Count + 1).ToString());
                            Listview.Items[Listview.Items.Count - 1].SubItems.Add(Path.GetFileNameWithoutExtension(filenya));
                            Listview.Items[Listview.Items.Count - 1].SubItems.Add(Path.GetExtension(filenya));
                            numberOfFiles = Listview.Items.Count;
                        }
                        enableProcessB();
                    }
                }
                else
                {
                    int counter = 0;
                    Listview.Items.Clear();
                    foreach (string filenya in Directory.GetFiles(lokasi))
                    {
                       
                        Listview.Items.Add((Listview.Items.Count + 1).ToString());
                        Listview.Items[Listview.Items.Count - 1].SubItems.Add(Path.GetFileNameWithoutExtension(filenya));
                        Listview.Items[Listview.Items.Count - 1].SubItems.Add(Path.GetExtension(filenya));
                        numberOfFiles = Listview.Items.Count;
                        filePath.Add(filenya);
                        
                        fileName.Add(Path.GetFileNameWithoutExtension(filenya));
                        //Listview.Items[Listview.Items.Count - 1].SubItems.Add(VideoDurations[counter].ToString()+" Min ");
                        counter++;
                        
                    }

                }
                    
                             
            }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
          
        }
        public void Prev_click(object sender, EventArgs e)
        {
            RenamedListView.Items.Clear();
            for (int x = 0; x < originalListView.Items.Count; x++)
            {
                string namaFile = originalListView.Items[x].SubItems[1].Text;
                string extensiFile = originalListView.Items[x].SubItems[2].Text;
                RenamedListView.Items.Add((RenamedListView.Items.Count + 1).ToString());
                for (int i = 0; i < ListBox1.Items.Count; i++)
                {
                    namaFile = namaFile.ToLower().Replace(ListBox1.Items[i].ToString().ToLower(), "");
                }
                if (replaceCheck.Checked == true)
                {
                    namaFile = namaFile.Replace(replaceT.Text, replace2T.Text);
                   
                }
                
                if (LowercaseR.Checked == true)
                {
                    namaFile = namaFile.ToLower();
                }
                else
                {
                    namaFile = namaFile.ToUpper();
                }
                
                RenamedListView.Items[RenamedListView.Items.Count - 1].SubItems.Add(namaFile);
                RenamedListView.Items[RenamedListView.Items.Count - 1].SubItems.Add(extensiFile);
            }

        }
        private  void PreviewB_Click(object sender, EventArgs e)
        {
             Prev_click(sender,e);
        }

        private  void Process_Click(object sender, EventArgs e)
        {
            if (Process.Text == "Process")
            {
                if (originalListView.Items.Count < 1)
                    return;
                PreviewB.PerformClick();
                for (int x = 0; x < originalListView.Items.Count; x++)
                {
                    try
                    {
                      File.Move(textBox1.Text + "\\" + originalListView.Items[x].SubItems[1].Text + originalListView.Items[x].SubItems[2].Text, textBox1.Text + "\\" + RenamedListView.Items[x].SubItems[1].Text + RenamedListView.Items[x].SubItems[2].Text);
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                Process.Text = "Done";
                loadFiles(textBox1.Text, originalListView, BrowseFiles.Name);
            }
        }

        private void AddExcludeExtensionT_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && AddExcludeExtensionT.Text.Length >= 5)
            {
                exlusioinList.Items.Add(AddExcludeExtensionT.Text);
                AddExcludeExtensionT.Text = ""; loadFiles(textBox1.Text, originalListView, BrowseFiles.Name);
            }
        }

        private void replaceToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (replaceToggle.Checked == true)
            {
                replacePanel.Enabled = true;
            }
            else
            {
                replacePanel.Enabled = false;
            }
        }

        private void extcludeToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (extcludeToggle.Checked == true)
            {
                ExcludePanel.Enabled = true;
            }
            else
            {
                ExcludePanel.Enabled = false;
            }
        }

        private void replaceCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (replaceCheck.Checked == true)
            {
                replaceP.Enabled = true;

            }
            else
                replaceP.Enabled = false;
        }
#endregion file_processing
        
        #region video_processing
        //this calls BrowseVideos
        private  void BrowseVideos_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = false;
                    fbd.Description = "Browse Dir";
                    fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = fbd.SelectedPath;
                        outputFolder = fbd.SelectedPath;
                        //await Task.Run(() => videoTimings(fbd.SelectedPath));
                        loadFiles(fbd.SelectedPath, MainListView, BrowseVideos.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //process Video
        private async void processTile_Click(object sender, EventArgs e)
        {
            this.processTile.Enabled = false;
            try
            {
                metroProgressSpinner1.Visible = true;
                int i = 0,j=0;
                var stopwatch = Stopwatch.StartNew();
                this.StartTimeT.Text = DateTime.Now.ToShortTimeString();

                var progress = new Progress<int>(v =>
                {
                    metroProgressSpinner1.Value = v;
                });


                while (i <= numberOfFiles)
                {
                    stopwatch.Start();
                    inforLabel.Text = "Now Processing video:"+j++ ;
                    await Task.Run(() => processVideo(i, progress));
                    stopwatch.Stop();
                    this.richTextBox1.Text += "Video " +j+ " took:" + stopwatch.Elapsed.ToString() + Environment.NewLine;
                    if (i == numberOfFiles)
                    {
                        MessageBox.Show("All the files have been processed.");
                        metroProgressSpinner1.Visible = false;
                        this.processTile.Enabled = true;
                        break;
                    }
                    else
                        i++;
                }
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                this.totalTimeT.Text = elapsed.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw;
            }
            finally
            {
                MessageBox.Show("Your work is done.");
            }
        }
        private void processVideo(int index, IProgress<int> progress)
        {
            try
            {
                int FrameNo = 1;
                
                string newFolder = CreateNewFolder(index).ToString();//gives new folder path
                Directory.CreateDirectory(newFolder).ToString();//creates directory
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(filePath[index]);
                    for (int i = 1; i <= reader.FrameCount; i++)
                    {
                        if (FrameNo == i)
                        {
                            string path = newFolder + @"\" + FrameNo + ".jpg";

                            reader.ReadVideoFrame().Save(path);
                            FrameNo += FPS * Convert.ToInt32(reader.FrameRate.Value);
                            
                            extractedFrames++;
                        }
                        if (progress != null)
                            progress.Report(Convert.ToInt32((i + 1) * 100 / reader.FrameCount));
                    }
                    reader.Close();
                    FPS += 5;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw;
            }

        }
        public void Clock()
        {
            this.timeLabel.Text = DateTime.Now.ToString();  
        }
        //endprocessing Video

        private void videoTimings(string lokasi)
        {
            try
            {
                var ffprobe = new NReco.VideoInfo.FFProbe();
                foreach (string filenya in Directory.GetFiles(lokasi))
                {
                    var Info = ffprobe.GetMediaInfo(filenya.ToString());
                    VideoDurations.Add(Convert.ToInt32(Info.Duration.Minutes));
                }
                
               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            //now This method will sort the videos based on length.
            

        }
        private string CreateNewFolder(int index)
        {
            return (outputFolder + @"\" + fileName[index]).ToString();
        }
        #region videoSorting-nextupdate
        public void SortVideos()
        {
            MoveToDirectory(30.ToString(), @"E:\MoreAryVideo\ARY_NEWS_20170421_180338.mp4");
        }
        
        private void MoveToDirectory(string duration,string filePath)
        {
            try
            {
               
                //string dir = outputFolder + @"\" + duration + @"\" + "ARY_NEWS_20170421_180338";
                string dir = @"E:\30\ARY_NEWS_20170421_180338.mp4";
                if (Directory.Exists(dir))
                {
                    File.Move(@"E:\MoreAryVideo\ARY_NEWS_20170421_180338.mp4", dir);
                }
                else
                {
                    Directory.CreateDirectory(dir);
                    File.Move(filePath, dir);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                throw;
            }
            
            

        }

        #endregion

        #endregion video_processing

        private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            FPS = metroTrackBar1.Value;
            this.FPSlabel.Text = metroTrackBar1.Value.ToString();
        }

        private void metroTile3_Click(object sender, EventArgs e)
        {
            this.MainListView.Clear();
            this.StartTimeT.Text = "";
            this.richTextBox1.Clear();

        }

       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.Start("http://narutovids54.wixsite.com/myportfolio");
        }
    }
}
