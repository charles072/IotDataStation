using System;
using IotDataServer.Common.DataModel;

namespace IotDataServer.Common.Util
{
    public class CoordinateUtils
    {
        public static PinPoint MapPointTransfer(PinRect inputRect, PinRect targetRect, PinPoint inputPoint)
        {
            PinPoint outputPoint = new PinPoint();

            outputPoint.X = CoordinateTransfer(inputRect.Left, inputRect.Right, targetRect.Left, targetRect.Right, inputPoint.X);
            outputPoint.Y = CoordinateTransfer(inputRect.Top, inputRect.Bottom, targetRect.Top, targetRect.Bottom, inputPoint.Y);
            return outputPoint;
        }

        private static double CoordinateTransfer(double inputStart, double inputEnd, double targetStart, double targetEnd, double inputPosition)
        {

            bool isInputReverse= (inputEnd < inputStart);
            bool isTargetReverse = (targetEnd < targetStart);
            double inputLength = Math.Abs(inputEnd - inputStart);
            double targetLength = Math.Abs(targetEnd - targetStart);

            double inputOffset = (isInputReverse) ? inputLength - (inputPosition - inputEnd) : inputPosition - inputStart ;

            double targetBaseOffset = (targetLength * inputOffset / inputLength);

            double targetPosition = (isTargetReverse) ? targetEnd + (targetLength - targetBaseOffset) : targetStart + targetBaseOffset;

            return targetPosition;
        }
    }
}
