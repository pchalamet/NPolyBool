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

using System.Collections.Generic;

namespace PolyBool
{
    internal static class SegmentSelector
    {
        private static List<Segment> select(List<Segment> segments, int[] selection)
        {
            var result = new List<Segment>();
            foreach (var seg in segments)
            {
                var index = (seg.MyFill.Above == true ? 8 : 0)
                            + (seg.MyFill.Below == true ? 4 : 0)
                            + (seg.OtherFill?.Above == true ? 2 : 0)
                            + (seg.OtherFill?.Below == true ? 1 : 0);

                if (selection[index] != 0)
                {
                    result.Add(new Segment
                    {
                        Start = seg.Start,
                        End = seg.End,
                        MyFill = new Fill
                        {
                            Above = selection[index] == 1,
                            Below = selection[index] == 2
                        }
                    });
                }
            }

            return result;
        }

        public static List<Segment> Union(List<Segment> segments)
        {
            //                                      primary | secondary
            // above1 below1 above2 below2    Keep?               Value
            //    0      0      0      0   =>   no                  0
            //    0      0      0      1   =>   yes filled below    2
            //    0      0      1      0   =>   yes filled above    1
            //    0      0      1      1   =>   no                  0
            //    0      1      0      0   =>   yes filled below    2
            //    0      1      0      1   =>   yes filled below    2
            //    0      1      1      0   =>   no                  0
            //    0      1      1      1   =>   no                  0
            //    1      0      0      0   =>   yes filled above    1
            //    1      0      0      1   =>   no                  0
            //    1      0      1      0   =>   yes filled above    1
            //    1      0      1      1   =>   no                  0
            //    1      1      0      0   =>   no                  0
            //    1      1      0      1   =>   no                  0
            //    1      1      1      0   =>   no                  0
            //    1      1      1      1   =>   no                  0
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  2, 2, 0, 0,
                                  1, 0, 1, 0,
                                  0, 0, 0, 0 });
        }

        public static List<Segment> Intersect(List<Segment> segments)
        {
            //                                      primary & secondary
            // above1 below1 above2 below2    Keep?               Value
            //    0      0      0      0   =>   no                  0
            //    0      0      0      1   =>   no                  0
            //    0      0      1      0   =>   no                  0
            //    0      0      1      1   =>   no                  0
            //    0      1      0      0   =>   no                  0
            //    0      1      0      1   =>   yes filled below    2
            //    0      1      1      0   =>   no                  0
            //    0      1      1      1   =>   yes filled below    2
            //    1      0      0      0   =>   no                  0
            //    1      0      0      1   =>   no                  0
            //    1      0      1      0   =>   yes filled above    1
            //    1      0      1      1   =>   yes filled above    1
            //    1      1      0      0   =>   no                  0
            //    1      1      0      1   =>   yes filled below    2
            //    1      1      1      0   =>   yes filled above    1
            //    1      1      1      1   =>   no                  0
            return select(segments,
                          new[] { 0, 0, 0, 0,
                                  0, 2, 0, 2,
                                  0, 0, 1, 1,
                                  0, 2, 1, 0 });
        }

        public static List<Segment> Difference(List<Segment> segments)
        {
            //                                      primary - secondary
            // above1 below1 above2 below2    Keep?               Value
            //    0      0      0      0   =>   no                  0
            //    0      0      0      1   =>   no                  0
            //    0      0      1      0   =>   no                  0
            //    0      0      1      1   =>   no                  0
            //    0      1      0      0   =>   yes filled below    2
            //    0      1      0      1   =>   no                  0
            //    0      1      1      0   =>   yes filled below    2
            //    0      1      1      1   =>   no                  0
            //    1      0      0      0   =>   yes filled above    1
            //    1      0      0      1   =>   yes filled above    1
            //    1      0      1      0   =>   no                  0
            //    1      0      1      1   =>   no                  0
            //    1      1      0      0   =>   no                  0
            //    1      1      0      1   =>   yes filled above    1
            //    1      1      1      0   =>   yes filled below    2
            //    1      1      1      1   =>   no                  0
            return select(segments,
                          new[] { 0, 0, 0, 0,
                                  2, 0, 2, 0,
                                  1, 1, 0, 0,
                                  0, 1, 2, 0 });
        }

        public static List<Segment> DifferenceRev(List<Segment> segments)
        {
            //                                      secondary - primary
            // above1 below1 above2 below2    Keep?               Value
            //    0      0      0      0   =>   no                  0
            //    0      0      0      1   =>   yes filled below    2
            //    0      0      1      0   =>   yes filled above    1
            //    0      0      1      1   =>   no                  0
            //    0      1      0      0   =>   no                  0
            //    0      1      0      1   =>   no                  0
            //    0      1      1      0   =>   yes filled above    1
            //    0      1      1      1   =>   yes filled above    1
            //    1      0      0      0   =>   no                  0
            //    1      0      0      1   =>   yes filled below    2
            //    1      0      1      0   =>   no                  0
            //    1      0      1      1   =>   yes filled below    2
            //    1      1      0      0   =>   no                  0
            //    1      1      0      1   =>   no                  0
            //    1      1      1      0   =>   no                  0
            //    1      1      1      1   =>   no                  0
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  0, 0, 1, 1,
                                  0, 2, 0, 2,
                                  0, 0, 0, 0 });
        }

        public static List<Segment> Xor(List<Segment> segments)
        {
            //                                      primary ^ secondary
            // above1 below1 above2 below2    Keep?               Value
            //    0      0      0      0   =>   no                  0
            //    0      0      0      1   =>   yes filled below    2
            //    0      0      1      0   =>   yes filled above    1
            //    0      0      1      1   =>   no                  0
            //    0      1      0      0   =>   yes filled below    2
            //    0      1      0      1   =>   no                  0
            //    0      1      1      0   =>   no                  0
            //    0      1      1      1   =>   yes filled above    1
            //    1      0      0      0   =>   yes filled above    1
            //    1      0      0      1   =>   no                  0
            //    1      0      1      0   =>   no                  0
            //    1      0      1      1   =>   yes filled below    2
            //    1      1      0      0   =>   no                  0
            //    1      1      0      1   =>   yes filled above    1
            //    1      1      1      0   =>   yes filled below    2
            //    1      1      1      1   =>   no                  0
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  2, 0, 0, 1,
                                  1, 0, 0, 2,
                                  0, 1, 2, 0 });
        }
    }
}
