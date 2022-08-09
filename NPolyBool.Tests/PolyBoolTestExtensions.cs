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

using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;
using PolyBool;

namespace NPolyBool.Tests
{
    public class PointEqualityComparer : IEqualityComparer<Point> {
        public bool Equals(Point? p1, Point? p2) {
            return (p1, p2) switch {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (Point x, Point y) => x.X == x.X && y.Y == y.Y
            };
        }

        public int GetHashCode([DisallowNull] Point obj) {
            throw new NotImplementedException();
        }
    }

    public class RegionEqualityComparer : IEqualityComparer<Region> {
        public bool Equals(Region? r1, Region? r2) {
            return (r1, r2) switch {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (Region x, Region y) => Enumerable.SequenceEqual(x.Points, y.Points, new PointEqualityComparer())
            };
        }

        public int GetHashCode([DisallowNull] Region obj) {
            throw new NotImplementedException();
        }
    }

    public class PolygonEqualityComparer : IEqualityComparer<Polygon> {
        public bool Equals(global::PolyBool.Polygon? p1, global::PolyBool.Polygon? p2) {
            return (p1, p2) switch {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (Polygon x, Polygon y) => x.Inverted == y.Inverted && Enumerable.SequenceEqual(x.Regions, y.Regions, new RegionEqualityComparer())
            };
        }

        public int GetHashCode([DisallowNull] Polygon obj) {
            throw new NotImplementedException();
        }
    }


    public static class PolyBoolTestExtensions
    {
        public static bool IsEqual(this Polygon polygon1, Polygon polygon2) {
            return new PolygonEqualityComparer().Equals(polygon1, polygon2);
        }

        public static void Dump(this Polygon polygon, string message) {
            Console.WriteLine($"===== {message} =====");
            Console.WriteLine($"Difference = {polygon.Inverted}");
            foreach (var region in polygon.Regions) {
                Console.WriteLine("Region:");
                foreach (var point in region.Points) {
                    Console.WriteLine($"  Point: {point.X}, {point.Y}");
                }
            }
        }
    }
}