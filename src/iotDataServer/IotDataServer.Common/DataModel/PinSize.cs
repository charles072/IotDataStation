namespace IotDataServer.Common.DataModel
{
    public class PinSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsEmpty => (Width == 0 || Height == 0);

        public PinSize(double width = 0, double height = 0)
        {
            Width = width;
            Height = height;
        }
    }
}