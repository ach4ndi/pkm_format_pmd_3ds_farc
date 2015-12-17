using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using unFARC.archive;
using unFARC.frame;

namespace unFARC
{
    public partial class MainForm : Form
    {
        private FARC farcfile;

        public MainForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            Method.EnableTab(tabPage1, false);
            Method.EnableTab(tabPage2, false);
            Method.EnableTab(tabPage5, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            meta.configuration.path_over = Method.strOpenFile();

            if (meta.configuration.path_over != "")
            {
                textBox9.Text = meta.configuration.path_over;

                if (farcfile == null)
                {
                    farcfile = new FARC(meta.configuration.path_over);
                }
                else
                {
                    farcfile.Stream.Close();
                    farcfile = new FARC(meta.configuration.path_over);
                }

                if (farcfile.isFarc)
                {
                    Method.EnableTab(tabPage1, true);
                    Method.EnableTab(tabPage2, true);
                    Method.EnableTab(tabPage5, true);
                    tx_sir0_1.Text = farcfile.Header.Volume[0].Type + "";
                    tx_sir0_2.Text = "0x" + farcfile.Header.Volume[0].Sir0.Offset.toHexString();
                    tx_sir0_3.Text = farcfile.Header.Volume[0].Sir0.Lenght + "";
                    tx_dat_1.Text = "0x" + farcfile.Header.Volume[0].DAT.Offset.toHexString();
                    tx_dat_2.Text = farcfile.Header.Volume[0].DAT.Lenght + "";

                    tx_u_1.Text = farcfile.Header.u1.toHexString();
                    tx_u_2.Text = farcfile.Header.u2.toHexString();
                    tx_u_3.Text = farcfile.Header.u3.toHexString();
                    tx_u_4.Text = farcfile.Header.u4.toHexString();
                    tx_u_5.Text = farcfile.Header.u5.toHexString();
                    tx_u_6.Text = farcfile.Header.u6.toHexString();
                    tx_u_7.Text = farcfile.Header.u7.toHexString();

                    if (farcfile.Header.Volume[0].Type == 5)
                    {
                        tx_sir0_count.Text = (int) ((SIR0f5) farcfile.FARCFAT).Header.FileCount + "";
                        tx_sir0_flag.Text = (int) ((SIR0f5) farcfile.FARCFAT).Header.Type + "";

                        dataGridView1.Rows.Clear();

                        if (checkBox1.Checked)
                        {
                            int lenghtfile = (int)((SIR0f5)farcfile.FARCFAT).Header.FileCount;
                            this.tabPage5.Text = "Files [" + lenghtfile + "]";

                            for (int i = 0; i < lenghtfile; i++)
                            {
                                dataGridView1.Rows.Add(i, ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].FilenameString,
                                    "0x" + ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].DataOffset.toHexString(),
                                    ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].DataLenght,
                                    "0x" + ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].FileNameOffset.toHexString());
                            }
                        }
                    }
                    else
                    {
                        tx_sir0_count.Text = (int) ((SIR0f4) farcfile.FARCFAT).Header.FileCount + "";
                        tx_sir0_flag.Text = (int) ((SIR0f4) farcfile.FARCFAT).Header.Type + "";

                        dataGridView1.Rows.Clear();

                        if (checkBox1.Checked)
                        {
                            int lenghtfile = (int) ((SIR0f4) farcfile.FARCFAT).Header.FileCount;
                            this.tabPage5.Text = "Files [" + lenghtfile + "]";

                            for (int i = 0; i < lenghtfile; i++)
                            {
                                dataGridView1.Rows.Add(i, ((SIR0f4) farcfile.FARCFAT).Header.FAT[i].FilenameString,
                                    "0x" + ((SIR0f4) farcfile.FARCFAT).Header.FAT[i].DataOffset.toHexString(),
                                    ((SIR0f4) farcfile.FARCFAT).Header.FAT[i].DataLenght,
                                    "0x" + ((SIR0f4) farcfile.FARCFAT).Header.FAT[i].FileNameOffset.toHexString()+" "+ ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].un1.toHexString());
                            }
                        }
                    }
                }
                else
                {
                    Method.EnableTab(tabPage1, false);
                    Method.EnableTab(tabPage2, false);
                    Method.EnableTab(tabPage5, false);
                }

                farcfile.StreamClose();
            }

            GC.Collect();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileStream openFile = new FileStream(meta.configuration.path_over,FileMode.Open);

            Method.CopySection(openFile,Method.strSaveFile(), (int) farcfile.Header.Volume[0].Sir0.Lenght, farcfile.Header.Volume[0].Sir0.Offset);
            openFile.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FileStream openFile = new FileStream(meta.configuration.path_over, FileMode.Open);

            Method.CopySection(openFile, Method.strSaveFile(), (int)farcfile.Header.Volume[0].DAT.Lenght, farcfile.Header.Volume[0].DAT.Offset);
            openFile.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (farcfile.Header.Volume[0].Type == 5)
            {
                progressBar1.Maximum = (int)((SIR0f5)farcfile.FARCFAT).Header.FileCount;
            }
            else
            {
                progressBar1.Maximum = (int)((SIR0f4)farcfile.FARCFAT).Header.FileCount;
            }

            string path_open = "";

            if (radioButton1.Checked)
            {
                path_open = Application.StartupPath + @"\" + System.IO.Path.GetFileName(meta.configuration.path_over).Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0] + @"_unpack\";
            }

            if (radioButton2.Checked)
            {
                path_open = Path.GetFullPath(meta.configuration.path_over).Split(new []{"."}, StringSplitOptions.RemoveEmptyEntries)[0] + @"_unpack\";
            }

            if (radioButton3.Checked)
            {
                path_open = textBox8.Text.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0] + @"_unpack\";
            }

            if (Directory.Exists(path_open))
            {
                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(path_open);

                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            progressBar1.Value = 0;
            progressBar1.Visible = true;
            label8.Visible = true;
            tabControl1.Enabled = false;

            bool check1 = radioButton6.Checked;
            int pil = comboBox1.SelectedIndex;

            Thread thread = new Thread(() => {
                if (check1)
                {
                    farcfile.Extract(path_open, this);

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.progressBar1.Value = 0;
                        progressBar1.Visible = true;
                        label8.Visible = true;
                    });
                    switch (pil)
                    {
                        case 0:
                            farcfile.Extract(path_open, this, TypeExtract.MSG5);
                            break;
                        case 1:
                            farcfile.Extract(path_open, this, TypeExtract.MSG4);
                            break;
                        case 2:
                            farcfile.Extract(path_open, this, TypeExtract.facegrap);
                            break;
                        case 3:
                            farcfile.Extract(path_open, this, TypeExtract.ctes);
                            break;
                    }
                    
                }
                else
                {
                    farcfile.Extract(path_open, this);
                }
            });
            thread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox8.Text = Method.strOpenFile();
        }

        private void CLOSE(object sender, FormClosedEventArgs e)
        {
            Program.Messages.Sort((emp1, emp2) => emp1.code.CompareTo(emp2.code));

            File.WriteAllText("msg_code.json", JsonConvert.SerializeObject(Program.Messages, Formatting.Indented));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (farcfile.Header.Volume[0].Type == 5)
            {
                dataGridView1.Rows.Clear();

                int lenghtfile = (int)((SIR0f5)farcfile.FARCFAT).Header.FileCount;
                this.tabPage5.Text = "Files [" + lenghtfile + "]";

                for (int i = 0; i < lenghtfile; i++)
                {
                    dataGridView1.Rows.Add(i, ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].FilenameString,
                        "0x" + ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].DataOffset.toHexString(),
                        ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].DataLenght,
                        "0x" + ((SIR0f5)farcfile.FARCFAT).Header.FAT[i].FileNameOffset.toHexString()+" ("+ ((int)((SIR0f5)farcfile.FARCFAT).Header.FAT[i].FileNameOffset)+")");
                }
            }
            else
            {
                tx_sir0_count.Text = (int)((SIR0f4)farcfile.FARCFAT).Header.FileCount + "";
                tx_sir0_flag.Text = (int)((SIR0f4)farcfile.FARCFAT).Header.Type + "";

                dataGridView1.Rows.Clear();

                int lenghtfile = (int)((SIR0f4)farcfile.FARCFAT).Header.FileCount;
                this.tabPage5.Text = "Files [" + lenghtfile + "]";

                for (int i = 0; i < lenghtfile; i++)
                {
                    dataGridView1.Rows.Add(i, ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].FilenameString,
                        "0x" + ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].DataOffset.toHexString(),
                        ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].DataLenght,
                        "0x" + ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].FileNameOffset.toHexString()+" "+ ((SIR0f4)farcfile.FARCFAT).Header.FAT[i].un1.toHexString());
                    
                }
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageView view = new MessageView();
            view.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            meta.configuration.path_over = Method.strOpenFile();

            if (meta.configuration.path_over != "")
            {
                using (FileStream stream = new FileStream(meta.configuration.path_over, FileMode.Open))
                {
                    BinaryReader stre = new BinaryReader(stream);
                    stre.BaseStream.Position = 3;
                    MemoryStream inpa =
                        new MemoryStream(
                            Ionic.Zlib.ZlibStream.UncompressBuffer(stre.ReadBytes((int) stre.BaseStream.Length)));
                    File.WriteAllBytes(Application.StartupPath+@"\s",inpa.ToArray());
                }
            }
        }
    }
}
