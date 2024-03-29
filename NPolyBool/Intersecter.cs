﻿// The MIT License (MIT)

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
using System.Collections.Generic;

namespace PolyBool
{
    internal class Intersecter
    {
        private readonly bool selfIntersection;
        private readonly Epsilon eps;

        private Intersecter(bool selfIntersection, Epsilon eps)
        {
            this.selfIntersection = selfIntersection;
            this.eps = eps;
        }

        //
        // segment creation
        //

        private Segment segmentNew(Point start, Point end)
        {
            return new Segment
            {
                Start = start,
                End = end,
                MyFill = new Fill()
            };
        }

        private Segment segmentCopy(Point start, Point end, Segment seg)
        {
            return new Segment()
            {
                Start = start,
                End = end,
                MyFill = new Fill()
                {
                    Above = seg.MyFill.Above,
                    Below = seg.MyFill.Below
                },
                OtherFill = null
            };
        }

        private readonly LinkedList eventRoot = new LinkedList();

        private int eventCompare(bool p1IsStart, Point p11, Point p12, bool p2IsStart, Point p21, Point p22)
        {
            // compare the selected points first
            var comp = eps.PointsCompare(p11, p21);
            if (comp != 0)
                return comp;
            // the selected points are the same

            if (eps.PointsSame(p12, p22)) // if the non-selected points are the same too...
                return 0; // then the segments are equal

            if (p1IsStart != p2IsStart) // if one is a start and the other isn"t...
                return p1IsStart ? 1 : -1; // favor the one that isn"t the start

            // otherwise, we"ll have to calculate which one is below the other manually
            return eps.PointAboveOrOnLine(p12,
                                          p2IsStart ? p21 : p22, // order matters
                                          p2IsStart ? p22 : p21) ? 1 : -1;
        }

        private void eventAdd(Node ev, Point otherPt)
        {
            bool check(Node here)
            {
                // should ev be inserted before here?
                var comp = eventCompare(
                    ev.IsStart, ev.Pt, otherPt,
                    here.IsStart, here.Pt, here.Other.Pt
                );
                return comp < 0;
            }

            eventRoot.InsertBefore(ev, check);
        }

        private Node eventAddSegmentStart(Segment seg, bool primary)
        {
            var newNode = new Node
            {
                IsStart = true,
                Pt = seg.Start,
                Seg = seg,
                Primary = primary,
                Other = null,
                Status = null
            };
            var evStart = LinkedList.Node(newNode);
            eventAdd(evStart, seg.End);
            return evStart;
        }

        private void eventAddSegmentEnd(Node evStart, Segment seg, bool primary)
        {
            var newNode = new Node
            {
                IsStart = false,
                Pt = seg.End,
                Seg = seg,
                Primary = primary,
                Other = evStart,
                Status = null
            };
            var evEnd = LinkedList.Node(newNode);
            evStart.Other = evEnd;
            eventAdd(evEnd, evStart.Pt);
        }

        private void eventAddSegment(Segment seg, bool primary)
        {
            var evStart = eventAddSegmentStart(seg, primary);
            eventAddSegmentEnd(evStart, seg, primary);
        }

        private void eventUpdateEnd(Node ev, Point end)
        {
            // slides an end backwards
            //   (start)------------(end)    to:
            //   (start)---(end)

            ev.Other.Remove();
            ev.Seg.End = end;
            ev.Other.Pt = end;
            eventAdd(ev.Other, ev.Pt);
        }

        private void eventDivide(Node ev, Point pt)
        {
            var ns = segmentCopy(pt, ev.Seg.End, ev.Seg);
            eventUpdateEnd(ev, pt);
            eventAddSegment(ns, ev.Primary);
        }

