// The MIT License (MIT)

// Original source code Copyright (c) 2016 Sean Connelly(@voidqk, web: syntheti.cc)
// Ported source code Copyright (c) 2018 - 2022 Pierre Chalamet

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

using System;

namespace PolyBool
{
    public class PolyBool
    {
        private readonly Epsilon eps;

        public PolyBool(Epsilon eps = null)
        {
            this.eps = eps ?? Epsilon.Default;
        }

        public PolySegments Segments(Polygon poly)
        {
            var i = new Intersecter.RegionIntersecter(eps);
            foreach (Region region in poly.Regions)
            {
                i.AddRegion(region);
            }

            return new PolySegments
            {
                Segments = i.Calculate(poly.Inverted),
                Inverted = poly.Inverted
            };
        }

        public CombinedPolySegments Combine(PolySegments segments1, PolySegments segments2)
        {
            var i3 = new Intersecter.SegmentIntersecter(eps);
            return new CombinedPolySegments
            {
                Combined = i3.Calculate(segments1.Segments, segments1.Inverted, segments2.Segments, segments2.Inverted),
                                        Inverted1 = segments1.Inverted,
                                        Inverted2 = segments2.Inverted
            };
        }

        public PolySegments SelectUnion(CombinedPolySegments combined)
        {
            return new PolySegments
            {
                Segments = SegmentSelector.Union(combined.Combined),
                Inverted = combined.Inverted1 || combined.Inverted2
            };
        }

        public PolySegments SelectIntersect(CombinedPolySegments combined)
        {
            return new PolySegments
            {
                Segments = SegmentSelector.Intersect(combined.Combined),
                Inverted = combined.Inverted1 && combined.Inverted2
            };
        }

        public PolySegments SelectDifference(CombinedPolySegments combined)
        {
            return new PolySegments
            {
                Segments = SegmentSelector.Difference(combined.Combined),
                Inverted = combined.Inverted1 && !combined.Inverted2
            };
        }

        public PolySegments SelectDifferenceRev(CombinedPolySegments combined)
        {
            return new PolySegments
            {
                Segments = SegmentSelector.DifferenceRev(combined.Combined),
                Inverted = !combined.Inverted1 && combined.Inverted2
            };
        }

        public PolySegments SelectXor(CombinedPolySegments combined)
        {
            return new PolySegments
            {
                Segments = SegmentSelector.Xor(combined.Combined),
                Inverted = combined.Inverted1 != combined.Inverted2
            };
        }

        public Polygon Polygon(PolySegments polySegments)
        {
            return new Polygon(SegmentChainer.Chain(polySegments.Segments, eps).ToArray(), polySegments.Inverted);
        }


        public Polygon Operate(Polygon poly1, Polygon poly2, Func<CombinedPolySegments, PolySegments> selector)
        {
            var seg1 = Segments(poly1);
            var seg2 = Segments(poly2);
            var comb = Combine(seg1, seg2);
            var seg3 = selector(comb);
            return Polygon(seg3);
        }

        public Polygon Union(Polygon poly1, Polygon poly2)
        {
            return Operate(poly1, poly2, SelectUnion);
        }

        public Polygon Intersect(Polygon poly1, Polygon poly2)
        {
            return Operate(poly1, poly2, SelectIntersect);
        }

        public Polygon Difference(Polygon poly1, Polygon poly2)
        {
            return Operate(poly1, poly2, SelectDifference);
        }

        public Polygon DifferenceRev(Polygon poly1, Polygon poly2)
        {
            return Operate(poly1, poly2, SelectDifferenceRev);
        }

        public Polygon Xor(Polygon poly1, Polygon poly2)
        {
            return Operate(poly1, poly2, SelectXor);
        }
    }
}