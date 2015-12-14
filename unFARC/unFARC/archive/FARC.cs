using System;
using System.IO;

namespace unFARC.archive
{
    public class FARC
    {
        private Stream _Stream;
        private FARCHead _Header;
        private BinaryReader _Reader;
        public object FARCFAT;
        public bool isFarc= true;
        public string pathz;

        #region Returning Value
        public FARCHead Header
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
            set { _Stream = value; }
        }
        #endregion

        #region SIR0i Open File
        public FARC(string Path)
        {
            pathz = Path;
            _Stream = new FileStream(Path, FileMode.Open);
            _Header.Lenght = (uint) _Stream.Length;
            _Reader = new BinaryReader(_Stream);
            ReadSIR0Header();
        }

        #endregion

        #region FARC Read
        private void ReadSIR0Header()
        {
            if (Method.ckExtension(_Reader.ReadBytes(4), "FARC"))
            {
                try
                {
                    isFarc = true;

                    _Header.u1 = _Reader.ReadUInt32();
                    _Header.u2 = _Reader.ReadUInt32();
                    _Header.u3 = _Reader.ReadUInt32();
                    _Header.u4 = _Reader.ReadUInt32();
                    _Header.u5 = _Reader.ReadUInt32();
                    _Header.u6 = _Reader.ReadUInt32();
                    _Header.u7 = _Reader.ReadUInt32();
                    _Header.Volume = new FARCVol[3];
                    _Header.Volume[0].Type = _Reader.ReadInt32();
                    _Header.Volume[0].Sir0.Offset = _Reader.ReadUInt32();
                    _Header.Volume[0].Sir0.Lenght = _Reader.ReadUInt32();
                    _Header.Volume[0].DAT.Offset = _Reader.ReadUInt32();
                    _Header.Volume[0].DAT.Lenght = _Reader.ReadUInt32();

                    _Reader.BaseStream.Position = _Header.Volume[0].Sir0.Offset;
                    SIR0 sir0files = new SIR0(new MemoryStream(_Reader.ReadBytes((int)_Header.Volume[0].Sir0.Lenght)));

                    if (_Header.Volume[0].Type == 5)
                    {
                        FARCFAT = new SIR0f5(sir0files, _Header.Volume[0].Sir0.Offset, _Header.Volume[0].DAT.Offset,true);

                        //((SIR0f5)FARCFAT).getHeaderDump();
                    }
                    else
                    {
                        FARCFAT = new SIR0f4(sir0files, _Header.Volume[0].Sir0.Offset, _Header.Volume[0].DAT.Offset, true);
                    }

                    sir0files.Close();
                }
                catch (Exception exception)
                {
                    isFarc = false;
                    _Stream.Close();
                }

            }
            else
            {
                isFarc = false;
                _Stream.Close();
            }
        }

        #endregion

        public void StreamClose()
        {
            if (_Stream != null)
            {
                _Stream.Close();
            }
        }
    }
}
