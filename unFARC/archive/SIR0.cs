using System;
using System.IO;

namespace unFARC.archive
{
    public class SIR0
    {
        /* NOTES :
         * This basic sir0 reader, only cover header offset and pointer offset,
         * without pointer recalculation or to get offset on header. is just
         * simple sir0 format for used on another class.
         */

        private Stream _Stream;
        private SIR0Header _Header;
        private BinaryReader _Reader;
        private object _sir0fat;
        public bool isSir0 = true;

        #region SIR0 Open File
        public SIR0(string Path)
        {
            _Stream = new FileStream(Path, FileMode.Open);
            _Header.Lenght.File = _Stream.Length;
            _Reader = new BinaryReader(_Stream);
            ReadSIR0Header();
        }

        public SIR0(string Path, FileMode mode)
        {
            _Stream = new FileStream(Path, mode);
            _Header.Lenght.File = _Stream.Length;
            _Reader = new BinaryReader(_Stream);
            ReadSIR0Header();
        }

        public SIR0(MemoryStream str)
        {
            _Stream = str;
            _Header.Lenght.File = _Stream.Length;
            _Reader = new BinaryReader(_Stream);
            ReadSIR0Header();
        }

        public SIR0(byte[] str)
        {
            _Stream = new MemoryStream(str);
            _Header.Lenght.File = _Stream.Length;
            _Reader = new BinaryReader(_Stream);
            ReadSIR0Header();
        }

        public void Close()
        {
            if (_Stream != null)
            {
                _Stream.Close();
            }
        }
        #endregion

        #region Returning Value
        public SIR0Header Header
        {
            get { return _Header; }
        }

        public BinaryReader BReader
        {
            get { return _Reader; }
        }

        public Stream Stream
        {
            get { return _Stream; }
        }
        #endregion
        
        #region SIR0 Read
        private void ReadSIR0Header()
        {
            if (Method.ckExtension(_Reader.ReadBytes(4), "SIR0"))
            {
                try
                {
                    _Header.HeaderOffset = _Reader.ReadUInt32();
                    _Header.PointerOffset = _Reader.ReadUInt32();
                    _Header.Lenght.Header = _Header.PointerOffset - _Header.HeaderOffset;
                    _Header.Lenght.Pointer = _Header.Lenght.File - _Header.PointerOffset;
                    _Header.Lenght.DAT = _Header.Lenght.File - 16 - _Header.Lenght.Header - _Header.Lenght.Pointer;
                }
                catch (Exception exception)
                {
                    isSir0 = false;
                    _Stream.Close();
                }
            }
            else
            {
                isSir0 = false;
                _Stream.Close();
            }
        }
        #endregion
    }

    #region struct
    public struct SIR0Header
    {
        public uint HeaderOffset;
        public uint PointerOffset;
        public SIR0Lenght Lenght;
    }

    public struct SIR0Lenght
    {
        public long File;
        public long Header;
        public long Pointer;
        public long DAT;
    }
    #endregion
}
