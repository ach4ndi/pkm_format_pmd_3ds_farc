using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace unFARC.archive
{
    public class SIR0f5
    {
        private BinaryReader _BS;
        private HeaderSir5FAT _Header;
        private SIR0 _inSir0; //Basic Sir0
        private bool farcOffset = true;
        private uint Sir0Offset;
        private uint Dat0Offset;

        public HeaderSir5FAT Header
        {
            get { return _Header; }
        }

        public SIR0f5(SIR0 input, uint OffsetSir0, uint OffsetDat0, bool realoffset = true)
        {
            _inSir0 = input;
            farcOffset = realoffset;
            Sir0Offset = OffsetSir0;
            Dat0Offset = OffsetDat0;

            sir0fat5();
        }

        public void getHeaderDump()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < _Header.FAT.Length; i++)
            {
                builder.Append(Header.FAT[i].index).Append(",");
                builder.Append(Header.FAT[i].FileNameOffset).Append(",");
                builder.Append(Header.FAT[i].DataOffset).Append(",");
                builder.Append(Header.FAT[i].DataLenght);
                builder.Append(Environment.NewLine);
            }

            File.WriteAllText(Application.StartupPath + @"\" + "FATSIR05.txt", builder.ToString());
        }

        private void sir0fat5()
        {
            _BS = _inSir0.BReader;
            _BS.BaseStream.Position = _inSir0.Header.HeaderOffset;

            _Header.Offset = _BS.ReadUInt32();
            _Header.FileCount = _BS.ReadUInt32();
            _Header.Type = _BS.ReadUInt32();

            _Header.FAT = new FSir0Dat5[_Header.FileCount];
            _BS.BaseStream.Position = _Header.Offset;

            uint npe = 0;

            for (int i = 0; i < _Header.FAT.Length; i++)
            {
                _Header.FAT[i].index = i;

                if (farcOffset)
                {
                    _Header.FAT[i].FileNameOffset = _BS.ReadUInt32() + Sir0Offset;
                    _Header.FAT[i].DataOffset = _BS.ReadUInt32() + Dat0Offset;
                }
                else
                {
                    _Header.FAT[i].FileNameOffset = _BS.ReadUInt32();
                    _Header.FAT[i].DataOffset = _BS.ReadUInt32();
                }
                _Header.FAT[i].DataLenght = _BS.ReadUInt32();

                if (_Header.Type == 1)
                {
                    _Header.FAT[i].isHaveFileName = false;
                }
                else
                {
                    _Header.FAT[i].isHaveFileName = true;
                }
            }

            if (_Header.Type == 0) //Has FileNames
            {
                for (int i = 0; i < _Header.FAT.Length; i++)
                {
                    npe = _Header.FAT[i].FileNameOffset;

                    if (_Header.FAT[i].isHaveFileName)
                    {
                        if (i == _Header.FAT.Length - 1)
                        {
                            if (farcOffset)
                            {
                                _Header.FAT[i].FileNameLenght = (_Header.Offset + Sir0Offset) - npe;
                            }
                            else
                            {
                                _Header.FAT[i].FileNameLenght = (_Header.Offset) - npe;
                            }
                            
                        }
                        else
                        {
                            _Header.FAT[i].FileNameLenght = _Header.FAT[i + 1].FileNameOffset - npe;
                        }

                        if (farcOffset)
                        {
                            _Header.FAT[i].FilenameString =
                            Method.ReadAsStringUnicode(_BS,
                                _Header.FAT[i].FileNameOffset- Sir0Offset,
                                _Header.FAT[i].FileNameLenght);
                        }
                        else
                        {
                            _Header.FAT[i].FilenameString =
                            Method.ReadAsStringUnicode(_BS,
                                _Header.FAT[i].FileNameOffset,
                                _Header.FAT[i].FileNameLenght);
                        }
                    }
                }
            }
        }
    }
}