        private List<Segment> calculate(bool primaryPolyInverted, bool secondaryPolyInverted)
        {
            // if selfIntersection is true then there is no secondary polygon, so that isn't used

            //
            // status logic
            //

            var statusRoot = new LinkedList();

            int statusCompare(Node ev1, Node ev2)
            {
                var a1 = ev1.Seg.Start;
                var a2 = ev1.Seg.End;
                var b1 = ev2.Seg.Start;
                var b2 = ev2.Seg.End;

                if (eps.PointsCollinear(a1, b1, b2))
                {
                    if (eps.PointsCollinear(a2, b1, b2))
                        return 1;
                    return eps.PointAboveOrOnLine(a2, b1, b2) ? 1 : -1;
                }
                return eps.PointAboveOrOnLine(a1, b1, b2) ? 1 : -1;
            }

            Transition statusFindSurrounding(Node ev)
            {
                bool check(Node here)
                {
                    var comp = statusCompare(ev, here.Ev);
                    return comp > 0;
                }
                return statusRoot.FindTransition(check);
            }

            Node checkIntersection(Node ev1, Node ev2)
            {
                // returns the segment equal to ev1, or false if nothing equal

                var seg1 = ev1.Seg;
                var seg2 = ev2.Seg;
                var a1 = seg1.Start;
                var a2 = seg1.End;
                var b1 = seg2.Start;
                var b2 = seg2.End;

                var i = eps.LinesIntersect(a1, a2, b1, b2);
                if (i == null)
                {
                    // segments are parallel or coincident

                    // if points aren"t collinear, then the segments are parallel, so no intersections
                    if (!eps.PointsCollinear(a1, a2, b1))
                        return null;
                    // otherwise, segments are on top of each other somehow (aka coincident)

                    if (eps.PointsSame(a1, b2) || eps.PointsSame(a2, b1))
                        return null; // segments touch at endpoints... no intersection

                    var a1EquB1 = eps.PointsSame(a1, b1);
                    var a2EquB2 = eps.PointsSame(a2, b2);

                    if (a1EquB1 && a2EquB2)
                        return ev2; // segments are exactly equal

                    var a1Between = !a1EquB1 && eps.PointBetween(a1, b1, b2);
                    var a2Between = !a2EquB2 && eps.PointBetween(a2, b1, b2);

                    if (a1EquB1)
                    {
                        if (a2Between)
                        {
                            //  (a1)---(a2)
                            //  (b1)----------(b2)
                            eventDivide(ev2, a2);
                        }
                        else
                        {
                            //  (a1)----------(a2)
                            //  (b1)---(b2)
                            eventDivide(ev1, b2);
                        }
                        return ev2;
                    }
                    else if (a1Between)
                    {
                        if (!a2EquB2)
                        {
                            // make a2 equal to b2
                            if (a2Between)
                            {
                                //         (a1)---(a2)
                                //  (b1)-----------------(b2)
                                eventDivide(ev2, a2);
                            }
                            else
                            {
                                //         (a1)----------(a2)
                                //  (b1)----------(b2)
                                eventDivide(ev1, b2);
                            }
                        }

                        //         (a1)---(a2)
                        //  (b1)----------(b2)
                        eventDivide(ev2, a1);
                    }
                }
                else
                {
                    // otherwise, lines intersect at i.pt, which may or may not be between the endpoints

                    // is A divided between its endpoints? (exclusive)
                    if (i.AlongA == 0)
                    {
                        if (i.AlongB == -1) // yes, at exactly b1
                        {
                            eventDivide(ev1, b1);
                        }
                        else if (i.AlongB == 0) // yes, somewhere between B"s endpoints
                        {
                            eventDivide(ev1, i.Pt);
                        }
                        else if (i.AlongB == 1) // yes, at exactly b2
                        {
                            eventDivide(ev1, b2);
                        }
                    }

                    // is B divided between its endpoints? (exclusive)
                    if (i.AlongB == 0)
                    {
                        if (i.AlongA == -1) // yes, at exactly a1
                        {
                            eventDivide(ev2, a1);
                        }
                        else if (i.AlongA == 0) // yes, somewhere between A"s endpoints (exclusive)
                        {
                            eventDivide(ev2, i.Pt);
                        }
                        else if (i.AlongA == 1) // yes, at exactly a2
                        {
                            eventDivide(ev2, a2);
                        }
                    }
                }
                return null;
            }

            //
            // main event loop
            //
            var segments = new List<Segment>();
            while (!eventRoot.IsEmpty())
            {
                var ev = eventRoot.GetHead();

                if (ev.IsStart)
                {
                    var surrounding = statusFindSurrounding(ev);
                    var above = surrounding.Before?.Ev;
                    var below = surrounding.After?.Ev;

                    Node checkBothIntersections()
                    {
                        if (above != null)
                        {
                            var node = checkIntersection(ev, above);
                            if (node != null)
                                return node;
                        }
                        if (below != null)
                            return checkIntersection(ev, below);
                        return null;
                    }

                    var eve = checkBothIntersections();
                    if (eve != null)
                    {
                        // ev and eve are equal
                        // we'll keep eve and throw away ev

                        // merge ev.seg's fill information into eve.seg

                        if (selfIntersection)
                        {
                            bool toggle; // are we a toggling edge?
                            if (ev.Seg.MyFill.Below == null)
                                toggle = true;
                            else
                                toggle = ev.Seg.MyFill.Above != ev.Seg.MyFill.Below;

                            // merge two segments that belong to the same polygon
                            // think of this as sandwiching two segments together, where `eve.seg` is
                            // the bottom -- this will cause the above fill flag to toggle
                            if (toggle)
                                eve.Seg.MyFill.Above = !eve.Seg.MyFill.Above;
                        }
                        else
                        {
                            // merge two segments that belong to different polygons
                            // each segment has distinct knowledge, so no special logic is needed
                            // note that this can only happen once per segment in this phase, because we
                            // are guaranteed that all self-intersections are gone
                            eve.Seg.OtherFill = ev.Seg.MyFill;
                        }

                        ev.Other.Remove();
                        ev.Remove();
                    }

                    if (!Equals(eventRoot.GetHead(), ev))
                    {
                        // something was inserted before us in the event queue, so loop back around and
                        // process it before continuing
                        continue;
                    }

                    //
                    // calculate fill flags
                    //
                    if (selfIntersection)
                    {
                        bool toggle; // are we a toggling edge?
                        if (ev.Seg.MyFill.Below == null) // if we are a new segment...
                            toggle = true; // then we toggle
                        else // we are a segment that has previous knowledge from a division
                            toggle = ev.Seg.MyFill.Above != ev.Seg.MyFill.Below; // calculate toggle

                        // next, calculate whether we are filled below us
                        if (below == null)
                        {
                            // if nothing is below us...
                            // we are filled below us if the polygon is inverted
                            ev.Seg.MyFill.Below = primaryPolyInverted;
                        }
                        else
                        {
                            // otherwise, we know the answer -- it"s the same if whatever is below
                            // us is filled above it
                            ev.Seg.MyFill.Below = below.Seg.MyFill.Above;
                        }

                        // since now we know if we"re filled below us, we can calculate whether
                        // we"re filled above us by applying toggle to whatever is below us
                        if (toggle)
                            ev.Seg.MyFill.Above = !ev.Seg.MyFill.Below;
                        else
                            ev.Seg.MyFill.Above = ev.Seg.MyFill.Below;
                    }
                    else
                    {
                        // now we fill in any missing transition information, since we are all-knowing
                        // at this point

                        if (ev.Seg.OtherFill == null)
                        {
                            // if we don"t have other information, then we need to figure out if we"re
                            // inside the other polygon
                            bool inside;
                            if (below == null)
                            {
                                // if nothing is below us, then we"re inside if the other polygon is
                                // inverted
                                inside = ev.Primary ? secondaryPolyInverted : primaryPolyInverted;
                            }
                            else
                            {
                                // otherwise, something is below us
                                // so copy the below segment"s other polygon"s above
                                if (ev.Primary == below.Primary)
                                    inside = below.Seg.OtherFill.Above.Value;
                                else
                                    inside = below.Seg.MyFill.Above.Value;
                            }
                            ev.Seg.OtherFill = new Fill()
                            {
                                Above = inside,
                                Below = inside
                            };
                        }
                    }

                    // insert the status and remember it for later removal
                    ev.Other.Status = surrounding.Insert(LinkedList.Node(new Node() { Ev = ev }));
                }
                else
                {
                    var st = ev.Status;

                    if (st == null)
                    {
                        throw new Exception("PolyBool: Zero-length segment detected; your epsilon is probably too small or too large");
                    }

                    // removing the status will create two new adjacent edges, so we"ll need to check
                    // for those
                    if (statusRoot.Exists(st.Previous) && statusRoot.Exists(st.Next))
                    {
                        checkIntersection(st.Previous.Ev, st.Next.Ev);
                    }

                    // remove the status
                    st.Remove();

                    // if we"ve reached this point, we"ve calculated everything there is to know, so
                    // save the segment for reporting
                    if (!ev.Primary)
                    {
                        // make sure `seg.myFill` actually points to the primary polygon though
                        var s = ev.Seg.MyFill;
                        ev.Seg.MyFill = ev.Seg.OtherFill;
                        ev.Seg.OtherFill = s;
                    }
                    segments.Add(ev.Seg);
                }

                // remove the event and continue
                eventRoot.GetHead().Remove();
            }

            return segments;
        }

