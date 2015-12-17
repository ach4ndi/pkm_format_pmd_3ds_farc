using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using unFARC.archive;

namespace unFARC.frame
{
    public partial class MessageView : Form
    {
        private SIR0 sir0file;
        private SIR0Msg sir0mess;
        public MessageView()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            meta.configuration.path_over = Method.strOpenFile();

            if (meta.configuration.path_over != "")
            {
                textBox9.Text = meta.configuration.path_over;

                sir0file = new SIR0(meta.configuration.path_over);
                sir0mess = new SIR0Msg(sir0file,false);
                Array.Sort(sir0mess.Header.FAT, (x, y) => x.unk1.CompareTo(y.unk1));
                listBox1.Items.Clear();

                for (int i = 0; i < sir0mess.Header.FileCount; i++)
                {
                    listBox1.Items.Add(i);
                }

                listBox1.SelectedIndex = 0;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            sir0file.BReader.BaseStream.Position = sir0mess.Header.FAT[listBox1.SelectedIndex].Offset;
            byte[] data = sir0file.BReader.ReadBytes((int) sir0mess.Header.FAT[listBox1.SelectedIndex].Size);
            fastColoredTextBox1.Text = Method.ReadUniCod(sir0file.BReader, sir0mess.Header.FAT[listBox1.SelectedIndex].Offset, sir0mess.Header.FAT[listBox1.SelectedIndex].Size);
            Method.setHexBox(hexBox1, data);

            textBox1.Text = sir0mess.Header.FAT[listBox1.SelectedIndex].index+"";
            textBox4.Text = sir0mess.Header.FAT[listBox1.SelectedIndex].Offset.toHexString();
            textBox2.Text = (int)sir0mess.Header.FAT[listBox1.SelectedIndex].unk1 +"";
            textBox3.Text = sir0mess.Header.FAT[listBox1.SelectedIndex].unk2.toHexString();
        }
    }
}
