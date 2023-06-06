using System;
using OpenCvSharp;

namespace OpenCvLogicWarp
{
    public class Warper
    {
        public int Padding { get; set; } = 6;
        private ImreadModes ImreadMode { get; set; } = ImreadModes.AnyColor;
        private Mat QrPointsFounder(Point[] points, byte[] bytes)
        {
            if (points.Length != 4) return null;
            Mat src = Cv2.ImDecode(bytes, ImreadMode);
            int minX = Math.Min(points[0].X, points[3].X);
            int minY = Math.Min(points[0].Y, points[1].Y);
            int w = Math.Max(points[1].X - minX, points[3].Y - minY);                
            return src[new Rect(location: new Point(minX - Padding, minY - Padding), size: new Size(w + (2 * Padding), w + (2 * Padding)))];
        }
        public System.Drawing.Bitmap QrFounder(byte[] bytes)
        {
            Cv2.SetUseOptimized(true);
            Point2f[] points2f = Detect(bytes);
            Point[] points = new Point[points2f.Length];
            for (int i = 0; i < points2f.Length; i++)
                points[i] = new Point(points2f[i].X, points2f[i].Y);

            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(QrPointsFounder(points, bytes));
        }
        public Point2f[] Detect(byte[] bytes)
        {
            QRCodeDetector det = new QRCodeDetector();
            det.DetectMulti(Cv2.ImDecode(bytes, ImreadMode), out Point2f[] points2f);
            return points2f;
        }
    }
}