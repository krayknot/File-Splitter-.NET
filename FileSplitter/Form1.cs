using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace FileSplitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            openFileDialog1.ShowDialog();
            txtInputFile.Text = openFileDialog1.FileName;

            txtOutputDirectory.Enabled = true;
            btnBrowseOutput.Enabled = true;
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtOutputDirectory.Text = folderBrowserDialog1.SelectedPath;

            txtChunkSize.Enabled = true;
            btnSplit.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            if (txtInputFile.Text.Length > 0 && txtOutputDirectory.Text.Length > 0 && txtChunkSize.Text.Length > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                SplitFile(txtInputFile.Text.Trim(), Convert.ToInt32(txtChunkSize.Text.Trim()), txtOutputDirectory.Text.Trim());
                Cursor.Current = Cursors.Arrow;

                string message = "Split operation has completed. Open Destination folder?";
                if (MessageBox.Show(message, "Information", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(txtOutputDirectory.Text.Trim());
                }
            }
            else
            {
                MessageBox.Show("One of the Input/Output path or chunk size text box is empty.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void SplitFile1(string inputFile, int chunkSize, string path)
        {
            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;
                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + index + Path.GetExtension(inputFile)))
                    {
                        int remaining = chunkSize, bytesRead;
                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                                Math.Min(remaining, BUFFER_SIZE))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            remaining -= bytesRead;
                        }
                    }
                    index++;
                    Thread.Sleep(500); // experimental; perhaps try it
                }
            }
        }

        public bool SplitFile(string SourceFile, int nNoofFiles, string OutputPath)
        {
            bool Split = false;
            try
            {
                FileStream fs = new FileStream(SourceFile, FileMode.Open, FileAccess.Read);
                int SizeofEachFile = (int)Math.Ceiling((double)fs.Length / nNoofFiles);
                for (int i = 0; i < nNoofFiles; i++)
                {
                    string baseFileName = Path.GetFileNameWithoutExtension(SourceFile);
                    string Extension = Path.GetExtension(SourceFile);
                    FileStream outputFile = new FileStream(OutputPath + "\\" + baseFileName + "." +
                        Extension, FileMode.Create, FileAccess.Write);
                    //mergeFolder = OutputPath;// Path.GetDirectoryName(SourceFile);
                    int bytesRead = 0;
                    byte[] buffer = new byte[SizeofEachFile];
                    if ((bytesRead = fs.Read(buffer, 0, SizeofEachFile)) > 0)
                    {
                        outputFile.Write(buffer, 0, bytesRead);
                        //outp.Write(buffer, 0, BytesRead);
                        //string packet = baseFileName + "." + i.ToString().PadLeft(3, Convert.ToChar("0")) + Extension.ToString();
                        //Packets.Add(packet);
                    }
                    outputFile.Close();
                }
                fs.Close();
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }

            return Split;
        }
    }
}
