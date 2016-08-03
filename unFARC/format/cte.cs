
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using _3DS.GPU;

namespace unFARC.format
{
    public class cte
    {
        public int Width;
        public int Height;
        private int PixelLenght;
        private uint unknown1;
        private uint PixelOffset;
        public bool isCTE=false;
        private BinaryReader internalBS;
        private Textures.ImageFormat PixelFormat;

        public cte(BinaryReader bread)
        {
            internalBS = bread;
            bread.BaseStream.Position = 0;

            if (Method.ckExtension(bread.ReadBytes(4), "cte"))
            {
                isCTE = true;
                ReadCTE();
            }
            else
            {
                isCTE = false;
            }
        }

        public Bitmap getBitmap()
        {
            internalBS.BaseStream.Position = PixelOffset;

            return Textures.ToBitmap(internalBS.ReadBytes((int) (internalBS.BaseStream.Length - PixelOffset)), 0, Width,
                Height, PixelFormat, false);
        }

        private void ReadCTE()
        {
            uint pixelformat = internalBS.ReadUInt32();
            switch (pixelformat)
            {
                case 2:
                    PixelFormat = Textures.ImageFormat.RGB8;
                    break;
                case 3:
                    PixelFormat = Textures.ImageFormat.RGBA8;
                    break;
                case 4:
                    PixelFormat = Textures.ImageFormat.ETC1;
                    break;
                case 5:
                    PixelFormat = Textures.ImageFormat.ETC1A4;
                    break;
                case 9:
                    PixelFormat = Textures.ImageFormat.RGBA4;
                    break;
                default:
                    MessageBox.Show("ImageFormat : " + pixelformat);
                    break;
            }

            Width = (int) internalBS.ReadUInt32();
            Height = (int)internalBS.ReadUInt32();
            PixelLenght = (int)internalBS.ReadUInt32();
            unknown1 = internalBS.ReadUInt32();
            PixelOffset = internalBS.ReadUInt32();
        }
    }
}
