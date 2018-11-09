namespace IotDataServer.Common.DataModel
{
    public class PinPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PinPoint(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X + "," +Y;
        }
    }
}