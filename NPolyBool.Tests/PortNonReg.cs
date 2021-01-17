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