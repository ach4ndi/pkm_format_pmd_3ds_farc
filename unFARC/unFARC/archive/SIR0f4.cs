using System.IO;

namespace unFARC.archive
{
    public class SIR0f4
    {
        private BinaryReader _BS;
        private HeaderSir4FAT _Header;
        private SIR0 _inSir0; //Basic Sir0
        private bool farcOffset = true;
        private uint Sir0Offset;
        private uint Dat0Offset;

        public SIR0f4(SIR0 input, uint OffsetSir0, uint OffsetDat0, bool realoffset = true)
        {
            _inSir0 = input;
            farcOffset = realoffset;
            Sir0Offset = OffsetSir0;
            Dat0Offset = OffsetDat0;

            sir0fat4();
        }

        public HeaderSir4FAT Header
        {
            get { return _Header; }
        }

        private void sir0fat4()
        {
            _BS = _inSir0.BReader;
            _BS.BaseStream.Position = _inSir0.Header.HeaderOffset;

            _Header.Offset = _BS.ReadUInt32();
            _Header.FileCount = _BS.ReadUInt32();
            _Header.Type = _BS.ReadUInt32();

            _Header.FAT = new FSir0Dat4[_Header.FileCount];
            _BS.BaseStream.Position = _Header.Offset;

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
                _Header.FAT[i].un1 = _BS.ReadUInt32();
            }

            if (_Header.Type == 0)
            {
                uint npe;
                for (int i = 0; i < _Header.FAT.Length; i++)
                {
                    npe = _Header.FAT[i].FileNameOffset;

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
                            _Header.FAT[i].FileNameOffset - Sir0Offset,
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