        public class SegmentIntersecter : Intersecter
        {
            public SegmentIntersecter(Epsilon eps = null) : base(false, eps)
            {
            }

            public List<Segment> Calculate(List<Segment> segments1, bool inverted1, List<Segment> segments2, bool inverted2)
            {
                // returns segments that can be used for further operations
                foreach (Segment seg in segments1)
                    eventAddSegment(segmentCopy(seg.Start, seg.End, seg), true);

                foreach (Segment seg in segments2)
                    eventAddSegment(segmentCopy(seg.Start, seg.End, seg), false);

                return calculate(inverted1, inverted2);
            }
        }

        public class RegionIntersecter : Intersecter
        {
            public RegionIntersecter(Epsilon eps = null) : base(true, eps)
            {
            }

            public void AddRegion(Region region)
            {
                // skip empty region
                if (region.Points.Length == 0)
                    return;

                // regions are a list of points:
                //  [ [0, 0], [100, 0], [50, 100] ]
                // you can add multiple regions before running calculate
                Point pt1;
                var pt2 = region.Points[region.Points.Length - 1];
                for (int i = 0; i < region.Points.Length; i++)
                {
                    pt1 = pt2;
                    pt2 = region.Points[i];

                    var forward = eps.PointsCompare(pt1, pt2);
                    if (forward == 0) // points are equal, so we have a zero-length segment
                        continue; // just skip it

                    eventAddSegment(
                        segmentNew(
                            forward < 0 ? pt1 : pt2,
                            forward < 0 ? pt2 : pt1
                        ),
                        true
                    );
                }
            }

            internal List<Segment> Calculate(bool inverted)
            {
                return calculate(inverted, false);
            }
        }
    }
}
