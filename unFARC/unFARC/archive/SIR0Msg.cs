using System;
using System.IO;

namespace unFARC.archive
{
    public struct SIR0cHead
    {
        public uint FileCount;
        public uint Offset;

        public SIR0cFAT[] FAT;
    }

    public struct SIR0cFAT
    {
        public int index;
        public uint Offset;
        public uint Size;
        public uint unk1;
        public byte[] unk2;
    }

    public class SIR0Msg
    {
        private SIR0cHead _header;
        private BinaryReader bst;

        public SIR0cHead Header
        {
            get { return _header; }
        }

        public SIR0Msg(SIR0 input, bool giver = true)
        {
            bst = new BinaryReader(input.Stream);
            bst.BaseStream.Position = (long) input.Header.HeaderOffset;
            _header.FileCount = bst.ReadUInt32();
            _header.Offset = bst.ReadUInt32();
            
            _header.FAT = new SIR0cFAT[_header.FileCount];
            bst.BaseStream.Position = _header.Offset;

            for (int i = 0; i < _header.FAT.Length; i++)
            {
                if (giver)
                {
                    _header.FAT[i].index = i;
                    _header.FAT[i].Offset = bst.ReadUInt32();
                    _header.FAT[i].unk1 = bst.ReadUInt32();
                }
                else
                {
                    _header.FAT[i].index = i;
                    _header.FAT[i].Offset = bst.ReadUInt32();
                    _header.FAT[i].unk1 = bst.ReadUInt32();
                    _header.FAT[i].unk2 = bst.ReadBytes(4);
                }
            }

            if (!giver)
            {
                Array.Sort(_header.FAT, (x, y) => x.Offset.CompareTo(y.Offset));
            }

            uint lengkt = _header.FAT[0].Offset;

            for (int j = 0; j < _header.FAT.Length; j++)
            {
                if (j == _header.FAT.Length - 1)
                {
                    _header.FAT[j].Size = _header.Offset - lengkt;
                }
                else
                {
                    _header.FAT[j].Size = _header.FAT[j + 1].Offset - lengkt;
                    lengkt = _header.FAT[j + 1].Offset;
                }
            }
        }

        public uint getCount()
        {
            return _header.FileCount;
        }

        public uint getFATOffset()
        {
            return _header.FileCount;
        }
    }
}
