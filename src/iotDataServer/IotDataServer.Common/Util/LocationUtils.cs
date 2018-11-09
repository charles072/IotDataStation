using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Util
{
    public class LocationUtils
    {
        public static PinPoint ParseLocation(string location, string gisLocation)
        {
            PinPoint nodePoint = new PinPoint();

            if (!string.IsNullOrWhiteSpace(location))
            {
                double x, y;
                string[] splitLocation = StringUtils.Split(location, ',');
                if (splitLocation.Length != 2)
                {
                    return null;
                }
                if (!double.TryParse(splitLocation[0], out x))
                {
                    return null;
                }
                if (!double.TryParse(splitLocation[1], out y))
                {
                    return null;
                }
                nodePoint.X = x;
                nodePoint.Y = y;
            }
            else if (!string.IsNullOrWhiteSpace(gisLocation))
            {
                double x, y;
                string[] splitLocation = StringUtils.Split(gisLocation, ',');
                if (splitLocation.Length != 2)
                {
                    return null;
                }
                if (!double.TryParse(splitLocation[1], out x))
                {
                    return null;
                }
                if (!double.TryParse(splitLocation[0], out y))
                {
                    return null;
                }
                nodePoint.X = x;
                nodePoint.Y = y;
            }

            return nodePoint;
        }
    }
}
