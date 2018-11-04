namespace Iot.Common.DataModel
{
    public class PinNode
    {
        public string Id { get; set; }
        public PinPoint PinPoint { get; set; }
        public PinSize Size { get; set; }

        public PinAlignment PinAlignment { get; set; }
        public PinPoint LeftTopPoint => GetLeftTopPoint();

        public PinNode(string id, PinPoint pinPoint, PinSize size = null, PinAlignment pinAlignment = PinAlignment.CenterMiddle)
        {
            Id = id;
            PinPoint = pinPoint;
            Size = size ?? new PinSize();
            PinAlignment = pinAlignment;
        }

        public PinNode Clone()
        {
            PinNode cloneMapNode = new PinNode(Id, PinPoint, Size, PinAlignment);
            return cloneMapNode;
        }

        private PinPoint GetLeftTopPoint()
        {
            PinPoint point = new PinPoint(PinPoint.X, PinPoint.Y);

            if (Size.IsEmpty)
            {
                return point;
            }

            switch (PinAlignment)
            {
                case PinAlignment.LeftTop:
                    break;
                case PinAlignment.CenterTop:
                    point.X -= Size.Width / 2;
                    break;
                case PinAlignment.RightTop:
                    point.X -= Size.Width;
                    break;
                case PinAlignment.LeftMiddle:
                    point.Y -= Size.Height/2;
                    break;
                case PinAlignment.CenterMiddle:
                    point.X -= Size.Width / 2;
                    point.Y -= Size.Height / 2;
                    break;
                case PinAlignment.RightMiddle:
                    point.X -= Size.Width;
                    point.Y -= Size.Height / 2;
                    break;
                case PinAlignment.LeftBottom:
                    point.Y -= Size.Height;
                    break;
                case PinAlignment.CenterBottom:
                    point.X -= Size.Width / 2;
                    point.Y -= Size.Height;
                    break;
                case PinAlignment.RightBottom:
                    point.X -= Size.Width;
                    point.Y -= Size.Height;
                    break;
            }

            return point;
        }
    }
}
