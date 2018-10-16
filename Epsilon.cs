using System;

namespace PolyBool
{
    public class IntersectionPoint
    {
        public Point Pt { get; set; }
        public int AlongA { get; set; }
        public int AlongB { get; set; }

        public IntersectionPoint(int alongA, int alongB, Point pt)
        {
            AlongA = alongA;
            AlongB = alongB;
            Pt = pt;
        }
    }

    public class Epsilon
    {
        private readonly double eps;

        public Epsilon(double eps)
        {
            this.eps = eps;
        }

        public static Epsilon Default = new Epsilon(0.0000000001);

        public bool PointAboveOrOnLine(Point pt, Point left, Point right)
        {
            var Ax = left.X;
            var Ay = left.Y;
            var Bx = right.X;
            var By = right.Y;
            var Cx = pt.X;
            var Cy = pt.Y;
            return (Bx - Ax) * (Cy - Ay) - (By - Ay) * (Cx - Ax) >= -eps;
        }

        public bool PointBetween(Point p, Point left, Point right)
        {
            // p must be collinear with left->right
            // returns false if p == left, p == right, or left == right
            double dPyLy = p.Y - left.Y;
            double dRxLx = right.X - left.X;
            double dPxLx = p.X - left.X;
            double dRyLy = right.Y - left.Y;

            double dot = dPxLx * dRxLx + dPyLy * dRyLy;

            if (dot < eps)
                return false;

            double sqlen = dRxLx * dRxLx + dRyLy * dRyLy;
            if (dot - sqlen > -eps)
                return false;

            return true;
        }

        private bool PointsSameX(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) < eps;
        }

        private bool PointsSameY(Point p1, Point p2)
        {
            return Math.Abs(p1.Y - p2.Y) < eps;
        }

        public bool PointsSame(Point p1, Point p2)
        {
            return PointsSameX(p1, p2) && PointsSameY(p1, p2);
        }

        public int PointsCompare(Point p1, Point p2)
        {
            if (PointsSameX(p1, p2))
                return PointsSameY(p1, p2) ? 0 : (p1.Y < p2.Y ? -1 : 1);
            return p1.X < p2.X ? -1 : 1;
        }

        public bool PointsCollinear(Point pt1, Point pt2, Point pt3)
        {
            var dx1 = pt1.X - pt2.X;
            var dy1 = pt1.Y - pt2.Y;
            var dx2 = pt2.X - pt3.X;
            var dy2 = pt2.Y - pt3.Y;
            return Math.Abs(dx1 * dy2 - dx2 * dy1) < eps;
        }

        public IntersectionPoint LinesIntersect(Point a0, Point a1, Point b0, Point b1)
        {
            double adx = a1.X - a0.X;
            double ady = a1.Y - a0.Y;
            double bdx = b1.X - b0.X;
            double bdy = b1.Y - b0.Y;

            double axb = adx * bdy - ady * bdx;

            if (Math.Abs(axb) < eps)
                return null;

            double dx = a0.X - b0.X;
            double dy = a0.Y - b0.Y;

            double A = (bdx * dy - bdy * dx) / axb;
            double B = (adx * dy - ady * dx) / axb;

            int calcAlongUsingValue(double value)
            {
                if (value <= -eps) return -2;
                else if (value < eps) return -1;
                else if (value - 1.0 <= -eps) return 0;
                else if (value - 1.0 < eps) return 1;
                return 2;
            }

            var alongA = calcAlongUsingValue(A);
            var alongB = calcAlongUsingValue(B);
            var pt = new Point(a0.X + A * adx, a0.Y + A * ady);
            return new IntersectionPoint(alongA, alongB, pt);
        }
    }
}