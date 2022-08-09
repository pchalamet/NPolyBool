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

using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace NPolyBool.Tests
{
    public class Tests
    {


        [Test]
        public void TestIssue1() {
            var regions = new List<PolyBool.Region>();

            var points = new List<PolyBool.Point>();
            points.Add(new PolyBool.Point(10, 10));
            points.Add(new PolyBool.Point(20, 10));
            points.Add(new PolyBool.Point(20, 20));
            points.Add(new PolyBool.Point(10, 20));
            points.Add(new PolyBool.Point(10, 10));
            regions.Add(new PolyBool.Region(points.ToArray()));
            var polygon = new PolyBool.Polygon(regions.ToArray(), false);

            var regions2 = new List<PolyBool.Region>();
            var points2 = new List<PolyBool.Point>();
            points2.Add(new PolyBool.Point(12, 12));
            points2.Add(new PolyBool.Point(18, 12));
            points2.Add(new PolyBool.Point(18, 18));
            points2.Add(new PolyBool.Point(12, 18));
            points2.Add(new PolyBool.Point(12, 12));
            regions2.Add(new PolyBool.Region(points2.ToArray()));
            var poly2 = new PolyBool.Polygon(regions2.ToArray(), false);

            var pbjs = new PolyBoolJS.PolyBool();
            var differencejs = pbjs.Difference(polygon, poly2);

            var pb = new PolyBool.PolyBool();
            var difference = pb.Difference(polygon, poly2);

            differencejs.Dump("PolyBoolJS");
            difference.Dump("NPolyBool");
            Assert.IsTrue(difference.IsEqual(differencejs));
        }
    }
}