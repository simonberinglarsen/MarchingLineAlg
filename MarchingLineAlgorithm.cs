using System;
using System.Drawing;

namespace WindowsFormsApp1
{
    internal class MarchingLineAlgorithm
    {
        public event Action<int, int, Face> VisitVoxel;
        public MarchingLineAlgorithm()
        {
        }

        public enum Face
        {
            North,
            South,
            East,
            West, 
            None,
            Unknown
        }

        internal void March(PointF start, PointF end)
        {
            Vector2 p0 = new Vector2(start.X, start.Y);
            Vector2 d = new Vector2(end.X - start.X, end.Y - start.Y);
            float totalLength = d.Length;
            d.Normalize();

            int sx = Math.Sign(d.X);
            int sy = Math.Sign(d.Y);
            int vx = (int)p0.X - (p0.X < 0 ? 1 : 0);
            int vy = (int)p0.Y - (p0.Y < 0 ? 1 : 0);
            Face face = Face.None;
            while (true)
            {
                VisitVoxel(vx, vy, face);

                float x2 = vx + (sx < 0 ? 0 : 1);
                float y2 = vy + (sy < 0 ? 0 : 1);
                float t1 = (x2 - p0.X) / d.X;
                float t2 = (y2 - p0.Y) / d.Y;

                if (t1 < t2)
                {
                    vx += sx;
                    totalLength -= t1;
                    p0.X = x2;
                    p0.Y = t1 * d.Y + p0.Y;
                    face = sx < 0 ? Face.East : Face.West;
                }
                else //(t2 <= t1)
                {
                    vy += sy;
                    totalLength -= t2;
                    p0.Y = y2;
                    p0.X = t2 * d.X + p0.X;
                    face = sy < 0 ? Face.North : Face.South;
                }
                if (totalLength < 0.001f) break;
            }
        }







        class Vector2
        {
            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }
            public void Normalize()
            {
                float l = Length;
                if (l == 0) return;
                X = X / l;
                Y = Y / l;
            }
            public float Length
            {
                get
                {
                    return (float)(Math.Sqrt(X * X + Y * Y));
                }
            }
            public float X { get; set; }
            public float Y { get; set; }
        }

    }
}