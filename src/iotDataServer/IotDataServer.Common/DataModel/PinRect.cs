namespace IotDataServer.Common.DataModel
{
    public class PinRect
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public double Width => Right - Left;
        public double Height => Bottom - Top;
        public bool IsEmpty => Width == 0 || Height == 0;

        public PinRect(double left = 0, double top = 0, double right = 0, double bottom = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}