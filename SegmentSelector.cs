using System.Collections.Generic;

namespace PolyBool
{
    internal static class SegmentSelector
    {
        private static List<Segment> select(List<Segment> segments, int[] selection)
        {
            List<Segment> result = new List<Segment>();
            foreach (Segment seg in segments)
            {
                int index = (seg.MyFill.Above == true ? 8 : 0)
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
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  2, 2, 0, 0,
                                  1, 0, 1, 0,
                                  0, 0, 0, 0 });
        }

        public static List<Segment> Intersect(List<Segment> segments)
        {
            return select(segments,
                          new[] { 0, 0, 0, 0,
                                  0, 2, 0, 2,
                                  0, 0, 1, 1,
                                  0, 2, 1, 0 });
        }

        public static List<Segment> Difference(List<Segment> segments)
        {
            return select(segments,
                          new[] { 0, 0, 0, 0,
                                  2, 0, 2, 0,
                                  1, 1, 0, 0,
                                  0, 1, 2, 0 });
        }

        public static List<Segment> DifferenceRev(List<Segment> segments)
        {
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  0, 0, 1, 1,
                                  0, 2, 0, 2,
                                  0, 0, 0, 0 });
        }

        public static List<Segment> Xor(List<Segment> segments)
        {
            return select(segments,
                          new[] { 0, 2, 1, 0,
                                  2, 0, 0, 1,
                                  1, 0, 0, 2,
                                  0, 1, 2, 0 });
        }
    }
}
