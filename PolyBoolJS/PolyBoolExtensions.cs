// The MIT License (MIT)

// Copyright (c) 2018 - 2022 Pierre Chalamet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
