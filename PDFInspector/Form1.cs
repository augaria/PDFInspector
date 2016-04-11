using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using System.IO;
using System.ComponentModel;

namespace PDFInspector
{
    public partial class Form1 : Form
    {
        int total;
        int brokenCount;
        Stack folders;

        private BackgroundWorker bkWorker;


        public Form1()
        {
            try
            {
                InitializeComponent();


                //configure layout for different dpi
                Graphics graphics = this.CreateGraphics();
                if (graphics.DpiX == 120)
                {
                    foreach (Control ct in this.Controls)
                        ct.Font = new System.Drawing.Font(ct.Font.FontFamily, (float)(ct.Font.Size / 1.25), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                total = 0;
                brokenCount = 0;
                label1.Text = "0";
                label4.Text = "0";
                textBox1.Text = System.Environment.CurrentDirectory;
                folders = new Stack();


            }
            catch (Exception ept)
            {
                textBox3.Text += ept.Message;
            }
        }

        //check if a pdf is broken 
        bool inspect(string file)
        {
            try
            {
                PdfReader reader = new PdfReader(file);
                reader.Close();
                bkWorker.ReportProgress(0, "");
                return true;
            }
            catch(IOException e)
            {
                string[] report = { file, e.Message };
                bkWorker.ReportProgress(1,report);
                return false;
            }
        }

        //start scanning on backgroud
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.Clear();
                total = 0;
                brokenCount = 0;
                label1.Text = "0";
                label4.Text = "0";
                folders.Clear();
                Update();

                bkWorker = new BackgroundWorker();
                bkWorker.WorkerReportsProgress = true;
                bkWorker.WorkerSupportsCancellation = true;
                bkWorker.DoWork += new DoWorkEventHandler(scan);
                bkWorker.ProgressChanged += new ProgressChangedEventHandler(updateProgress);
                bkWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(completeWork);


                DirectoryInfo folder = new DirectoryInfo(textBox1.Text);
                label8.Text = "scanning";
                folders.Push(folder);
                bkWorker.RunWorkerAsync();
            }
            catch (Exception ept)
            {
                textBox3.Text += ept.Message;
            }
        }

        //update information while scanning
        private void updateProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                total++;
                label1.Text = total.ToString();
            }
            else if (e.ProgressPercentage == 1)
            {
                string[] report = (string[])e.UserState;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = report[0];
                lvi.SubItems.Add(report[1]);
                listView1.Items.Add(lvi);
                listView1.Items[(listView1.Items.Count) - 1].EnsureVisible();
                brokenCount++;
                label4.Text = brokenCount.ToString();
                total++;
                label1.Text = total.ToString();
            }
            else if (e.ProgressPercentage == 2)
            {
                textBox3.Text += (string)e.UserState +"\r\n";
            }
            
            Update();
        }

        //scan under a certian path using DFS
        private void scan(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (folders.Count>0)
                {
                    DirectoryInfo folder = (DirectoryInfo)folders.Pop();

                    DirectoryInfo[] subFolders;
                    FileInfo[] files;
                    try
                    {
                        subFolders = folder.GetDirectories();
                        files = folder.GetFiles("*.pdf");
                    }
                    catch (Exception ept0)
                    {
                        bkWorker.ReportProgress(2, ept0.Message);
                        continue;
                    }

                    foreach (FileInfo file in files)
                        inspect(file.FullName);

                    foreach (DirectoryInfo subFolder in subFolders)
                        folders.Push(subFolder);

                }
            }
            catch(Exception ept)
            {
                bkWorker.ReportProgress(2, ept.Message);
            }
        }

        private void completeWork(object sender, RunWorkerCompletedEventArgs e)
        {
            label8.Text = "done";
        }

        //browse folders
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = false;
                string path = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                    path = dialog.SelectedPath;
                if (!path.Equals(""))
                    textBox1.Text = path;
            }
            catch (Exception ept)
            {
                textBox3.Text += ept.Message;
            }
        }

        //sort by column
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (listView1.Columns[e.Column].Tag == null)
            {
                listView1.Columns[e.Column].Tag = true;
            }
            bool tabK = (bool)listView1.Columns[e.Column].Tag;
            if (tabK)
            {
                listView1.Columns[e.Column].Tag = false;
            }
            else
            {
                listView1.Columns[e.Column].Tag = true;
            }
            listView1.ListViewItemSorter = new ListViewSort(e.Column, listView1.Columns[e.Column].Tag);
            listView1.Sort();
        }

        //open the pdf
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(listView1.SelectedItems[0].Text);
        }
    }
}
