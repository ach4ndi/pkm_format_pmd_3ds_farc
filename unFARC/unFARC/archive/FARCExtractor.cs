using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using unFARC.format;
using _3DS.GPU;

namespace unFARC.archive
{
    public static class FARCExtractor
    {
        internal static string paths;

        private static void ExtractRaw(FARC value, string path, MainForm form)
        {
            if (value.Header.Volume[0].Type == 5)
            {
                for (int i = 0; i < ((SIR0f5)value.FARCFAT).Header.FAT.Length; i++)
                {
                    if (((SIR0f5)value.FARCFAT).Header.Type == 0)
                    {
                        Method.CopySection(value.Stream,
                            paths + ((SIR0f5)value.FARCFAT).Header.FAT[i].FilenameString,
                            (int)((SIR0f5)value.FARCFAT).Header.FAT[i].DataLenght,
                            ((SIR0f5)value.FARCFAT).Header.FAT[i].DataOffset);
                    }
                    else
                    {
                        string names = Method.getExtensionString(value.Stream,
                            ((SIR0f5) value.FARCFAT).Header.FAT[i].DataOffset);

                        Method.CopySection(value.Stream, paths + i+names,
                        (int)((SIR0f5)value.FARCFAT).Header.FAT[i].DataLenght, ((SIR0f5)value.FARCFAT).Header.FAT[i].DataOffset);
                    }

                    form.BeginInvoke((MethodInvoker)delegate
                    {
                        form.progressBar1.Value++; form.label8.Text = form.progressBar1.Value + "/" + form.progressBar1.Maximum;
                    });
                }
            }
            else if (value.Header.Volume[0].Type == 4)
            {
                for (int i = 0; i < ((SIR0f4)value.FARCFAT).Header.FAT.Length; i++)
                {
                    if (((SIR0f4)value.FARCFAT).Header.Type == 0)
                    {
                        Method.CopySection(value.Stream,
                            paths + ((SIR0f4)value.FARCFAT).Header.FAT[i].FilenameString,
                            (int)((SIR0f4)value.FARCFAT).Header.FAT[i].DataLenght,
                            ((SIR0f4)value.FARCFAT).Header.FAT[i].DataOffset);
                    }
                    else
                    {
                        string names = Method.getExtensionString(value.Stream,
                            ((SIR0f5)value.FARCFAT).Header.FAT[i].DataOffset);
                        Method.CopySection(value.Stream, paths + i + names,
                        (int)((SIR0f4)value.FARCFAT).Header.FAT[i].DataLenght, ((SIR0f4)value.FARCFAT).Header.FAT[i].DataOffset);
                    }

                    form.BeginInvoke((MethodInvoker)delegate
                    {
                        form.progressBar1.Value++;
                        form.label8.Text = form.progressBar1.Value + "/" + form.progressBar1.Maximum;
                    });
                }
            }
        }

        private static void ExtractMessage(FARC value, string path, MainForm form, TypeExtract typeex)
        {
            string[] dirlist = Directory.GetFiles(paths);

            for (int i = 0; i < dirlist.Length; i++)
            {
                SIR0 dps = new SIR0(dirlist[i]);
                SIR0Msg msgs;

                switch (typeex)
                {
                    case TypeExtract.MSG4:
                        msgs = new SIR0Msg(dps, true);
                        break;
                    case TypeExtract.MSG5:
                        msgs = new SIR0Msg(dps, false);
                        break;
                    default:
                        msgs = new SIR0Msg(dps, false);
                        break;
                }
                StringBuilder builder = new StringBuilder();

                for (int j = 0; j < msgs.Header.FAT.Length; j++)
                {
                    builder.Append(Method.ReadUniCod(dps.BReader, msgs.Header.FAT[j].Offset, msgs.Header.FAT[j].Size)).
                        Append(Environment.NewLine);
                }

                File.WriteAllText(paths + Path.GetFileNameWithoutExtension(dirlist[i]) + ".txt", builder.ToString());

                form.BeginInvoke((MethodInvoker)delegate
                {
                    form.progressBar1.Value++;
                    form.label8.Text = form.progressBar1.Value + "/" + form.progressBar1.Maximum;
                });
            }
        }

        private static void ExtractPotrait(FARC value, string path, MainForm form)
        {
            string[] dirlist = Directory.GetFiles(paths);

            for (int i = 0; i < dirlist.Length; i++)
            {
                FileStream filest = new FileStream(dirlist[i], FileMode.Open);
                BinaryReader binarys = new BinaryReader(filest);
                Bitmap op = Textures.ToBitmap(binarys.ReadBytes((int)binarys.BaseStream.Length), 0, 64, 64, Textures.ImageFormat.RGB8);
                op.RotateFlip(RotateFlipType.Rotate180FlipX);
                op.Save(paths + Path.GetFileNameWithoutExtension(dirlist[i]) + ".png", ImageFormat.Png);

                form.BeginInvoke((MethodInvoker)delegate
                {
                    form.progressBar1.Value++; form.label8.Text = form.progressBar1.Value + "/" + form.progressBar1.Maximum;
                });
            }
        }

        private static void ExtractCTE(FARC value, string path, MainForm form)
        {
            string[] dirlist = Directory.GetFiles(paths);

            for (int i = 0; i < dirlist.Length; i++)
            {
                FileStream filest = new FileStream(dirlist[i], FileMode.Open);
                BinaryReader binarys = new BinaryReader(filest);
                cte ctefile = new cte(binarys);

                if (ctefile.isCTE)
                {
                    Bitmap op = ctefile.getBitmap(); op.RotateFlip(RotateFlipType.Rotate180FlipX);
                    op.Save(paths + Path.GetFileNameWithoutExtension(dirlist[i]) + ".png", ImageFormat.Png);
                }

                form.BeginInvoke((MethodInvoker)delegate
                {
                    form.progressBar1.Value++;
                    form.label8.Text = form.progressBar1.Value + "/" + form.progressBar1.Maximum;
                });
            }
        }

        public static void Extract(this FARC value, string path, MainForm form, TypeExtract typeex = TypeExtract.RAW)
        {
            value.Stream = new FileStream(value.pathz, FileMode.Open);

            paths = path +@"\";

            if (!Directory.Exists(paths))
            {
                Directory.CreateDirectory(paths);
            }

            switch (typeex)
            {
                case TypeExtract.facegrap:
                    ExtractPotrait(value, paths, form);
                    break;
                case TypeExtract.MSG4:
                case TypeExtract.MSG5:
                    ExtractMessage(value, paths, form, typeex);
                    break;
                case TypeExtract.ctes:
                    ExtractCTE(value, paths, form);
                    break;
                case TypeExtract.RAW:
                default:
                    ExtractRaw(value, paths, form);
                    break;
            }

            form.BeginInvoke((MethodInvoker)delegate
            {
                form.progressBar1.Visible = false;
                form.label8.Visible = false;
                form.tabControl1.Enabled = true;
            });

            value.Stream.Close();
        }
    }
}
