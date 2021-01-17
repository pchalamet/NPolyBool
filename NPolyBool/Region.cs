namespace PolyBool
{
    public class Region
    {
        public Region(Point[] points)
        {
            this.Points = points;
        }

        public Point[] Points { get; }
    }
}