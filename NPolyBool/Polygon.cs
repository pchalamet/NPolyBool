namespace PolyBool
{
    public class Polygon
    {
        public Polygon(Region[] regions, bool inverted)
        {
            this.Regions = regions;
            this.Inverted = inverted;
        }

        public Region[] Regions { get; }

        public bool Inverted { get; }
    }
}