using System.Collections.Generic;
using System.Linq;

namespace PolyBoolJS
{
    public static class PolyBoolExtensions
    {
        // Point
        public static object ToJavaScript(this global::PolyBool.Point @this)
        {
            return new double[] { @this.X, @this.Y };
        }

        public static global::PolyBool.Point PointFromJavaScript(this object jspoint)
        {
            var point = (object[])jspoint;
            dynamic x = point[0];
            dynamic y = point[1];
            return new global::PolyBool.Point(x, y);
        }

        // Region
        public static object ToJavaScript(this global::PolyBool.Region @this)
        {
            return @this.Points.Select(x => x.ToJavaScript()).ToArray();
        }

        public static global::PolyBool.Region RegionFromJavaScript(this object jsregion)
        {
            var region = (object[])jsregion;
            var points = new global::PolyBool.Point[region.Length];
            for (int i = 0; i < points.Length; ++i)
            {
                var point = region[i].PointFromJavaScript();
                points[i] = point;
            }

            return new global::PolyBool.Region(points);
        }

        // Polygon
        public static object ToJavaScript(this global::PolyBool.Polygon @this)
        {
            var jsregions = @this.Regions.Select(x => x.ToJavaScript()).ToArray();
            return new Dictionary<string, object>() { { "regions", jsregions },
                                                      { "inverted", @this.Inverted } };
        }

        public static global::PolyBool.Polygon PolygonFromJavaScript(this object jspolygon)
        {
            var polygon = (Dictionary<string, object>)jspolygon;
            var regions = (object[])polygon["regions"];
            var csinverted = (bool)polygon["inverted"];

            var csregion = new global::PolyBool.Region[regions.Length];
            for (int i = 0; i < regions.Length; ++i)
            {
                var region = regions[i].RegionFromJavaScript();
                csregion[i] = region;
            }

            return new global::PolyBool.Polygon(csregion, csinverted);
        }

    }
}
