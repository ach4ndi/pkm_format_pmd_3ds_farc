using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unFARC
{
    public static class Method
    {
        public static bool ckExtension(byte[] input, string obj)
        {
            return ReadAsStringASCII(input).Equals(obj);
        }

        public static string ckExtension(BinaryReader input, uint offset, uint lenght)
        {
            input.BaseStream.Position = offset;

            return ReadAsStringASCII(input.ReadBytes((int)lenght));
        }

        public static string getExtensionString(Stream input,uint offset)
        {
            BinaryReader inpu = new BinaryReader(input);
            string extensionring = ckExtension(inpu, offset, 4);

            switch (extensionring)
            {
                case "SIR0":
                    return ".sir0";
                case "FARC":
                    return ".farc";
                case "cte":
                    return ".cte";
                case "BCH":
                    return ".bch";
                default:
                    return ".bin";
            }
        }

        public static string ReadAsStringASCII(byte[] input)
        {
            string a = "";
            char b = 'a';

            for (int i = 0; i < (input.Length); i++)
            {
                b = (char)input[i];
                if (b == 0)
                {
                    a += "";
                }
                else if (b > 32 || b < 127)
                {
                    a += b.ToString();
                }
                else if (char.IsSeparator(b) || char.IsNumber(b) || char.IsLetterOrDigit(b))
                {
                    a += b.ToString();
                }
                else
                {
                    a += @"\x" + ((int)b);
                }
            }

            return a;
        }

        public static string toHexString(this uint value)
        {
            return String.Format("{0:X}", value);
        }

        public static string toHexString(this long value)
        {
            return String.Format("{0:X}", value);
        }

        public static string ReadAsStringUnicode(BinaryReader bst, uint offset, uint lenght)
        {
            string a = "";
            char b = 'a';

            bst.BaseStream.Position = offset;
            for (int i = 0; i < (lenght / 2); i++)
            {
                b = Encoding.Unicode.GetString(bst.ReadBytes(2)).ToCharArray()[0];

                if (b == 0)
                {
                    a += "";
                }
                else if (b > 32 || b < 127)
                {
                    a += b.ToString();
                }
                else if (char.IsSeparator(b) || char.IsNumber(b) || char.IsLetterOrDigit(b))
                {
                    a += b.ToString();
                }
                else
                {
                    a += @"\x" + ((int)b);
                }
            }

            return a;
        }

        
        public static string ReadUniCod(BinaryReader bst, uint offset, uint lenght)
        {
            string a = "";
            int b = 'a';
            bst.BaseStream.Position = offset;

            for (int i = 0; i < (lenght / 2); i++)
            {
                b = bst.ReadUInt16();

                if (b == 0)
                {
                    a += "";
                    break;
                }
                else if ((b > 0 && b < 14))
                {
                    a += @"\x" + ((int)b);
                }
                else if (b > 32 && b < 127)
                {
                    a += ((char)b).ToString();
                }
                else if (char.IsSymbol((char)b))
                {
                    a += ((char)b).ToString();
                }
                else if (Program.Messages.Exists(msgc => Convert.ToInt32(msgc.code, 16) == (int)b))
                {
                    string g1 = Program.Messages.Where(msgc => Convert.ToInt32(msgc.code, 16) == (int)b).Select(msgc => msgc.code_name).First();
                    int g2 = Program.Messages.Where(msgc => Convert.ToInt32(msgc.code, 16) == (int)b).Select(msgc => msgc.values).First();

                    switch (g1)
                    {
                        case "-":
                        case "":
                            a += @"\x" + ((long)b).toHexString();
                            break;
                        case null:
                            a += "";
                            break;
                        default:
                            if (g2 != 0)
                            {
                                a += @"[" + g1 + "_";
                            }
                            else
                            {
                                a += @"[" + g1;
                            }
                            break;
                    }

                    switch (g2)
                    {
                        case 0:
                            break;
                        default:
                            for (int j = 0; j < g2; j++)
                            {
                                a += ((int)Encoding.Unicode.GetString(bst.ReadBytes(2)).ToCharArray()[0]);
                                if (j != (g2 - 1))
                                {
                                    a += "_";
                                }
                            }
                            break;
                    }

                    switch (g1)
                    {
                        case "-":
                        case "":
                            a += @"";
                            break;
                        default:
                            a += "]";
                            break;
                    }
                }
                else if (char.IsSeparator((char)b) || char.IsNumber((char)b) || char.IsLetterOrDigit((char)b))
                {
                    a += ((char)b).ToString();
                }
                else if (b > 41728)
                {
                    a += @"\x" + ((int)b);
                }
                else
                {
                    a += @"\x" + ((int)b);
                }
            }

            return a;
        }

        public static void CopySection(Stream input, string targetFile, int length, uint position)
        {
            if (targetFile != "")
            {
                byte[] buffer = new byte[8192];

                using (Stream output = File.OpenWrite(targetFile))
                {
                    input.Position = position;
                    int bytesRead = 1;
                    // This will finish silently if we couldn't read "length" bytes.
                    // An alternative would be to throw an exception
                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = input.Read(buffer, 0, Math.Min(length, buffer.Length));
                        output.Write(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
        }

        public static string strOpenFile()
        {
            OpenFileDialog openFileDia = new OpenFileDialog();
            openFileDia.InitialDirectory = meta.configuration.path_over;

            if (openFileDia.ShowDialog() == DialogResult.OK)
            {
                return openFileDia.FileName;
            }
            else
            {
                return "";
            }
        }

        public static string strSaveFile()
        {
            SaveFileDialog openFileDia = new SaveFileDialog();
            openFileDia.InitialDirectory = meta.configuration.path_over;

            if (openFileDia.ShowDialog() == DialogResult.OK)
            {
                return openFileDia.FileName;
            }
            else
            {
                return "";
            }
        }
    }

    #region FARC SIR0
    public struct HeaderSir5FAT
    {
        public uint FileCount;
        public uint Offset;
        public uint Type;
        public FSir0Dat5[] FAT;
    }

    public struct FSir0Dat5
    {
        //Extend
        public int index;

        //Fat 5 structure
        public uint FileNameOffset;
        public uint DataOffset;
        public uint DataLenght;

        //MoreForFilename
        public bool isHaveFileName;
        public uint FileNameLenght;
        public string FilenameString;
    }

    public enum TypeExtract
    {
        RAW,MSG5,MSG4,facegrap,ctes
    }

    public struct HeaderSir4FAT
    {
        public uint FileCount;
        public uint Offset;
        public uint Type;
        public FSir0Dat4[] FAT;
    }

    public struct FSir0Dat4
    {
        //Extend
        public int index;

        //Fat 5 structure
        public uint FileNameOffset;
        public uint DataOffset;
        public uint DataLenght;
        public uint un1;

        //MoreForFilename
        public bool isHaveFileName;
        public uint FileNameLenght;
        public string FilenameString;
    }
    #endregion

    #region FARC struct
    public struct FARCHead
    {
        public uint u1;
        public uint u2;
        public uint u3;
        public uint u4;
        public uint u5;
        public uint u6;
        public uint u7;
        public FARCVol[] Volume;
        public uint Lenght;
    }

    public struct FARCVol
    {
        public int Type;
        public FARCSIR0 Sir0;
        public FARCDAT DAT;
    }

    public struct FARCSIR0
    {
        public uint Offset;
        public uint Lenght;
    }

    public struct FARCDAT
    {
        public uint Offset;
        public uint Lenght;
    }
    #endregion
}
