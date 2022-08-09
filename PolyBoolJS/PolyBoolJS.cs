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
using Noesis.Javascript;

namespace PolyBoolJS
{
    public class PolyBool : IDisposable
    {
        private JavascriptContext context;

        private static string PolyBoolJs()
        {
            // return System.IO.File.ReadAllText("../../../../node_modules/polybooljs/dist/polybool.js");
            return System.IO.File.ReadAllText("../../../../polybool.js");
        }

        private JavascriptContext GetContext()
        {
            if (context == null)
            {
                context = new JavascriptContext();
                var script = PolyBoolJs();
                context.Run(script);
            }

            return context;
        }

        private global::PolyBool.Polygon Operate(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2, string selector)
        {
            var context = GetContext();
            context.SetParameter("poly1", poly1.ToJavaScript());
            context.SetParameter("poly2", poly2.ToJavaScript());

            var polybooljs = PolyBoolJs();

            var script = $"PolyBool.{selector}(poly1, poly2);";
            var poly3 = context.Run(script);
            return poly3.PolygonFromJavaScript();
        }

        public global::PolyBool.Polygon Union(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2)
        {
            return Operate(poly1, poly2, "union");
        }

        public global::PolyBool.Polygon Difference(global::PolyBool.Polygon poly1, global::PolyBool.Polygon poly2)
        {
            return Operate(poly1, poly2, "difference");
        }

        public void Dispose()
        {
            using (context) { }
        }
    }
}
