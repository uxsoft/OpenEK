namespace OpenEK.API
{
    public class LedData
    {
        public byte Model;
        public LedMode Mode;
        public byte Color;
        public byte Speed;
        public byte Brightness;
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte RgbReload;
        public LedMode LastMode;
        public byte LastColor;
        public byte LastSpd;
        public byte LastBrightness;
        public byte LastRed;
        public byte LastGreen;
        public byte LastBlue;
        public byte Reserve0;
    }
}