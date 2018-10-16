using System.Collections.Generic;

namespace PolyBool
{
    public class Fill
    {
        public bool? Below { get; set; }

        public bool? Above { get; set; }
    }

    public class Segment
    {
        public Point End { get; set; }
        public Point Start { get; set; }
        public Fill MyFill { get; set; }
        public Fill OtherFill { get; set; }
    }

    public class PolySegments
    {
        public bool Inverted { get; set; }
        public List<Segment> Segments { get; set; }
    }

    public class CombinedPolySegments
    {
        public bool Inverted1 { get; set; }
        public bool Inverted2 { get; set; }
        public List<Segment> Combined { get; set; }
    }
}
